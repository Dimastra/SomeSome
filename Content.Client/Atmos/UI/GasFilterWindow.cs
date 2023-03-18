﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using CompiledRobustXaml;
using Content.Shared.Atmos.Prototypes;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Localization;
using Robust.Shared.Maths;

namespace Content.Client.Atmos.UI
{
	// Token: 0x02000439 RID: 1081
	[GenerateTypedNameReferences]
	public sealed class GasFilterWindow : DefaultWindow
	{
		// Token: 0x1400009D RID: 157
		// (add) Token: 0x06001A81 RID: 6785 RVA: 0x00098700 File Offset: 0x00096900
		// (remove) Token: 0x06001A82 RID: 6786 RVA: 0x00098738 File Offset: 0x00096938
		[Nullable(2)]
		[method: NullableContext(2)]
		[Nullable(2)]
		public event Action ToggleStatusButtonPressed;

		// Token: 0x1400009E RID: 158
		// (add) Token: 0x06001A83 RID: 6787 RVA: 0x00098770 File Offset: 0x00096970
		// (remove) Token: 0x06001A84 RID: 6788 RVA: 0x000987A8 File Offset: 0x000969A8
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[Nullable(new byte[]
		{
			2,
			1
		})]
		public event Action<string> FilterTransferRateChanged;

		// Token: 0x1400009F RID: 159
		// (add) Token: 0x06001A85 RID: 6789 RVA: 0x000987E0 File Offset: 0x000969E0
		// (remove) Token: 0x06001A86 RID: 6790 RVA: 0x00098818 File Offset: 0x00096A18
		[Nullable(2)]
		[method: NullableContext(2)]
		[Nullable(2)]
		public event Action SelectGasPressed;

