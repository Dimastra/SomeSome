﻿using System;
using System.Runtime.CompilerServices;
using CompiledRobustXaml;
using Content.Shared.Atmos.Monitor.Components;
using Content.Shared.Atmos.Piping.Unary.Components;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Localization;
using Robust.Shared.Maths;

namespace Content.Client.Atmos.Monitor.UI.Widgets
{
	// Token: 0x0200044B RID: 1099
	[NullableContext(1)]
	[Nullable(0)]
	[GenerateTypedNameReferences]
	public sealed class PumpControl : BoxContainer
	{
		// Token: 0x140000AD RID: 173
		// (add) Token: 0x06001B41 RID: 6977 RVA: 0x0009D460 File Offset: 0x0009B660
		// (remove) Token: 0x06001B42 RID: 6978 RVA: 0x0009D498 File Offset: 0x0009B698
		[Nullable(new byte[]
		{
			2,
			1,
			1
		})]
		[Nullable(new byte[]
		{
			2,
			1,
			1
		})]
		public event Action<string, IAtmosDeviceData> PumpDataChanged;

		// Token: 0x170005AB RID: 1451
		// (get) Token: 0x06001B43 RID: 6979 RVA: 0x0009D4CD File Offset: 0x0009B6CD
		private CheckBox _enabled
		{
			get
			{
				return this.CEnableDevice;
			}
		}

		// Token: 0x170005AC RID: 1452
		// (get) Token: 0x06001B44 RID: 6980 RVA: 0x0009D4D5 File Offset: 0x0009B6D5
		private CollapsibleHeading _addressLabel
		{
			get
			{
				return this.CAddress;
			}
		}

		// Token: 0x170005AD RID: 1453
		// (get) Token: 0x06001B45 RID: 6981 RVA: 0x0009D4DD File Offset: 0x0009B6DD
		private OptionButton _pumpDirection
		{
			get
			{
				return this.CPumpDirection;
			}
		}

		// Token: 0x170005AE RID: 1454
		// (get) Token: 0x06001B46 RID: 6982 RVA: 0x0009D4E5 File Offset: 0x0009B6E5
		private OptionButton _pressureCheck
		{
			get
			{
				return this.CPressureCheck;
			}
		}

		// Token: 0x170005AF RID: 1455
		// (get) Token: 0x06001B47 RID: 6983 RVA: 0x0009D4ED File Offset: 0x0009B6ED
		private FloatSpinBox _externalBound
		{
			get
			{
				return this.CExternalBound;
			}
		}

		// Token: 0x170005B0 RID: 1456
		// (get) Token: 0x06001B48 RID: 6984 RVA: 0x0009D4F5 File Offset: 0x0009B6F5
		private FloatSpinBox _internalBound
		{
			get
			{
				return this.CInternalBound;
			}
		}

