using System;
using System.Runtime.CompilerServices;
using Content.Server.Body.Systems;
using Content.Server.Drone.Components;
using Content.Server.Ghost.Components;
using Content.Server.Ghost.Roles.Components;
using Content.Server.Mind.Components;
using Content.Server.Popups;
using Content.Server.Tools.Innate;
using Content.Server.UserInterface;
using Content.Shared.Body.Components;
using Content.Shared.Drone;
using Content.Shared.Emoting;
using Content.Shared.Examine;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction.Components;
using Content.Shared.Interaction.Events;
using Content.Shared.Item;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Tag;
using Content.Shared.Throwing;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Timing;

namespace Content.Server.Drone
{
	// Token: 0x02000538 RID: 1336
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DroneSystem : SharedDroneSystem
	{
		// Token: 0x06001BE3 RID: 7139 RVA: 0x00094B9C File Offset: 0x00092D9C
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<DroneComponent, InteractionAttemptEvent>(new ComponentEventHandler<DroneComponent, InteractionAttemptEvent>(this.OnInteractionAttempt), null, null);
			base.SubscribeLocalEvent<DroneComponent, UserOpenActivatableUIAttemptEvent>(new ComponentEventHandler<DroneComponent, UserOpenActivatableUIAttemptEvent>(this.OnActivateUIAttempt), null, null);
			base.SubscribeLocalEvent<DroneComponent, MobStateChangedEvent>(new ComponentEventHandler<DroneComponent, MobStateChangedEvent>(this.OnMobStateChanged), null, null);
			base.SubscribeLocalEvent<DroneComponent, ExaminedEvent>(new ComponentEventHandler<DroneComponent, ExaminedEvent>(this.OnExamined), null, null);
			base.SubscribeLocalEvent<DroneComponent, MindAddedMessage>(new ComponentEventHandler<DroneComponent, MindAddedMessage>(this.OnMindAdded), null, null);
			base.SubscribeLocalEvent<DroneComponent, MindRemovedMessage>(new ComponentEventHandler<DroneComponent, MindRemovedMessage>(this.OnMindRemoved), null, null);
			base.SubscribeLocalEvent<DroneComponent, EmoteAttemptEvent>(new ComponentEventHandler<DroneComponent, EmoteAttemptEvent>(this.OnEmoteAttempt), null, null);
			base.SubscribeLocalEvent<DroneComponent, ThrowAttemptEvent>(new ComponentEventHandler<DroneComponent, ThrowAttemptEvent>(this.OnThrowAttempt), null, null);
		}

		// Token: 0x06001BE4 RID: 7140 RVA: 0x00094C50 File Offset: 0x00092E50
		private void OnInteractionAttempt(EntityUid uid, DroneComponent component, InteractionAttemptEvent args)
		{
			if (args.Target != null && !base.HasComp<UnremoveableComponent>(args.Target) && this.NonDronesInRange(uid, component))
			{
				args.Cancel();
			}
			if (base.HasComp<ItemComponent>(args.Target) && !base.HasComp<UnremoveableComponent>(args.Target) && !this._tagSystem.HasAnyTag(args.Target.Value, new string[]
			{
				"DroneUsable",
				"Trash"
			}))
			{
				args.Cancel();
			}
		}

		// Token: 0x06001BE5 RID: 7141 RVA: 0x00094CDE File Offset: 0x00092EDE
		private void OnActivateUIAttempt(EntityUid uid, DroneComponent component, UserOpenActivatableUIAttemptEvent args)
		{
			if (!this._tagSystem.HasTag(args.Target, "DroneUsable"))
			{
				args.Cancel();
			}
		}

		// Token: 0x06001BE6 RID: 7142 RVA: 0x00094D00 File Offset: 0x00092F00
		private void OnExamined(EntityUid uid, DroneComponent component, ExaminedEvent args)
		{
			MindComponent mind;
			if (base.TryComp<MindComponent>(uid, ref mind) && mind.HasMind)
			{
				args.PushMarkup(Loc.GetString("drone-active"));
				return;
			}
			args.PushMarkup(Loc.GetString("drone-dormant"));
		}

