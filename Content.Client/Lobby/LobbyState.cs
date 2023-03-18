using System;
using System.Runtime.CompilerServices;
using Content.Client.GameTicking.Managers;
using Content.Client.LateJoin;
using Content.Client.Lobby.UI;
using Content.Client.Preferences;
using Content.Client.Preferences.UI;
using Content.Client.UserInterface.Systems.Chat;
using Content.Client.Voting;
using Robust.Client;
using Robust.Client.Console;
using Robust.Client.ResourceManagement;
using Robust.Client.State;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Configuration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.ViewVariables;

namespace Content.Client.Lobby
{
	// Token: 0x0200025A RID: 602
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class LobbyState : State
	{
		// Token: 0x17000340 RID: 832
		// (get) Token: 0x06000F5D RID: 3933 RVA: 0x0005C0A6 File Offset: 0x0005A2A6
		[Nullable(2)]
		protected override Type LinkedScreenType { [NullableContext(2)] get; } = typeof(LobbyGui);

		// Token: 0x06000F5E RID: 3934 RVA: 0x0005C0B0 File Offset: 0x0005A2B0
		protected override void Startup()
		{
			if (this._userInterfaceManager.ActiveScreen == null)
			{
				return;
			}
			this._lobby = (LobbyGui)this._userInterfaceManager.ActiveScreen;
			ChatUIController uicontroller = this._userInterfaceManager.GetUIController<ChatUIController>();
			this._gameTicker = this._entityManager.System<ClientGameTicker>();
			this._characterSetup = new CharacterSetupGui(this._entityManager, this._resourceCache, this._preferencesManager, this._prototypeManager, this._configurationManager);
			LayoutContainer.SetAnchorPreset(this._characterSetup, 15, false);
			this._lobby.CharacterSetupState.AddChild(this._characterSetup);
			uicontroller.SetMainChat(true);
			this._characterSetup.CloseButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				this._lobby.SwitchState(LobbyGui.LobbyGuiState.Default);
			};
			this._characterSetup.SaveButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				this._characterSetup.Save();
				this._lobby.CharacterPreview.UpdateUI();
			};
			LayoutContainer.SetAnchorPreset(this._lobby, 15, false);
			Label serverName = this._lobby.ServerName;
			ServerInfo gameInfo = this._baseClient.GameInfo;
			serverName.Text = ((gameInfo != null) ? gameInfo.ServerName : null);
			this.UpdateLobbyUi();
			this._lobby.CharacterPreview.CharacterSetupButton.OnPressed += this.OnSetupPressed;
			this._lobby.ReadyButton.OnPressed += this.OnReadyPressed;
			this._lobby.ReadyButton.OnToggled += this.OnReadyToggled;
			this._gameTicker.InfoBlobUpdated += this.UpdateLobbyUi;
			this._gameTicker.LobbyStatusUpdated += this.LobbyStatusUpdated;
			this._gameTicker.LobbyLateJoinStatusUpdated += this.LobbyLateJoinStatusUpdated;
		}

		// Token: 0x06000F5F RID: 3935 RVA: 0x0005C264 File Offset: 0x0005A464
		protected override void Shutdown()
		{
			this._userInterfaceManager.GetUIController<ChatUIController>().SetMainChat(false);
			this._gameTicker.InfoBlobUpdated -= this.UpdateLobbyUi;
			this._gameTicker.LobbyStatusUpdated -= this.LobbyStatusUpdated;
			this._gameTicker.LobbyLateJoinStatusUpdated -= this.LobbyLateJoinStatusUpdated;
			this._lobby.CharacterPreview.CharacterSetupButton.OnPressed -= this.OnSetupPressed;
			this._lobby.ReadyButton.OnPressed -= this.OnReadyPressed;
			this._lobby.ReadyButton.OnToggled -= this.OnReadyToggled;
			this._lobby = null;
			CharacterSetupGui characterSetup = this._characterSetup;
			if (characterSetup != null)
			{
				characterSetup.Dispose();
			}
			this._characterSetup = null;
		}

		// Token: 0x06000F60 RID: 3936 RVA: 0x0005C33F File Offset: 0x0005A53F
		private void OnSetupPressed(BaseButton.ButtonEventArgs args)
		{
			this.SetReady(false);
			this._lobby.SwitchState(LobbyGui.LobbyGuiState.CharacterSetup);
		}

		// Token: 0x06000F61 RID: 3937 RVA: 0x0005C354 File Offset: 0x0005A554
		private void OnReadyPressed(BaseButton.ButtonEventArgs args)
		{
			if (!this._gameTicker.IsGameStarted)
			{
				return;
			}
			new LateJoinGui().OpenCentered();
		}

		// Token: 0x06000F62 RID: 3938 RVA: 0x0005C36E File Offset: 0x0005A56E
		private void OnReadyToggled(BaseButton.ButtonToggledEventArgs args)
		{
			this.SetReady(args.Pressed);
		}

		// Token: 0x06000F63 RID: 3939 RVA: 0x0005C37C File Offset: 0x0005A57C
		public override void FrameUpdate(FrameEventArgs e)
		{
			if (this._gameTicker.IsGameStarted)
			{
				this._lobby.StartTime.Text = string.Empty;
				TimeSpan timeSpan = this._gameTiming.CurTime.Subtract(this._gameTicker.RoundStartTimeSpan);
				this._lobby.StationTime.Text = Loc.GetString("lobby-state-player-status-round-time", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("hours", timeSpan.Hours),
					new ValueTuple<string, object>("minutes", timeSpan.Minutes)
				});
				return;
			}
			string item;
			if (this._gameTicker.Paused)
			{
				item = Loc.GetString("lobby-state-paused");
			}
			else
			{
				TimeSpan timeSpan2 = this._gameTicker.StartTime - this._gameTiming.CurTime;
				double totalSeconds = timeSpan2.TotalSeconds;
				if (totalSeconds < 0.0)
				{
					item = Loc.GetString((totalSeconds < -5.0) ? "lobby-state-right-now-question" : "lobby-state-right-now-confirmation");
				}
				else
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 2);
					defaultInterpolatedStringHandler.AppendFormatted<int>(timeSpan2.Minutes);
					defaultInterpolatedStringHandler.AppendLiteral(":");
					defaultInterpolatedStringHandler.AppendFormatted<int>(timeSpan2.Seconds, "D2");
					item = defaultInterpolatedStringHandler.ToStringAndClear();
				}
			}
			this._lobby.StationTime.Text = Loc.GetString("lobby-state-player-status-round-not-started");
			this._lobby.StartTime.Text = Loc.GetString("lobby-state-round-start-countdown-text", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("timeLeft", item)
			});
		}

		// Token: 0x06000F64 RID: 3940 RVA: 0x0005C522 File Offset: 0x0005A722
		private void LobbyStatusUpdated()
		{
			this.UpdateLobbyBackground();
			this.UpdateLobbyUi();
		}

		// Token: 0x06000F65 RID: 3941 RVA: 0x0005C530 File Offset: 0x0005A730
		private void LobbyLateJoinStatusUpdated()
		{
			this._lobby.ReadyButton.Disabled = this._gameTicker.DisallowedLateJoin;
		}

		// Token: 0x06000F66 RID: 3942 RVA: 0x0005C550 File Offset: 0x0005A750
		private void UpdateLobbyUi()
		{
			if (this._gameTicker.IsGameStarted)
			{
				this._lobby.ReadyButton.Text = Loc.GetString("lobby-state-ready-button-join-state");
				this._lobby.ReadyButton.ToggleMode = false;
				this._lobby.ReadyButton.Pressed = false;
				this._lobby.ObserveButton.Disabled = false;
			}
			else
			{
				this._lobby.StartTime.Text = string.Empty;
				this._lobby.ReadyButton.Text = Loc.GetString(this._lobby.ReadyButton.Pressed ? "lobby-state-player-status-ready" : "lobby-state-player-status-not-ready");
				this._lobby.ReadyButton.ToggleMode = true;
				this._lobby.ReadyButton.Disabled = false;
				this._lobby.ReadyButton.Pressed = this._gameTicker.AreWeReady;
				this._lobby.ObserveButton.Disabled = true;
			}
			if (this._gameTicker.ServerInfoBlob != null)
			{
				this._lobby.ServerInfo.SetInfoBlob(this._gameTicker.ServerInfoBlob);
			}
		}

		// Token: 0x06000F67 RID: 3943 RVA: 0x0005C67C File Offset: 0x0005A87C
		private void UpdateLobbyBackground()
		{
			if (this._gameTicker.LobbyBackground != null)
			{
				this._lobby.Background.Texture = this._resourceCache.GetResource<TextureResource>(this._gameTicker.LobbyBackground, true);
				return;
			}
			this._lobby.Background.Texture = null;
		}

		// Token: 0x06000F68 RID: 3944 RVA: 0x0005C6D4 File Offset: 0x0005A8D4
		private void SetReady(bool newReady)
		{
			if (this._gameTicker.IsGameStarted)
			{
				return;
			}
			IConsoleHost consoleHost = this._consoleHost;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(12, 1);
			defaultInterpolatedStringHandler.AppendLiteral("toggleready ");
			defaultInterpolatedStringHandler.AppendFormatted<bool>(newReady);
			consoleHost.ExecuteCommand(defaultInterpolatedStringHandler.ToStringAndClear());
		}

		// Token: 0x04000797 RID: 1943
		[Dependency]
		private readonly IBaseClient _baseClient;

		// Token: 0x04000798 RID: 1944
		[Dependency]
		private readonly IClientConsoleHost _consoleHost;

		// Token: 0x04000799 RID: 1945
		[Dependency]
		private readonly IEntityManager _entityManager;

		// Token: 0x0400079A RID: 1946
		[Dependency]
		private readonly IResourceCache _resourceCache;

		// Token: 0x0400079B RID: 1947
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x0400079C RID: 1948
		[Dependency]
		private readonly IUserInterfaceManager _userInterfaceManager;

		// Token: 0x0400079D RID: 1949
		[Dependency]
		private readonly IClientPreferencesManager _preferencesManager;

		// Token: 0x0400079E RID: 1950
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x0400079F RID: 1951
		[Dependency]
		private readonly IVoteManager _voteManager;

		// Token: 0x040007A0 RID: 1952
		[Dependency]
		private readonly IConfigurationManager _configurationManager;

		// Token: 0x040007A1 RID: 1953
		[Nullable(2)]
		[ViewVariables]
		private CharacterSetupGui _characterSetup;

		// Token: 0x040007A2 RID: 1954
		private ClientGameTicker _gameTicker;

		// Token: 0x040007A4 RID: 1956
		[Nullable(2)]
		private LobbyGui _lobby;
	}
}
