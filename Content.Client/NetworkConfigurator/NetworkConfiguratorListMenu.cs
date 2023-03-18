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
	// Token: 0x02000223 RID: 547
	[GenerateTypedNameReferences]
	public sealed class NetworkConfiguratorListMenu : FancyWindow
	{
		// Token: 0x06000E3D RID: 3645 RVA: 0x00056420 File Offset: 0x00054620
		[NullableContext(1)]
		public NetworkConfiguratorListMenu(NetworkConfiguratorBoundUserInterface ui)
		{
			NetworkConfiguratorListMenu.!XamlIlPopulateTrampoline(this);
			this._ui = ui;
		}

		// Token: 0x06000E3E RID: 3646 RVA: 0x00056438 File Offset: 0x00054638
		[NullableContext(1)]
		public void UpdateState(NetworkConfiguratorUserInterfaceState state)
		{
			this.DeviceCountLabel.Text = Loc.GetString("network-configurator-ui-count-label", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("count", state.DeviceList.Count)
			});
			this.DeviceList.UpdateState(this._ui, state.DeviceList);
		}

		// Token: 0x1700030D RID: 781
		// (get) Token: 0x06000E3F RID: 3647 RVA: 0x000559BF File Offset: 0x00053BBF
		private NetworkConfiguratorDeviceList DeviceList
		{
			get
			{
				return base.FindControl<NetworkConfiguratorDeviceList>("DeviceList");
			}
		}

		// Token: 0x1700030E RID: 782
		// (get) Token: 0x06000E40 RID: 3648 RVA: 0x00056498 File Offset: 0x00054698
		private Label DeviceCountLabel
		{
			get
			{
				return base.FindControl<Label>("DeviceCountLabel");
			}
		}

		// Token: 0x1700030F RID: 783
		// (get) Token: 0x06000E41 RID: 3649 RVA: 0x0001E1D6 File Offset: 0x0001C3D6
		public Button ClearButton
		{
			get
			{
				return base.FindControl<Button>("ClearButton");
			}
		}

		// Token: 0x06000E42 RID: 3650 RVA: 0x000564A8 File Offset: 0x000546A8
		static void xaml(IServiceProvider A_0, FancyWindow A_1)
		{
			XamlIlContext.Context<FancyWindow> context = new XamlIlContext.Context<FancyWindow>(A_0, null, "resm:Content.Client.NetworkConfigurator.NetworkConfiguratorListMenu.xaml?assembly=Content.Client");
			context.RootObject = A_1;
			context.IntermediateRoot = A_1;
			A_1.Title = "Network Configurator";
			A_1.MinSize = new Vector2(220f, 400f);
			BoxContainer boxContainer = new BoxContainer();
			boxContainer.Orientation = 1;
			boxContainer.VerticalExpand = true;
			NetworkConfiguratorDeviceList networkConfiguratorDeviceList = new NetworkConfiguratorDeviceList();
			networkConfiguratorDeviceList.Name = "DeviceList";
			Control control = networkConfiguratorDeviceList;
			context.RobustNameScope.Register("DeviceList", control);
			control = networkConfiguratorDeviceList;
			boxContainer.XamlChildren.Add(control);
			BoxContainer boxContainer2 = new BoxContainer();
			boxContainer2.Orientation = 0;
			boxContainer2.Margin = new Thickness(8f, 8f, 8f, 8f);
			Label label = new Label();
			label.Name = "DeviceCountLabel";
			control = label;
			context.RobustNameScope.Register("DeviceCountLabel", control);
			label.Margin = new Thickness(16f, 0f, 0f, 0f);
			label.MaxWidth = 64f;
			control = label;
			boxContainer2.XamlChildren.Add(control);
			control = new Control
			{
				HorizontalExpand = true
			};
			boxContainer2.XamlChildren.Add(control);
			Button button = new Button();
			button.Name = "ClearButton";
			control = button;
			context.RobustNameScope.Register("ClearButton", control);
			button.Access = new AccessLevel?(0);
			button.Text = (string)new LocExtension("network-configurator-ui-clear-button").ProvideValue();
			control = button;
			boxContainer2.XamlChildren.Add(control);
			control = boxContainer2;
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

		// Token: 0x06000E43 RID: 3651 RVA: 0x000566E6 File Offset: 0x000548E6
		private static void !XamlIlPopulateTrampoline(NetworkConfiguratorListMenu A_0)
		{
			NetworkConfiguratorListMenu.Populate:Content.Client.NetworkConfigurator.NetworkConfiguratorListMenu.xaml(null, A_0);
		}

		// Token: 0x0400070B RID: 1803
		[Nullable(1)]
		private readonly NetworkConfiguratorBoundUserInterface _ui;
	}
}