		// Token: 0x06001B49 RID: 6985 RVA: 0x0009D500 File Offset: 0x0009B700
		public PumpControl(GasVentPumpData data, string address)
		{
			PumpControl.!XamlIlPopulateTrampoline(this);
			base.Name = address;
			this._data = data;
			this._address = address;
			this._addressLabel.Title = Loc.GetString("air-alarm-ui-atmos-net-device-label", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("address", address ?? "")
			});
			this._enabled.Pressed = data.Enabled;
			this._enabled.OnToggled += delegate(BaseButton.ButtonToggledEventArgs _)
			{
				this._data.Enabled = this._enabled.Pressed;
				Action<string, IAtmosDeviceData> pumpDataChanged = this.PumpDataChanged;
				if (pumpDataChanged == null)
				{
					return;
				}
				pumpDataChanged(this._address, this._data);
			};
			this._internalBound.Value = this._data.InternalPressureBound;
			this._internalBound.OnValueChanged += delegate(FloatSpinBox.FloatSpinBoxEventArgs _)
			{
				this._data.InternalPressureBound = this._internalBound.Value;
				Action<string, IAtmosDeviceData> pumpDataChanged = this.PumpDataChanged;
				if (pumpDataChanged == null)
				{
					return;
				}
				pumpDataChanged(this._address, this._data);
			};
			FloatSpinBox internalBound = this._internalBound;
			internalBound.IsValid = (Func<float, bool>)Delegate.Combine(internalBound.IsValid, new Func<float, bool>((float value) => value >= 0f));
			this._externalBound.Value = this._data.ExternalPressureBound;
			this._externalBound.OnValueChanged += delegate(FloatSpinBox.FloatSpinBoxEventArgs _)
			{
				this._data.ExternalPressureBound = this._externalBound.Value;
				Action<string, IAtmosDeviceData> pumpDataChanged = this.PumpDataChanged;
				if (pumpDataChanged == null)
				{
					return;
				}
				pumpDataChanged(this._address, this._data);
			};
			FloatSpinBox externalBound = this._externalBound;
			externalBound.IsValid = (Func<float, bool>)Delegate.Combine(externalBound.IsValid, new Func<float, bool>((float value) => value >= 0f));
			foreach (VentPumpDirection value3 in Enum.GetValues<VentPumpDirection>())
			{
				OptionButton pumpDirection = this._pumpDirection;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
				defaultInterpolatedStringHandler.AppendFormatted<VentPumpDirection>(value3);
				pumpDirection.AddItem(Loc.GetString(defaultInterpolatedStringHandler.ToStringAndClear()), new int?((int)value3));
			}
			this._pumpDirection.SelectId((int)this._data.PumpDirection);
			this._pumpDirection.OnItemSelected += delegate(OptionButton.ItemSelectedEventArgs args)
			{
				this._pumpDirection.SelectId(args.Id);
				this._data.PumpDirection = (VentPumpDirection)args.Id;
				Action<string, IAtmosDeviceData> pumpDataChanged = this.PumpDataChanged;
				if (pumpDataChanged == null)
				{
					return;
				}
				pumpDataChanged(this._address, this._data);
			};
			foreach (VentPressureBound value2 in Enum.GetValues<VentPressureBound>())
			{
				OptionButton pressureCheck = this._pressureCheck;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
				defaultInterpolatedStringHandler.AppendFormatted<VentPressureBound>(value2);
				pressureCheck.AddItem(Loc.GetString(defaultInterpolatedStringHandler.ToStringAndClear()), new int?((int)value2));
			}
			this._pressureCheck.SelectId((int)this._data.PressureChecks);
			this._pressureCheck.OnItemSelected += delegate(OptionButton.ItemSelectedEventArgs args)
			{
				this._pressureCheck.SelectId(args.Id);
				this._data.PressureChecks = (VentPressureBound)args.Id;
				Action<string, IAtmosDeviceData> pumpDataChanged = this.PumpDataChanged;
				if (pumpDataChanged == null)
				{
					return;
				}
				pumpDataChanged(this._address, this._data);
			};
		}

		// Token: 0x06001B4A RID: 6986 RVA: 0x0009D748 File Offset: 0x0009B948
		public void ChangeData(GasVentPumpData data)
		{
			this._data.Enabled = data.Enabled;
			this._enabled.Pressed = this._data.Enabled;
			this._data.PumpDirection = data.PumpDirection;
			this._pumpDirection.SelectId((int)this._data.PumpDirection);
			this._data.PressureChecks = data.PressureChecks;
			this._pressureCheck.SelectId((int)this._data.PressureChecks);
			this._data.ExternalPressureBound = data.ExternalPressureBound;
			this._externalBound.Value = this._data.ExternalPressureBound;
			this._data.InternalPressureBound = data.InternalPressureBound;
			this._internalBound.Value = this._data.InternalPressureBound;
		}

		// Token: 0x170005B1 RID: 1457
		// (get) Token: 0x06001B4B RID: 6987 RVA: 0x0009D818 File Offset: 0x0009BA18
		[Nullable(0)]
		private CollapsibleHeading CAddress
		{
			[NullableContext(0)]
			get
			{
				return base.FindControl<CollapsibleHeading>("CAddress");
			}
		}

		// Token: 0x170005B2 RID: 1458
		// (get) Token: 0x06001B4C RID: 6988 RVA: 0x0009D825 File Offset: 0x0009BA25
		[Nullable(0)]
		private CheckBox CEnableDevice
		{
			[NullableContext(0)]
			get
			{
				return base.FindControl<CheckBox>("CEnableDevice");
			}
		}

