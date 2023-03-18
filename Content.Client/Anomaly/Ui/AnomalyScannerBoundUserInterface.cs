using System;
using System.Runtime.CompilerServices;
using Content.Shared.Anomaly;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.Anomaly.Ui
{
	// Token: 0x0200046D RID: 1133
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AnomalyScannerBoundUserInterface : BoundUserInterface
	{
		// Token: 0x06001C23 RID: 7203 RVA: 0x000021BC File Offset: 0x000003BC
		public AnomalyScannerBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
		}

		// Token: 0x06001C24 RID: 7204 RVA: 0x000A34B1 File Offset: 0x000A16B1
		protected override void Open()
		{
			base.Open();
			this._menu = new AnomalyScannerMenu();
			this._menu.OpenCentered();
		}

		// Token: 0x06001C25 RID: 7205 RVA: 0x000A34D0 File Offset: 0x000A16D0
		protected override void UpdateState(BoundUserInterfaceState state)
		{
			base.UpdateState(state);
			AnomalyScannerUserInterfaceState anomalyScannerUserInterfaceState = state as AnomalyScannerUserInterfaceState;
			if (anomalyScannerUserInterfaceState == null)
			{
				return;
			}
			if (this._menu == null)
			{
				return;
			}
			this._menu.LastMessage = anomalyScannerUserInterfaceState.Message;
			this._menu.NextPulseTime = anomalyScannerUserInterfaceState.NextPulseTime;
			this._menu.UpdateMenu();
		}

		// Token: 0x06001C26 RID: 7206 RVA: 0x000A3525 File Offset: 0x000A1725
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!disposing)
			{
				return;
			}
			AnomalyScannerMenu menu = this._menu;
			if (menu == null)
			{
				return;
			}
			menu.Dispose();
		}

		// Token: 0x04000E22 RID: 3618
		[Nullable(2)]
		private AnomalyScannerMenu _menu;
	}
}
