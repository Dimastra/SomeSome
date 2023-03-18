using System;
using System.Collections.Generic;
using Content.Server.Atmos.Monitor.Components;
using Content.Shared.Atmos.Piping.Unary.Components;
using Robust.Shared.GameObjects;

namespace Content.Server.Atmos.Monitor
{
	// Token: 0x02000778 RID: 1912
	public sealed class AirAlarmFilterMode : AirAlarmModeExecutor
	{
		// Token: 0x0600288F RID: 10383 RVA: 0x000D3768 File Offset: 0x000D1968
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
				this.AirAlarmSystem.SetData(uid, addr, GasVentPumpData.FilterModePreset);
			}
			foreach (KeyValuePair<string, GasVentScrubberData> keyValuePair2 in alarm.ScrubberData)
			{
				string text;
				GasVentScrubberData gasVentScrubberData;
				keyValuePair2.Deconstruct(out text, out gasVentScrubberData);
				string addr2 = text;
				this.AirAlarmSystem.SetData(uid, addr2, GasVentScrubberData.FilterModePreset);
			}
		}
	}
}
