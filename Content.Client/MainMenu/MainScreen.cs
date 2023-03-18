using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Content.Client.Audio;
using Content.Client.MainMenu.UI;
using Content.Client.UserInterface.Systems.EscapeMenu;
using Robust.Client;
using Robust.Client.GameObjects;
using Robust.Client.ResourceManagement;
using Robust.Client.State;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared;
using Robust.Shared.Audio;
using Robust.Shared.AuthLib;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Network;
using Robust.Shared.Utility;

namespace Content.Client.MainMenu
{
	// Token: 0x0200024F RID: 591
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MainScreen : State
	{
		// Token: 0x06000EE0 RID: 3808 RVA: 0x000599A4 File Offset: 0x00057BA4
		protected override void Startup()
		{
			this._mainMenuControl = new MainMenuControl(this._resourceCache, this._configurationManager);
			this._userInterfaceManager.StateRoot.AddChild(this._mainMenuControl);
			this._mainMenuControl.QuitButton.OnPressed += this.QuitButtonPressed;
			this._mainMenuControl.OptionsButton.OnPressed += this.OptionsButtonPressed;
			this._mainMenuControl.DirectConnectButton.OnPressed += this.DirectConnectButtonPressed;
			this._mainMenuControl.AddressBox.OnTextEntered += this.AddressBoxEntered;
			this._mainMenuControl.ChangelogButton.OnPressed += this.ChangelogButtonPressed;
			this._client.RunLevelChanged += this.RunLevelChanged;
			this._ambient = this._uiAudio.Play("/Audio/UI/main_menu_ambient.ogg", new AudioParams?(AudioParams.Default.WithLoop(true).WithVolume(-5f)));
		}

		// Token: 0x06000EE1 RID: 3809 RVA: 0x00059AB4 File Offset: 0x00057CB4
		protected override void Shutdown()
		{
			this._client.RunLevelChanged -= this.RunLevelChanged;
			this._netManager.ConnectFailed -= this._onConnectFailed;
			this._mainMenuControl.Dispose();
			AudioSystem.PlayingStream ambient = this._ambient;
			if (ambient == null)
			{
				return;
			}
			ambient.Stop();
		}

		// Token: 0x06000EE2 RID: 3810 RVA: 0x00059B0A File Offset: 0x00057D0A
		private void ChangelogButtonPressed(BaseButton.ButtonEventArgs args)
		{
			this._userInterfaceManager.GetUIController<ChangelogUIController>().ToggleWindow();
		}

		// Token: 0x06000EE3 RID: 3811 RVA: 0x00059B1C File Offset: 0x00057D1C
		private void OptionsButtonPressed(BaseButton.ButtonEventArgs args)
		{
			this._userInterfaceManager.GetUIController<OptionsUIController>().ToggleWindow();
		}

		// Token: 0x06000EE4 RID: 3812 RVA: 0x00059B2E File Offset: 0x00057D2E
		private void QuitButtonPressed(BaseButton.ButtonEventArgs args)
		{
			this._controllerProxy.Shutdown(null);
		}

		// Token: 0x06000EE5 RID: 3813 RVA: 0x00059B3C File Offset: 0x00057D3C
		private void DirectConnectButtonPressed(BaseButton.ButtonEventArgs args)
		{
			LineEdit addressBox = this._mainMenuControl.AddressBox;
			this.TryConnect(addressBox.Text);
		}

		// Token: 0x06000EE6 RID: 3814 RVA: 0x00059B61 File Offset: 0x00057D61
		private void AddressBoxEntered(LineEdit.LineEditEventArgs args)
		{
			if (this._isConnecting)
			{
				return;
			}
			this.TryConnect(args.Text);
		}

		// Token: 0x06000EE7 RID: 3815 RVA: 0x00059B78 File Offset: 0x00057D78
		private void TryConnect(string address)
		{
			string text = this._mainMenuControl.UsernameBox.Text.Trim();
			UsernameHelpers.UsernameInvalidReason usernameInvalidReason;
			if (!UsernameHelpers.IsNameValid(text, ref usernameInvalidReason))
			{
				string @string = Loc.GetString(UsernameHelpersExt.ToText(usernameInvalidReason));
				this._userInterfaceManager.Popup(Loc.GetString("main-menu-invalid-username-with-reason", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("invalidReason", @string)
				}), Loc.GetString("main-menu-invalid-username"));
				return;
			}
			string cvar = this._configurationManager.GetCVar<string>(CVars.PlayerName);
			if (this._mainMenuControl.UsernameBox.Text != cvar)
			{
				this._configurationManager.SetCVar<string>(CVars.PlayerName, text, false);
				this._configurationManager.SaveToFile();
			}
			this._setConnectingState(true);
			this._netManager.ConnectFailed += this._onConnectFailed;
			try
			{
				this._client.StopSinglePlayer();
				string text2;
				ushort num;
				this.ParseAddress(address, out text2, out num);
				this._client.ConnectToServer(text2, num);
			}
			catch (ArgumentException ex)
			{
				this._userInterfaceManager.Popup("Unable to connect: " + ex.Message, "Connection error.");
				Logger.Warning(ex.ToString());
				this._netManager.ConnectFailed -= this._onConnectFailed;
				this._setConnectingState(false);
				this._client.StartSinglePlayer();
			}
		}

