using System;
using System.Runtime.CompilerServices;
using Content.Shared.ActionBlocker;
using Content.Shared.Actions;
using Content.Shared.Hands;
using Content.Shared.Interaction.Events;
using Content.Shared.Item;
using Content.Shared.Movement.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.PAI
{
	// Token: 0x020002A7 RID: 679
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedPAISystem : EntitySystem
	{
		// Token: 0x06000799 RID: 1945 RVA: 0x00019BCC File Offset: 0x00017DCC
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<PAIComponent, UseAttemptEvent>(new ComponentEventHandler<PAIComponent, UseAttemptEvent>(this.OnUseAttempt), null, null);
			base.SubscribeLocalEvent<PAIComponent, InteractionAttemptEvent>(new ComponentEventHandler<PAIComponent, InteractionAttemptEvent>(this.OnInteractAttempt), null, null);
			base.SubscribeLocalEvent<PAIComponent, DropAttemptEvent>(new ComponentEventHandler<PAIComponent, DropAttemptEvent>(this.OnDropAttempt), null, null);
			base.SubscribeLocalEvent<PAIComponent, PickupAttemptEvent>(new ComponentEventHandler<PAIComponent, PickupAttemptEvent>(this.OnPickupAttempt), null, null);
			base.SubscribeLocalEvent<PAIComponent, UpdateCanMoveEvent>(new ComponentEventHandler<PAIComponent, UpdateCanMoveEvent>(this.OnMoveAttempt), null, null);
			base.SubscribeLocalEvent<PAIComponent, ChangeDirectionAttemptEvent>(new ComponentEventHandler<PAIComponent, ChangeDirectionAttemptEvent>(this.OnChangeDirectionAttempt), null, null);
			base.SubscribeLocalEvent<PAIComponent, ComponentStartup>(new ComponentEventHandler<PAIComponent, ComponentStartup>(this.OnStartup), null, null);
			base.SubscribeLocalEvent<PAIComponent, ComponentShutdown>(new ComponentEventHandler<PAIComponent, ComponentShutdown>(this.OnShutdown), null, null);
		}

		// Token: 0x0600079A RID: 1946 RVA: 0x00019C80 File Offset: 0x00017E80
		private void OnStartup(EntityUid uid, PAIComponent component, ComponentStartup args)
		{
			this._blocker.UpdateCanMove(uid, null);
			if (component.MidiAction != null)
			{
				this._actionsSystem.AddAction(uid, component.MidiAction, null, null, true);
			}
		}

		// Token: 0x0600079B RID: 1947 RVA: 0x00019CC0 File Offset: 0x00017EC0
		private void OnShutdown(EntityUid uid, PAIComponent component, ComponentShutdown args)
		{
			this._blocker.UpdateCanMove(uid, null);
			if (component.MidiAction != null)
			{
				this._actionsSystem.RemoveAction(uid, component.MidiAction, null);
			}
		}

		// Token: 0x0600079C RID: 1948 RVA: 0x00019CEB File Offset: 0x00017EEB
		private void OnMoveAttempt(EntityUid uid, PAIComponent component, UpdateCanMoveEvent args)
		{
			if (component.LifeStage > 6)
			{
				return;
			}
			args.Cancel();
		}

		// Token: 0x0600079D RID: 1949 RVA: 0x00019CFD File Offset: 0x00017EFD
		private void OnChangeDirectionAttempt(EntityUid uid, PAIComponent component, ChangeDirectionAttemptEvent args)
		{
			args.Cancel();
		}

		// Token: 0x0600079E RID: 1950 RVA: 0x00019D05 File Offset: 0x00017F05
		private void OnUseAttempt(EntityUid uid, PAIComponent component, UseAttemptEvent args)
		{
			args.Cancel();
		}

		// Token: 0x0600079F RID: 1951 RVA: 0x00019D0D File Offset: 0x00017F0D
		private void OnInteractAttempt(EntityUid uid, PAIComponent component, InteractionAttemptEvent args)
		{
			args.Cancel();
		}

		// Token: 0x060007A0 RID: 1952 RVA: 0x00019D15 File Offset: 0x00017F15
		private void OnDropAttempt(EntityUid uid, PAIComponent component, DropAttemptEvent args)
		{
			args.Cancel();
		}

		// Token: 0x060007A1 RID: 1953 RVA: 0x00019D1D File Offset: 0x00017F1D
		private void OnPickupAttempt(EntityUid uid, PAIComponent component, PickupAttemptEvent args)
		{
			args.Cancel();
		}

		// Token: 0x040007B0 RID: 1968
		[Dependency]
		private readonly ActionBlockerSystem _blocker;

		// Token: 0x040007B1 RID: 1969
		[Dependency]
		private readonly SharedActionsSystem _actionsSystem;
	}
}
