using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Client.Administration.Managers;
using Content.Client.Administration.Systems;
using Content.Client.Gameplay;
using Content.Client.UserInterface.Controls;
using Content.Client.UserInterface.Systems.MenuBar.Widgets;
using Content.Shared.Administration;
using Content.Shared.CCVar;
using Content.Shared.Input;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Audio;
using Robust.Shared.Configuration;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Players;

namespace Content.Client.UserInterface.Systems.Bwoink
{
	// Token: 0x020000B1 RID: 177
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AHelpUIController : UIController, IOnStateChanged<GameplayState>, IOnStateEntered<GameplayState>, IOnStateExited<GameplayState>, IOnSystemChanged<BwoinkSystem>, IOnSystemLoaded<BwoinkSystem>, IOnSystemUnloaded<BwoinkSystem>
	{
		// Token: 0x170000BC RID: 188
		// (get) Token: 0x0600049D RID: 1181 RVA: 0x0001A52B File Offset: 0x0001872B
		[Nullable(2)]
		private MenuButton AhelpButton
		{
			[NullableContext(2)]
			get
			{
				GameTopMenuBar activeUIWidgetOrNull = this.UIManager.GetActiveUIWidgetOrNull<GameTopMenuBar>();
				if (activeUIWidgetOrNull == null)
				{
					return null;
				}
				return activeUIWidgetOrNull.AHelpButton;
			}
		}

		// Token: 0x0600049E RID: 1182 RVA: 0x0001A543 File Offset: 0x00018743
		public override void Initialize()
		{
			base.Initialize();
			this.defaultBwoinkVolume = CCVars.BwoinkVolume.DefaultValue;
			this._cfg.OnValueChanged<float>(CCVars.BwoinkVolume, delegate(float volume)
			{
				this.adminBwoinkVolume = volume;
			}, false);
		}

		// Token: 0x0600049F RID: 1183 RVA: 0x0001A578 File Offset: 0x00018778
		public void OnStateEntered(GameplayState state)
		{
			this._adminManager.AdminStatusUpdated += this.OnAdminStatusUpdated;
			CommandBinds.Builder.Bind(ContentKeyFunctions.OpenAHelp, InputCmdHandler.FromDelegate(delegate(ICommonSession _)
			{
				this.ToggleWindow();
			}, null, true, true)).Register<AHelpUIController>();
		}

		// Token: 0x060004A0 RID: 1184 RVA: 0x0001A5C5 File Offset: 0x000187C5
		public void UnloadButton()
		{
			if (this.AhelpButton == null)
			{
				return;
			}
			this.AhelpButton.OnPressed -= this.AHelpButtonPressed;
		}

		// Token: 0x060004A1 RID: 1185 RVA: 0x0001A5E7 File Offset: 0x000187E7
		public void LoadButton()
		{
			if (this.AhelpButton == null)
			{
				return;
			}
			this.AhelpButton.OnPressed += this.AHelpButtonPressed;
		}

		// Token: 0x060004A2 RID: 1186 RVA: 0x0001A60C File Offset: 0x0001880C
		private void OnAdminStatusUpdated()
		{
			IAHelpUIHandler uihelper = this.UIHelper;
			if (uihelper == null || !uihelper.IsOpen)
			{
				return;
			}
			this.EnsureUIHelper();
		}

		// Token: 0x060004A3 RID: 1187 RVA: 0x0001A632 File Offset: 0x00018832
		private void AHelpButtonPressed(BaseButton.ButtonEventArgs obj)
		{
			this.EnsureUIHelper();
			this.UIHelper.ToggleWindow();
		}

		// Token: 0x060004A4 RID: 1188 RVA: 0x0001A645 File Offset: 0x00018845
		public void OnStateExited(GameplayState state)
		{
			this.SetAHelpPressed(false);
			this._adminManager.AdminStatusUpdated -= this.OnAdminStatusUpdated;
			IAHelpUIHandler uihelper = this.UIHelper;
			if (uihelper != null)
			{
				uihelper.Dispose();
			}
			this.UIHelper = null;
			CommandBinds.Unregister<AHelpUIController>();
		}

		// Token: 0x060004A5 RID: 1189 RVA: 0x0001A682 File Offset: 0x00018882
		public void OnSystemLoaded(BwoinkSystem system)
		{
			this._bwoinkSystem = system;
			this._bwoinkSystem.OnBwoinkTextMessageRecieved += this.RecievedBwoink;
		}

		// Token: 0x060004A6 RID: 1190 RVA: 0x0001A6A2 File Offset: 0x000188A2
		public void OnSystemUnloaded(BwoinkSystem system)
		{
			this._bwoinkSystem.OnBwoinkTextMessageRecieved -= this.RecievedBwoink;
			this._bwoinkSystem = null;
		}

		// Token: 0x060004A7 RID: 1191 RVA: 0x0001A6C2 File Offset: 0x000188C2
		private void SetAHelpPressed(bool pressed)
		{
			if (this.AhelpButton == null || this.AhelpButton.Pressed == pressed)
			{
				return;
			}
			this.AhelpButton.StyleClasses.Remove("topButtonLabel");
			this.AhelpButton.Pressed = pressed;
		}

		// Token: 0x060004A8 RID: 1192 RVA: 0x0001A700 File Offset: 0x00018900
		private void RecievedBwoink([Nullable(2)] object sender, SharedBwoinkSystem.BwoinkTextMessage message)
		{
			string text = "c.s.go.es.bwoink";
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 2);
			defaultInterpolatedStringHandler.AppendLiteral("@");
			defaultInterpolatedStringHandler.AppendFormatted<NetUserId>(message.UserId);
			defaultInterpolatedStringHandler.AppendLiteral(": ");
			defaultInterpolatedStringHandler.AppendFormatted(message.Text);
			Logger.InfoS(text, defaultInterpolatedStringHandler.ToStringAndClear());
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			if (localPlayer == null)
			{
				return;
			}
			float volume = this._clientAdminManager.IsActive() ? this.adminBwoinkVolume : this.defaultBwoinkVolume;
			AudioParams audioParams;
			audioParams..ctor();
			audioParams.Volume = volume;
			AudioParams value = audioParams;
			if (localPlayer.UserId != message.TrueSender)
			{
				SoundSystem.Play("/Audio/Effects/adminhelp.ogg", Filter.Local(), new AudioParams?(value));
				this._clyde.RequestWindowAttention();
			}
			this.EnsureUIHelper();
			if (!this.UIHelper.IsOpen)
			{
				MenuButton ahelpButton = this.AhelpButton;
				if (ahelpButton != null)
				{
					ahelpButton.StyleClasses.Add("topButtonLabel");
				}
			}
			this.UIHelper.Receive(message);
		}

