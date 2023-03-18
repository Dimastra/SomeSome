using System;
using System.Collections.Generic;
using Content.Server.Atmos.Monitor.Components;
using Content.Shared.Atmos.Piping.Unary.Components;
using Robust.Shared.GameObjects;

namespace Content.Server.Atmos.Monitor
{
	// Token: 0x02000777 RID: 1911
	public sealed class AirAlarmNoneMode : AirAlarmModeExecutor
	{
		// Token: 0x0600288D RID: 10381 RVA: 0x000D3674 File Offset: 0x000D1874
		public override void Execute(EntityUid uid)
		{
			AirAlarmComponent alarm;
			if (!this.EntityManager.TryGetComponent<AirAlarmComponent>(uid, ref alarm))
			{
				return;
			}
			foreach (KeyValuePair<string, GasVentPumpData> keyValuePair in alarm.VentData)
			{
				string text;
				GasVentPumpData gasVentPumpData;
				keyValuePair.Deconstruct(out text, out gasVentPumpData);
				string addr = text;
				GasVentPumpData device = gasVentPumpData;
				device.Enabled = false;
				this.AirAlarmSystem.SetData(uid, addr, device);
			}
			foreach (KeyValuePair<string, GasVentScrubberData> keyValuePair2 in alarm.ScrubberData)
			{
				string text;
				GasVentScrubberData gasVentScrubberData;
				keyValuePair2.Deconstruct(out text, out gasVentScrubberData);
				string addr2 = text;
				GasVentScrubberData device2 = gasVentScrubberData;
				device2.Enabled = false;
				this.AirAlarmSystem.SetData(uid, addr2, device2);
			}
		}
	}
}
