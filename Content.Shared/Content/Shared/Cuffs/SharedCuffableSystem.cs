using System;
using System.Runtime.CompilerServices;
using Content.Shared.ActionBlocker;
using Content.Shared.Alert;
using Content.Shared.Cuffs.Components;
using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory.Events;
using Content.Shared.Item;
using Content.Shared.Movement.Events;
using Content.Shared.Physics.Pull;
using Content.Shared.Pulling.Components;
using Content.Shared.Pulling.Events;
using Content.Shared.Rejuvenate;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Cuffs
{
	// Token: 0x02000548 RID: 1352
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedCuffableSystem : EntitySystem
	{
		// Token: 0x06001070 RID: 4208 RVA: 0x00035D80 File Offset: 0x00033F80
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<SharedCuffableComponent, EntRemovedFromContainerMessage>(new ComponentEventHandler<SharedCuffableComponent, EntRemovedFromContainerMessage>(this.OnCuffCountChanged), null, null);
			base.SubscribeLocalEvent<SharedCuffableComponent, EntInsertedIntoContainerMessage>(new ComponentEventHandler<SharedCuffableComponent, EntInsertedIntoContainerMessage>(this.OnCuffCountChanged), null, null);
			base.SubscribeLocalEvent<SharedCuffableComponent, RejuvenateEvent>(new ComponentEventHandler<SharedCuffableComponent, RejuvenateEvent>(this.OnRejuvenate), null, null);
			base.SubscribeLocalEvent<SharedCuffableComponent, StopPullingEvent>(new ComponentEventHandler<SharedCuffableComponent, StopPullingEvent>(this.HandleStopPull), null, null);
			base.SubscribeLocalEvent<SharedCuffableComponent, UpdateCanMoveEvent>(new ComponentEventHandler<SharedCuffableComponent, UpdateCanMoveEvent>(this.HandleMoveAttempt), null, null);
			base.SubscribeLocalEvent<SharedCuffableComponent, AttackAttemptEvent>(new ComponentEventHandler<SharedCuffableComponent, AttackAttemptEvent>(this.CheckAct), null, null);
			base.SubscribeLocalEvent<SharedCuffableComponent, UseAttemptEvent>(new ComponentEventHandler<SharedCuffableComponent, UseAttemptEvent>(this.CheckAct), null, null);
			base.SubscribeLocalEvent<SharedCuffableComponent, InteractionAttemptEvent>(new ComponentEventHandler<SharedCuffableComponent, InteractionAttemptEvent>(this.CheckAct), null, null);
			base.SubscribeLocalEvent<SharedCuffableComponent, IsEquippingAttemptEvent>(new ComponentEventHandler<SharedCuffableComponent, IsEquippingAttemptEvent>(this.OnEquipAttempt), null, null);
			base.SubscribeLocalEvent<SharedCuffableComponent, IsUnequippingAttemptEvent>(new ComponentEventHandler<SharedCuffableComponent, IsUnequippingAttemptEvent>(this.OnUnequipAttempt), null, null);
			base.SubscribeLocalEvent<SharedCuffableComponent, DropAttemptEvent>(new ComponentEventHandler<SharedCuffableComponent, DropAttemptEvent>(this.CheckAct), null, null);
			base.SubscribeLocalEvent<SharedCuffableComponent, PickupAttemptEvent>(new ComponentEventHandler<SharedCuffableComponent, PickupAttemptEvent>(this.CheckAct), null, null);
			base.SubscribeLocalEvent<SharedCuffableComponent, BeingPulledAttemptEvent>(new ComponentEventHandler<SharedCuffableComponent, BeingPulledAttemptEvent>(this.OnBeingPulledAttempt), null, null);
			base.SubscribeLocalEvent<SharedCuffableComponent, PullStartedMessage>(new ComponentEventHandler<SharedCuffableComponent, PullStartedMessage>(this.OnPull), null, null);
			base.SubscribeLocalEvent<SharedCuffableComponent, PullStoppedMessage>(new ComponentEventHandler<SharedCuffableComponent, PullStoppedMessage>(this.OnPull), null, null);
		}

		// Token: 0x06001071 RID: 4209 RVA: 0x00035EC0 File Offset: 0x000340C0
		private void OnRejuvenate(EntityUid uid, SharedCuffableComponent component, RejuvenateEvent args)
		{
			this._container.EmptyContainer(component.Container, true, null, true, null);
		}

		// Token: 0x06001072 RID: 4210 RVA: 0x00035EEA File Offset: 0x000340EA
		private void OnCuffCountChanged(EntityUid uid, SharedCuffableComponent component, ContainerModifiedMessage args)
		{
			if (args.Container == component.Container)
			{
				this.UpdateCuffState(uid, component);
			}
		}

		// Token: 0x06001073 RID: 4211 RVA: 0x00035F04 File Offset: 0x00034104
		public void UpdateCuffState(EntityUid uid, SharedCuffableComponent component)
		{
			SharedHandsComponent hands;
			bool canInteract = base.TryComp<SharedHandsComponent>(uid, ref hands) && hands.Hands.Count > component.CuffedHandCount;
			if (canInteract == component.CanStillInteract)
			{
				return;
			}
			component.CanStillInteract = canInteract;
			base.Dirty(component, null);
			this._blocker.UpdateCanMove(uid, null);
			if (component.CanStillInteract)
			{
				this._alerts.ClearAlert(uid, AlertType.Handcuffed);
			}
			else
			{
				this._alerts.ShowAlert(uid, AlertType.Handcuffed, null, null);
			}
			CuffedStateChangeEvent ev = default(CuffedStateChangeEvent);
			base.RaiseLocalEvent<CuffedStateChangeEvent>(uid, ref ev, false);
		}

		// Token: 0x06001074 RID: 4212 RVA: 0x00035FA8 File Offset: 0x000341A8
		private void OnBeingPulledAttempt(EntityUid uid, SharedCuffableComponent component, BeingPulledAttemptEvent args)
		{
			SharedPullableComponent pullable;
			if (!base.TryComp<SharedPullableComponent>(uid, ref pullable))
			{
				return;
			}
			if (pullable.Puller != null && !component.CanStillInteract)
			{
				args.Cancel();
			}
		}

		// Token: 0x06001075 RID: 4213 RVA: 0x00035FDF File Offset: 0x000341DF
		private void OnPull(EntityUid uid, SharedCuffableComponent component, PullMessage args)
		{
			if (!component.CanStillInteract)
			{
				this._blocker.UpdateCanMove(uid, null);
			}
		}

		// Token: 0x06001076 RID: 4214 RVA: 0x00035FF8 File Offset: 0x000341F8
		private void HandleMoveAttempt(EntityUid uid, SharedCuffableComponent component, UpdateCanMoveEvent args)
		{
			SharedPullableComponent pullable;
			if (component.CanStillInteract || !this.EntityManager.TryGetComponent<SharedPullableComponent>(uid, ref pullable) || !pullable.BeingPulled)
			{
				return;
			}
			args.Cancel();
		}

		// Token: 0x06001077 RID: 4215 RVA: 0x0003602C File Offset: 0x0003422C
		private void HandleStopPull(EntityUid uid, SharedCuffableComponent component, StopPullingEvent args)
		{
			if (args.User == null || !this.EntityManager.EntityExists(args.User.Value))
			{
				return;
			}
			if (args.User.Value == component.Owner && !component.CanStillInteract)
			{
				args.Cancel();
			}
		}

		// Token: 0x06001078 RID: 4216 RVA: 0x0003608E File Offset: 0x0003428E
		private void CheckAct(EntityUid uid, SharedCuffableComponent component, CancellableEntityEventArgs args)
		{
			if (!component.CanStillInteract)
			{
				args.Cancel();
			}
		}

		// Token: 0x06001079 RID: 4217 RVA: 0x0003609E File Offset: 0x0003429E
		private void OnEquipAttempt(EntityUid uid, SharedCuffableComponent component, IsEquippingAttemptEvent args)
		{
			if (args.Equipee == uid)
			{
				this.CheckAct(uid, component, args);
			}
		}

		// Token: 0x0600107A RID: 4218 RVA: 0x000360B7 File Offset: 0x000342B7
		private void OnUnequipAttempt(EntityUid uid, SharedCuffableComponent component, IsUnequippingAttemptEvent args)
		{
			if (args.Unequipee == uid)
			{
				this.CheckAct(uid, component, args);
			}
		}

		// Token: 0x04000F79 RID: 3961
		[Dependency]
		private readonly ActionBlockerSystem _blocker;

		// Token: 0x04000F7A RID: 3962
		[Dependency]
		private readonly SharedContainerSystem _container;

		// Token: 0x04000F7B RID: 3963
		[Dependency]
		private readonly AlertsSystem _alerts;
	}
}
