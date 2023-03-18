using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Monitor;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Server.Atmos.Monitor.Components
{
	// Token: 0x02000786 RID: 1926
	[NullableContext(2)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class AtmosMonitorComponent : Component
	{
		// Token: 0x04001975 RID: 6517
		[ViewVariables]
		public bool NetEnabled = true;

		// Token: 0x04001976 RID: 6518
		[DataField("temperatureThreshold", false, 1, false, false, typeof(PrototypeIdSerializer<AtmosAlarmThreshold>))]
		public readonly string TemperatureThresholdId;

		// Token: 0x04001977 RID: 6519
		[ViewVariables]
		public AtmosAlarmThreshold TemperatureThreshold;

		// Token: 0x04001978 RID: 6520
		[DataField("pressureThreshold", false, 1, false, false, typeof(PrototypeIdSerializer<AtmosAlarmThreshold>))]
		public readonly string PressureThresholdId;

		// Token: 0x04001979 RID: 6521
		[ViewVariables]
		public AtmosAlarmThreshold PressureThreshold;

		// Token: 0x0400197A RID: 6522
		[DataField("monitorFire", false, 1, false, false, null)]
		public bool MonitorFire;

		// Token: 0x0400197B RID: 6523
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[DataField("gasThresholds", false, 1, false, false, null)]
		public Dictionary<Gas, string> GasThresholdIds;

		// Token: 0x0400197C RID: 6524
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[ViewVariables]
		public Dictionary<Gas, AtmosAlarmThreshold> GasThresholds;

		// Token: 0x0400197D RID: 6525
		[ViewVariables]
		public GasMixture TileGas;

		// Token: 0x0400197E RID: 6526
		[ViewVariables]
		public AtmosAlarmType LastAlarmState = AtmosAlarmType.Normal;

		// Token: 0x0400197F RID: 6527
		[Nullable(1)]
		[ViewVariables]
		public HashSet<AtmosMonitorThresholdType> TrippedThresholds = new HashSet<AtmosMonitorThresholdType>();

		// Token: 0x04001980 RID: 6528
		[Nullable(1)]
		[ViewVariables]
		public HashSet<string> RegisteredDevices = new HashSet<string>();
	}
}
