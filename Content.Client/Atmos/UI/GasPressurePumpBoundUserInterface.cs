using System;
using System.Runtime.CompilerServices;
using Content.Shared.Atmos.Piping.Binary.Components;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.Atmos.UI
{
	// Token: 0x0200043C RID: 1084
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GasPressurePumpBoundUserInterface : BoundUserInterface
	{
		// Token: 0x06001ABE RID: 6846 RVA: 0x000021BC File Offset: 0x000003BC
		public GasPressurePumpBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
		}

		// Token: 0x06001ABF RID: 6847 RVA: 0x00099D2C File Offset: 0x00097F2C
		protected override void Open()
		{
			base.Open();
			this._window = new GasPressurePumpWindow();
			if (base.State != null)
			{
				this.UpdateState(base.State);
			}
			this._window.OpenCentered();
			this._window.OnClose += base.Close;
			this._window.ToggleStatusButtonPressed += this.OnToggleStatusButtonPressed;
			this._window.PumpOutputPressureChanged += this.OnPumpOutputPressurePressed;
		}

		// Token: 0x06001AC0 RID: 6848 RVA: 0x00099DAE File Offset: 0x00097FAE
		private void OnToggleStatusButtonPressed()
		{
			if (this._window == null)
			{
				return;
			}
			base.SendMessage(new GasPressurePumpToggleStatusMessage(this._window.PumpStatus));
		}

		// Token: 0x06001AC1 RID: 6849 RVA: 0x00099DD0 File Offset: 0x00097FD0
		private void OnPumpOutputPressurePressed(string value)
		{
			float num2;
			float num = float.TryParse(value, out num2) ? num2 : 0f;
			if (num > 4500f)
			{
				num = 4500f;
			}
			base.SendMessage(new GasPressurePumpChangeOutputPressureMessage(num));
		}

		// Token: 0x06001AC2 RID: 6850 RVA: 0x00099E0C File Offset: 0x0009800C
		protected override void UpdateState(BoundUserInterfaceState state)
		{
			base.UpdateState(state);
			if (this._window != null)
			{
				GasPressurePumpBoundUserInterfaceState gasPressurePumpBoundUserInterfaceState = state as GasPressurePumpBoundUserInterfaceState;
				if (gasPressurePumpBoundUserInterfaceState != null)
				{
					this._window.Title = gasPressurePumpBoundUserInterfaceState.PumpLabel;
					this._window.SetPumpStatus(gasPressurePumpBoundUserInterfaceState.Enabled);
					this._window.SetOutputPressure(gasPressurePumpBoundUserInterfaceState.OutputPressure);
					return;
				}
			}
		}

		// Token: 0x06001AC3 RID: 6851 RVA: 0x00099E66 File Offset: 0x00098066
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!disposing)
			{
				return;
			}
			GasPressurePumpWindow window = this._window;
			if (window == null)
			{
				return;
			}
			window.Dispose();
		}

		// Token: 0x04000D75 RID: 3445
		[Nullable(2)]
		private GasPressurePumpWindow _window;

		// Token: 0x04000D76 RID: 3446
		private const float MaxPressure = 4500f;
	}
}
