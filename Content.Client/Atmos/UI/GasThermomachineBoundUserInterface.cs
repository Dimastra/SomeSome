using System;
using System.Runtime.CompilerServices;
using Content.Shared.Atmos.Piping.Unary.Components;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Maths;

namespace Content.Client.Atmos.UI
{
	// Token: 0x0200043E RID: 1086
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GasThermomachineBoundUserInterface : BoundUserInterface
	{
		// Token: 0x06001AD6 RID: 6870 RVA: 0x000021BC File Offset: 0x000003BC
		public GasThermomachineBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
		}

		// Token: 0x06001AD7 RID: 6871 RVA: 0x0009A4FC File Offset: 0x000986FC
		protected override void Open()
		{
			base.Open();
			this._window = new GasThermomachineWindow();
			if (base.State != null)
			{
				this.UpdateState(base.State);
			}
			this._window.OpenCentered();
			this._window.OnClose += base.Close;
			this._window.ToggleStatusButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				this.OnToggleStatusButtonPressed();
			};
			this._window.TemperatureSpinbox.OnValueChanged += delegate(FloatSpinBox.FloatSpinBoxEventArgs _)
			{
				this.OnTemperatureChanged(this._window.TemperatureSpinbox.Value);
			};
		}

		// Token: 0x06001AD8 RID: 6872 RVA: 0x0009A588 File Offset: 0x00098788
		private void OnToggleStatusButtonPressed()
		{
			if (this._window == null)
			{
				return;
			}
			this._window.SetActive(!this._window.Active);
			base.SendMessage(new GasThermomachineToggleMessage());
		}

		// Token: 0x06001AD9 RID: 6873 RVA: 0x0009A5B8 File Offset: 0x000987B8
		private void OnTemperatureChanged(float value)
		{
			float num = Math.Clamp(value, this._minTemp, this._maxTemp);
			if (MathHelper.CloseTo((double)num, (double)value, 0.09))
			{
				base.SendMessage(new GasThermomachineChangeTemperatureMessage(num));
				return;
			}
			GasThermomachineWindow window = this._window;
			if (window == null)
			{
				return;
			}
			window.SetTemperature(num);
		}

		// Token: 0x06001ADA RID: 6874 RVA: 0x0009A60C File Offset: 0x0009880C
		protected override void UpdateState(BoundUserInterfaceState state)
		{
			base.UpdateState(state);
			if (this._window != null)
			{
				GasThermomachineBoundUserInterfaceState gasThermomachineBoundUserInterfaceState = state as GasThermomachineBoundUserInterfaceState;
				if (gasThermomachineBoundUserInterfaceState != null)
				{
					this._minTemp = gasThermomachineBoundUserInterfaceState.MinTemperature;
					this._maxTemp = gasThermomachineBoundUserInterfaceState.MaxTemperature;
					this._window.SetTemperature(gasThermomachineBoundUserInterfaceState.Temperature);
					this._window.SetActive(gasThermomachineBoundUserInterfaceState.Enabled);
					GasThermomachineWindow window = this._window;
					ThermoMachineMode mode = gasThermomachineBoundUserInterfaceState.Mode;
					string title;
					if (mode != ThermoMachineMode.Freezer)
					{
						if (mode != ThermoMachineMode.Heater)
						{
							title = string.Empty;
						}
						else
						{
							title = Loc.GetString("comp-gas-thermomachine-ui-title-heater");
						}
					}
					else
					{
						title = Loc.GetString("comp-gas-thermomachine-ui-title-freezer");
					}
					window.Title = title;
					return;
				}
			}
		}

		// Token: 0x06001ADB RID: 6875 RVA: 0x0009A6AB File Offset: 0x000988AB
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!disposing)
			{
				return;
			}
			GasThermomachineWindow window = this._window;
			if (window == null)
			{
				return;
			}
			window.Dispose();
		}

		// Token: 0x04000D7A RID: 3450
		[Nullable(2)]
		private GasThermomachineWindow _window;

		// Token: 0x04000D7B RID: 3451
		private float _minTemp;

		// Token: 0x04000D7C RID: 3452
		private float _maxTemp;
	}
}
