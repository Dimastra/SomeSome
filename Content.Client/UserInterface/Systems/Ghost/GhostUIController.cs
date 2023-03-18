using System;
using System.Runtime.CompilerServices;
using Content.Client.Ghost;
using Content.Client.UserInterface.Systems.Ghost.Controls;
using Content.Client.UserInterface.Systems.Ghost.Widgets;
using Content.Shared.Ghost;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.UserInterface.Systems.Ghost
{
	// Token: 0x02000085 RID: 133
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GhostUIController : UIController, IOnSystemChanged<GhostSystem>, IOnSystemLoaded<GhostSystem>, IOnSystemUnloaded<GhostSystem>
	{
		// Token: 0x17000080 RID: 128
		// (get) Token: 0x060002F7 RID: 759 RVA: 0x00012DA7 File Offset: 0x00010FA7
		[Nullable(2)]
		private GhostGui Gui
		{
			[NullableContext(2)]
			get
			{
				return this.UIManager.GetActiveUIWidgetOrNull<GhostGui>();
			}
		}

		// Token: 0x060002F8 RID: 760 RVA: 0x00012DB4 File Offset: 0x00010FB4
		public void OnSystemLoaded(GhostSystem system)
		{
			system.PlayerRemoved += this.OnPlayerRemoved;
			system.PlayerUpdated += this.OnPlayerUpdated;
			system.PlayerAttached += this.OnPlayerAttached;
			system.PlayerDetached += this.OnPlayerDetached;
			system.GhostWarpsResponse += this.OnWarpsResponse;
			system.GhostRoleCountUpdated += this.OnRoleCountUpdated;
		}

		// Token: 0x060002F9 RID: 761 RVA: 0x00012E30 File Offset: 0x00011030
		public void OnSystemUnloaded(GhostSystem system)
		{
			system.PlayerRemoved -= this.OnPlayerRemoved;
			system.PlayerUpdated -= this.OnPlayerUpdated;
			system.PlayerAttached -= this.OnPlayerAttached;
			system.PlayerDetached -= this.OnPlayerDetached;
			system.GhostWarpsResponse -= this.OnWarpsResponse;
			system.GhostRoleCountUpdated -= this.OnRoleCountUpdated;
		}

		// Token: 0x060002FA RID: 762 RVA: 0x00012EAC File Offset: 0x000110AC
		public void UpdateGui()
		{
			if (this.Gui == null)
			{
				return;
			}
			Control gui = this.Gui;
			GhostSystem system = this._system;
			gui.Visible = (system != null && system.IsGhost);
			GhostGui gui2 = this.Gui;
			GhostSystem system2 = this._system;
			int? roles = (system2 != null) ? new int?(system2.AvailableGhostRoleCount) : null;
			GhostSystem system3 = this._system;
			bool? canReturnToBody;
			if (system3 == null)
			{
				canReturnToBody = null;
			}
			else
			{
				GhostComponent player = system3.Player;
				canReturnToBody = ((player != null) ? new bool?(player.CanReturnToBody) : null);
			}
			gui2.Update(roles, canReturnToBody);
		}

		// Token: 0x060002FB RID: 763 RVA: 0x00012F3C File Offset: 0x0001113C
		private void OnPlayerRemoved(GhostComponent component)
		{
			GhostGui gui = this.Gui;
			if (gui == null)
			{
				return;
			}
			gui.Hide();
		}

		// Token: 0x060002FC RID: 764 RVA: 0x00012F4E File Offset: 0x0001114E
		private void OnPlayerUpdated(GhostComponent component)
		{
			this.UpdateGui();
		}

		// Token: 0x060002FD RID: 765 RVA: 0x00012F56 File Offset: 0x00011156
		private void OnPlayerAttached(GhostComponent component)
		{
			if (this.Gui == null)
			{
				return;
			}
			this.Gui.Visible = true;
			this.UpdateGui();
		}

		// Token: 0x060002FE RID: 766 RVA: 0x00012F3C File Offset: 0x0001113C
		private void OnPlayerDetached()
		{
			GhostGui gui = this.Gui;
			if (gui == null)
			{
				return;
			}
			gui.Hide();
		}

		// Token: 0x060002FF RID: 767 RVA: 0x00012F74 File Offset: 0x00011174
		private void OnWarpsResponse(GhostWarpsResponseEvent msg)
		{
			GhostGui gui = this.Gui;
			GhostTargetWindow ghostTargetWindow = (gui != null) ? gui.TargetWindow : null;
			if (ghostTargetWindow == null)
			{
				return;
			}
			ghostTargetWindow.UpdateWarps(msg.Warps);
			ghostTargetWindow.Populate();
		}

		// Token: 0x06000300 RID: 768 RVA: 0x00012F4E File Offset: 0x0001114E
		private void OnRoleCountUpdated(GhostUpdateGhostRoleCountEvent msg)
		{
			this.UpdateGui();
		}

		// Token: 0x06000301 RID: 769 RVA: 0x00012FAC File Offset: 0x000111AC
		private void OnWarpClicked(EntityUid player)
		{
			GhostWarpToTargetRequestEvent ghostWarpToTargetRequestEvent = new GhostWarpToTargetRequestEvent(player);
			this._net.SendSystemNetworkMessage(ghostWarpToTargetRequestEvent, true);
		}

		// Token: 0x06000302 RID: 770 RVA: 0x00012FD0 File Offset: 0x000111D0
		public void LoadGui()
		{
			if (this.Gui == null)
			{
				return;
			}
			this.Gui.RequestWarpsPressed += this.RequestWarps;
			this.Gui.ReturnToBodyPressed += this.ReturnToBody;
			this.Gui.GhostRolesPressed += this.GhostRolesPressed;
			this.Gui.TargetWindow.WarpClicked += this.OnWarpClicked;
			this.Gui.ReturnToRoundPressed += this.ReturnToRound;
			this.UpdateGui();
		}

		// Token: 0x06000303 RID: 771 RVA: 0x00013064 File Offset: 0x00011264
		public void UnloadGui()
		{
			if (this.Gui == null)
			{
				return;
			}
			this.Gui.RequestWarpsPressed -= this.RequestWarps;
			this.Gui.ReturnToBodyPressed -= this.ReturnToBody;
			this.Gui.GhostRolesPressed -= this.GhostRolesPressed;
			this.Gui.TargetWindow.WarpClicked -= this.OnWarpClicked;
			this.Gui.ReturnToRoundPressed -= this.ReturnToRound;
			this.Gui.Hide();
		}

		// Token: 0x06000304 RID: 772 RVA: 0x000130FD File Offset: 0x000112FD
		private void ReturnToRound()
		{
			GhostSystem system = this._system;
			if (system == null)
			{
				return;
			}
			system.ReturnToRound();
		}

		// Token: 0x06000305 RID: 773 RVA: 0x0001310F File Offset: 0x0001130F
		private void ReturnToBody()
		{
			GhostSystem system = this._system;
			if (system == null)
			{
				return;
			}
			system.ReturnToBody();
		}

		// Token: 0x06000306 RID: 774 RVA: 0x00013121 File Offset: 0x00011321
		private void RequestWarps()
		{
			GhostSystem system = this._system;
			if (system != null)
			{
				system.RequestWarps();
			}
			GhostGui gui = this.Gui;
			if (gui != null)
			{
				gui.TargetWindow.Populate();
			}
			GhostGui gui2 = this.Gui;
			if (gui2 == null)
			{
				return;
			}
			gui2.TargetWindow.OpenCentered();
		}

		// Token: 0x06000307 RID: 775 RVA: 0x0001315F File Offset: 0x0001135F
		private void GhostRolesPressed()
		{
			GhostSystem system = this._system;
			if (system == null)
			{
				return;
			}
			system.OpenGhostRoles();
		}

		// Token: 0x04000186 RID: 390
		[Dependency]
		private readonly IEntityNetworkManager _net;

		// Token: 0x04000187 RID: 391
		[Nullable(2)]
		[UISystemDependency]
		private readonly GhostSystem _system;
	}
}
