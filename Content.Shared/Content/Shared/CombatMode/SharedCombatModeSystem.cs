using System;
using System.Runtime.CompilerServices;
using Content.Shared.Actions;
using Content.Shared.Actions.ActionTypes;
using Content.Shared.Targeting;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.CombatMode
{
	// Token: 0x0200059D RID: 1437
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedCombatModeSystem : EntitySystem
	{
		// Token: 0x06001189 RID: 4489 RVA: 0x00039388 File Offset: 0x00037588
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<SharedCombatModeComponent, ComponentStartup>(new ComponentEventHandler<SharedCombatModeComponent, ComponentStartup>(this.OnStartup), null, null);
			base.SubscribeLocalEvent<SharedCombatModeComponent, ComponentShutdown>(new ComponentEventHandler<SharedCombatModeComponent, ComponentShutdown>(this.OnShutdown), null, null);
			base.SubscribeLocalEvent<SharedCombatModeComponent, ToggleCombatActionEvent>(new ComponentEventHandler<SharedCombatModeComponent, ToggleCombatActionEvent>(this.OnActionPerform), null, null);
		}

		// Token: 0x0600118A RID: 4490 RVA: 0x000393D8 File Offset: 0x000375D8
		private void OnStartup(EntityUid uid, SharedCombatModeComponent component, ComponentStartup args)
		{
			InstantActionPrototype toggleProto;
			if (component.CombatToggleAction == null && this._protoMan.TryIndex<InstantActionPrototype>(component.CombatToggleActionId, ref toggleProto))
			{
				component.CombatToggleAction = new InstantAction(toggleProto);
			}
			if (component.CombatToggleAction != null)
			{
				this._actionsSystem.AddAction(uid, component.CombatToggleAction, null, null, true);
			}
		}

		// Token: 0x0600118B RID: 4491 RVA: 0x00039433 File Offset: 0x00037633
		private void OnShutdown(EntityUid uid, SharedCombatModeComponent component, ComponentShutdown args)
		{
			if (component.CombatToggleAction != null)
			{
				this._actionsSystem.RemoveAction(uid, component.CombatToggleAction, null);
			}
		}

		// Token: 0x0600118C RID: 4492 RVA: 0x00039450 File Offset: 0x00037650
		[NullableContext(2)]
		public bool IsInCombatMode(EntityUid? entity, SharedCombatModeComponent component = null)
		{
			return entity != null && base.Resolve<SharedCombatModeComponent>(entity.Value, ref component, false) && component.IsInCombatMode;
		}

		// Token: 0x0600118D RID: 4493 RVA: 0x00039475 File Offset: 0x00037675
		private void OnActionPerform(EntityUid uid, SharedCombatModeComponent component, ToggleCombatActionEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			component.IsInCombatMode = !component.IsInCombatMode;
			args.Handled = true;
		}

		// Token: 0x04001035 RID: 4149
		[Dependency]
		private readonly IPrototypeManager _protoMan;

		// Token: 0x04001036 RID: 4150
		[Dependency]
		private readonly SharedActionsSystem _actionsSystem;

		// Token: 0x0200084E RID: 2126
		[NullableContext(0)]
		[NetSerializable]
		[Serializable]
		protected sealed class CombatModeComponentState : ComponentState
		{
			// Token: 0x17000526 RID: 1318
			// (get) Token: 0x0600194D RID: 6477 RVA: 0x0004FD7F File Offset: 0x0004DF7F
			public bool IsInCombatMode { get; }

			// Token: 0x17000527 RID: 1319
			// (get) Token: 0x0600194E RID: 6478 RVA: 0x0004FD87 File Offset: 0x0004DF87
			public TargetingZone TargetingZone { get; }

			// Token: 0x0600194F RID: 6479 RVA: 0x0004FD8F File Offset: 0x0004DF8F
			public CombatModeComponentState(bool isInCombatMode, TargetingZone targetingZone)
			{
				this.IsInCombatMode = isInCombatMode;
				this.TargetingZone = targetingZone;
			}
		}
	}
}
