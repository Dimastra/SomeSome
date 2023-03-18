using System;
using System.Runtime.CompilerServices;
using Content.Server.Audio;
using Content.Server.Body.Systems;
using Content.Server.GameTicking;
using Content.Server.Mind;
using Content.Server.Players;
using Content.Server.Popups;
using Content.Server.Power.Components;
using Content.Server.Power.EntitySystems;
using Content.Server.Recycling.Components;
using Content.Shared.Audio;
using Content.Shared.Body.Components;
using Content.Shared.Emag.Components;
using Content.Shared.Emag.Systems;
using Content.Shared.Examine;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction.Events;
using Content.Shared.Popups;
using Content.Shared.Recycling;
using Content.Shared.Tag;
using Robust.Server.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Physics.Events;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Server.Recycling
{
	// Token: 0x02000248 RID: 584
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RecyclerSystem : EntitySystem
	{
		// Token: 0x06000BAF RID: 2991 RVA: 0x0003D8D8 File Offset: 0x0003BAD8
		public override void Initialize()
		{
			base.SubscribeLocalEvent<RecyclerComponent, ExaminedEvent>(new ComponentEventHandler<RecyclerComponent, ExaminedEvent>(this.OnExamined), null, null);
			base.SubscribeLocalEvent<RecyclerComponent, StartCollideEvent>(new ComponentEventRefHandler<RecyclerComponent, StartCollideEvent>(this.OnCollide), null, null);
			base.SubscribeLocalEvent<RecyclerComponent, GotEmaggedEvent>(new ComponentEventRefHandler<RecyclerComponent, GotEmaggedEvent>(this.OnEmagged), null, null);
			base.SubscribeLocalEvent<RecyclerComponent, SuicideEvent>(new ComponentEventHandler<RecyclerComponent, SuicideEvent>(this.OnSuicide), null, null);
			base.SubscribeLocalEvent<RecyclerComponent, PowerChangedEvent>(new ComponentEventRefHandler<RecyclerComponent, PowerChangedEvent>(this.OnPowerChanged), null, null);
		}

		// Token: 0x06000BB0 RID: 2992 RVA: 0x0003D949 File Offset: 0x0003BB49
		private void OnExamined(EntityUid uid, RecyclerComponent component, ExaminedEvent args)
		{
			args.PushMarkup(Loc.GetString("recycler-count-items", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("items", component.ItemsProcessed)
			}));
		}

		// Token: 0x06000BB1 RID: 2993 RVA: 0x0003D980 File Offset: 0x0003BB80
		private void OnSuicide(EntityUid uid, RecyclerComponent component, SuicideEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			args.SetHandled(SuicideKind.Bloodloss);
			EntityUid victim = args.Victim;
			ActorComponent actor;
			if (base.TryComp<ActorComponent>(victim, ref actor))
			{
				PlayerData playerData = actor.PlayerSession.ContentData();
				Mind mind = (playerData != null) ? playerData.Mind : null;
				if (mind != null)
				{
					this._ticker.OnGhostAttempt(mind, false, false);
					EntityUid? ownedEntity = mind.OwnedEntity;
					if (ownedEntity != null)
					{
						EntityUid entity = ownedEntity.GetValueOrDefault();
						if (entity.Valid)
						{
							this._popup.PopupEntity(Loc.GetString("recycler-component-suicide-message"), entity, PopupType.Small);
						}
					}
				}
			}
			this._popup.PopupEntity(Loc.GetString("recycler-component-suicide-message-others", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("victim", Identity.Entity(victim, this.EntityManager))
			}), victim, Filter.PvsExcept(victim, 2f, this.EntityManager), true, PopupType.Small);
			BodyComponent body;
			if (base.TryComp<BodyComponent>(victim, ref body))
			{
				this._bodySystem.GibBody(new EntityUid?(victim), true, body, false);
			}
			this.Bloodstain(component);
		}

		// Token: 0x06000BB2 RID: 2994 RVA: 0x0003DA90 File Offset: 0x0003BC90
		public void EnableRecycler(RecyclerComponent component)
		{
			if (component.Enabled)
			{
				return;
			}
			component.Enabled = true;
			ApcPowerReceiverComponent apcPower;
			if (base.TryComp<ApcPowerReceiverComponent>(component.Owner, ref apcPower))
			{
				this._ambience.SetAmbience(component.Owner, apcPower.Powered, null);
				return;
			}
			this._ambience.SetAmbience(component.Owner, true, null);
		}

		// Token: 0x06000BB3 RID: 2995 RVA: 0x0003DAE9 File Offset: 0x0003BCE9
		public void DisableRecycler(RecyclerComponent component)
		{
			if (!component.Enabled)
			{
				return;
			}
			component.Enabled = false;
			this._ambience.SetAmbience(component.Owner, false, null);
		}

		// Token: 0x06000BB4 RID: 2996 RVA: 0x0003DB0E File Offset: 0x0003BD0E
		private void OnPowerChanged(EntityUid uid, RecyclerComponent component, ref PowerChangedEvent args)
		{
			if (component.Enabled)
			{
				this._ambience.SetAmbience(uid, args.Powered, null);
			}
		}

		// Token: 0x06000BB5 RID: 2997 RVA: 0x0003DB2C File Offset: 0x0003BD2C
		private void OnCollide(EntityUid uid, RecyclerComponent component, ref StartCollideEvent args)
		{
			if (component.Enabled && args.OurFixture.ID != "brrt")
			{
				return;
			}
			ApcPowerReceiverComponent apcPower;
			if (base.TryComp<ApcPowerReceiverComponent>(uid, ref apcPower) && !apcPower.Powered)
			{
				return;
			}
			this.Recycle(component, args.OtherFixture.Body.Owner);
		}

		// Token: 0x06000BB6 RID: 2998 RVA: 0x0003DB84 File Offset: 0x0003BD84
		private void Recycle(RecyclerComponent component, EntityUid entity)
		{
			RecyclableComponent recyclable = null;
			if (!this._tags.HasAnyTag(entity, new string[]
			{
				"Trash",
				"Recyclable"
			}) && (!base.TryComp<RecyclableComponent>(entity, ref recyclable) || (!recyclable.Safe && !base.HasComp<EmaggedComponent>(component.Owner))))
			{
				return;
			}
			if (this.CanGib(component, entity))
			{
				this._bodySystem.GibBody(new EntityUid?(entity), true, base.Comp<BodyComponent>(entity), false);
				this.Bloodstain(component);
				return;
			}
			if (recyclable == null)
			{
				base.QueueDel(entity);
			}
			else
			{
				this.Recycle(recyclable, component.Efficiency);
			}
			if (component.Sound != null && (this._timing.CurTime - component.LastSound).TotalSeconds > 0.800000011920929)
			{
				this._soundSystem.PlayPvs(component.Sound, component.Owner, new AudioParams?(AudioHelpers.WithVariation(0.01f).WithVolume(-3f)));
				component.LastSound = this._timing.CurTime;
			}
			component.ItemsProcessed++;
		}

		// Token: 0x06000BB7 RID: 2999 RVA: 0x0003DCA3 File Offset: 0x0003BEA3
		private bool CanGib(RecyclerComponent component, EntityUid entity)
		{
			return base.HasComp<BodyComponent>(entity) && base.HasComp<EmaggedComponent>(component.Owner) && this.IsPowered(component.Owner, this.EntityManager, null);
		}

		// Token: 0x06000BB8 RID: 3000 RVA: 0x0003DCD4 File Offset: 0x0003BED4
		public void Bloodstain(RecyclerComponent component)
		{
			AppearanceComponent appearance;
			if (this.EntityManager.TryGetComponent<AppearanceComponent>(component.Owner, ref appearance))
			{
				this._appearanceSystem.SetData(component.Owner, RecyclerVisuals.Bloody, true, appearance);
			}
		}

		// Token: 0x06000BB9 RID: 3001 RVA: 0x0003DD14 File Offset: 0x0003BF14
		private void Recycle(RecyclableComponent component, float efficiency = 1f)
		{
			if (!string.IsNullOrEmpty(component.Prototype))
			{
				TransformComponent xform = base.Transform(component.Owner);
				int i = 0;
				while ((float)i < Math.Max((float)component.Amount * efficiency, 1f))
				{
					base.Spawn(component.Prototype, xform.Coordinates);
					i++;
				}
			}
			base.QueueDel(component.Owner);
		}

		// Token: 0x06000BBA RID: 3002 RVA: 0x0003DD79 File Offset: 0x0003BF79
		private void OnEmagged(EntityUid uid, RecyclerComponent component, ref GotEmaggedEvent args)
		{
			args.Handled = true;
		}

		// Token: 0x0400072B RID: 1835
		[Dependency]
		private readonly IGameTiming _timing;

		// Token: 0x0400072C RID: 1836
		[Dependency]
		private readonly AmbientSoundSystem _ambience;

		// Token: 0x0400072D RID: 1837
		[Dependency]
		private readonly BodySystem _bodySystem;

		// Token: 0x0400072E RID: 1838
		[Dependency]
		private readonly GameTicker _ticker;

		// Token: 0x0400072F RID: 1839
		[Dependency]
		private readonly PopupSystem _popup;

		// Token: 0x04000730 RID: 1840
		[Dependency]
		private readonly TagSystem _tags;

		// Token: 0x04000731 RID: 1841
		[Dependency]
		private readonly AudioSystem _soundSystem;

		// Token: 0x04000732 RID: 1842
		[Dependency]
		private readonly SharedAppearanceSystem _appearanceSystem;

		// Token: 0x04000733 RID: 1843
		private const string RecyclerColliderName = "brrt";

		// Token: 0x04000734 RID: 1844
		private const float RecyclerSoundCooldown = 0.8f;
	}
}
