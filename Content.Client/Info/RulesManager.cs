using System;
using System.Runtime.CompilerServices;
using Content.Client.Gameplay;
using Content.Client.Lobby;
using Content.Shared.CCVar;
using Content.Shared.Info;
using Robust.Client.Console;
using Robust.Client.State;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Configuration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;

namespace Content.Client.Info
{
	// Token: 0x020002C1 RID: 705
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RulesManager : SharedRulesManager
	{
		// Token: 0x060011B5 RID: 4533 RVA: 0x000691BC File Offset: 0x000673BC
		public void Initialize()
		{
			this._netManager.RegisterNetMessage<SharedRulesManager.ShouldShowRulesPopupMessage>(new ProcessMessage<SharedRulesManager.ShouldShowRulesPopupMessage>(this.OnShouldShowRules), 3);
			this._netManager.RegisterNetMessage<SharedRulesManager.ShowRulesPopupMessage>(new ProcessMessage<SharedRulesManager.ShowRulesPopupMessage>(this.OnShowRulesPopupMessage), 3);
			this._netManager.RegisterNetMessage<SharedRulesManager.RulesAcceptedMessage>(null, 3);
			this._stateManager.OnStateChanged += this.OnStateChanged;
			this._consoleHost.RegisterCommand("fuckrules", "", "", delegate(IConsoleShell _, string _, string[] _)
			{
				this.OnAcceptPressed();
			}, false);
		}

		// Token: 0x060011B6 RID: 4534 RVA: 0x00069244 File Offset: 0x00067444
		private void OnShouldShowRules(SharedRulesManager.ShouldShowRulesPopupMessage message)
		{
			this._shouldShowRules = true;
		}

		// Token: 0x060011B7 RID: 4535 RVA: 0x0006924D File Offset: 0x0006744D
		private void OnShowRulesPopupMessage(SharedRulesManager.ShowRulesPopupMessage message)
		{
			this.ShowRules(message.PopupTime);
		}

		// Token: 0x060011B8 RID: 4536 RVA: 0x0006925C File Offset: 0x0006745C
		private void OnStateChanged(StateChangedEventArgs args)
		{
			State newState = args.NewState;
			if (!(newState is GameplayState) && !(newState is LobbyState))
			{
				return;
			}
			if (!this._shouldShowRules)
			{
				return;
			}
			this._shouldShowRules = false;
			this.ShowRules(this._configManager.GetCVar<float>(CCVars.RulesWaitTime));
		}

		// Token: 0x060011B9 RID: 4537 RVA: 0x000692A8 File Offset: 0x000674A8
		private void ShowRules(float time)
		{
			if (this._activePopup != null)
			{
				return;
			}
			this._activePopup = new RulesPopup
			{
				Timer = time
			};
			this._activePopup.OnQuitPressed += this.OnQuitPressed;
			this._activePopup.OnAcceptPressed += this.OnAcceptPressed;
			this._userInterfaceManager.WindowRoot.AddChild(this._activePopup);
			LayoutContainer.SetAnchorPreset(this._activePopup, 15, false);
		}

		// Token: 0x060011BA RID: 4538 RVA: 0x00069322 File Offset: 0x00067522
		private void OnQuitPressed()
		{
			this._consoleHost.ExecuteCommand("quit");
		}

		// Token: 0x060011BB RID: 4539 RVA: 0x00069334 File Offset: 0x00067534
		private void OnAcceptPressed()
		{
			this._netManager.ClientSendMessage(new SharedRulesManager.RulesAcceptedMessage());
			RulesPopup activePopup = this._activePopup;
			if (activePopup != null)
			{
				activePopup.Orphan();
			}
			this._activePopup = null;
		}

		// Token: 0x060011BC RID: 4540 RVA: 0x00069360 File Offset: 0x00067560
		public void UpdateRules()
		{
			RulesMessage rules = this._sysMan.GetEntitySystem<InfoSystem>().Rules;
			this.rulesSection.SetText(rules.Title, rules.Text, true);
		}

		// Token: 0x060011BD RID: 4541 RVA: 0x00069396 File Offset: 0x00067596
		public Control RulesSection()
		{
			this.rulesSection = new InfoSection("", "", false);
			this.UpdateRules();
			return this.rulesSection;
		}

		// Token: 0x040008B3 RID: 2227
		[Dependency]
		private readonly IConfigurationManager _configManager;

		// Token: 0x040008B4 RID: 2228
		[Dependency]
		private readonly IUserInterfaceManager _userInterfaceManager;

		// Token: 0x040008B5 RID: 2229
		[Dependency]
		private readonly IStateManager _stateManager;

		// Token: 0x040008B6 RID: 2230
		[Dependency]
		private readonly IClientConsoleHost _consoleHost;

		// Token: 0x040008B7 RID: 2231
		[Dependency]
		private readonly INetManager _netManager;

		// Token: 0x040008B8 RID: 2232
		[Dependency]
		private readonly IEntitySystemManager _sysMan;

		// Token: 0x040008B9 RID: 2233
		private InfoSection rulesSection = new InfoSection("", "", false);

		// Token: 0x040008BA RID: 2234
		private bool _shouldShowRules;

		// Token: 0x040008BB RID: 2235
		[Nullable(2)]
		private RulesPopup _activePopup;
	}
}
