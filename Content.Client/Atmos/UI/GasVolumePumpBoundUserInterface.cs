using System;
using System.Runtime.CompilerServices;
using Content.Shared.Atmos.Piping.Binary.Components;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.Atmos.UI
{
	// Token: 0x02000440 RID: 1088
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GasVolumePumpBoundUserInterface : BoundUserInterface
	{
		// Token: 0x06001AE6 RID: 6886 RVA: 0x000021BC File Offset: 0x000003BC
		public GasVolumePumpBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
		}

		// Token: 0x06001AE7 RID: 6887 RVA: 0x0009AA20 File Offset: 0x00098C20
		protected override void Open()
		{
			base.Open();
			this._window = new GasVolumePumpWindow();
			if (base.State != null)
			{
				this.UpdateState(base.State);
			}
			this._window.OpenCentered();
			this._window.OnClose += base.Close;
			this._window.ToggleStatusButtonPressed += this.OnToggleStatusButtonPressed;
			this._window.PumpTransferRateChanged += this.OnPumpTransferRatePressed;
		}

		// Token: 0x06001AE8 RID: 6888 RVA: 0x0009AAA2 File Offset: 0x00098CA2
		private void OnToggleStatusButtonPressed()
		{
			if (this._window == null)
			{
				return;
			}
			base.SendMessage(new GasVolumePumpToggleStatusMessage(this._window.PumpStatus));
		}

		// Token: 0x06001AE9 RID: 6889 RVA: 0x0009AAC4 File Offset: 0x00098CC4
		private void OnPumpTransferRatePressed(string value)
		{
			float num2;
			float num = float.TryParse(value, out num2) ? num2 : 0f;
			if (num > 200f)
			{
				num = 200f;
			}
			base.SendMessage(new GasVolumePumpChangeTransferRateMessage(num));
		}

		// Token: 0x06001AEA RID: 6890 RVA: 0x0009AB00 File Offset: 0x00098D00
		protected override void UpdateState(BoundUserInterfaceState state)
		{
			base.UpdateState(state);
			if (this._window != null)
			{
				GasVolumePumpBoundUserInterfaceState gasVolumePumpBoundUserInterfaceState = state as GasVolumePumpBoundUserInterfaceState;
				if (gasVolumePumpBoundUserInterfaceState != null)
				{
					this._window.Title = gasVolumePumpBoundUserInterfaceState.PumpLabel;
					this._window.SetPumpStatus(gasVolumePumpBoundUserInterfaceState.Enabled);
					this._window.SetTransferRate(gasVolumePumpBoundUserInterfaceState.TransferRate);
					return;
				}
			}
		}

		// Token: 0x06001AEB RID: 6891 RVA: 0x0009AB5A File Offset: 0x00098D5A
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!disposing)
			{
				return;
			}
			GasVolumePumpWindow window = this._window;
			if (window == null)
			{
				return;
			}
			window.Dispose();
		}

		// Token: 0x04000D7F RID: 3455
		[Nullable(2)]
		private GasVolumePumpWindow _window;

		// Token: 0x04000D80 RID: 3456
		private const float MaxTransferRate = 200f;
	}
}
