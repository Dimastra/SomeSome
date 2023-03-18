﻿using System;
using System.Runtime.CompilerServices;
using CompiledRobustXaml;
using Content.Client.UserInterface.Controls;
using Content.Shared.DeviceNetwork;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Localization;
using Robust.Shared.Maths;

namespace Content.Client.NetworkConfigurator
{
	// Token: 0x0200021F RID: 543
	[GenerateTypedNameReferences]
	public sealed class NetworkConfiguratorConfigurationMenu : FancyWindow
	{
		// Token: 0x06000E26 RID: 3622 RVA: 0x00055929 File Offset: 0x00053B29
		public NetworkConfiguratorConfigurationMenu()
		{
			NetworkConfiguratorConfigurationMenu.!XamlIlPopulateTrampoline(this);
			this.Clear.StyleClasses.Add("OpenLeft");
			this.Clear.StyleClasses.Add("ButtonColorRed");
		}

		// Token: 0x06000E27 RID: 3623 RVA: 0x00055964 File Offset: 0x00053B64
		[NullableContext(1)]
		public void UpdateState(DeviceListUserInterfaceState state)
		{
			this.DeviceList.UpdateState(null, state.DeviceList);
			this.Count.Text = Loc.GetString("network-configurator-ui-count-label", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("count", state.DeviceList.Count)
			});
		}

		// Token: 0x17000304 RID: 772
		// (get) Token: 0x06000E28 RID: 3624 RVA: 0x000559BF File Offset: 0x00053BBF
		private NetworkConfiguratorDeviceList DeviceList
		{
			get
			{
				return base.FindControl<NetworkConfiguratorDeviceList>("DeviceList");
			}
		}

		// Token: 0x17000305 RID: 773
		// (get) Token: 0x06000E29 RID: 3625 RVA: 0x000559CC File Offset: 0x00053BCC
		public Button Set
		{
			get
			{
				return base.FindControl<Button>("Set");
			}
		}

		// Token: 0x17000306 RID: 774
		// (get) Token: 0x06000E2A RID: 3626 RVA: 0x000559D9 File Offset: 0x00053BD9
		public Button Add
		{
			get
			{
				return base.FindControl<Button>("Add");
			}
		}

		// Token: 0x17000307 RID: 775
		// (get) Token: 0x06000E2B RID: 3627 RVA: 0x000559E6 File Offset: 0x00053BE6
		public Button Clear
		{
			get
			{
				return base.FindControl<Button>("Clear");
			}
		}

		// Token: 0x17000308 RID: 776
		// (get) Token: 0x06000E2C RID: 3628 RVA: 0x000559F3 File Offset: 0x00053BF3
		public Button Copy
		{
			get
			{
				return base.FindControl<Button>("Copy");
			}
		}

		// Token: 0x17000309 RID: 777
		// (get) Token: 0x06000E2D RID: 3629 RVA: 0x00055A00 File Offset: 0x00053C00
		public Button Show
		{
			get
			{
				return base.FindControl<Button>("Show");
			}
		}

		// Token: 0x1700030A RID: 778
		// (get) Token: 0x06000E2E RID: 3630 RVA: 0x00055A0D File Offset: 0x00053C0D
		private Label Count
		{
			get
			{
				return base.FindControl<Label>("Count");
			}
		}

