using System;
using System.Runtime.CompilerServices;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Monitor;
using Content.Shared.Atmos.Monitor.Components;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.Atmos.Monitor.UI
{
	// Token: 0x02000449 RID: 1097
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AirAlarmBoundUserInterface : BoundUserInterface
	{
		// Token: 0x06001B0B RID: 6923 RVA: 0x000021BC File Offset: 0x000003BC
		public AirAlarmBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
		}

		// Token: 0x06001B0C RID: 6924 RVA: 0x0009C0B4 File Offset: 0x0009A2B4
		protected override void Open()
		{
			base.Open();
			this._window = new AirAlarmWindow(base.Owner);
			if (base.State != null)
			{
				this.UpdateState(base.State);
			}
			this._window.OpenCentered();
			this._window.OnClose += base.Close;
			this._window.AtmosDeviceDataChanged += this.OnDeviceDataChanged;
			this._window.AtmosAlarmThresholdChanged += this.OnThresholdChanged;
			this._window.AirAlarmModeChanged += this.OnAirAlarmModeChanged;
			this._window.ResyncAllRequested += this.ResyncAllDevices;
			this._window.AirAlarmTabChange += this.OnTabChanged;
		}

		// Token: 0x06001B0D RID: 6925 RVA: 0x0009C181 File Offset: 0x0009A381
		private void ResyncAllDevices()
		{
			base.SendMessage(new AirAlarmResyncAllDevicesMessage());
		}

		// Token: 0x06001B0E RID: 6926 RVA: 0x0009C18E File Offset: 0x0009A38E
		private void OnDeviceDataChanged(string address, IAtmosDeviceData data)
		{
			base.SendMessage(new AirAlarmUpdateDeviceDataMessage(address, data));
		}

		// Token: 0x06001B0F RID: 6927 RVA: 0x0009C19D File Offset: 0x0009A39D
		private void OnAirAlarmModeChanged(AirAlarmMode mode)
		{
			base.SendMessage(new AirAlarmUpdateAlarmModeMessage(mode));
		}

		// Token: 0x06001B10 RID: 6928 RVA: 0x0009C1AB File Offset: 0x0009A3AB
		private void OnThresholdChanged(string address, AtmosMonitorThresholdType type, AtmosAlarmThreshold threshold, Gas? gas = null)
		{
			base.SendMessage(new AirAlarmUpdateAlarmThresholdMessage(address, type, threshold, gas));
		}

		// Token: 0x06001B11 RID: 6929 RVA: 0x0009C1BD File Offset: 0x0009A3BD
		private void OnTabChanged(AirAlarmTab tab)
		{
			base.SendMessage(new AirAlarmTabSetMessage(tab));
		}

		// Token: 0x06001B12 RID: 6930 RVA: 0x0009C1CC File Offset: 0x0009A3CC
		protected override void UpdateState(BoundUserInterfaceState state)
		{
			base.UpdateState(state);
			AirAlarmUIState airAlarmUIState = state as AirAlarmUIState;
			if (airAlarmUIState == null || this._window == null)
			{
				return;
			}
			this._window.UpdateState(airAlarmUIState);
		}

		// Token: 0x06001B13 RID: 6931 RVA: 0x0009C1FF File Offset: 0x0009A3FF
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				AirAlarmWindow window = this._window;
				if (window == null)
				{
					return;
				}
				window.Dispose();
			}
		}

		// Token: 0x04000D9D RID: 3485
		[Nullable(2)]
		private AirAlarmWindow _window;
	}
}