		// Token: 0x06001A87 RID: 6791 RVA: 0x00098850 File Offset: 0x00096A50
		[NullableContext(1)]
		public GasFilterWindow(IEnumerable<GasPrototype> gases)
		{
			GasFilterWindow.!XamlIlPopulateTrampoline(this);
			this.PopulateGasList(gases);
			this.ToggleStatusButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				this.SetFilterStatus(!this.FilterStatus);
			};
			this.ToggleStatusButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				Action toggleStatusButtonPressed = this.ToggleStatusButtonPressed;
				if (toggleStatusButtonPressed == null)
				{
					return;
				}
				toggleStatusButtonPressed();
			};
			this.FilterTransferRateInput.OnTextChanged += delegate(LineEdit.LineEditEventArgs _)
			{
				this.SetFilterRate.Disabled = false;
			};
			this.SetFilterRate.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				Action<string> filterTransferRateChanged = this.FilterTransferRateChanged;
				if (filterTransferRateChanged != null)
				{
					filterTransferRateChanged(this.FilterTransferRateInput.Text);
				}
				this.SetFilterRate.Disabled = true;
			};
			this.SelectGasButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				Action selectGasPressed = this.SelectGasPressed;
				if (selectGasPressed == null)
				{
					return;
				}
				selectGasPressed();
			};
			this.GasList.OnItemSelected += this.GasListOnItemSelected;
			this.GasList.OnItemDeselected += this.GasListOnItemDeselected;
		}

		// Token: 0x06001A88 RID: 6792 RVA: 0x00098923 File Offset: 0x00096B23
		public void SetTransferRate(float rate)
		{
			this.FilterTransferRateInput.Text = rate.ToString(CultureInfo.InvariantCulture);
		}

		// Token: 0x06001A89 RID: 6793 RVA: 0x0009893C File Offset: 0x00096B3C
		public void SetFilterStatus(bool enabled)
		{
			this.FilterStatus = enabled;
			if (enabled)
			{
				this.ToggleStatusButton.Text = Loc.GetString("comp-gas-filter-ui-status-enabled");
				return;
			}
			this.ToggleStatusButton.Text = Loc.GetString("comp-gas-filter-ui-status-disabled");
		}

		// Token: 0x06001A8A RID: 6794 RVA: 0x00098974 File Offset: 0x00096B74
		[NullableContext(1)]
		public void SetGasFiltered([Nullable(2)] string id, string name)
		{
			this.CurrentGasId = id;
			this.CurrentGasLabel.Text = Loc.GetString("comp-gas-filter-ui-filter-gas-current") + " " + name;
			this.GasList.ClearSelected(null);
			this.SelectGasButton.Disabled = true;
		}

		// Token: 0x06001A8B RID: 6795 RVA: 0x000989C8 File Offset: 0x00096BC8
		[NullableContext(1)]
		private void PopulateGasList(IEnumerable<GasPrototype> gases)
		{
			this.GasList.Add(new ItemList.Item(this.GasList)
			{
				Metadata = null,
				Text = Loc.GetString("comp-gas-filter-ui-filter-gas-none")
			});
			foreach (GasPrototype gasPrototype in gases)
			{
				string @string = Loc.GetString(gasPrototype.Name);
				this.GasList.Add(GasFilterWindow.GetGasItem(gasPrototype.ID, @string, this.GasList));
			}
		}

		// Token: 0x06001A8C RID: 6796 RVA: 0x00098A60 File Offset: 0x00096C60
		[NullableContext(1)]
		private static ItemList.Item GetGasItem(string id, string name, ItemList itemList)
		{
			return new ItemList.Item(itemList)
			{
				Metadata = id,
				Text = name
			};
		}

		// Token: 0x06001A8D RID: 6797 RVA: 0x00098A78 File Offset: 0x00096C78
		[NullableContext(1)]
		private void GasListOnItemSelected(ItemList.ItemListSelectedEventArgs obj)
		{
			this.SelectedGas = (string)obj.ItemList.get_IndexItem(obj.ItemIndex).Metadata;
			if (this.SelectedGas != this.CurrentGasId)
			{
				this.SelectGasButton.Disabled = false;
			}
		}

		// Token: 0x06001A8E RID: 6798 RVA: 0x00098AC5 File Offset: 0x00096CC5
		[NullableContext(1)]
		private void GasListOnItemDeselected(ItemList.ItemListDeselectedEventArgs obj)
		{
			this.SelectedGas = this.CurrentGasId;
			this.SelectGasButton.Disabled = true;
		}

		// Token: 0x1700057A RID: 1402
		// (get) Token: 0x06001A8F RID: 6799 RVA: 0x00098ADF File Offset: 0x00096CDF
		private Button ToggleStatusButton
		{
			get
			{
				return base.FindControl<Button>("ToggleStatusButton");
			}
		}

		// Token: 0x1700057B RID: 1403
		// (get) Token: 0x06001A90 RID: 6800 RVA: 0x00098AEC File Offset: 0x00096CEC
		private LineEdit FilterTransferRateInput
		{
			get
			{
				return base.FindControl<LineEdit>("FilterTransferRateInput");
			}
		}

		// Token: 0x1700057C RID: 1404
		// (get) Token: 0x06001A91 RID: 6801 RVA: 0x00098AF9 File Offset: 0x00096CF9
		private Button SetFilterRate
		{
			get
			{
				return base.FindControl<Button>("SetFilterRate");
			}
		}

		// Token: 0x1700057D RID: 1405
		// (get) Token: 0x06001A92 RID: 6802 RVA: 0x00098B06 File Offset: 0x00096D06
		private Label CurrentGasLabel
		{
			get
			{
				return base.FindControl<Label>("CurrentGasLabel");
			}
		}

		// Token: 0x1700057E RID: 1406
		// (get) Token: 0x06001A93 RID: 6803 RVA: 0x00098B13 File Offset: 0x00096D13
		private ItemList GasList
		{
			get
			{
				return base.FindControl<ItemList>("GasList");
			}
		}

		// Token: 0x1700057F RID: 1407
		// (get) Token: 0x06001A94 RID: 6804 RVA: 0x00098B20 File Offset: 0x00096D20
		private Button SelectGasButton
		{
			get
			{
				return base.FindControl<Button>("SelectGasButton");
			}
		}

		// Token: 0x06001A9A RID: 6810 RVA: 0x00098B9C File Offset: 0x00096D9C
		static void xaml(IServiceProvider A_0, DefaultWindow A_1)
		{
			XamlIlContext.Context<DefaultWindow> context = new XamlIlContext.Context<DefaultWindow>(A_0, null, "resm:Content.Client.Atmos.UI.GasFilterWindow.xaml?assembly=Content.Client");
			context.RootObject = A_1;
			context.IntermediateRoot = A_1;
			A_1.MinSize = new Vector2(480f, 400f);
			A_1.Title = "Filter";
			BoxContainer boxContainer = new BoxContainer();
			boxContainer.Orientation = 1;
			boxContainer.Margin = new Thickness(5f, 5f, 5f, 5f);
			boxContainer.SeparationOverride = new int?(10);
			BoxContainer boxContainer2 = new BoxContainer();
			boxContainer2.Orientation = 0;
			boxContainer2.HorizontalExpand = true;
			Control control = new Label
			{
				Text = (string)new LocExtension("comp-gas-filter-ui-filter-status").ProvideValue()
			};
			boxContainer2.XamlChildren.Add(control);
			Button button = new Button();
			button.Name = "ToggleStatusButton";
			control = button;
			context.RobustNameScope.Register("ToggleStatusButton", control);
			control = button;
			boxContainer2.XamlChildren.Add(control);
			control = boxContainer2;
			boxContainer.XamlChildren.Add(control);
			BoxContainer boxContainer3 = new BoxContainer();
			boxContainer3.Orientation = 0;
			boxContainer3.HorizontalExpand = true;
			control = new Label
			{
				Text = (string)new LocExtension("comp-gas-filter-ui-filter-transfer-rate").ProvideValue()
			};
			boxContainer3.XamlChildren.Add(control);
			LineEdit lineEdit = new LineEdit();
			lineEdit.Name = "FilterTransferRateInput";
			control = lineEdit;
			context.RobustNameScope.Register("FilterTransferRateInput", control);
			lineEdit.MinSize = new Vector2(40f, 0f);
			control = lineEdit;
			boxContainer3.XamlChildren.Add(control);
			Button button2 = new Button();
			button2.Name = "SetFilterRate";
			control = button2;
			context.RobustNameScope.Register("SetFilterRate", control);
			button2.Text = (string)new LocExtension("comp-gas-filter-ui-filter-set-rate").ProvideValue();
			button2.Disabled = true;
			control = button2;
			boxContainer3.XamlChildren.Add(control);
			control = boxContainer3;
			boxContainer.XamlChildren.Add(control);
			BoxContainer boxContainer4 = new BoxContainer();
			boxContainer4.Orientation = 0;
			boxContainer4.HorizontalExpand = true;
			Label label = new Label();
			label.Name = "CurrentGasLabel";
			control = label;
			context.RobustNameScope.Register("CurrentGasLabel", control);
			control = label;
			boxContainer4.XamlChildren.Add(control);
			control = boxContainer4;
			boxContainer.XamlChildren.Add(control);
			BoxContainer boxContainer5 = new BoxContainer();
			boxContainer5.Orientation = 1;
			boxContainer5.VerticalExpand = true;
			control = new Label
			{
				Text = (string)new LocExtension("comp-gas-filter-ui-filter-gas-select").ProvideValue()
			};
			boxContainer5.XamlChildren.Add(control);
			ItemList itemList = new ItemList();
			itemList.Name = "GasList";
			control = itemList;
			context.RobustNameScope.Register("GasList", control);
			itemList.SelectMode = 1;
			itemList.VerticalExpand = true;
			itemList.SizeFlagsStretchRatio = 0.9f;
			control = itemList;
			boxContainer5.XamlChildren.Add(control);
			Button button3 = new Button();
			button3.Name = "SelectGasButton";
			control = button3;
			context.RobustNameScope.Register("SelectGasButton", control);
			button3.Text = (string)new LocExtension("comp-gas-filter-ui-filter-gas-confirm").ProvideValue();
			button3.HorizontalExpand = true;
			button3.Disabled = true;
			control = button3;
			boxContainer5.XamlChildren.Add(control);
			control = boxContainer5;
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

		// Token: 0x06001A9B RID: 6811 RVA: 0x00098FE3 File Offset: 0x000971E3
		private static void !XamlIlPopulateTrampoline(GasFilterWindow A_0)
		{
			GasFilterWindow.Populate:Content.Client.Atmos.UI.GasFilterWindow.xaml(null, A_0);
		}

		// Token: 0x04000D67 RID: 3431
		[Nullable(1)]
		private readonly ButtonGroup _buttonGroup = new ButtonGroup();

		// Token: 0x04000D68 RID: 3432
		public bool FilterStatus = true;

		// Token: 0x04000D69 RID: 3433
		[Nullable(2)]
		public string SelectedGas;

		// Token: 0x04000D6A RID: 3434
		[Nullable(2)]
		public string CurrentGasId;
	}
}