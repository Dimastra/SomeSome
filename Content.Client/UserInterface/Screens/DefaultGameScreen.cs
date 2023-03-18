﻿using System;
using CompiledRobustXaml;
using Content.Client.UserInterface.Controls;
using Content.Client.UserInterface.Systems.Actions.Widgets;
using Content.Client.UserInterface.Systems.Alerts.Widgets;
using Content.Client.UserInterface.Systems.Chat.Widgets;
using Content.Client.UserInterface.Systems.Ghost.Widgets;
using Content.Client.UserInterface.Systems.Hotbar.Widgets;
using Content.Client.UserInterface.Systems.MenuBar.Widgets;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;

namespace Content.Client.UserInterface.Screens
{
	// Token: 0x020000CE RID: 206
	[GenerateTypedNameReferences]
	public sealed class DefaultGameScreen : UIScreen
	{
		// Token: 0x060005CA RID: 1482 RVA: 0x0001FB00 File Offset: 0x0001DD00
		public DefaultGameScreen()
		{
			DefaultGameScreen.!XamlIlPopulateTrampoline(this);
			base.AutoscaleMaxResolution = new Vector2i(1080, 770);
			LayoutContainer.SetAnchorPreset(this.MainViewport, 15, false);
			LayoutContainer.SetAnchorPreset(this.ViewportContainer, 15, false);
			LayoutContainer.SetAnchorAndMarginPreset(this.TopBar, 0, 0, 10);
			LayoutContainer.SetAnchorAndMarginPreset(this.Actions, 2, 0, 10);
			LayoutContainer.SetAnchorAndMarginPreset(this.Ghost, 12, 0, 80);
			LayoutContainer.SetAnchorAndMarginPreset(this.Hotbar, 12, 0, 5);
			LayoutContainer.SetAnchorAndMarginPreset(this.Chat, 1, 0, 10);
			LayoutContainer.SetAnchorAndMarginPreset(this.Alerts, 1, 0, 10);
			this.Chat.OnResized += this.ChatOnResized;
		}

		// Token: 0x060005CB RID: 1483 RVA: 0x0001FBBC File Offset: 0x0001DDBC
		private void ChatOnResized()
		{
			float value = this.Chat.GetValue<float>(LayoutContainer.MarginBottomProperty);
			LayoutContainer.SetMarginTop(this.Alerts, value);
		}

		// Token: 0x170000E5 RID: 229
		// (get) Token: 0x060005CC RID: 1484 RVA: 0x0001FBE6 File Offset: 0x0001DDE6
		private DefaultGameScreen DefaultHud
		{
			get
			{
				return base.FindControl<DefaultGameScreen>("DefaultHud");
			}
		}

		// Token: 0x170000E6 RID: 230
		// (get) Token: 0x060005CD RID: 1485 RVA: 0x0001FBF3 File Offset: 0x0001DDF3
		private LayoutContainer ViewportContainer
		{
			get
			{
				return base.FindControl<LayoutContainer>("ViewportContainer");
			}
		}

		// Token: 0x170000E7 RID: 231
		// (get) Token: 0x060005CE RID: 1486 RVA: 0x0001FC00 File Offset: 0x0001DE00
		private MainViewport MainViewport
		{
			get
			{
				return base.FindControl<MainViewport>("MainViewport");
			}
		}

		// Token: 0x170000E8 RID: 232
		// (get) Token: 0x060005CF RID: 1487 RVA: 0x0001FC0D File Offset: 0x0001DE0D
		private GameTopMenuBar TopBar
		{
			get
			{
				return base.FindControl<GameTopMenuBar>("TopBar");
			}
		}

		// Token: 0x170000E9 RID: 233
		// (get) Token: 0x060005D0 RID: 1488 RVA: 0x0001FC1A File Offset: 0x0001DE1A
		private GhostGui Ghost
		{
			get
			{
				return base.FindControl<GhostGui>("Ghost");
			}
		}

		// Token: 0x170000EA RID: 234
		// (get) Token: 0x060005D1 RID: 1489 RVA: 0x0001FC27 File Offset: 0x0001DE27
		private HotbarGui Hotbar
		{
			get
			{
				return base.FindControl<HotbarGui>("Hotbar");
			}
		}

		// Token: 0x170000EB RID: 235
		// (get) Token: 0x060005D2 RID: 1490 RVA: 0x0001FC34 File Offset: 0x0001DE34
		private ActionsBar Actions
		{
			get
			{
				return base.FindControl<ActionsBar>("Actions");
			}
		}

