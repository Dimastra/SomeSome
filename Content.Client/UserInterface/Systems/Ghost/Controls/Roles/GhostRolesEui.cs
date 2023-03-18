using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Client.Eui;
using Content.Shared.Eui;
using Content.Shared.Ghost.Roles;
using Robust.Client.UserInterface.Controls;

namespace Content.Client.UserInterface.Systems.Ghost.Controls.Roles
{
	// Token: 0x0200008E RID: 142
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GhostRolesEui : BaseEui
	{
		// Token: 0x06000349 RID: 841 RVA: 0x00014498 File Offset: 0x00012698
		public GhostRolesEui()
		{
			this._window = new GhostRolesWindow();
			this._window.OnRoleRequested += delegate(GhostRoleInfo info)
			{
				if (this._windowRules != null)
				{
					this._windowRules.Close();
				}
				this._windowRules = new GhostRoleRulesWindow(info.Rules, delegate(BaseButton.ButtonEventArgs _)
				{
					this.SendMessage(new GhostRoleTakeoverRequestMessage(info.Identifier));
				});
				this._windowRulesId = info.Identifier;
				this._windowRules.OnClose += delegate()
				{
					this._windowRules = null;
				};
				this._windowRules.OpenCentered();
			};
			this._window.OnRoleFollow += delegate(GhostRoleInfo info)
			{
				base.SendMessage(new GhostRoleFollowRequestMessage(info.Identifier));
			};
			this._window.OnClose += delegate()
			{
				base.SendMessage(new GhostRoleWindowCloseMessage());
			};
		}

		// Token: 0x0600034A RID: 842 RVA: 0x000144FB File Offset: 0x000126FB
		public override void Opened()
		{
			base.Opened();
			this._window.OpenCentered();
		}

		// Token: 0x0600034B RID: 843 RVA: 0x0001450E File Offset: 0x0001270E
		public override void Closed()
		{
			base.Closed();
			this._window.Close();
			GhostRoleRulesWindow windowRules = this._windowRules;
			if (windowRules == null)
			{
				return;
			}
			windowRules.Close();
		}

		// Token: 0x0600034C RID: 844 RVA: 0x00014534 File Offset: 0x00012734
		public override void HandleState(EuiStateBase state)
		{
			base.HandleState(state);
			GhostRolesEuiState ghostRolesEuiState = state as GhostRolesEuiState;
			if (ghostRolesEuiState == null)
			{
				return;
			}
			this._window.ClearEntries();
			foreach (IGrouping<ValueTuple<string, string>, GhostRoleInfo> grouping in from role in ghostRolesEuiState.GhostRoles
			group role by new ValueTuple<string, string>(role.Name, role.Description))
			{
				string item = grouping.Key.Item1;
				string item2 = grouping.Key.Item2;
				this._window.AddEntry(item, item2, grouping);
			}
			if (ghostRolesEuiState.GhostRoles.All((GhostRoleInfo role) => role.Identifier != this._windowRulesId))
			{
				GhostRoleRulesWindow windowRules = this._windowRules;
				if (windowRules == null)
				{
					return;
				}
				windowRules.Close();
			}
		}

		// Token: 0x0400019C RID: 412
		private readonly GhostRolesWindow _window;

		// Token: 0x0400019D RID: 413
		[Nullable(2)]
		private GhostRoleRulesWindow _windowRules;

		// Token: 0x0400019E RID: 414
		private uint _windowRulesId;
	}
}