		// Token: 0x06000EE8 RID: 3816 RVA: 0x00059CE0 File Offset: 0x00057EE0
		private void RunLevelChanged([Nullable(2)] object obj, RunLevelChangedEventArgs args)
		{
			ClientRunLevel newLevel = args.NewLevel;
			if (newLevel != 1)
			{
				if (newLevel == 2)
				{
					this._setConnectingState(true);
					return;
				}
			}
			else
			{
				this._setConnectingState(false);
				this._netManager.ConnectFailed -= this._onConnectFailed;
			}
		}

		// Token: 0x06000EE9 RID: 3817 RVA: 0x00059D24 File Offset: 0x00057F24
		private void ParseAddress(string address, out string ip, out ushort port)
		{
			Match match = MainScreen.IPv6Regex.Match(address);
			if (match != Match.Empty)
			{
				ip = match.Groups[1].Value;
				if (!match.Groups[2].Success)
				{
					port = this._client.DefaultPort;
					return;
				}
				if (!ushort.TryParse(match.Groups[2].Value, out port))
				{
					throw new ArgumentException("Not a valid port.");
				}
				return;
			}
			else
			{
				string[] array = address.Split(':', StringSplitOptions.None);
				ip = address;
				port = this._client.DefaultPort;
				if (array.Length > 2)
				{
					throw new ArgumentException("Not a valid Address.");
				}
				if (array.Length == 2)
				{
					ip = array[0];
					if (!ushort.TryParse(array[1], out port))
					{
						throw new ArgumentException("Not a valid port.");
					}
				}
				return;
			}
		}

		// Token: 0x06000EEA RID: 3818 RVA: 0x00059DEC File Offset: 0x00057FEC
		private void _onConnectFailed([Nullable(2)] object _, NetConnectFailArgs args)
		{
			this._userInterfaceManager.Popup(Loc.GetString("main-menu-failed-to-connect", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("reason", args.Reason)
			}), "Alert!");
			this._netManager.ConnectFailed -= this._onConnectFailed;
			this._setConnectingState(false);
			this._client.StartSinglePlayer();
		}

		// Token: 0x06000EEB RID: 3819 RVA: 0x00059E59 File Offset: 0x00058059
		private void _setConnectingState(bool state)
		{
			this._isConnecting = state;
			this._mainMenuControl.DirectConnectButton.Disabled = state;
		}

		// Token: 0x0400076B RID: 1899
		[Dependency]
		private readonly IBaseClient _client;

		// Token: 0x0400076C RID: 1900
		[Dependency]
		private readonly IClientNetManager _netManager;

		// Token: 0x0400076D RID: 1901
		[Dependency]
		private readonly IConfigurationManager _configurationManager;

		// Token: 0x0400076E RID: 1902
		[Dependency]
		private readonly IGameController _controllerProxy;

		// Token: 0x0400076F RID: 1903
		[Dependency]
		private readonly IResourceCache _resourceCache;

		// Token: 0x04000770 RID: 1904
		[Dependency]
		private readonly IUserInterfaceManager _userInterfaceManager;

		// Token: 0x04000771 RID: 1905
		[Dependency]
		private readonly UIAudioManager _uiAudio;

		// Token: 0x04000772 RID: 1906
		private MainMenuControl _mainMenuControl;

		// Token: 0x04000773 RID: 1907
		private bool _isConnecting;

		// Token: 0x04000774 RID: 1908
		[Nullable(2)]
		private AudioSystem.PlayingStream _ambient;

		// Token: 0x04000775 RID: 1909
		private static readonly Regex IPv6Regex = new Regex("\\[(.*:.*:.*)](?::(\\d+))?");
	}
}
