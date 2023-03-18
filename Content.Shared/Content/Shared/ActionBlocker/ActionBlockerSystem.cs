using System;
using System.Runtime.CompilerServices;
using Content.Shared.Body.Events;
using Content.Shared.Emoting;
using Content.Shared.Hands;
using Content.Shared.Interaction.Events;
using Content.Shared.Item;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Events;
using Content.Shared.Speech;
using Content.Shared.Throwing;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.ActionBlocker
{
	// Token: 0x0200076C RID: 1900
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ActionBlockerSystem : EntitySystem
	{
		// Token: 0x06001768 RID: 5992 RVA: 0x0004C15B File Offset: 0x0004A35B
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<InputMoverComponent, ComponentStartup>(new ComponentEventHandler<InputMoverComponent, ComponentStartup>(this.OnMoverStartup), null, null);
		}

		// Token: 0x06001769 RID: 5993 RVA: 0x0004C177 File Offset: 0x0004A377
		private void OnMoverStartup(EntityUid uid, InputMoverComponent component, ComponentStartup args)
		{
			this.UpdateCanMove(uid, component);
		}

		// Token: 0x0600176A RID: 5994 RVA: 0x0004C182 File Offset: 0x0004A382
		[NullableContext(2)]
		public bool CanMove(EntityUid uid, InputMoverComponent component = null)
		{
			return base.Resolve<InputMoverComponent>(uid, ref component, false) && component.CanMove;
		}

		// Token: 0x0600176B RID: 5995 RVA: 0x0004C198 File Offset: 0x0004A398
		[NullableContext(2)]
		public bool UpdateCanMove(EntityUid uid, InputMoverComponent component = null)
		{
			if (!base.Resolve<InputMoverComponent>(uid, ref component, false))
			{
				return false;
			}
			UpdateCanMoveEvent ev = new UpdateCanMoveEvent(uid);
			base.RaiseLocalEvent<UpdateCanMoveEvent>(uid, ev, false);
			if (component.CanMove == ev.Cancelled)
			{
				base.Dirty(component, null);
			}
			component.CanMove = !ev.Cancelled;
			return !ev.Cancelled;
		}

		// Token: 0x0600176C RID: 5996 RVA: 0x0004C1F4 File Offset: 0x0004A3F4
		public bool CanInteract(EntityUid user, EntityUid? target)
		{
			InteractionAttemptEvent ev = new InteractionAttemptEvent(user, target);
			base.RaiseLocalEvent<InteractionAttemptEvent>(user, ev, false);
			if (ev.Cancelled)
			{
				return false;
			}
			if (target == null)
			{
				return true;
			}
			GettingInteractedWithAttemptEvent targetEv = new GettingInteractedWithAttemptEvent(user, target);
			base.RaiseLocalEvent<GettingInteractedWithAttemptEvent>(target.Value, targetEv, false);
			return !targetEv.Cancelled;
		}

		// Token: 0x0600176D RID: 5997 RVA: 0x0004C248 File Offset: 0x0004A448
		public bool CanUseHeldEntity(EntityUid user)
		{
			UseAttemptEvent ev = new UseAttemptEvent(user);
			base.RaiseLocalEvent<UseAttemptEvent>(user, ev, false);
			return !ev.Cancelled;
		}

		// Token: 0x0600176E RID: 5998 RVA: 0x0004C270 File Offset: 0x0004A470
		public bool CanThrow(EntityUid user, EntityUid itemUid)
		{
			ThrowAttemptEvent ev = new ThrowAttemptEvent(user, itemUid);
			base.RaiseLocalEvent<ThrowAttemptEvent>(user, ev, false);
			return !ev.Cancelled;
		}

		// Token: 0x0600176F RID: 5999 RVA: 0x0004C298 File Offset: 0x0004A498
		public bool CanSpeak(EntityUid uid)
		{
			SpeakAttemptEvent ev = new SpeakAttemptEvent(uid);
			base.RaiseLocalEvent<SpeakAttemptEvent>(uid, ev, true);
			return !ev.Cancelled;
		}

		// Token: 0x06001770 RID: 6000 RVA: 0x0004C2C0 File Offset: 0x0004A4C0
		public bool CanDrop(EntityUid uid)
		{
			DropAttemptEvent ev = new DropAttemptEvent();
			base.RaiseLocalEvent<DropAttemptEvent>(uid, ev, false);
			return !ev.Cancelled;
		}

		// Token: 0x06001771 RID: 6001 RVA: 0x0004C2E8 File Offset: 0x0004A4E8
		public bool CanPickup(EntityUid user, EntityUid item)
		{
			PickupAttemptEvent userEv = new PickupAttemptEvent(user, item);
			base.RaiseLocalEvent<PickupAttemptEvent>(user, userEv, false);
			if (userEv.Cancelled)
			{
				return false;
			}
			GettingPickedUpAttemptEvent itemEv = new GettingPickedUpAttemptEvent(user, item);
			base.RaiseLocalEvent<GettingPickedUpAttemptEvent>(item, itemEv, false);
			return !itemEv.Cancelled;
		}

		// Token: 0x06001772 RID: 6002 RVA: 0x0004C32C File Offset: 0x0004A52C
		public bool CanEmote(EntityUid uid)
		{
			EmoteAttemptEvent ev = new EmoteAttemptEvent(uid);
			base.RaiseLocalEvent<EmoteAttemptEvent>(uid, ev, true);
			return !ev.Cancelled;
		}

		// Token: 0x06001773 RID: 6003 RVA: 0x0004C354 File Offset: 0x0004A554
		public bool CanAttack(EntityUid uid, EntityUid? target = null)
		{
			IContainer outerContainer;
			this._container.TryGetOuterContainer(uid, base.Transform(uid), ref outerContainer);
			if (target != null)
			{
				EntityUid? entityUid = target;
				if (entityUid != ((outerContainer != null) ? new EntityUid?(outerContainer.Owner) : null) && this._container.IsEntityInContainer(uid, null))
				{
					CanAttackFromContainerEvent containerEv = new CanAttackFromContainerEvent(uid, target);
					base.RaiseLocalEvent<CanAttackFromContainerEvent>(uid, containerEv, false);
					return containerEv.CanAttack;
				}
			}
			AttackAttemptEvent ev = new AttackAttemptEvent(uid, target);
			base.RaiseLocalEvent<AttackAttemptEvent>(uid, ev, false);
			if (ev.Cancelled)
			{
				return false;
			}
			if (target != null)
			{
				GettingAttackedAttemptEvent tev = default(GettingAttackedAttemptEvent);
				base.RaiseLocalEvent<GettingAttackedAttemptEvent>(target.Value, ref tev, false);
				return !tev.Cancelled;
			}
			return true;
		}

		// Token: 0x06001774 RID: 6004 RVA: 0x0004C444 File Offset: 0x0004A644
		public bool CanChangeDirection(EntityUid uid)
		{
			ChangeDirectionAttemptEvent ev = new ChangeDirectionAttemptEvent(uid);
			base.RaiseLocalEvent<ChangeDirectionAttemptEvent>(uid, ev, false);
			return !ev.Cancelled;
		}

		// Token: 0x06001775 RID: 6005 RVA: 0x0004C46C File Offset: 0x0004A66C
		public bool CanShiver(EntityUid uid)
		{
			ShiverAttemptEvent ev = new ShiverAttemptEvent(uid);
			base.RaiseLocalEvent<ShiverAttemptEvent>(uid, ev, false);
			return !ev.Cancelled;
		}

		// Token: 0x06001776 RID: 6006 RVA: 0x0004C494 File Offset: 0x0004A694
		public bool CanSweat(EntityUid uid)
		{
			SweatAttemptEvent ev = new SweatAttemptEvent(uid);
			base.RaiseLocalEvent<SweatAttemptEvent>(uid, ev, false);
			return !ev.Cancelled;
		}

		// Token: 0x04001740 RID: 5952
		[Dependency]
		private readonly SharedContainerSystem _container;
	}
}
