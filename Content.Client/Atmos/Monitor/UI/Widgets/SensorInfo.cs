﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CompiledRobustXaml;
using Content.Client.Message;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Monitor;
using Content.Shared.Temperature;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Localization;
using Robust.Shared.Maths;

namespace Content.Client.Atmos.Monitor.UI.Widgets
{
	// Token: 0x02000450 RID: 1104
	[GenerateTypedNameReferences]
	public sealed class SensorInfo : BoxContainer
	{
		// Token: 0x06001B77 RID: 7031 RVA: 0x0009EA38 File Offset: 0x0009CC38
		[NullableContext(1)]
		public SensorInfo(AtmosSensorData data, string address)
		{
			SensorInfo.!XamlIlPopulateTrampoline(this);
			this._address = address;
			CollapsibleHeading sensorAddress = this.SensorAddress;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 2);
			defaultInterpolatedStringHandler.AppendFormatted(address);
			defaultInterpolatedStringHandler.AppendLiteral(" : ");
			defaultInterpolatedStringHandler.AppendFormatted<AtmosAlarmType>(data.AlarmState);
			sensorAddress.Title = defaultInterpolatedStringHandler.ToStringAndClear();
			RichTextLabel alarmStateLabel = this.AlarmStateLabel;
			string text = "air-alarm-ui-window-alarm-state-indicator";
			ValueTuple<string, object>[] array = new ValueTuple<string, object>[2];
			array[0] = new ValueTuple<string, object>("color", AirAlarmWindow.ColorForAlarm(data.AlarmState));
			int num = 1;
			string item = "state";
			defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
			defaultInterpolatedStringHandler.AppendFormatted<AtmosAlarmType>(data.AlarmState);
			array[num] = new ValueTuple<string, object>(item, defaultInterpolatedStringHandler.ToStringAndClear());
			alarmStateLabel.SetMarkup(Loc.GetString(text, array));
			RichTextLabel pressureLabel = this.PressureLabel;
			string text2 = "air-alarm-ui-window-pressure-indicator";
			ValueTuple<string, object>[] array2 = new ValueTuple<string, object>[2];
			array2[0] = new ValueTuple<string, object>("color", AirAlarmWindow.ColorForThreshold(data.Pressure, data.PressureThreshold));
			int num2 = 1;
			string item2 = "pressure";
			defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
			defaultInterpolatedStringHandler.AppendFormatted<float>(data.Pressure, "0.##");
			array2[num2] = new ValueTuple<string, object>(item2, defaultInterpolatedStringHandler.ToStringAndClear());
			pressureLabel.SetMarkup(Loc.GetString(text2, array2));
			RichTextLabel temperatureLabel = this.TemperatureLabel;
			string text3 = "air-alarm-ui-window-temperature-indicator";
			ValueTuple<string, object>[] array3 = new ValueTuple<string, object>[3];
			array3[0] = new ValueTuple<string, object>("color", AirAlarmWindow.ColorForThreshold(data.Temperature, data.TemperatureThreshold));
			int num3 = 1;
			string item3 = "tempC";
			defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
			defaultInterpolatedStringHandler.AppendFormatted<float>(TemperatureHelpers.KelvinToCelsius(data.Temperature), "0.#");
			array3[num3] = new ValueTuple<string, object>(item3, defaultInterpolatedStringHandler.ToStringAndClear());
			int num4 = 2;
			string item4 = "temperature";
			defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
			defaultInterpolatedStringHandler.AppendFormatted<float>(data.Temperature, "0.##");
			array3[num4] = new ValueTuple<string, object>(item4, defaultInterpolatedStringHandler.ToStringAndClear());
			temperatureLabel.SetMarkup(Loc.GetString(text3, array3));
			foreach (KeyValuePair<Gas, float> keyValuePair in data.Gases)
			{
				Gas gas;
				float num5;
				keyValuePair.Deconstruct(out gas, out num5);
				Gas gas2 = gas;
				float num6 = num5;
				RichTextLabel richTextLabel = new RichTextLabel();
				float num7 = num6 / data.TotalMoles;
				RichTextLabel label = richTextLabel;
				string text4 = "air-alarm-ui-gases-indicator";
				ValueTuple<string, object>[] array4 = new ValueTuple<string, object>[4];
				int num8 = 0;
				string item5 = "gas";
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
				defaultInterpolatedStringHandler.AppendFormatted<Gas>(gas2);
				array4[num8] = new ValueTuple<string, object>(item5, defaultInterpolatedStringHandler.ToStringAndClear());
				array4[1] = new ValueTuple<string, object>("color", AirAlarmWindow.ColorForThreshold(num7, data.GasThresholds[gas2]));
				int num9 = 2;
				string item6 = "amount";
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
				defaultInterpolatedStringHandler.AppendFormatted<float>(num6, "0.####");
				array4[num9] = new ValueTuple<string, object>(item6, defaultInterpolatedStringHandler.ToStringAndClear());
				int num10 = 3;
				string item7 = "percentage";
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
				defaultInterpolatedStringHandler.AppendFormatted<float>(100f * num7, "0.##");
				array4[num10] = new ValueTuple<string, object>(item7, defaultInterpolatedStringHandler.ToStringAndClear());
				label.SetMarkup(Loc.GetString(text4, array4));
				this.GasContainer.AddChild(richTextLabel);
				this._gasLabels.Add(gas2, richTextLabel);
				AtmosAlarmThreshold threshold2 = data.GasThresholds[gas2];
				string text5 = "air-alarm-ui-thresholds-gas-title";
				ValueTuple<string, object>[] array5 = new ValueTuple<string, object>[1];
				int num11 = 0;
				string item8 = "gas";
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
				defaultInterpolatedStringHandler.AppendFormatted<Gas>(gas2);
				array5[num11] = new ValueTuple<string, object>(item8, defaultInterpolatedStringHandler.ToStringAndClear());
				ThresholdControl thresholdControl = new ThresholdControl(Loc.GetString(text5, array5), threshold2, AtmosMonitorThresholdType.Gas, new Gas?(gas2), 100f);
				thresholdControl.Margin = new Thickness(20f, 2f, 2f, 2f);
				thresholdControl.ThresholdDataChanged += delegate(AtmosMonitorThresholdType type, AtmosAlarmThreshold threshold, Gas? arg3)
				{
					this.OnThresholdUpdate(this._address, type, threshold, arg3);
				};
				this._gasThresholds.Add(gas2, thresholdControl);
				this.GasContainer.AddChild(thresholdControl);
			}
			this._pressureThreshold = new ThresholdControl(Loc.GetString("air-alarm-ui-thresholds-pressure-title"), data.PressureThreshold, AtmosMonitorThresholdType.Pressure, null, 1f);
			this.PressureThresholdContainer.AddChild(this._pressureThreshold);
			this._temperatureThreshold = new ThresholdControl(Loc.GetString("air-alarm-ui-thresholds-temperature-title"), data.TemperatureThreshold, AtmosMonitorThresholdType.Temperature, null, 1f);
			this.TemperatureThresholdContainer.AddChild(this._temperatureThreshold);
			this._pressureThreshold.ThresholdDataChanged += delegate(AtmosMonitorThresholdType type, AtmosAlarmThreshold threshold, Gas? arg3)
			{
				this.OnThresholdUpdate(this._address, type, threshold, arg3);
			};
			this._temperatureThreshold.ThresholdDataChanged += delegate(AtmosMonitorThresholdType type, AtmosAlarmThreshold threshold, Gas? arg3)
			{
				this.OnThresholdUpdate(this._address, type, threshold, arg3);
			};
		}

