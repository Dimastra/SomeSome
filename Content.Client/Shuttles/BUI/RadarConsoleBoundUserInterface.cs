using System;
using System.Runtime.CompilerServices;
using Content.Client.Shuttles.UI;
using Content.Shared.Shuttles.BUIStates;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.Shuttles.BUI
{
	// Token: 0x0200015A RID: 346
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RadarConsoleBoundUserInterface : BoundUserInterface
	{
		// Token: 0x06000919 RID: 2329 RVA: 0x000021BC File Offset: 0x000003BC
		public RadarConsoleBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
		}

		// Token: 0x0600091A RID: 2330 RVA: 0x00035A28 File Offset: 0x00033C28
		protected override void Open()
		{
			base.Open();
			this._window = new RadarConsoleWindow();
			this._window.OnClose += base.Close;
			this._window.OpenCentered();
		}

		// Token: 0x0600091B RID: 2331 RVA: 0x00035A5D File Offset: 0x00033C5D
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				RadarConsoleWindow window = this._window;
				if (window == null)
				{
					return;
				}
				window.Dispose();
			}
		}

		// Token: 0x0600091C RID: 2332 RVA: 0x00035A7C File Offset: 0x00033C7C
		protected override void UpdateState(BoundUserInterfaceState state)
		{
			base.UpdateState(state);
			RadarConsoleBoundInterfaceState radarConsoleBoundInterfaceState = state as RadarConsoleBoundInterfaceState;
			if (radarConsoleBoundInterfaceState == null)
			{
				return;
			}
			RadarConsoleWindow window = this._window;
			if (window != null)
			{
				window.SetMatrix(radarConsoleBoundInterfaceState.Coordinates, radarConsoleBoundInterfaceState.Angle);
			}
			RadarConsoleWindow window2 = this._window;
			if (window2 == null)
			{
				return;
			}
			window2.UpdateState(radarConsoleBoundInterfaceState);
		}

		// Token: 0x04000490 RID: 1168
		[Nullable(2)]
		private RadarConsoleWindow _window;
	}
}
