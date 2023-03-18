using System;
using System.Runtime.CompilerServices;
using Content.Client.Atmos.EntitySystems;
using Content.Shared.Atmos.Piping.Trinary.Components;
using Content.Shared.Atmos.Prototypes;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Client.Atmos.UI
{
	// Token: 0x02000438 RID: 1080
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GasFilterBoundUserInterface : BoundUserInterface
	{
		// Token: 0x06001A7A RID: 6778 RVA: 0x000021BC File Offset: 0x000003BC
		public GasFilterBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
		}

		// Token: 0x06001A7B RID: 6779 RVA: 0x000984BC File Offset: 0x000966BC
		protected override void Open()
		{
			base.Open();
			AtmosphereSystem entitySystem = IoCManager.Resolve<IEntitySystemManager>().GetEntitySystem<AtmosphereSystem>();
			this._window = new GasFilterWindow(entitySystem.Gases);
			if (base.State != null)
			{
				this.UpdateState(base.State);
			}
			this._window.OpenCentered();
			this._window.OnClose += base.Close;
			this._window.ToggleStatusButtonPressed += this.OnToggleStatusButtonPressed;
			this._window.FilterTransferRateChanged += this.OnFilterTransferRatePressed;
			this._window.SelectGasPressed += this.OnSelectGasPressed;
		}

		// Token: 0x06001A7C RID: 6780 RVA: 0x00098566 File Offset: 0x00096766
		private void OnToggleStatusButtonPressed()
		{
			if (this._window == null)
			{
				return;
			}
			base.SendMessage(new GasFilterToggleStatusMessage(this._window.FilterStatus));
		}

		// Token: 0x06001A7D RID: 6781 RVA: 0x00098588 File Offset: 0x00096788
		private void OnFilterTransferRatePressed(string value)
		{
			float num2;
			float num = float.TryParse(value, out num2) ? num2 : 0f;
			if (num > 200f)
			{
				num = 200f;
			}
			base.SendMessage(new GasFilterChangeRateMessage(num));
		}

		// Token: 0x06001A7E RID: 6782 RVA: 0x000985C4 File Offset: 0x000967C4
		private void OnSelectGasPressed()
		{
			if (this._window == null)
			{
				return;
			}
			if (this._window.SelectedGas == null)
			{
				base.SendMessage(new GasFilterSelectGasMessage(null));
				return;
			}
			int value;
			if (!int.TryParse(this._window.SelectedGas, out value))
			{
				return;
			}
			base.SendMessage(new GasFilterSelectGasMessage(new int?(value)));
		}

		// Token: 0x06001A7F RID: 6783 RVA: 0x00098624 File Offset: 0x00096824
		protected override void UpdateState(BoundUserInterfaceState state)
		{
			base.UpdateState(state);
			if (this._window != null)
			{
				GasFilterBoundUserInterfaceState gasFilterBoundUserInterfaceState = state as GasFilterBoundUserInterfaceState;
				if (gasFilterBoundUserInterfaceState != null)
				{
					this._window.Title = gasFilterBoundUserInterfaceState.FilterLabel;
					this._window.SetFilterStatus(gasFilterBoundUserInterfaceState.Enabled);
					this._window.SetTransferRate(gasFilterBoundUserInterfaceState.TransferRate);
					if (gasFilterBoundUserInterfaceState.FilteredGas != null)
					{
						GasPrototype gas = IoCManager.Resolve<IEntitySystemManager>().GetEntitySystem<AtmosphereSystem>().GetGas(gasFilterBoundUserInterfaceState.FilteredGas.Value);
						string @string = Loc.GetString(gas.Name);
						this._window.SetGasFiltered(gas.ID, @string);
						return;
					}
					this._window.SetGasFiltered(null, Loc.GetString("comp-gas-filter-ui-filter-gas-none"));
					return;
				}
			}
		}

		// Token: 0x06001A80 RID: 6784 RVA: 0x000986E1 File Offset: 0x000968E1
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!disposing)
			{
				return;
			}
			GasFilterWindow window = this._window;
			if (window == null)
			{
				return;
			}
			window.Dispose();
		}

		// Token: 0x04000D65 RID: 3429
		[Nullable(2)]
		private GasFilterWindow _window;

		// Token: 0x04000D66 RID: 3430
		private const float MaxTransferRate = 200f;
	}
}
