using System;
using System.Runtime.CompilerServices;
using Content.Shared.Atmos.Piping.Binary.Components;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.Atmos.UI
{
	// Token: 0x02000436 RID: 1078
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GasCanisterBoundUserInterface : BoundUserInterface
	{
		// Token: 0x06001A50 RID: 6736 RVA: 0x000021BC File Offset: 0x000003BC
		public GasCanisterBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
		}

		// Token: 0x06001A51 RID: 6737 RVA: 0x00097514 File Offset: 0x00095714
		protected override void Open()
		{
			base.Open();
			this._window = new GasCanisterWindow();
			if (base.State != null)
			{
				this.UpdateState(base.State);
			}
			this._window.OpenCentered();
			this._window.OnClose += base.Close;
			this._window.ReleaseValveCloseButtonPressed += this.OnReleaseValveClosePressed;
			this._window.ReleaseValveOpenButtonPressed += this.OnReleaseValveOpenPressed;
			this._window.ReleasePressureSet += this.OnReleasePressureSet;
			this._window.TankEjectButtonPressed += this.OnTankEjectPressed;
		}

		// Token: 0x06001A52 RID: 6738 RVA: 0x000975C4 File Offset: 0x000957C4
		private void OnTankEjectPressed()
		{
			base.SendMessage(new GasCanisterHoldingTankEjectMessage());
		}

		// Token: 0x06001A53 RID: 6739 RVA: 0x000975D1 File Offset: 0x000957D1
		private void OnReleasePressureSet(float value)
		{
			base.SendMessage(new GasCanisterChangeReleasePressureMessage(value));
		}

		// Token: 0x06001A54 RID: 6740 RVA: 0x000975DF File Offset: 0x000957DF
		private void OnReleaseValveOpenPressed()
		{
			base.SendMessage(new GasCanisterChangeReleaseValveMessage(true));
		}

		// Token: 0x06001A55 RID: 6741 RVA: 0x000975ED File Offset: 0x000957ED
		private void OnReleaseValveClosePressed()
		{
			base.SendMessage(new GasCanisterChangeReleaseValveMessage(false));
		}

		// Token: 0x06001A56 RID: 6742 RVA: 0x000975FC File Offset: 0x000957FC
		protected override void UpdateState(BoundUserInterfaceState state)
		{
			base.UpdateState(state);
			if (this._window != null)
			{
				GasCanisterBoundUserInterfaceState gasCanisterBoundUserInterfaceState = state as GasCanisterBoundUserInterfaceState;
				if (gasCanisterBoundUserInterfaceState != null)
				{
					this._window.SetCanisterLabel(gasCanisterBoundUserInterfaceState.CanisterLabel);
					this._window.SetCanisterPressure(gasCanisterBoundUserInterfaceState.CanisterPressure);
					this._window.SetPortStatus(gasCanisterBoundUserInterfaceState.PortStatus);
					this._window.SetTankLabel(gasCanisterBoundUserInterfaceState.TankLabel);
					this._window.SetTankPressure(gasCanisterBoundUserInterfaceState.TankPressure);
					this._window.SetReleasePressureRange(gasCanisterBoundUserInterfaceState.ReleasePressureMin, gasCanisterBoundUserInterfaceState.ReleasePressureMax);
					this._window.SetReleasePressure(gasCanisterBoundUserInterfaceState.ReleasePressure);
					this._window.SetReleaseValve(gasCanisterBoundUserInterfaceState.ReleaseValve);
					return;
				}
			}
		}

		// Token: 0x06001A57 RID: 6743 RVA: 0x000976B1 File Offset: 0x000958B1
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!disposing)
			{
				return;
			}
			GasCanisterWindow window = this._window;
			if (window == null)
			{
				return;
			}
			window.Dispose();
		}

		// Token: 0x04000D5F RID: 3423
		[Nullable(2)]
		private GasCanisterWindow _window;
	}
}
