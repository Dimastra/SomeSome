﻿using System;
using System.Runtime.CompilerServices;
using CompiledRobustXaml;
using Content.Client.Changelog;
using Content.Client.UserInterface.Controls;
using Robust.Client.AutoGenerated;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared;
using Robust.Shared.Configuration;
using Robust.Shared.Maths;

namespace Content.Client.MainMenu.UI
{
	// Token: 0x02000250 RID: 592
	[GenerateTypedNameReferences]
	public sealed class MainMenuControl : Control
	{
		// Token: 0x06000EEE RID: 3822 RVA: 0x00059E84 File Offset: 0x00058084
		[NullableContext(1)]
		public MainMenuControl(IResourceCache resCache, IConfigurationManager configMan)
		{
			MainMenuControl.!XamlIlPopulateTrampoline(this);
			LayoutContainer.SetAnchorPreset(this, 15, false);
			LayoutContainer.SetAnchorPreset(this.VBox, 8, false);
			LayoutContainer.SetGrowHorizontal(this.VBox, 2);
			LayoutContainer.SetGrowVertical(this.VBox, 2);
			string cvar = configMan.GetCVar<string>(CVars.PlayerName);
			this.UsernameBox.Text = cvar;
		}

		// Token: 0x17000325 RID: 805
		// (get) Token: 0x06000EEF RID: 3823 RVA: 0x00059EE3 File Offset: 0x000580E3
		private BoxContainer VBox
		{
			get
			{
				return base.FindControl<BoxContainer>("VBox");
			}
		}

		// Token: 0x17000326 RID: 806
		// (get) Token: 0x06000EF0 RID: 3824 RVA: 0x0000F66E File Offset: 0x0000D86E
		private Label Title
		{
			get
			{
				return base.FindControl<Label>("Title");
			}
		}

		// Token: 0x17000327 RID: 807
		// (get) Token: 0x06000EF1 RID: 3825 RVA: 0x00059EF0 File Offset: 0x000580F0
		public LineEdit UsernameBox
		{
			get
			{
				return base.FindControl<LineEdit>("UsernameBox");
			}
		}

		// Token: 0x17000328 RID: 808
		// (get) Token: 0x06000EF2 RID: 3826 RVA: 0x00059EFD File Offset: 0x000580FD
		public LineEdit AddressBox
		{
			get
			{
				return base.FindControl<LineEdit>("AddressBox");
			}
		}

		// Token: 0x17000329 RID: 809
		// (get) Token: 0x06000EF3 RID: 3827 RVA: 0x00059F0A File Offset: 0x0005810A
		public CommonButton DirectConnectButton
		{
			get
			{
				return base.FindControl<CommonButton>("DirectConnectButton");
			}
		}

		// Token: 0x1700032A RID: 810
		// (get) Token: 0x06000EF4 RID: 3828 RVA: 0x00059F17 File Offset: 0x00058117
		public CommonButton OptionsButton
		{
			get
			{
				return base.FindControl<CommonButton>("OptionsButton");
			}
		}

		// Token: 0x1700032B RID: 811
		// (get) Token: 0x06000EF5 RID: 3829 RVA: 0x0004B2C4 File Offset: 0x000494C4
		public ChangelogButton ChangelogButton
		{
			get
			{
				return base.FindControl<ChangelogButton>("ChangelogButton");
			}
		}

		// Token: 0x1700032C RID: 812
		// (get) Token: 0x06000EF6 RID: 3830 RVA: 0x00059F24 File Offset: 0x00058124
		public CommonButton QuitButton
		{
			get
			{
				return base.FindControl<CommonButton>("QuitButton");
			}
		}