		// Token: 0x06001BE7 RID: 7143 RVA: 0x00094D44 File Offset: 0x00092F44
		private void OnMobStateChanged(EntityUid uid, DroneComponent drone, MobStateChangedEvent args)
		{
			if (args.NewMobState == MobState.Dead)
			{
				InnateToolComponent innate;
				if (base.TryComp<InnateToolComponent>(uid, ref innate))
				{
					this._innateToolSystem.Cleanup(uid, innate);
				}
				BodyComponent body;
				if (base.TryComp<BodyComponent>(uid, ref body))
				{
					this._bodySystem.GibBody(new EntityUid?(uid), false, body, false);
				}
				base.QueueDel(uid);
			}
		}

		// Token: 0x06001BE8 RID: 7144 RVA: 0x00094D9A File Offset: 0x00092F9A
		private void OnMindAdded(EntityUid uid, DroneComponent drone, MindAddedMessage args)
		{
			this.UpdateDroneAppearance(uid, SharedDroneSystem.DroneStatus.On);
			this._popupSystem.PopupEntity(Loc.GetString("drone-activated"), uid, PopupType.Large);
		}

		// Token: 0x06001BE9 RID: 7145 RVA: 0x00094DBB File Offset: 0x00092FBB
		private void OnMindRemoved(EntityUid uid, DroneComponent drone, MindRemovedMessage args)
		{
			this.UpdateDroneAppearance(uid, SharedDroneSystem.DroneStatus.Off);
			base.EnsureComp<GhostTakeoverAvailableComponent>(uid);
		}

		// Token: 0x06001BEA RID: 7146 RVA: 0x00094DCD File Offset: 0x00092FCD
		private void OnEmoteAttempt(EntityUid uid, DroneComponent component, EmoteAttemptEvent args)
		{
			args.Cancel();
		}

		// Token: 0x06001BEB RID: 7147 RVA: 0x00094DD5 File Offset: 0x00092FD5
		private void OnThrowAttempt(EntityUid uid, DroneComponent drone, ThrowAttemptEvent args)
		{
			args.Cancel();
		}

		// Token: 0x06001BEC RID: 7148 RVA: 0x00094DE0 File Offset: 0x00092FE0
		private void UpdateDroneAppearance(EntityUid uid, SharedDroneSystem.DroneStatus status)
		{
			AppearanceComponent appearance;
			if (base.TryComp<AppearanceComponent>(uid, ref appearance))
			{
				this._appearance.SetData(uid, SharedDroneSystem.DroneVisuals.Status, status, appearance);
			}
		}

		// Token: 0x06001BED RID: 7149 RVA: 0x00094E14 File Offset: 0x00093014
		private bool NonDronesInRange(EntityUid uid, DroneComponent component)
		{
			TransformComponent xform = base.Comp<TransformComponent>(uid);
			foreach (EntityUid entity in this._lookup.GetEntitiesInRange(xform.MapPosition, component.InteractionBlockRange, 46))
			{
				MobStateComponent entityMobState;
				if (base.HasComp<MindComponent>(entity) && !base.HasComp<DroneComponent>(entity) && !base.HasComp<GhostComponent>(entity) && (!base.TryComp<MobStateComponent>(entity, ref entityMobState) || !base.HasComp<GhostTakeoverAvailableComponent>(entity) || !this._mobStateSystem.IsDead(entity, entityMobState)))
				{
					if (this._gameTiming.IsFirstTimePredicted)
					{
						this._popupSystem.PopupEntity(Loc.GetString("drone-too-close", new ValueTuple<string, object>[]
						{
							new ValueTuple<string, object>("being", Identity.Entity(entity, this.EntityManager))
						}), uid, uid, PopupType.Small);
					}
					return true;
				}
			}
			return false;
		}

		// Token: 0x040011ED RID: 4589
		[Dependency]
		private readonly BodySystem _bodySystem;

		// Token: 0x040011EE RID: 4590
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x040011EF RID: 4591
		[Dependency]
		private readonly TagSystem _tagSystem;

		// Token: 0x040011F0 RID: 4592
		[Dependency]
		private readonly EntityLookupSystem _lookup;

		// Token: 0x040011F1 RID: 4593
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x040011F2 RID: 4594
		[Dependency]
		private readonly InnateToolSystem _innateToolSystem;

		// Token: 0x040011F3 RID: 4595
		[Dependency]
		private readonly MobStateSystem _mobStateSystem;

		// Token: 0x040011F4 RID: 4596
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;
	}
}
