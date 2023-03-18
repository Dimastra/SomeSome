using System;
using System.Runtime.CompilerServices;
using Content.Shared.Anomaly;
using Content.Shared.Gravity;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.Anomaly.Ui
{
	// Token: 0x0200046B RID: 1131
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AnomalyGeneratorBoundUserInterface : BoundUserInterface
	{
		// Token: 0x06001C0F RID: 7183 RVA: 0x000021BC File Offset: 0x000003BC
		public AnomalyGeneratorBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
		}

		// Token: 0x06001C10 RID: 7184 RVA: 0x000A2C30 File Offset: 0x000A0E30
		protected override void Open()
		{
			base.Open();
			this._window = new AnomalyGeneratorWindow(base.Owner.Owner);
			this._window.OpenCentered();
			this._window.OnClose += base.Close;
			AnomalyGeneratorWindow window = this._window;
			window.OnGenerateButtonPressed = (Action)Delegate.Combine(window.OnGenerateButtonPressed, new Action(delegate()
			{
				base.SendMessage(new AnomalyGeneratorGenerateButtonPressedEvent());
			}));
		}

		// Token: 0x06001C11 RID: 7185 RVA: 0x000A2CA4 File Offset: 0x000A0EA4
		protected override void UpdateState(BoundUserInterfaceState state)
		{
			base.UpdateState(state);
			AnomalyGeneratorUserInterfaceState anomalyGeneratorUserInterfaceState = state as AnomalyGeneratorUserInterfaceState;
			if (anomalyGeneratorUserInterfaceState == null)
			{
				return;
			}
			AnomalyGeneratorWindow window = this._window;
			if (window == null)
			{
				return;
			}
			window.UpdateState(anomalyGeneratorUserInterfaceState);
		}

		// Token: 0x06001C12 RID: 7186 RVA: 0x000A2CD4 File Offset: 0x000A0ED4
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!disposing)
			{
				return;
			}
			AnomalyGeneratorWindow window = this._window;
			if (window == null)
			{
				return;
			}
			window.Dispose();
		}

		// Token: 0x06001C13 RID: 7187 RVA: 0x00071D71 File Offset: 0x0006FF71
		public void SetPowerSwitch(bool on)
		{
			base.SendMessage(new SharedGravityGeneratorComponent.SwitchGeneratorMessage(on));
		}

		// Token: 0x04000E1C RID: 3612
		[Nullable(2)]
		private AnomalyGeneratorWindow _window;
	}
}