		// Token: 0x170005B3 RID: 1459
		// (get) Token: 0x06001B4D RID: 6989 RVA: 0x0009D832 File Offset: 0x0009BA32
		[Nullable(0)]
		private OptionButton CPumpDirection
		{
			[NullableContext(0)]
			get
			{
				return base.FindControl<OptionButton>("CPumpDirection");
			}
		}

		// Token: 0x170005B4 RID: 1460
		// (get) Token: 0x06001B4E RID: 6990 RVA: 0x0009D83F File Offset: 0x0009BA3F
		[Nullable(0)]
		private OptionButton CPressureCheck
		{
			[NullableContext(0)]
			get
			{
				return base.FindControl<OptionButton>("CPressureCheck");
			}
		}

		// Token: 0x170005B5 RID: 1461
		// (get) Token: 0x06001B4F RID: 6991 RVA: 0x0009D84C File Offset: 0x0009BA4C
		[Nullable(0)]
		private FloatSpinBox CExternalBound
		{
			[NullableContext(0)]
			get
			{
				return base.FindControl<FloatSpinBox>("CExternalBound");
			}
		}

		// Token: 0x170005B6 RID: 1462
		// (get) Token: 0x06001B50 RID: 6992 RVA: 0x0009D859 File Offset: 0x0009BA59
		[Nullable(0)]
		private FloatSpinBox CInternalBound
		{
			[NullableContext(0)]
			get
			{
				return base.FindControl<FloatSpinBox>("CInternalBound");
			}
		}

