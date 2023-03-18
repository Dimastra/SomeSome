using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Server.Borgs;
using Content.Server.Flash.Components;
using Content.Server.Light.EntitySystems;
using Content.Server.Stunnable;
using Content.Shared.Examine;
using Content.Shared.Flash;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.Tag;
using Content.Shared.Weapons.Melee.Events;
using Robust.Server.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Server.Flash
{
	// Token: 0x020004FA RID: 1274
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class FlashSystem : SharedFlashSystem
	{
		// Token: 0x06001A32 RID: 6706 RVA: 0x0008A218 File Offset: 0x00088418
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<FlashComponent, MeleeHitEvent>(new ComponentEventHandler<FlashComponent, MeleeHitEvent>(this.OnFlashMeleeHit), null, null);
			base.SubscribeLocalEvent<FlashComponent, UseInHandEvent>(new ComponentEventHandler<FlashComponent, UseInHandEvent>(this.OnFlashUseInHand), new Type[]
			{
				typeof(HandheldLightSystem)
			}, null);
			base.SubscribeLocalEvent<FlashComponent, ExaminedEvent>(new ComponentEventHandler<FlashComponent, ExaminedEvent>(this.OnFlashExamined), null, null);
			base.SubscribeLocalEvent<InventoryComponent, FlashAttemptEvent>(new ComponentEventHandler<InventoryComponent, FlashAttemptEvent>(this.OnInventoryFlashAttempt), null, null);
			base.SubscribeLocalEvent<FlashImmunityComponent, FlashAttemptEvent>(new ComponentEventHandler<FlashImmunityComponent, FlashAttemptEvent>(this.OnFlashImmunityFlashAttempt), null, null);
		}

		// Token: 0x06001A33 RID: 6707 RVA: 0x0008A2A4 File Offset: 0x000884A4
		private void OnFlashMeleeHit(EntityUid uid, FlashComponent comp, MeleeHitEvent args)
		{
			if (!args.IsHit || !args.HitEntities.Any<EntityUid>() || !this.UseFlash(comp, args.User))
			{
				return;
			}
			args.Handled = true;
			foreach (EntityUid e in args.HitEntities)
			{
				this.Flash(e, new EntityUid?(args.User), new EntityUid?(uid), (float)comp.FlashDuration, comp.SlowTo, true, null, false);
			}
		}

		// Token: 0x06001A34 RID: 6708 RVA: 0x0008A340 File Offset: 0x00088540
		private void OnFlashUseInHand(EntityUid uid, FlashComponent comp, UseInHandEvent args)
		{
			if (args.Handled || !this.UseFlash(comp, args.User))
			{
				return;
			}
			args.Handled = true;
			this.FlashArea(uid, new EntityUid?(args.User), comp.Range, (float)comp.AoeFlashDuration, comp.SlowTo, true, null);
		}

		// Token: 0x06001A35 RID: 6709 RVA: 0x0008A394 File Offset: 0x00088594
		private bool UseFlash(FlashComponent comp, EntityUid user)
		{
			if (!comp.HasUses)
			{
				return false;
			}
			SpriteComponent sprite;
			if (!this.EntityManager.TryGetComponent<SpriteComponent>(comp.Owner, ref sprite))
			{
				return false;
			}
			FlashComponent comp2 = comp;
			int num = comp2.Uses - 1;
			comp2.Uses = num;
			if (num == 0 && !comp.AutoRecharge)
			{
				sprite.LayerSetState(0, "burnt");
				this._tagSystem.AddTag(comp.Owner, "Trash");
				comp.Owner.PopupMessage(user, Loc.GetString("flash-component-becomes-empty"));
			}
			else if (!comp.Flashing)
			{
				int animLayer = sprite.AddLayerWithState("flashing");
				comp.Flashing = true;
				TimerExtensions.SpawnTimer(comp.Owner, 400, delegate()
				{
					sprite.RemoveLayer(animLayer);
					comp.Flashing = false;
				}, default(CancellationToken));
			}
			SoundSystem.Play(comp.Sound.GetSound(null, null), Filter.Pvs(comp.Owner, 2f, null, null, null), comp.Owner, new AudioParams?(AudioParams.Default));
			return true;
		}

		// Token: 0x06001A36 RID: 6710 RVA: 0x0008A4F0 File Offset: 0x000886F0
		[NullableContext(2)]
		public void Flash(EntityUid target, EntityUid? user, EntityUid? used, float flashDuration, float slowTo, bool displayPopup = true, FlashableComponent flashable = null, bool massFlash = false)
		{
			if (!base.Resolve<FlashableComponent>(target, ref flashable, false))
			{
				return;
			}
			FlashAttemptEvent attempt = new FlashAttemptEvent(target, user, used, massFlash);
			base.RaiseLocalEvent<FlashAttemptEvent>(target, attempt, true);
			if (attempt.Cancelled)
			{
				return;
			}
			if (base.HasComp<BorgComponent>(target))
			{
				if (target == user)
				{
					return;
				}
				this._stunSystem.TryParalyze(target, TimeSpan.FromSeconds(3.5), true, null);
			}
			flashable.LastFlash = this._gameTiming.CurTime;
			flashable.Duration = flashDuration / 1000f;
			base.Dirty(flashable, null);
			this._stunSystem.TrySlowdown(target, TimeSpan.FromSeconds((double)(flashDuration / 1000f)), true, slowTo, slowTo, null);
			if (displayPopup && user != null && target != user && this.EntityManager.EntityExists(user.Value))
			{
				user.Value.PopupMessage(target, Loc.GetString("flash-component-user-blinds-you", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("user", Identity.Entity(user.Value, this.EntityManager))
				}));
			}
		}

		// Token: 0x06001A37 RID: 6711 RVA: 0x0008A640 File Offset: 0x00088840
		[NullableContext(2)]
		public void FlashArea(EntityUid source, EntityUid? user, float range, float duration, float slowTo = 0.8f, bool displayPopup = false, SoundSpecifier sound = null)
		{
			TransformComponent transform = this.EntityManager.GetComponent<TransformComponent>(source);
			MapCoordinates mapPosition = transform.MapPosition;
			List<EntityUid> flashableEntities = new List<EntityUid>();
			EntityQuery<FlashableComponent> flashableQuery = base.GetEntityQuery<FlashableComponent>();
			foreach (EntityUid entity in this._entityLookup.GetEntitiesInRange(transform.Coordinates, range, 46))
			{
				if (flashableQuery.HasComponent(entity))
				{
					flashableEntities.Add(entity);
				}
			}
			SharedInteractionSystem.Ignored <>9__0;
			foreach (EntityUid entity2 in flashableEntities)
			{
				SharedInteractionSystem interactionSystem = this._interactionSystem;
				EntityUid origin = entity2;
				MapCoordinates other = mapPosition;
				CollisionGroup collisionMask = CollisionGroup.Opaque;
				SharedInteractionSystem.Ignored predicate;
				if ((predicate = <>9__0) == null)
				{
					predicate = (<>9__0 = ((EntityUid e) => flashableEntities.Contains(e) || e == source));
				}
				if (interactionSystem.InRangeUnobstructed(origin, other, range, collisionMask, predicate, false))
				{
					this.Flash(entity2, user, new EntityUid?(source), duration, slowTo, displayPopup, flashableQuery.GetComponent(entity2), true);
				}
			}
			if (sound != null)
			{
				SoundSystem.Play(sound.GetSound(null, null), Filter.Pvs(transform, 2f), source, null);
			}
		}

		// Token: 0x06001A38 RID: 6712 RVA: 0x0008A7B0 File Offset: 0x000889B0
		private void OnFlashExamined(EntityUid uid, FlashComponent comp, ExaminedEvent args)
		{
			if (comp.AutoRecharge)
			{
				if (comp.Uses == comp.MaxCharges)
				{
					args.PushMarkup(Loc.GetString("emag-max-charges"));
					return;
				}
				double timeRemaining = Math.Round((comp.NextChargeTime - this._gameTiming.CurTime).TotalSeconds);
				args.PushMarkup(Loc.GetString("emag-recharging", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("seconds", timeRemaining)
				}));
			}
			if (comp != null && !comp.HasUses && !comp.AutoRecharge)
			{
				args.PushText(Loc.GetString("flash-component-examine-empty"));
				return;
			}
			if (args.IsInDetailsRange)
			{
				args.PushMarkup(Loc.GetString("flash-component-examine-detail-count", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("count", comp.Uses),
					new ValueTuple<string, object>("markupCountColor", "green")
				}));
			}
		}

		// Token: 0x06001A39 RID: 6713 RVA: 0x0008A8AC File Offset: 0x00088AAC
		private void OnInventoryFlashAttempt(EntityUid uid, InventoryComponent component, FlashAttemptEvent args)
		{
			foreach (string slot in new string[]
			{
				"head",
				"eyes",
				"mask"
			})
			{
				if (args.Cancelled)
				{
					break;
				}
				EntityUid? item;
				if (this._inventorySystem.TryGetSlotEntity(uid, slot, out item, component, null))
				{
					base.RaiseLocalEvent<FlashAttemptEvent>(item.Value, args, true);
				}
			}
		}

		// Token: 0x06001A3A RID: 6714 RVA: 0x0008A914 File Offset: 0x00088B14
		private void OnFlashImmunityFlashAttempt(EntityUid uid, FlashImmunityComponent component, FlashAttemptEvent args)
		{
			if (component.Enabled)
			{
				args.Cancel();
			}
		}

		// Token: 0x06001A3B RID: 6715 RVA: 0x0008A924 File Offset: 0x00088B24
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			foreach (FlashComponent flash in base.EntityQuery<FlashComponent>(false))
			{
				if (flash.AutoRecharge && flash.Uses != flash.MaxCharges && !(this._gameTiming.CurTime < flash.NextChargeTime))
				{
					this.ChangeFlashCharge(flash.Owner, 1, true, flash);
				}
			}
		}

		// Token: 0x06001A3C RID: 6716 RVA: 0x0008A9B0 File Offset: 0x00088BB0
		[NullableContext(2)]
		private bool ChangeFlashCharge(EntityUid uid, int change, bool resetTimer, FlashComponent component = null)
		{
			if (!base.Resolve<FlashComponent>(uid, ref component, true))
			{
				return false;
			}
			if (component.Uses + change < 0 || component.Uses + change > component.MaxCharges)
			{
				return false;
			}
			if (resetTimer || component.Uses == component.MaxCharges)
			{
				component.NextChargeTime = this._gameTiming.CurTime + component.RechargeDuration;
			}
			component.Uses += change;
			base.Dirty(component, null);
			return true;
		}

		// Token: 0x04001099 RID: 4249
		[Dependency]
		private readonly EntityLookupSystem _entityLookup;

		// Token: 0x0400109A RID: 4250
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x0400109B RID: 4251
		[Dependency]
		private readonly StunSystem _stunSystem;

		// Token: 0x0400109C RID: 4252
		[Dependency]
		private readonly InventorySystem _inventorySystem;

		// Token: 0x0400109D RID: 4253
		[Dependency]
		private readonly MetaDataSystem _metaSystem;

		// Token: 0x0400109E RID: 4254
		[Dependency]
		private readonly SharedInteractionSystem _interactionSystem;

		// Token: 0x0400109F RID: 4255
		[Dependency]
		private readonly TagSystem _tagSystem;
	}
}