		// Token: 0x06000EF7 RID: 3831 RVA: 0x00059F34 File Offset: 0x00058134
		static void xaml(IServiceProvider A_0, Control A_1)
		{
			XamlIlContext.Context<Control> context = new XamlIlContext.Context<Control>(A_0, null, "resm:Content.Client.MainMenu.UI.MainMenuControl.xaml?assembly=Content.Client");
			context.RootObject = A_1;
			context.IntermediateRoot = A_1;
			Control control = new BackgroundControl
			{
				TexturePath = "/Textures/main_menu_background.png",
				Stretch = 8
			};
			A_1.XamlChildren.Add(control);
			LayoutContainer layoutContainer = new LayoutContainer();
			BoxContainer boxContainer = new BoxContainer();
			boxContainer.Name = "VBox";
			control = boxContainer;
			context.RobustNameScope.Register("VBox", control);
			boxContainer.Orientation = 1;
			boxContainer.StyleIdentifier = "MainMenuBox";
			boxContainer.MinWidth = 300f;
			Label label = new Label();
			label.Name = "Title";
			control = label;
			context.RobustNameScope.Register("Title", control);
			label.Text = "WHITE DREAM";
			label.StyleIdentifier = "MainMenuTitle";
			label.Align = 1;
			control = label;
			boxContainer.XamlChildren.Add(control);
			BoxContainer boxContainer2 = new BoxContainer();
			boxContainer2.Orientation = 0;
			boxContainer2.SeparationOverride = new int?(4);
			LineEdit lineEdit = new LineEdit();
			lineEdit.Name = "UsernameBox";
			control = lineEdit;
			context.RobustNameScope.Register("UsernameBox", control);
			lineEdit.Access = new AccessLevel?(0);
			lineEdit.PlaceHolder = (string)new LocExtension("main-menu-username-text").ProvideValue();
			lineEdit.HorizontalExpand = true;
			control = lineEdit;
			boxContainer2.XamlChildren.Add(control);
			control = boxContainer2;
			boxContainer.XamlChildren.Add(control);
			LineEdit lineEdit2 = new LineEdit();
			lineEdit2.Name = "AddressBox";
			control = lineEdit2;
			context.RobustNameScope.Register("AddressBox", control);
			lineEdit2.Access = new AccessLevel?(0);
			lineEdit2.Text = "localhost";
			lineEdit2.PlaceHolder = "server address:port";
			lineEdit2.HorizontalExpand = true;
			control = lineEdit2;
			boxContainer.XamlChildren.Add(control);
			CommonButton commonButton = new CommonButton();
			commonButton.Name = "DirectConnectButton";
			control = commonButton;
			context.RobustNameScope.Register("DirectConnectButton", control);
			commonButton.Access = new AccessLevel?(0);
			commonButton.Text = (string)new LocExtension("main-menu-direct-connect-button").ProvideValue();
			commonButton.TextAlign = 1;
			string item = "MainMenu";
			commonButton.StyleClasses.Add(item);
			control = commonButton;
			boxContainer.XamlChildren.Add(control);
			control = new Control
			{
				MinSize = new Vector2(0f, 2f)
			};
			boxContainer.XamlChildren.Add(control);
			CommonButton commonButton2 = new CommonButton();
			commonButton2.Name = "OptionsButton";
			control = commonButton2;
			context.RobustNameScope.Register("OptionsButton", control);
			commonButton2.Access = new AccessLevel?(0);
			commonButton2.Text = (string)new LocExtension("main-menu-options-button").ProvideValue();
			commonButton2.TextAlign = 1;
			item = "MainMenu";
			commonButton2.StyleClasses.Add(item);
			control = commonButton2;
			boxContainer.XamlChildren.Add(control);
			ChangelogButton changelogButton = new ChangelogButton();
			changelogButton.Name = "ChangelogButton";
			control = changelogButton;
			context.RobustNameScope.Register("ChangelogButton", control);
			changelogButton.Access = new AccessLevel?(0);
			item = "MainMenu";
			changelogButton.StyleClasses.Add(item);
			control = changelogButton;
			boxContainer.XamlChildren.Add(control);
			CommonButton commonButton3 = new CommonButton();
			commonButton3.Name = "QuitButton";
			control = commonButton3;
			context.RobustNameScope.Register("QuitButton", control);
			commonButton3.Access = new AccessLevel?(0);
			commonButton3.Text = (string)new LocExtension("main-menu-quit-button").ProvideValue();
			commonButton3.TextAlign = 1;
			item = "MainMenu";
			commonButton3.StyleClasses.Add(item);
			control = commonButton3;
			boxContainer.XamlChildren.Add(control);
			control = boxContainer;
			layoutContainer.XamlChildren.Add(control);
			control = layoutContainer;
			A_1.XamlChildren.Add(control);
			if ((control = (A_1 as Control)) != null)
			{
				context.RobustNameScope.Absorb(control.NameScope);
				control.NameScope = context.RobustNameScope;
			}
			context.RobustNameScope.Complete();
		}

		// Token: 0x06000EF8 RID: 3832 RVA: 0x0005A409 File Offset: 0x00058609
		private static void !XamlIlPopulateTrampoline(MainMenuControl A_0)
		{
			MainMenuControl.Populate:Content.Client.MainMenu.UI.MainMenuControl.xaml(null, A_0);
		}
	}
}