		// Token: 0x170000EC RID: 236
		// (get) Token: 0x060005D3 RID: 1491 RVA: 0x0001FC41 File Offset: 0x0001DE41
		private ResizableChatBox Chat
		{
			get
			{
				return base.FindControl<ResizableChatBox>("Chat");
			}
		}

		// Token: 0x170000ED RID: 237
		// (get) Token: 0x060005D4 RID: 1492 RVA: 0x0001FC4E File Offset: 0x0001DE4E
		private AlertsUI Alerts
		{
			get
			{
				return base.FindControl<AlertsUI>("Alerts");
			}
		}

		// Token: 0x060005D5 RID: 1493 RVA: 0x0001FC5C File Offset: 0x0001DE5C
		static void xaml(IServiceProvider A_0, DefaultGameScreen A_1)
		{
			XamlIlContext.Context<DefaultGameScreen> context = new XamlIlContext.Context<DefaultGameScreen>(A_0, null, "resm:Content.Client.UserInterface.Screens.DefaultGameScreen.xaml?assembly=Content.Client");
			context.RootObject = A_1;
			context.IntermediateRoot = A_1;
			A_1.Name = "DefaultHud";
			context.RobustNameScope.Register("DefaultHud", A_1);
			A_1.VerticalExpand = false;
			A_1.VerticalAlignment = 3;
			A_1.HorizontalAlignment = 2;
			LayoutContainer layoutContainer = new LayoutContainer();
			layoutContainer.Name = "ViewportContainer";
			Control control = layoutContainer;
			context.RobustNameScope.Register("ViewportContainer", control);
			layoutContainer.HorizontalExpand = true;
			layoutContainer.VerticalExpand = true;
			MainViewport mainViewport = new MainViewport();
			mainViewport.Name = "MainViewport";
			control = mainViewport;
			context.RobustNameScope.Register("MainViewport", control);
			control = mainViewport;
			layoutContainer.XamlChildren.Add(control);
			control = layoutContainer;
			A_1.XamlChildren.Add(control);
			GameTopMenuBar gameTopMenuBar = new GameTopMenuBar();
			gameTopMenuBar.Name = "TopBar";
			control = gameTopMenuBar;
			context.RobustNameScope.Register("TopBar", control);
			gameTopMenuBar.Access = new AccessLevel?(1);
			control = gameTopMenuBar;
			A_1.XamlChildren.Add(control);
			GhostGui ghostGui = new GhostGui();
			ghostGui.Name = "Ghost";
			control = ghostGui;
			context.RobustNameScope.Register("Ghost", control);
			ghostGui.Access = new AccessLevel?(1);
			control = ghostGui;
			A_1.XamlChildren.Add(control);
			HotbarGui hotbarGui = new HotbarGui();
			hotbarGui.Name = "Hotbar";
			control = hotbarGui;
			context.RobustNameScope.Register("Hotbar", control);
			hotbarGui.Access = new AccessLevel?(1);
			control = hotbarGui;
			A_1.XamlChildren.Add(control);
			ActionsBar actionsBar = new ActionsBar();
			actionsBar.Name = "Actions";
			control = actionsBar;
			context.RobustNameScope.Register("Actions", control);
			actionsBar.Access = new AccessLevel?(1);
			control = actionsBar;
			A_1.XamlChildren.Add(control);
			ResizableChatBox resizableChatBox = new ResizableChatBox();
			resizableChatBox.Name = "Chat";
			control = resizableChatBox;
			context.RobustNameScope.Register("Chat", control);
			resizableChatBox.Access = new AccessLevel?(1);
			control = resizableChatBox;
			A_1.XamlChildren.Add(control);
			AlertsUI alertsUI = new AlertsUI();
			alertsUI.Name = "Alerts";
			control = alertsUI;
			context.RobustNameScope.Register("Alerts", control);
			alertsUI.Access = new AccessLevel?(1);
			control = alertsUI;
			A_1.XamlChildren.Add(control);
			if ((control = (A_1 as Control)) != null)
			{
				context.RobustNameScope.Absorb(control.NameScope);
				control.NameScope = context.RobustNameScope;
			}
			context.RobustNameScope.Complete();
		}

		// Token: 0x060005D6 RID: 1494 RVA: 0x0001FF8A File Offset: 0x0001E18A
		private static void !XamlIlPopulateTrampoline(DefaultGameScreen A_0)
		{
			DefaultGameScreen.Populate:Content.Client.UserInterface.Screens.DefaultGameScreen.xaml(null, A_0);
		}
	}
}