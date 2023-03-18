using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Atmos.Monitor.Components;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Monitor
{
	// Token: 0x020006D1 RID: 1745
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class AtmosSensorData : IAtmosDeviceData
	{
		// Token: 0x0600152B RID: 5419 RVA: 0x00045B6C File Offset: 0x00043D6C
		public AtmosSensorData(float pressure, float temperature, float totalMoles, AtmosAlarmType alarmState, Dictionary<Gas, float> gases, AtmosAlarmThreshold pressureThreshold, AtmosAlarmThreshold temperatureThreshold, Dictionary<Gas, AtmosAlarmThreshold> gasThresholds)
		{
			this.Pressure = pressure;
			this.Temperature = temperature;
			this.TotalMoles = totalMoles;
			this.AlarmState = alarmState;
			this.Gases = gases;
			this.PressureThreshold = pressureThreshold;
			this.TemperatureThreshold = temperatureThreshold;
			this.GasThresholds = gasThresholds;
		}

		// Token: 0x17000463 RID: 1123
		// (get) Token: 0x0600152C RID: 5420 RVA: 0x00045BBC File Offset: 0x00043DBC
		// (set) Token: 0x0600152D RID: 5421 RVA: 0x00045BC4 File Offset: 0x00043DC4
		public bool Enabled { get; set; }

		// Token: 0x17000464 RID: 1124
		// (get) Token: 0x0600152E RID: 5422 RVA: 0x00045BCD File Offset: 0x00043DCD
		// (set) Token: 0x0600152F RID: 5423 RVA: 0x00045BD5 File Offset: 0x00043DD5
		public bool Dirty { get; set; }

		// Token: 0x17000465 RID: 1125
		// (get) Token: 0x06001530 RID: 5424 RVA: 0x00045BDE File Offset: 0x00043DDE
		// (set) Token: 0x06001531 RID: 5425 RVA: 0x00045BE6 File Offset: 0x00043DE6
		public bool IgnoreAlarms { get; set; }

		// Token: 0x17000466 RID: 1126
		// (get) Token: 0x06001532 RID: 5426 RVA: 0x00045BEF File Offset: 0x00043DEF
		public float Pressure { get; }

		// Token: 0x17000467 RID: 1127
		// (get) Token: 0x06001533 RID: 5427 RVA: 0x00045BF7 File Offset: 0x00043DF7
		public float Temperature { get; }

		// Token: 0x17000468 RID: 1128
		// (get) Token: 0x06001534 RID: 5428 RVA: 0x00045BFF File Offset: 0x00043DFF
		public float TotalMoles { get; }

		// Token: 0x17000469 RID: 1129
		// (get) Token: 0x06001535 RID: 5429 RVA: 0x00045C07 File Offset: 0x00043E07
		public AtmosAlarmType AlarmState { get; }

		// Token: 0x1700046A RID: 1130
		// (get) Token: 0x06001536 RID: 5430 RVA: 0x00045C0F File Offset: 0x00043E0F
		public Dictionary<Gas, float> Gases { get; }

		// Token: 0x1700046B RID: 1131
		// (get) Token: 0x06001537 RID: 5431 RVA: 0x00045C17 File Offset: 0x00043E17
		public AtmosAlarmThreshold PressureThreshold { get; }

		// Token: 0x1700046C RID: 1132
		// (get) Token: 0x06001538 RID: 5432 RVA: 0x00045C1F File Offset: 0x00043E1F
		public AtmosAlarmThreshold TemperatureThreshold { get; }

		// Token: 0x1700046D RID: 1133
		// (get) Token: 0x06001539 RID: 5433 RVA: 0x00045C27 File Offset: 0x00043E27
		public Dictionary<Gas, AtmosAlarmThreshold> GasThresholds { get; }
	}
}
