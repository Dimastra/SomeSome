using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Monitor.Components
{
	// Token: 0x020006D6 RID: 1750
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class AirAlarmUIState : BoundUserInterfaceState
	{
		// Token: 0x06001540 RID: 5440 RVA: 0x00045C30 File Offset: 0x00043E30
		public AirAlarmUIState(string address, int deviceCount, float pressureAverage, float temperatureAverage, Dictionary<string, IAtmosDeviceData> deviceData, AirAlarmMode mode, AirAlarmTab tab, AtmosAlarmType alarmType)
		{
			this.Address = address;
			this.DeviceCount = deviceCount;
			this.PressureAverage = pressureAverage;
			this.TemperatureAverage = temperatureAverage;
			this.DeviceData = deviceData;
			this.Mode = mode;
			this.Tab = tab;
			this.AlarmType = alarmType;
		}

		// Token: 0x17000471 RID: 1137
		// (get) Token: 0x06001541 RID: 5441 RVA: 0x00045C80 File Offset: 0x00043E80
		public string Address { get; }

		// Token: 0x17000472 RID: 1138
		// (get) Token: 0x06001542 RID: 5442 RVA: 0x00045C88 File Offset: 0x00043E88
		public int DeviceCount { get; }

		// Token: 0x17000473 RID: 1139
		// (get) Token: 0x06001543 RID: 5443 RVA: 0x00045C90 File Offset: 0x00043E90
		public float PressureAverage { get; }

		// Token: 0x17000474 RID: 1140
		// (get) Token: 0x06001544 RID: 5444 RVA: 0x00045C98 File Offset: 0x00043E98
		public float TemperatureAverage { get; }

		// Token: 0x17000475 RID: 1141
		// (get) Token: 0x06001545 RID: 5445 RVA: 0x00045CA0 File Offset: 0x00043EA0
		public Dictionary<string, IAtmosDeviceData> DeviceData { get; }

		// Token: 0x17000476 RID: 1142
		// (get) Token: 0x06001546 RID: 5446 RVA: 0x00045CA8 File Offset: 0x00043EA8
		public AirAlarmMode Mode { get; }

		// Token: 0x17000477 RID: 1143
		// (get) Token: 0x06001547 RID: 5447 RVA: 0x00045CB0 File Offset: 0x00043EB0
		public AirAlarmTab Tab { get; }

		// Token: 0x17000478 RID: 1144
		// (get) Token: 0x06001548 RID: 5448 RVA: 0x00045CB8 File Offset: 0x00043EB8
		public AtmosAlarmType AlarmType { get; }
	}
}
