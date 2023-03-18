using System;
using System.Runtime.CompilerServices;
using Content.Shared.Atmos.Components;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.UserInterface.Systems.Atmos.GasTank
{
	// Token: 0x020000B6 RID: 182
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GasTankBoundUserInterface : BoundUserInterface
	{
		// Token: 0x060004EC RID: 1260 RVA: 0x000021BC File Offset: 0x000003BC
		public GasTankBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
		}

		// Token: 0x060004ED RID: 1261 RVA: 0x0001B234 File Offset: 0x00019434
		public void SetOutputPressure(in float value)
		{
			base.SendMessage(new GasTankSetPressureMessage
			{
				Pressure = value
			});
		}

		// Token: 0x060004EE RID: 1262 RVA: 0x0001B249 File Offset: 0x00019449
		public void ToggleInternals()
		{
			base.SendMessage(new GasTankToggleInternalsMessage());
		}

		// Token: 0x060004EF RID: 1263 RVA: 0x0001B256 File Offset: 0x00019456
		protected override void Open()
		{
			base.Open();
			this._window = new GasTankWindow(this);
			this._window.OnClose += base.Close;
			this._window.OpenCentered();
		}

		// Token: 0x060004F0 RID: 1264 RVA: 0x0001B28C File Offset: 0x0001948C
		protected override void UpdateState(BoundUserInterfaceState state)
		{
			base.UpdateState(state);
			GasTankBoundUserInterfaceState gasTankBoundUserInterfaceState = state as GasTankBoundUserInterfaceState;
			if (gasTankBoundUserInterfaceState != null)
			{
				GasTankWindow window = this._window;
				if (window == null)
				{
					return;
				}
				window.UpdateState(gasTankBoundUserInterfaceState);
			}
		}

		// Token: 0x060004F1 RID: 1265 RVA: 0x0001B2BB File Offset: 0x000194BB
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			GasTankWindow window = this._window;
			if (window == null)
			{
				return;
			}
			window.Close();
		}

		// Token: 0x04000247 RID: 583
		[Nullable(2)]
		private GasTankWindow _window;
	}
}
