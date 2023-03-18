using System;
using System.Runtime.CompilerServices;
using Content.Shared.StationRecords;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.StationRecords
{
	// Token: 0x02000130 RID: 304
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GeneralStationRecordConsoleBoundUserInterface : BoundUserInterface
	{
		// Token: 0x0600082F RID: 2095 RVA: 0x000021BC File Offset: 0x000003BC
		public GeneralStationRecordConsoleBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
		}

		// Token: 0x06000830 RID: 2096 RVA: 0x0002F8C4 File Offset: 0x0002DAC4
		protected override void Open()
		{
			base.Open();
			this._window = new GeneralStationRecordConsoleWindow();
			GeneralStationRecordConsoleWindow window = this._window;
			window.OnKeySelected = (Action<StationRecordKey?>)Delegate.Combine(window.OnKeySelected, new Action<StationRecordKey?>(this.OnKeySelected));
			this._window.OnClose += base.Close;
			this._window.OpenCentered();
		}

		// Token: 0x06000831 RID: 2097 RVA: 0x0002F92B File Offset: 0x0002DB2B
		private void OnKeySelected(StationRecordKey? key)
		{
			base.SendMessage(new SelectGeneralStationRecord(key));
		}

		// Token: 0x06000832 RID: 2098 RVA: 0x0002F93C File Offset: 0x0002DB3C
		protected override void UpdateState(BoundUserInterfaceState state)
		{
			base.UpdateState(state);
			GeneralStationRecordConsoleState generalStationRecordConsoleState = state as GeneralStationRecordConsoleState;
			if (generalStationRecordConsoleState == null)
			{
				return;
			}
			GeneralStationRecordConsoleWindow window = this._window;
			if (window == null)
			{
				return;
			}
			window.UpdateState(generalStationRecordConsoleState);
		}

		// Token: 0x06000833 RID: 2099 RVA: 0x0002F96C File Offset: 0x0002DB6C
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			GeneralStationRecordConsoleWindow window = this._window;
			if (window == null)
			{
				return;
			}
			window.Close();
		}

		// Token: 0x04000427 RID: 1063
		[Nullable(2)]
		private GeneralStationRecordConsoleWindow _window;
	}
}