		// Token: 0x060004A9 RID: 1193 RVA: 0x0001A808 File Offset: 0x00018A08
		public void EnsureUIHelper()
		{
			bool flag = this._adminManager.HasFlag(AdminFlags.Adminhelp);
			if (this.UIHelper != null && this.UIHelper.IsAdmin == flag)
			{
				return;
			}
			IAHelpUIHandler uihelper = this.UIHelper;
			if (uihelper != null)
			{
				uihelper.Dispose();
			}
			NetUserId userId2 = this._playerManager.LocalPlayer.UserId;
			IAHelpUIHandler uihelper2;
			if (!flag)
			{
				IAHelpUIHandler iahelpUIHandler = new UserAHelpUIHandler(userId2);
				uihelper2 = iahelpUIHandler;
			}
			else
			{
				IAHelpUIHandler iahelpUIHandler = new AdminAHelpUIHandler(userId2);
				uihelper2 = iahelpUIHandler;
			}
			this.UIHelper = uihelper2;
			this.UIHelper.SendMessageAction = delegate(NetUserId userId, string textMessage)
			{
				BwoinkSystem bwoinkSystem = this._bwoinkSystem;
				if (bwoinkSystem == null)
				{
					return;
				}
				bwoinkSystem.Send(userId, textMessage);
			};
			this.UIHelper.OnClose += delegate()
			{
				this.SetAHelpPressed(false);
			};
			this.UIHelper.OnOpen += delegate()
			{
				this.SetAHelpPressed(true);
			};
			this.SetAHelpPressed(this.UIHelper.IsOpen);
		}

