using System;
using System.Runtime.CompilerServices;
using Content.Client.ContextMenu.UI;
using Content.Shared.CombatMode;
using Content.Shared.Targeting;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.CombatMode
{
	// Token: 0x020003B3 RID: 947
	[RegisterComponent]
	[ComponentReference(typeof(SharedCombatModeComponent))]
	public sealed class CombatModeComponent : SharedCombatModeComponent
	{
		// Token: 0x170004D8 RID: 1240
		// (get) Token: 0x06001781 RID: 6017 RVA: 0x00086ECD File Offset: 0x000850CD
		// (set) Token: 0x06001782 RID: 6018 RVA: 0x00086ED5 File Offset: 0x000850D5
		public override bool IsInCombatMode
		{
			get
			{
				return base.IsInCombatMode;
			}
			set
			{
				base.IsInCombatMode = value;
				this.UpdateHud();
			}
		}

		// Token: 0x170004D9 RID: 1241
		// (get) Token: 0x06001783 RID: 6019 RVA: 0x00086EE4 File Offset: 0x000850E4
		// (set) Token: 0x06001784 RID: 6020 RVA: 0x00086EEC File Offset: 0x000850EC
		public override TargetingZone ActiveZone
		{
			get
			{
				return base.ActiveZone;
			}
			set
			{
				base.ActiveZone = value;
				this.UpdateHud();
			}
		}

		// Token: 0x06001785 RID: 6021 RVA: 0x00086EFC File Offset: 0x000850FC
		private void UpdateHud()
		{
			EntityUid owner = base.Owner;
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			if (owner != ((localPlayer != null) ? localPlayer.ControlledEntity : null))
			{
				return;
			}
			IoCManager.Resolve<IUserInterfaceManager>().GetUIController<ContextMenuUIController>().Close();
		}

		// Token: 0x04000C04 RID: 3076
		[Nullable(1)]
		[Dependency]
		private readonly IPlayerManager _playerManager;
	}
}