		// Token: 0x06001B56 RID: 6998 RVA: 0x0009D99C File Offset: 0x0009BB9C
		static void xaml(IServiceProvider A_0, BoxContainer A_1)
		{
			XamlIlContext.Context<BoxContainer> context = new XamlIlContext.Context<BoxContainer>(A_0, null, "resm:Content.Client.Atmos.Monitor.UI.Widgets.PumpControl.xaml?assembly=Content.Client");
			context.RootObject = A_1;
			context.IntermediateRoot = A_1;
			A_1.Orientation = 1;
			A_1.Margin = new Thickness(2f, 0f, 2f, 4f);
			Collapsible collapsible = new Collapsible();
			collapsible.Orientation = 1;
			CollapsibleHeading collapsibleHeading = new CollapsibleHeading();
			collapsibleHeading.Name = "CAddress";
			Control control = collapsibleHeading;
			context.RobustNameScope.Register("CAddress", control);
			control = collapsibleHeading;
			collapsible.XamlChildren.Add(control);
			CollapsibleBody collapsibleBody = new CollapsibleBody();
			collapsibleBody.Margin = new Thickness(20f, 0f, 0f, 0f);
			BoxContainer boxContainer = new BoxContainer();
			boxContainer.Orientation = 1;
			BoxContainer boxContainer2 = new BoxContainer();
			boxContainer2.Orientation = 0;
			boxContainer2.Margin = new Thickness(0f, 0f, 0f, 2f);
			CheckBox checkBox = new CheckBox();
			checkBox.Name = "CEnableDevice";
			control = checkBox;
			context.RobustNameScope.Register("CEnableDevice", control);
			checkBox.Text = (string)new LocExtension("air-alarm-ui-widget-enable").ProvideValue();
			control = checkBox;
			boxContainer2.XamlChildren.Add(control);
			control = boxContainer2;
			boxContainer.XamlChildren.Add(control);
			BoxContainer boxContainer3 = new BoxContainer();
			boxContainer3.Orientation = 0;
			boxContainer3.Margin = new Thickness(0f, 0f, 0f, 2f);
			boxContainer3.HorizontalExpand = true;
			BoxContainer boxContainer4 = new BoxContainer();
			boxContainer4.Orientation = 1;
			boxContainer4.HorizontalExpand = true;
			control = new Label
			{
				Text = (string)new LocExtension("air-alarm-ui-vent-pump-label").ProvideValue(),
				Margin = new Thickness(0f, 0f, 0f, 1f)
			};
			boxContainer4.XamlChildren.Add(control);
			OptionButton optionButton = new OptionButton();
			optionButton.Name = "CPumpDirection";
			control = optionButton;
			context.RobustNameScope.Register("CPumpDirection", control);
			control = optionButton;
			boxContainer4.XamlChildren.Add(control);
			control = boxContainer4;
			boxContainer3.XamlChildren.Add(control);
			BoxContainer boxContainer5 = new BoxContainer();
			boxContainer5.Orientation = 1;
			boxContainer5.HorizontalExpand = true;
			control = new Label
			{
				Text = (string)new LocExtension("air-alarm-ui-vent-pressure-label").ProvideValue(),
				Margin = new Thickness(0f, 0f, 0f, 1f)
			};
			boxContainer5.XamlChildren.Add(control);
			OptionButton optionButton2 = new OptionButton();
			optionButton2.Name = "CPressureCheck";
			control = optionButton2;
			context.RobustNameScope.Register("CPressureCheck", control);
			control = optionButton2;
			boxContainer5.XamlChildren.Add(control);
			control = boxContainer5;
			boxContainer3.XamlChildren.Add(control);
			control = boxContainer3;
			boxContainer.XamlChildren.Add(control);
			BoxContainer boxContainer6 = new BoxContainer();
			boxContainer6.Orientation = 0;
			boxContainer6.HorizontalExpand = true;
			BoxContainer boxContainer7 = new BoxContainer();
			boxContainer7.Orientation = 1;
			boxContainer7.HorizontalExpand = true;
			control = new Label
			{
				Text = (string)new LocExtension("air-alarm-ui-vent-external-bound-label").ProvideValue(),
				Margin = new Thickness(0f, 0f, 0f, 1f)
			};
			boxContainer7.XamlChildren.Add(control);
			FloatSpinBox floatSpinBox = new FloatSpinBox();
			floatSpinBox.Name = "CExternalBound";
			control = floatSpinBox;
			context.RobustNameScope.Register("CExternalBound", control);
			floatSpinBox.HorizontalExpand = true;
			control = floatSpinBox;
			boxContainer7.XamlChildren.Add(control);
			control = boxContainer7;
			boxContainer6.XamlChildren.Add(control);
			BoxContainer boxContainer8 = new BoxContainer();
			boxContainer8.Orientation = 1;
			boxContainer8.HorizontalExpand = true;
			control = new Label
			{
				Text = (string)new LocExtension("air-alarm-ui-vent-internal-bound-label").ProvideValue(),
				Margin = new Thickness(0f, 0f, 0f, 1f)
			};
			boxContainer8.XamlChildren.Add(control);
			FloatSpinBox floatSpinBox2 = new FloatSpinBox();
			floatSpinBox2.Name = "CInternalBound";
			control = floatSpinBox2;
			context.RobustNameScope.Register("CInternalBound", control);
			floatSpinBox2.HorizontalExpand = true;
			control = floatSpinBox2;
			boxContainer8.XamlChildren.Add(control);
			control = boxContainer8;
			boxContainer6.XamlChildren.Add(control);
			control = boxContainer6;
			boxContainer.XamlChildren.Add(control);
			control = boxContainer;
			collapsibleBody.XamlChildren.Add(control);
			control = collapsibleBody;
			collapsible.XamlChildren.Add(control);
			control = collapsible;
			A_1.XamlChildren.Add(control);
			if ((control = (A_1 as Control)) != null)
			{
				context.RobustNameScope.Absorb(control.NameScope);
				control.NameScope = context.RobustNameScope;
			}
			context.RobustNameScope.Complete();
		}

		// Token: 0x06001B57 RID: 6999 RVA: 0x0009DF28 File Offset: 0x0009C128
		private static void !XamlIlPopulateTrampoline(PumpControl A_0)
		{
			PumpControl.Populate:Content.Client.Atmos.Monitor.UI.Widgets.PumpControl.xaml(null, A_0);
		}

		// Token: 0x04000DA8 RID: 3496
		private GasVentPumpData _data;

		// Token: 0x04000DA9 RID: 3497
		private string _address;
	}
}