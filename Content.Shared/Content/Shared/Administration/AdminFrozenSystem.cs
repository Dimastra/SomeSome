using System;
using System.Runtime.CompilerServices;
using Content.Shared.ActionBlocker;
using Content.Shared.Interaction.Events;
using Content.Shared.Item;
using Content.Shared.Movement.Events;
using Content.Shared.Physics.Pull;
using Content.Shared.Pulling;
using Content.Shared.Pulling.Components;
using Content.Shared.Throwing;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Administration
{
	// Token: 0x02000733 RID: 1843
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AdminFrozenSystem : EntitySystem
	{
		// Token: 0x0600164F RID: 5711 RVA: 0x000490F4 File Offset: 0x000472F4
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<AdminFrozenComponent, UseAttemptEvent>(new ComponentEventHandler<AdminFrozenComponent, UseAttemptEvent>(this.OnAttempt), null, null);
			base.SubscribeLocalEvent<AdminFrozenComponent, PickupAttemptEvent>(new ComponentEventHandler<AdminFrozenComponent, PickupAttemptEvent>(this.OnAttempt), null, null);
			base.SubscribeLocalEvent<AdminFrozenComponent, ThrowAttemptEvent>(new ComponentEventHandler<AdminFrozenComponent, ThrowAttemptEvent>(this.OnAttempt), null, null);
			base.SubscribeLocalEvent<AdminFrozenComponent, InteractionAttemptEvent>(new ComponentEventHandler<AdminFrozenComponent, InteractionAttemptEvent>(this.OnAttempt), null, null);
			base.SubscribeLocalEvent<AdminFrozenComponent, ComponentStartup>(new ComponentEventHandler<AdminFrozenComponent, ComponentStartup>(this.OnStartup), null, null);
			base.SubscribeLocalEvent<AdminFrozenComponent, ComponentShutdown>(new ComponentEventHandler<AdminFrozenComponent, ComponentShutdown>(this.UpdateCanMove), null, null);
			base.SubscribeLocalEvent<AdminFrozenComponent, UpdateCanMoveEvent>(new ComponentEventHandler<AdminFrozenComponent, UpdateCanMoveEvent>(this.OnUpdateCanMove), null, null);
			base.SubscribeLocalEvent<AdminFrozenComponent, PullAttemptEvent>(new ComponentEventHandler<AdminFrozenComponent, PullAttemptEvent>(this.OnPullAttempt), null, null);
			base.SubscribeLocalEvent<AdminFrozenComponent, AttackAttemptEvent>(new ComponentEventHandler<AdminFrozenComponent, AttackAttemptEvent>(this.OnAttempt), null, null);
			base.SubscribeLocalEvent<AdminFrozenComponent, ChangeDirectionAttemptEvent>(new ComponentEventHandler<AdminFrozenComponent, ChangeDirectionAttemptEvent>(this.OnAttempt), null, null);
		}

		// Token: 0x06001650 RID: 5712 RVA: 0x000491CF File Offset: 0x000473CF
		private void OnAttempt(EntityUid uid, AdminFrozenComponent component, CancellableEntityEventArgs args)
		{
			args.Cancel();
		}

		// Token: 0x06001651 RID: 5713 RVA: 0x000491D7 File Offset: 0x000473D7
		private void OnPullAttempt(EntityUid uid, AdminFrozenComponent component, PullAttemptEvent args)
		{
			args.Cancelled = true;
		}

		// Token: 0x06001652 RID: 5714 RVA: 0x000491E0 File Offset: 0x000473E0
		private void OnStartup(EntityUid uid, AdminFrozenComponent component, ComponentStartup args)
		{
			SharedPullableComponent pullable;
			if (base.TryComp<SharedPullableComponent>(uid, ref pullable))
			{
				this._pulling.TryStopPull(pullable, null);
			}
			this.UpdateCanMove(uid, component, args);
		}

		// Token: 0x06001653 RID: 5715 RVA: 0x00049217 File Offset: 0x00047417
		private void OnUpdateCanMove(EntityUid uid, AdminFrozenComponent component, UpdateCanMoveEvent args)
		{
			if (component.LifeStage > 6)
			{
				return;
			}
			args.Cancel();
		}

		// Token: 0x06001654 RID: 5716 RVA: 0x00049229 File Offset: 0x00047429
		private void UpdateCanMove(EntityUid uid, AdminFrozenComponent component, EntityEventArgs args)
		{
			this._blocker.UpdateCanMove(uid, null);
		}

		// Token: 0x040016A9 RID: 5801
		[Dependency]
		private readonly ActionBlockerSystem _blocker;

		// Token: 0x040016AA RID: 5802
		[Dependency]
		private readonly SharedPullingSystem _pulling;
	}
}
