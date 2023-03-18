using System;
using System.Runtime.CompilerServices;
using Content.Client.Hands;
using Content.Client.UserInterface.Controls;
using Content.Client.UserInterface.Screens;
using Content.Client.UserInterface.Systems.Actions;
using Content.Client.UserInterface.Systems.Alerts;
using Content.Client.UserInterface.Systems.Chat;
using Content.Client.UserInterface.Systems.Ghost;
using Content.Client.UserInterface.Systems.Hotbar;
using Content.Client.UserInterface.Systems.MenuBar;
using Content.Client.UserInterface.Systems.Viewport;
using Content.Client.Viewport;
using Content.Shared.CCVar;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Client.Gameplay
{
	// Token: 0x02000308 RID: 776
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GameplayState : GameplayStateBase, IMainViewportState
	{
		// Token: 0x17000418 RID: 1048
		// (get) Token: 0x0600139A RID: 5018 RVA: 0x0007384C File Offset: 0x00071A4C
		public MainViewport Viewport
		{
			get
			{
				return this._uiManager.ActiveScreen.GetWidget<MainViewport>();
			}
		}

		// Token: 0x0600139B RID: 5019 RVA: 0x00073860 File Offset: 0x00071A60
		public GameplayState()
		{
			IoCManager.InjectDependencies<GameplayState>(this);
			this._ghostController = this._uiManager.GetUIController<GhostUIController>();
			this._actionController = this._uiManager.GetUIController<ActionUIController>();
			this._alertsController = this._uiManager.GetUIController<AlertsUIController>();
			this._hotbarController = this._uiManager.GetUIController<HotbarUIController>();
			this._chatController = this._uiManager.GetUIController<ChatUIController>();
			this._viewportController = this._uiManager.GetUIController<ViewportUIController>();
			this._menuController = this._uiManager.GetUIController<GameTopMenuBarUIController>();
		}

		// Token: 0x0600139C RID: 5020 RVA: 0x000738F4 File Offset: 0x00071AF4
		protected override void Startup()
		{
			base.Startup();
			this.LoadMainScreen();
			this._overlayManager.AddOverlay(new ShowHandItemOverlay());
			this._fpsCounter = new FpsCounter(this._gameTiming);
			this.UserInterfaceManager.PopupRoot.AddChild(this._fpsCounter);
			this._fpsCounter.Visible = this._configurationManager.GetCVar<bool>(CCVars.HudFpsCounterVisible);
			this._configurationManager.OnValueChanged<bool>(CCVars.HudFpsCounterVisible, delegate(bool show)
			{
				this._fpsCounter.Visible = show;
			}, false);
			this._configurationManager.OnValueChanged<string>(CCVars.UILayout, new Action<string>(this.ReloadMainScreenValueChange), false);
		}

		// Token: 0x0600139D RID: 5021 RVA: 0x0007399C File Offset: 0x00071B9C
		protected override void Shutdown()
		{
			this._overlayManager.RemoveOverlay<ShowHandItemOverlay>();
			base.Shutdown();
			this._eyeManager.MainViewport = this.UserInterfaceManager.MainViewport;
			this._fpsCounter.Dispose();
			this._uiManager.ClearWindows();
			this._configurationManager.UnsubValueChanged<string>(CCVars.UILayout, new Action<string>(this.ReloadMainScreenValueChange));
			this.UnloadMainScreen();
		}

		// Token: 0x0600139E RID: 5022 RVA: 0x00073A09 File Offset: 0x00071C09
		private void ReloadMainScreenValueChange(string _)
		{
			this.ReloadMainScreen();
		}

		// Token: 0x0600139F RID: 5023 RVA: 0x00073A11 File Offset: 0x00071C11
		public void ReloadMainScreen()
		{
			UIScreen activeScreen = this._uiManager.ActiveScreen;
			if (((activeScreen != null) ? activeScreen.GetWidget<MainViewport>() : null) == null)
			{
				return;
			}
			this.UnloadMainScreen();
			this.LoadMainScreen();
		}

		// Token: 0x060013A0 RID: 5024 RVA: 0x00073A39 File Offset: 0x00071C39
		private void UnloadMainScreen()
		{
			this._chatController.SetMainChat(false);
			this._menuController.UnloadButtons();
			this._ghostController.UnloadGui();
			this._actionController.UnloadGui();
			this._uiManager.UnloadScreen();
		}

		// Token: 0x060013A1 RID: 5025 RVA: 0x00073A74 File Offset: 0x00071C74
		private void LoadMainScreen()
		{
			ScreenType screenType;
			if (!Enum.TryParse<ScreenType>(this._configurationManager.GetCVar<string>(CCVars.UILayout), out screenType))
			{
				screenType = ScreenType.Default;
			}
			if (screenType != ScreenType.Default)
			{
				if (screenType == ScreenType.Separated)
				{
					this._uiManager.LoadScreen<SeparatedChatGameScreen>();
				}
			}
			else
			{
				this._uiManager.LoadScreen<DefaultGameScreen>();
			}
			this._chatController.SetMainChat(true);
			this._viewportController.ReloadViewport();
			this._menuController.LoadButtons();
			this._ghostController.LoadGui();
			this._actionController.LoadGui();
			this._alertsController.SyncAlerts();
			this._hotbarController.ReloadHotbar();
			LayoutContainer speechBubbleRoot = this._uiManager.ActiveScreen.FindControl<LayoutContainer>("ViewportContainer");
			this._chatController.SetSpeechBubbleRoot(speechBubbleRoot);
		}

		// Token: 0x060013A2 RID: 5026 RVA: 0x00073B2D File Offset: 0x00071D2D
		protected override void OnKeyBindStateChanged(ViewportBoundKeyEventArgs args)
		{
			if (args.Viewport == null)
			{
				base.OnKeyBindStateChanged(new ViewportBoundKeyEventArgs(args.KeyEventArgs, this.Viewport.Viewport));
				return;
			}
			base.OnKeyBindStateChanged(args);
		}

		// Token: 0x040009CA RID: 2506
		[Dependency]
		private readonly IEyeManager _eyeManager;

		// Token: 0x040009CB RID: 2507
		[Dependency]
		private readonly IOverlayManager _overlayManager;

		// Token: 0x040009CC RID: 2508
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x040009CD RID: 2509
		[Dependency]
		private readonly IUserInterfaceManager _uiManager;

		// Token: 0x040009CE RID: 2510
		[Dependency]
		private readonly IConfigurationManager _configurationManager;

		// Token: 0x040009CF RID: 2511
		private FpsCounter _fpsCounter;

		// Token: 0x040009D0 RID: 2512
		private readonly GhostUIController _ghostController;

		// Token: 0x040009D1 RID: 2513
		private readonly ActionUIController _actionController;

		// Token: 0x040009D2 RID: 2514
		private readonly AlertsUIController _alertsController;

		// Token: 0x040009D3 RID: 2515
		private readonly HotbarUIController _hotbarController;

		// Token: 0x040009D4 RID: 2516
		private readonly ChatUIController _chatController;

		// Token: 0x040009D5 RID: 2517
		private readonly ViewportUIController _viewportController;

		// Token: 0x040009D6 RID: 2518
		private readonly GameTopMenuBarUIController _menuController;
	}
}
