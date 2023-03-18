using System;
using System.Runtime.CompilerServices;
using Content.Shared.Administration.Logs;
using Content.Shared.Alert;
using Content.Shared.Database;
using Content.Shared.Hands;
using Content.Shared.Movement.Systems;
using Content.Shared.Physics.Pull;
using Content.Shared.Pulling.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Pulling.Systems
{
	// Token: 0x02000239 RID: 569
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SharedPullerSystem : EntitySystem
	{
		// Token: 0x0600065E RID: 1630 RVA: 0x00016E24 File Offset: 0x00015024
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<SharedPullerComponent, PullStartedMessage>(new ComponentEventHandler<SharedPullerComponent, PullStartedMessage>(this.PullerHandlePullStarted), null, null);
			base.SubscribeLocalEvent<SharedPullerComponent, PullStoppedMessage>(new ComponentEventHandler<SharedPullerComponent, PullStoppedMessage>(this.PullerHandlePullStopped), null, null);
			base.SubscribeLocalEvent<SharedPullerComponent, VirtualItemDeletedEvent>(new ComponentEventHandler<SharedPullerComponent, VirtualItemDeletedEvent>(this.OnVirtualItemDeleted), null, null);
			base.SubscribeLocalEvent<SharedPullerComponent, RefreshMovementSpeedModifiersEvent>(new ComponentEventHandler<SharedPullerComponent, RefreshMovementSpeedModifiersEvent>(this.OnRefreshMovespeed), null, null);
			base.SubscribeLocalEvent<SharedPullerComponent, ComponentShutdown>(new ComponentEventHandler<SharedPullerComponent, ComponentShutdown>(this.OnPullerShutdown), null, null);
		}

		// Token: 0x0600065F RID: 1631 RVA: 0x00016E9B File Offset: 0x0001509B
		private void OnPullerShutdown(EntityUid uid, SharedPullerComponent component, ComponentShutdown args)
		{
			this._why.ForceDisconnectPuller(component);
		}

		// Token: 0x06000660 RID: 1632 RVA: 0x00016EAC File Offset: 0x000150AC
		private void OnVirtualItemDeleted(EntityUid uid, SharedPullerComponent component, VirtualItemDeletedEvent args)
		{
			if (component.Pulling == null)
			{
				return;
			}
			EntityUid? pulling = component.Pulling;
			EntityUid blockingEntity = args.BlockingEntity;
			SharedPullableComponent comp;
			if (pulling != null && (pulling == null || pulling.GetValueOrDefault() == blockingEntity) && this.EntityManager.TryGetComponent<SharedPullableComponent>(args.BlockingEntity, ref comp))
			{
				this._pullSystem.TryStopPull(comp, new EntityUid?(uid));
			}
		}

		// Token: 0x06000661 RID: 1633 RVA: 0x00016F28 File Offset: 0x00015128
		private void PullerHandlePullStarted(EntityUid uid, SharedPullerComponent component, PullStartedMessage args)
		{
			if (args.Puller.Owner != uid)
			{
				return;
			}
			this._alertsSystem.ShowAlert(component.Owner, AlertType.Pulling, null, null);
			this.RefreshMovementSpeed(component);
		}

		// Token: 0x06000662 RID: 1634 RVA: 0x00016F78 File Offset: 0x00015178
		private void PullerHandlePullStopped(EntityUid uid, SharedPullerComponent component, PullStoppedMessage args)
		{
			if (args.Puller.Owner != uid)
			{
				return;
			}
			EntityUid euid = component.Owner;
			if (this._alertsSystem.IsShowingAlert(euid, AlertType.Pulling))
			{
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.Action;
				LogImpact impact = LogImpact.Low;
				LogStringHandler logStringHandler = new LogStringHandler(17, 2);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(euid), "user", "ToPrettyString(euid)");
				logStringHandler.AppendLiteral(" stopped pulling ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.Pulled.Owner), "target", "ToPrettyString(args.Pulled.Owner)");
				adminLogger.Add(type, impact, ref logStringHandler);
			}
			this._alertsSystem.ClearAlert(euid, AlertType.Pulling);
			this.RefreshMovementSpeed(component);
		}

		// Token: 0x06000663 RID: 1635 RVA: 0x00017025 File Offset: 0x00015225
		private void OnRefreshMovespeed(EntityUid uid, SharedPullerComponent component, RefreshMovementSpeedModifiersEvent args)
		{
			args.ModifySpeed(component.WalkSpeedModifier, component.SprintSpeedModifier);
		}

		// Token: 0x06000664 RID: 1636 RVA: 0x00017039 File Offset: 0x00015239
		private void RefreshMovementSpeed(SharedPullerComponent component)
		{
			this._movementSpeedModifierSystem.RefreshMovementSpeedModifiers(component.Owner, null);
		}

		// Token: 0x04000661 RID: 1633
		[Dependency]
		private readonly SharedPullingStateManagementSystem _why;

		// Token: 0x04000662 RID: 1634
		[Dependency]
		private readonly SharedPullingSystem _pullSystem;

		// Token: 0x04000663 RID: 1635
		[Dependency]
		private readonly MovementSpeedModifierSystem _movementSpeedModifierSystem;

		// Token: 0x04000664 RID: 1636
		[Dependency]
		private readonly AlertsSystem _alertsSystem;

		// Token: 0x04000665 RID: 1637
		[Dependency]
		private readonly ISharedAdminLogManager _adminLogger;
	}
}
