using System;
using System.Runtime.CompilerServices;
using Content.Shared.Medical.CrewMonitoring;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.Medical.CrewMonitoring
{
	// Token: 0x02000237 RID: 567
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class CrewMonitoringBoundUserInterface : BoundUserInterface
	{
		// Token: 0x06000E76 RID: 3702 RVA: 0x000021BC File Offset: 0x000003BC
		public CrewMonitoringBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
		}

		// Token: 0x06000E77 RID: 3703 RVA: 0x0005741C File Offset: 0x0005561C
		protected override void Open()
		{
			this._menu = new CrewMonitoringWindow();
			this._menu.OpenCentered();
			this._menu.OnClose += base.Close;
		}

		// Token: 0x06000E78 RID: 3704 RVA: 0x0005744C File Offset: 0x0005564C
		protected override void UpdateState(BoundUserInterfaceState state)
		{
			base.UpdateState(state);
			CrewMonitoringState crewMonitoringState = state as CrewMonitoringState;
			if (crewMonitoringState != null)
			{
				CrewMonitoringWindow menu = this._menu;
				if (menu == null)
				{
					return;
				}
				menu.ShowSensors(crewMonitoringState.Sensors, crewMonitoringState.WorldPosition, crewMonitoringState.Snap, crewMonitoringState.Precision);
			}
		}

		// Token: 0x06000E79 RID: 3705 RVA: 0x00057492 File Offset: 0x00055692
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!disposing)
			{
				return;
			}
			CrewMonitoringWindow menu = this._menu;
			if (menu == null)
			{
				return;
			}
			menu.Dispose();
		}

		// Token: 0x04000733 RID: 1843
		[Nullable(2)]
		private CrewMonitoringWindow _menu;
	}
}
