using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Medical.SuitSensor;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Medical.CrewMonitoring
{
	// Token: 0x020003B9 RID: 953
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(CrewMonitoringServerSystem)
	})]
	public sealed class CrewMonitoringServerComponent : Component
	{
		// Token: 0x04000C03 RID: 3075
		[Nullable(1)]
		public readonly Dictionary<string, SuitSensorStatus> SensorStatus = new Dictionary<string, SuitSensorStatus>();

		// Token: 0x04000C04 RID: 3076
		[DataField("sensorTimeout", false, 1, false, false, null)]
		[ViewVariables]
		public float SensorTimeout = 10f;

		// Token: 0x04000C05 RID: 3077
		[ViewVariables]
		public bool Available = true;

		// Token: 0x04000C06 RID: 3078
		[ViewVariables]
		public bool Active = true;
	}
}
