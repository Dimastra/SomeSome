using System;
using System.Runtime.CompilerServices;
using Content.Shared.CombatMode;
using Content.Shared.Targeting;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;

namespace Content.Client.CombatMode
{
	// Token: 0x020003B4 RID: 948
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class CombatModeSystem : SharedCombatModeSystem
	{
		// Token: 0x06001787 RID: 6023 RVA: 0x00086F63 File Offset: 0x00085163
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<SharedCombatModeComponent, ComponentHandleState>(new ComponentEventRefHandler<SharedCombatModeComponent, ComponentHandleState>(this.OnHandleState), null, null);
		}

		// Token: 0x06001788 RID: 6024 RVA: 0x00086F80 File Offset: 0x00085180
		private void OnHandleState(EntityUid uid, SharedCombatModeComponent component, ref ComponentHandleState args)
		{
			SharedCombatModeSystem.CombatModeComponentState combatModeComponentState = args.Current as SharedCombatModeSystem.CombatModeComponentState;
			if (combatModeComponentState == null)
			{
				return;
			}
			component.IsInCombatMode = combatModeComponentState.IsInCombatMode;
			component.ActiveZone = combatModeComponentState.TargetingZone;
		}

		// Token: 0x06001789 RID: 6025 RVA: 0x00086FB5 File Offset: 0x000851B5
		public override void Shutdown()
		{
			CommandBinds.Unregister<CombatModeSystem>();
			base.Shutdown();
		}

		// Token: 0x0600178A RID: 6026 RVA: 0x00086FC4 File Offset: 0x000851C4
		public bool IsInCombatMode()
		{
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			EntityUid? entityUid = (localPlayer != null) ? localPlayer.ControlledEntity : null;
			return entityUid != null && base.IsInCombatMode(new EntityUid?(entityUid.Value), null);
		}

		// Token: 0x0600178B RID: 6027 RVA: 0x0008700F File Offset: 0x0008520F
		private void OnTargetingZoneChanged(TargetingZone obj)
		{
			this.EntityManager.RaisePredictiveEvent<CombatModeSystemMessages.SetTargetZoneMessage>(new CombatModeSystemMessages.SetTargetZoneMessage(obj));
		}

		// Token: 0x04000C05 RID: 3077
		[Dependency]
		private readonly IPlayerManager _playerManager;
	}
}
