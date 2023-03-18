using System;
using System.Collections.Generic;
using Content.Server.Atmos.Monitor.Components;
using Content.Shared.Atmos.Piping.Unary.Components;
using Robust.Shared.GameObjects;

namespace Content.Server.Atmos.Monitor
{
	// Token: 0x0200077B RID: 1915
	public sealed class AirAlarmFillMode : AirAlarmModeExecutor
	{
		// Token: 0x06002895 RID: 10389 RVA: 0x000D3A14 File Offset: 0x000D1C14
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
				this.AirAlarmSystem.SetData(uid, addr, GasVentPumpData.FillModePreset);
			}
			foreach (KeyValuePair<string, GasVentScrubberData> keyValuePair2 in alarm.ScrubberData)
			{
				string text;
				GasVentScrubberData gasVentScrubberData;
				keyValuePair2.Deconstruct(out text, out gasVentScrubberData);
				string addr2 = text;
				this.AirAlarmSystem.SetData(uid, addr2, GasVentScrubberData.FillModePreset);
			}
		}
	}
}