		// Token: 0x060004AA RID: 1194 RVA: 0x0001A8D0 File Offset: 0x00018AD0
		public void Close()
		{
			IAHelpUIHandler uihelper = this.UIHelper;
			if (uihelper == null)
			{
				return;
			}
			uihelper.Close();
		}

		// Token: 0x060004AB RID: 1195 RVA: 0x0001A8E4 File Offset: 0x00018AE4
		public void Open()
		{
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			if (localPlayer == null)
			{
				return;
			}
			this.EnsureUIHelper();
			if (this.UIHelper.IsOpen)
			{
				return;
			}
			this.UIHelper.Open(localPlayer.UserId);
		}

		// Token: 0x060004AC RID: 1196 RVA: 0x0001A926 File Offset: 0x00018B26
		public void Open(NetUserId userId)
		{
			this.EnsureUIHelper();
			if (!this.UIHelper.IsAdmin)
			{
				return;
			}
			IAHelpUIHandler uihelper = this.UIHelper;
			if (uihelper == null)
			{
				return;
			}
			uihelper.Open(userId);
		}

		// Token: 0x060004AD RID: 1197 RVA: 0x0001A94D File Offset: 0x00018B4D
		public void ToggleWindow()
		{
			this.EnsureUIHelper();
			IAHelpUIHandler uihelper = this.UIHelper;
			if (uihelper == null)
			{
				return;
			}
			uihelper.ToggleWindow();
		}

		// Token: 0x060004AE RID: 1198 RVA: 0x0001A968 File Offset: 0x00018B68
		public void PopOut()
		{
			this.EnsureUIHelper();
			AdminAHelpUIHandler adminAHelpUIHandler = this.UIHelper as AdminAHelpUIHandler;
			if (adminAHelpUIHandler == null)
			{
				return;
			}
			if (adminAHelpUIHandler.Window == null || adminAHelpUIHandler.Control == null)
			{
				return;
			}
			adminAHelpUIHandler.Control.Orphan();
			adminAHelpUIHandler.Window.Dispose();
			adminAHelpUIHandler.Window = null;
			adminAHelpUIHandler.EverOpened = false;
			IClydeMonitor monitor = this._clyde.EnumerateMonitors().First<IClydeMonitor>();
			adminAHelpUIHandler.ClydeWindow = this._clyde.CreateWindow(new WindowCreateParameters
			{
				Maximized = false,
				Title = "Admin Help",
				Monitor = monitor,
				Width = 900,
				Height = 500
			});
			adminAHelpUIHandler.ClydeWindow.RequestClosed += adminAHelpUIHandler.OnRequestClosed;
			adminAHelpUIHandler.ClydeWindow.DisposeOnClose = true;
			adminAHelpUIHandler.WindowRoot = this._uiManager.CreateWindowRoot(adminAHelpUIHandler.ClydeWindow);
			adminAHelpUIHandler.WindowRoot.AddChild(adminAHelpUIHandler.Control);
			adminAHelpUIHandler.Control.PopOut.Disabled = true;
			adminAHelpUIHandler.Control.PopOut.Visible = false;
		}

		// Token: 0x0400022B RID: 555
		[Dependency]
		private readonly IClientAdminManager _adminManager;

		// Token: 0x0400022C RID: 556
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x0400022D RID: 557
		[Dependency]
		private readonly IClyde _clyde;

		// Token: 0x0400022E RID: 558
		[Dependency]
		private readonly IUserInterfaceManager _uiManager;

		// Token: 0x0400022F RID: 559
		[Dependency]
		private readonly IClientAdminManager _clientAdminManager;

		// Token: 0x04000230 RID: 560
		[Dependency]
		private readonly IConfigurationManager _cfg;

		// Token: 0x04000231 RID: 561
		[Nullable(2)]
		private BwoinkSystem _bwoinkSystem;

		// Token: 0x04000232 RID: 562
		[Nullable(2)]
		public IAHelpUIHandler UIHelper;

		// Token: 0x04000233 RID: 563
		private float defaultBwoinkVolume;

		// Token: 0x04000234 RID: 564
		private float adminBwoinkVolume;
	}
}
