using System;
using System.Runtime.CompilerServices;
using Content.Shared.Atmos.Piping.Trinary.Components;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.Atmos.UI
{
	// Token: 0x0200043A RID: 1082
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GasMixerBoundUserInterface : BoundUserInterface
	{
		// Token: 0x06001A9C RID: 6812 RVA: 0x000021BC File Offset: 0x000003BC
		public GasMixerBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
		}

		// Token: 0x06001A9D RID: 6813 RVA: 0x00098FEC File Offset: 0x000971EC
		protected override void Open()
		{
			base.Open();
			this._window = new GasMixerWindow();
			if (base.State != null)
			{
				this.UpdateState(base.State);
			}
			this._window.OpenCentered();
			this._window.OnClose += base.Close;
			this._window.ToggleStatusButtonPressed += this.OnToggleStatusButtonPressed;
			this._window.MixerOutputPressureChanged += this.OnMixerOutputPressurePressed;
			this._window.MixerNodePercentageChanged += this.OnMixerSetPercentagePressed;
		}

		// Token: 0x06001A9E RID: 6814 RVA: 0x00099085 File Offset: 0x00097285
		private void OnToggleStatusButtonPressed()
		{
			if (this._window == null)
			{
				return;
			}
			base.SendMessage(new GasMixerToggleStatusMessage(this._window.MixerStatus));
		}

		// Token: 0x06001A9F RID: 6815 RVA: 0x000990A8 File Offset: 0x000972A8
		private void OnMixerOutputPressurePressed(string value)
		{
			float num2;
			float num = float.TryParse(value, out num2) ? num2 : 0f;
			if (num > 4500f)
			{
				num = 4500f;
			}
			base.SendMessage(new GasMixerChangeOutputPressureMessage(num));
		}

		// Token: 0x06001AA0 RID: 6816 RVA: 0x000990E4 File Offset: 0x000972E4
		private void OnMixerSetPercentagePressed(string value)
		{
			float num2;
			float num = float.TryParse(value, out num2) ? num2 : 1f;
			num = Math.Clamp(num, 0f, 100f);
			if (this._window != null)
			{
				num = (this._window.NodeOneLastEdited ? num : (100f - num));
			}
			base.SendMessage(new GasMixerChangeNodePercentageMessage(num));
		}

		// Token: 0x06001AA1 RID: 6817 RVA: 0x00099144 File Offset: 0x00097344
		protected override void UpdateState(BoundUserInterfaceState state)
		{
			base.UpdateState(state);
			if (this._window != null)
			{
				GasMixerBoundUserInterfaceState gasMixerBoundUserInterfaceState = state as GasMixerBoundUserInterfaceState;
				if (gasMixerBoundUserInterfaceState != null)
				{
					this._window.Title = gasMixerBoundUserInterfaceState.MixerLabel;
					this._window.SetMixerStatus(gasMixerBoundUserInterfaceState.Enabled);
					this._window.SetOutputPressure(gasMixerBoundUserInterfaceState.OutputPressure);
					this._window.SetNodePercentages(gasMixerBoundUserInterfaceState.NodeOne);
					return;
				}
			}
		}

		// Token: 0x06001AA2 RID: 6818 RVA: 0x000991AF File Offset: 0x000973AF
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!disposing)
			{
				return;
			}
			GasMixerWindow window = this._window;
			if (window == null)
			{
				return;
			}
			window.Dispose();
		}

		// Token: 0x04000D6E RID: 3438
		[Nullable(2)]
		private GasMixerWindow _window;

		// Token: 0x04000D6F RID: 3439
		private const float MaxPressure = 4500f;
	}
}
