using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Medical.SuitSensor;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Medical.CrewMonitoring
{
	// Token: 0x02000312 RID: 786
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class CrewMonitoringState : BoundUserInterfaceState
	{
		// Token: 0x06000910 RID: 2320 RVA: 0x0001E7A3 File Offset: 0x0001C9A3
		public CrewMonitoringState(List<SuitSensorStatus> sensors, Vector2 worldPosition, bool snap, float precision)
		{
			this.Sensors = sensors;
			this.WorldPosition = worldPosition;
			this.Snap = snap;
			this.Precision = precision;
		}

		// Token: 0x04000900 RID: 2304
		public List<SuitSensorStatus> Sensors;

		// Token: 0x04000901 RID: 2305
		public readonly Vector2 WorldPosition;

		// Token: 0x04000902 RID: 2306
		public readonly bool Snap;

		// Token: 0x04000903 RID: 2307
		public readonly float Precision;
	}
}