		// Token: 0x06001B78 RID: 7032 RVA: 0x0009EEE0 File Offset: 0x0009D0E0
		[NullableContext(1)]
		public void ChangeData(AtmosSensorData data)
		{
			CollapsibleHeading sensorAddress = this.SensorAddress;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 2);
			defaultInterpolatedStringHandler.AppendFormatted(this._address);
			defaultInterpolatedStringHandler.AppendLiteral(" : ");
			defaultInterpolatedStringHandler.AppendFormatted<AtmosAlarmType>(data.AlarmState);
			sensorAddress.Title = defaultInterpolatedStringHandler.ToStringAndClear();
			RichTextLabel alarmStateLabel = this.AlarmStateLabel;
			string text = "air-alarm-ui-window-alarm-state-indicator";
			ValueTuple<string, object>[] array = new ValueTuple<string, object>[2];
			array[0] = new ValueTuple<string, object>("color", AirAlarmWindow.ColorForAlarm(data.AlarmState));
			int num = 1;
			string item = "state";
			defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
			defaultInterpolatedStringHandler.AppendFormatted<AtmosAlarmType>(data.AlarmState);
			array[num] = new ValueTuple<string, object>(item, defaultInterpolatedStringHandler.ToStringAndClear());
			alarmStateLabel.SetMarkup(Loc.GetString(text, array));
			RichTextLabel pressureLabel = this.PressureLabel;
			string text2 = "air-alarm-ui-window-pressure-indicator";
			ValueTuple<string, object>[] array2 = new ValueTuple<string, object>[2];
			array2[0] = new ValueTuple<string, object>("color", AirAlarmWindow.ColorForThreshold(data.Pressure, data.PressureThreshold));
			int num2 = 1;
			string item2 = "pressure";
			defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
			defaultInterpolatedStringHandler.AppendFormatted<float>(data.Pressure, "0.##");
			array2[num2] = new ValueTuple<string, object>(item2, defaultInterpolatedStringHandler.ToStringAndClear());
			pressureLabel.SetMarkup(Loc.GetString(text2, array2));
			RichTextLabel temperatureLabel = this.TemperatureLabel;
			string text3 = "air-alarm-ui-window-temperature-indicator";
			ValueTuple<string, object>[] array3 = new ValueTuple<string, object>[3];
			array3[0] = new ValueTuple<string, object>("color", AirAlarmWindow.ColorForThreshold(data.Temperature, data.TemperatureThreshold));
			int num3 = 1;
			string item3 = "tempC";
			defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
			defaultInterpolatedStringHandler.AppendFormatted<float>(TemperatureHelpers.KelvinToCelsius(data.Temperature), "0.#");
			array3[num3] = new ValueTuple<string, object>(item3, defaultInterpolatedStringHandler.ToStringAndClear());
			int num4 = 2;
			string item4 = "temperature";
			defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
			defaultInterpolatedStringHandler.AppendFormatted<float>(data.Temperature, "0.##");
			array3[num4] = new ValueTuple<string, object>(item4, defaultInterpolatedStringHandler.ToStringAndClear());
			temperatureLabel.SetMarkup(Loc.GetString(text3, array3));
			foreach (KeyValuePair<Gas, float> keyValuePair in data.Gases)
			{
				Gas gas;
				float num5;
				keyValuePair.Deconstruct(out gas, out num5);
				Gas gas2 = gas;
				float num6 = num5;
				RichTextLabel richTextLabel;
				if (this._gasLabels.TryGetValue(gas2, out richTextLabel))
				{
					float num7 = num6 / data.TotalMoles;
					RichTextLabel label = richTextLabel;
					string text4 = "air-alarm-ui-gases-indicator";
					ValueTuple<string, object>[] array4 = new ValueTuple<string, object>[4];
					int num8 = 0;
					string item5 = "gas";
					defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
					defaultInterpolatedStringHandler.AppendFormatted<Gas>(gas2);
					array4[num8] = new ValueTuple<string, object>(item5, defaultInterpolatedStringHandler.ToStringAndClear());
					array4[1] = new ValueTuple<string, object>("color", AirAlarmWindow.ColorForThreshold(num7, data.GasThresholds[gas2]));
					int num9 = 2;
					string item6 = "amount";
					defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
					defaultInterpolatedStringHandler.AppendFormatted<float>(num6, "0.####");
					array4[num9] = new ValueTuple<string, object>(item6, defaultInterpolatedStringHandler.ToStringAndClear());
					int num10 = 3;
					string item7 = "percentage";
					defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
					defaultInterpolatedStringHandler.AppendFormatted<float>(100f * num7, "0.##");
					array4[num10] = new ValueTuple<string, object>(item7, defaultInterpolatedStringHandler.ToStringAndClear());
					label.SetMarkup(Loc.GetString(text4, array4));
				}
			}
			this._pressureThreshold.UpdateThresholdData(data.PressureThreshold, data.Pressure);
			this._temperatureThreshold.UpdateThresholdData(data.TemperatureThreshold, data.Temperature);
			foreach (KeyValuePair<Gas, ThresholdControl> keyValuePair2 in this._gasThresholds)
			{
				Gas gas;
				ThresholdControl thresholdControl;
				keyValuePair2.Deconstruct(out gas, out thresholdControl);
				Gas key = gas;
				ThresholdControl thresholdControl2 = thresholdControl;
				AtmosAlarmThreshold threshold;
				if (data.GasThresholds.TryGetValue(key, out threshold))
				{
					thresholdControl2.UpdateThresholdData(threshold, data.Gases[key] / data.TotalMoles);
				}
			}
		}

		// Token: 0x170005C3 RID: 1475
		// (get) Token: 0x06001B79 RID: 7033 RVA: 0x0009F2BC File Offset: 0x0009D4BC
		private CollapsibleHeading SensorAddress
		{
			get
			{
				return base.FindControl<CollapsibleHeading>("SensorAddress");
			}
		}

		// Token: 0x170005C4 RID: 1476
		// (get) Token: 0x06001B7A RID: 7034 RVA: 0x0009F2C9 File Offset: 0x0009D4C9
		private RichTextLabel AlarmStateLabel
		{
			get
			{
				return base.FindControl<RichTextLabel>("AlarmStateLabel");
			}
		}

		// Token: 0x170005C5 RID: 1477
		// (get) Token: 0x06001B7B RID: 7035 RVA: 0x0009F2D6 File Offset: 0x0009D4D6
		private RichTextLabel PressureLabel
		{
			get
			{
				return base.FindControl<RichTextLabel>("PressureLabel");
			}
		}

		// Token: 0x170005C6 RID: 1478
		// (get) Token: 0x06001B7C RID: 7036 RVA: 0x0009F2E3 File Offset: 0x0009D4E3
		private Control PressureThresholdContainer
		{
			get
			{
				return base.FindControl<Control>("PressureThresholdContainer");
			}
		}

		// Token: 0x170005C7 RID: 1479
		// (get) Token: 0x06001B7D RID: 7037 RVA: 0x0009F2F0 File Offset: 0x0009D4F0
		private RichTextLabel TemperatureLabel
		{
			get
			{
				return base.FindControl<RichTextLabel>("TemperatureLabel");
			}
		}

		// Token: 0x170005C8 RID: 1480
		// (get) Token: 0x06001B7E RID: 7038 RVA: 0x0009F2FD File Offset: 0x0009D4FD
		private Control TemperatureThresholdContainer
		{
			get
			{
				return base.FindControl<Control>("TemperatureThresholdContainer");
			}
		}

		// Token: 0x170005C9 RID: 1481
		// (get) Token: 0x06001B7F RID: 7039 RVA: 0x0009F30A File Offset: 0x0009D50A
		private BoxContainer GasContainer
		{
			get
			{
				return base.FindControl<BoxContainer>("GasContainer");
			}
		}

		// Token: 0x06001B83 RID: 7043 RVA: 0x0009F330 File Offset: 0x0009D530
		static void xaml(IServiceProvider A_0, BoxContainer A_1)
		{
			XamlIlContext.Context<BoxContainer> context = new XamlIlContext.Context<BoxContainer>(A_0, null, "resm:Content.Client.Atmos.Monitor.UI.Widgets.SensorInfo.xaml?assembly=Content.Client");
			context.RootObject = A_1;
			context.IntermediateRoot = A_1;
			A_1.HorizontalExpand = true;
			Collapsible collapsible = new Collapsible();
			collapsible.Orientation = 1;
			CollapsibleHeading collapsibleHeading = new CollapsibleHeading();
			collapsibleHeading.Name = "SensorAddress";
			Control control = collapsibleHeading;
			context.RobustNameScope.Register("SensorAddress", control);
			control = collapsibleHeading;
			collapsible.XamlChildren.Add(control);
			CollapsibleBody collapsibleBody = new CollapsibleBody();
			collapsibleBody.Margin = new Thickness(20f, 2f, 2f, 2f);
			BoxContainer boxContainer = new BoxContainer();
			boxContainer.Orientation = 1;
			boxContainer.HorizontalExpand = true;
			BoxContainer boxContainer2 = new BoxContainer();
			boxContainer2.Orientation = 1;
			boxContainer2.Margin = new Thickness(0f, 0f, 2f, 0f);
			boxContainer2.HorizontalExpand = true;
			RichTextLabel richTextLabel = new RichTextLabel();
			richTextLabel.Name = "AlarmStateLabel";
			control = richTextLabel;
			context.RobustNameScope.Register("AlarmStateLabel", control);
			control = richTextLabel;
			boxContainer2.XamlChildren.Add(control);
			RichTextLabel richTextLabel2 = new RichTextLabel();
			richTextLabel2.Name = "PressureLabel";
			control = richTextLabel2;
			context.RobustNameScope.Register("PressureLabel", control);
			control = richTextLabel2;
			boxContainer2.XamlChildren.Add(control);
			Control control2 = new Control();
			control2.Name = "PressureThresholdContainer";
			control = control2;
			context.RobustNameScope.Register("PressureThresholdContainer", control);
			control2.Margin = new Thickness(20f, 0f, 2f, 0f);
			control = control2;
			boxContainer2.XamlChildren.Add(control);
			RichTextLabel richTextLabel3 = new RichTextLabel();
			richTextLabel3.Name = "TemperatureLabel";
			control = richTextLabel3;
			context.RobustNameScope.Register("TemperatureLabel", control);
			control = richTextLabel3;
			boxContainer2.XamlChildren.Add(control);
			Control control3 = new Control();
			control3.Name = "TemperatureThresholdContainer";
			control = control3;
			context.RobustNameScope.Register("TemperatureThresholdContainer", control);
			control3.Margin = new Thickness(20f, 0f, 2f, 0f);
			control = control3;
			boxContainer2.XamlChildren.Add(control);
			control = boxContainer2;
			boxContainer.XamlChildren.Add(control);
			Collapsible collapsible2 = new Collapsible();
			collapsible2.Orientation = 1;
			collapsible2.Margin = new Thickness(2f, 2f, 2f, 2f);
			control = new CollapsibleHeading
			{
				Title = (string)new LocExtension("air-alarm-ui-sensor-gases").ProvideValue()
			};
			collapsible2.XamlChildren.Add(control);
			CollapsibleBody collapsibleBody2 = new CollapsibleBody();
			collapsibleBody2.Margin = new Thickness(20f, 0f, 0f, 0f);
			BoxContainer boxContainer3 = new BoxContainer();
			boxContainer3.Name = "GasContainer";
			control = boxContainer3;
			context.RobustNameScope.Register("GasContainer", control);
			boxContainer3.Orientation = 1;
			boxContainer3.Margin = new Thickness(2f, 2f, 2f, 2f);
			control = boxContainer3;
			collapsibleBody2.XamlChildren.Add(control);
			control = collapsibleBody2;
			collapsible2.XamlChildren.Add(control);
			control = collapsible2;
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

		// Token: 0x06001B84 RID: 7044 RVA: 0x0009F756 File Offset: 0x0009D956
		private static void !XamlIlPopulateTrampoline(SensorInfo A_0)
		{
			SensorInfo.Populate:Content.Client.Atmos.Monitor.UI.Widgets.SensorInfo.xaml(null, A_0);
		}

		// Token: 0x04000DB6 RID: 3510
		[Nullable(new byte[]
		{
			2,
			1,
			1
		})]
		public Action<string, AtmosMonitorThresholdType, AtmosAlarmThreshold, Gas?> OnThresholdUpdate;

		// Token: 0x04000DB7 RID: 3511
		[Nullable(1)]
		private string _address;

		// Token: 0x04000DB8 RID: 3512
		[Nullable(1)]
		private ThresholdControl _pressureThreshold;

		// Token: 0x04000DB9 RID: 3513
		[Nullable(1)]
		private ThresholdControl _temperatureThreshold;

		// Token: 0x04000DBA RID: 3514
		[Nullable(1)]
		private Dictionary<Gas, ThresholdControl> _gasThresholds = new Dictionary<Gas, ThresholdControl>();

		// Token: 0x04000DBB RID: 3515
		[Nullable(1)]
		private Dictionary<Gas, RichTextLabel> _gasLabels = new Dictionary<Gas, RichTextLabel>();
	}
}
