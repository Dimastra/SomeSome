using System;
using System.Runtime.CompilerServices;
using Content.Shared.SurveillanceCamera;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.SurveillanceCamera.UI
{
	// Token: 0x0200010C RID: 268
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SurveillanceCameraSetupBoundUi : BoundUserInterface
	{
		// Token: 0x06000781 RID: 1921 RVA: 0x000275A8 File Offset: 0x000257A8
		public SurveillanceCameraSetupBoundUi(ClientUserInterfaceComponent component, Enum uiKey) : base(component, uiKey)
		{
			if (uiKey is SurveillanceCameraSetupUiKey)
			{
				SurveillanceCameraSetupUiKey type = (SurveillanceCameraSetupUiKey)uiKey;
				this._type = type;
				return;
			}
		}

		// Token: 0x06000782 RID: 1922 RVA: 0x000275D8 File Offset: 0x000257D8
		protected override void Open()
		{
			this._window = new SurveillanceCameraSetupWindow();
			if (this._type == SurveillanceCameraSetupUiKey.Router)
			{
				this._window.HideNameSelector();
			}
			this._window.OpenCentered();
			SurveillanceCameraSetupWindow window = this._window;
			window.OnNameConfirm = (Action<string>)Delegate.Combine(window.OnNameConfirm, new Action<string>(this.SendDeviceName));
			SurveillanceCameraSetupWindow window2 = this._window;
			window2.OnNetworkConfirm = (Action<int>)Delegate.Combine(window2.OnNetworkConfirm, new Action<int>(this.SendSelectedNetwork));
		}

		// Token: 0x06000783 RID: 1923 RVA: 0x0002765D File Offset: 0x0002585D
		private void SendSelectedNetwork(int idx)
		{
			base.SendMessage(new SurveillanceCameraSetupSetNetwork(idx));
		}

		// Token: 0x06000784 RID: 1924 RVA: 0x0002766B File Offset: 0x0002586B
		private void SendDeviceName(string name)
		{
			base.SendMessage(new SurveillanceCameraSetupSetName(name));
		}

		// Token: 0x06000785 RID: 1925 RVA: 0x0002767C File Offset: 0x0002587C
		protected override void UpdateState(BoundUserInterfaceState state)
		{
			base.UpdateState(state);
			if (this._window != null)
			{
				SurveillanceCameraSetupBoundUiState surveillanceCameraSetupBoundUiState = state as SurveillanceCameraSetupBoundUiState;
				if (surveillanceCameraSetupBoundUiState != null)
				{
					this._window.UpdateState(surveillanceCameraSetupBoundUiState.Name, surveillanceCameraSetupBoundUiState.NameDisabled, surveillanceCameraSetupBoundUiState.NetworkDisabled);
					this._window.LoadAvailableNetworks(surveillanceCameraSetupBoundUiState.Network, surveillanceCameraSetupBoundUiState.Networks);
					return;
				}
			}
		}

		// Token: 0x06000786 RID: 1926 RVA: 0x000276D7 File Offset: 0x000258D7
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				this._window.Dispose();
			}
		}

		// Token: 0x04000378 RID: 888
		[Nullable(2)]
		private SurveillanceCameraSetupWindow _window;

		// Token: 0x04000379 RID: 889
		private SurveillanceCameraSetupUiKey _type;
	}
}