		// Token: 0x06000E2F RID: 3631 RVA: 0x00055A1C File Offset: 0x00053C1C
		static void xaml(IServiceProvider A_0, FancyWindow A_1)
		{
			XamlIlContext.Context<FancyWindow> context = new XamlIlContext.Context<FancyWindow>(A_0, null, "resm:Content.Client.NetworkConfigurator.NetworkConfiguratorConfigurationMenu.xaml?assembly=Content.Client");
			context.RootObject = A_1;
			context.IntermediateRoot = A_1;
			A_1.Title = "Network Configurator";
			A_1.MinSize = new Vector2(350f, 100f);
			BoxContainer boxContainer = new BoxContainer();
			boxContainer.Orientation = 1;
			boxContainer.VerticalExpand = true;
			boxContainer.HorizontalExpand = true;
			NetworkConfiguratorDeviceList networkConfiguratorDeviceList = new NetworkConfiguratorDeviceList();
			networkConfiguratorDeviceList.Name = "DeviceList";
			Control control = networkConfiguratorDeviceList;
			context.RobustNameScope.Register("DeviceList", control);
			networkConfiguratorDeviceList.MinHeight = 500f;
			control = networkConfiguratorDeviceList;
			boxContainer.XamlChildren.Add(control);
			BoxContainer boxContainer2 = new BoxContainer();
			boxContainer2.Orientation = 0;
			boxContainer2.HorizontalExpand = true;
			boxContainer2.Margin = new Thickness(8f, 8f, 8f, 1f);
			Button button = new Button();
			button.Name = "Set";
			control = button;
			context.RobustNameScope.Register("Set", control);
			button.Text = "Set";
			button.Access = new AccessLevel?(0);
			button.ToolTip = (string)new LocExtension("network-configurator-tooltip-set").ProvideValue();
			button.HorizontalExpand = true;
			string item = "ButtonSquare";
			button.StyleClasses.Add(item);
			control = button;
			boxContainer2.XamlChildren.Add(control);
			Button button2 = new Button();
			button2.Name = "Add";
			control = button2;
			context.RobustNameScope.Register("Add", control);
			button2.Text = "Add";
			button2.Access = new AccessLevel?(0);
			button2.ToolTip = (string)new LocExtension("network-configurator-tooltip-add").ProvideValue();
			button2.HorizontalExpand = true;
			item = "ButtonSquare";
			button2.StyleClasses.Add(item);
			control = button2;
			boxContainer2.XamlChildren.Add(control);
			Button button3 = new Button();
			button3.Name = "Clear";
			control = button3;
			context.RobustNameScope.Register("Clear", control);
			button3.Text = "Clear";
			button3.Access = new AccessLevel?(0);
			button3.ToolTip = (string)new LocExtension("network-configurator-tooltip-clear").ProvideValue();
			button3.HorizontalExpand = true;
			control = button3;
			boxContainer2.XamlChildren.Add(control);
			control = boxContainer2;
			boxContainer.XamlChildren.Add(control);
			BoxContainer boxContainer3 = new BoxContainer();
			boxContainer3.Orientation = 0;
			boxContainer3.HorizontalExpand = true;
			boxContainer3.Margin = new Thickness(8f, 0f, 8f, 8f);
			Button button4 = new Button();
			button4.Name = "Copy";
			control = button4;
			context.RobustNameScope.Register("Copy", control);
			button4.Text = "Copy";
			button4.Access = new AccessLevel?(0);
			button4.ToolTip = (string)new LocExtension("network-configurator-tooltip-copy").ProvideValue();
			button4.HorizontalExpand = true;
			item = "OpenRight";
			button4.StyleClasses.Add(item);
			control = button4;
			boxContainer3.XamlChildren.Add(control);
			Button button5 = new Button();
			button5.Name = "Show";
			control = button5;
			context.RobustNameScope.Register("Show", control);
			button5.Text = "Show";
			button5.Access = new AccessLevel?(0);
			button5.ToggleMode = true;
			button5.ToolTip = (string)new LocExtension("network-configurator-tooltip-show").ProvideValue();
			button5.HorizontalExpand = true;
			item = "ButtonSquare";
			button5.StyleClasses.Add(item);
			control = button5;
			boxContainer3.XamlChildren.Add(control);
			control = boxContainer3;
			boxContainer.XamlChildren.Add(control);
			Label label = new Label();
			label.Name = "Count";
			control = label;
			context.RobustNameScope.Register("Count", control);
			label.HorizontalAlignment = 3;
			control = label;
			boxContainer.XamlChildren.Add(control);
			control = boxContainer;
			A_1.XamlChildren.Add(control);
			if ((control = (A_1 as Control)) != null)
			{
				context.RobustNameScope.Absorb(control.NameScope);
				control.NameScope = context.RobustNameScope;
			}
			context.RobustNameScope.Complete();
		}

		// Token: 0x06000E30 RID: 3632 RVA: 0x00055F0B File Offset: 0x0005410B
		private static void !XamlIlPopulateTrampoline(NetworkConfiguratorConfigurationMenu A_0)
		{
			NetworkConfiguratorConfigurationMenu.Populate:Content.Client.NetworkConfigurator.NetworkConfiguratorConfigurationMenu.xaml(null, A_0);
		}
	}
}
