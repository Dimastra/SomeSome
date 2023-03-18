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
	// Token: 0x020003B7 RID: 951
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(CrewMonitoringConsoleSystem)
	})]
	public sealed class CrewMonitoringConsoleComponent : Component
	{
		// Token: 0x04000BFB RID: 3067
		[Nullable(1)]
		public Dictionary<string, SuitSensorStatus> ConnectedSensors = new Dictionary<string, SuitSensorStatus>();

		// Token: 0x04000BFC RID: 3068
		[DataField("sensorTimeout", false, 1, false, false, null)]
		[ViewVariables]
		public float SensorTimeout = 10f;

		// Token: 0x04000BFD RID: 3069
		[DataField("snap", false, 1, false, false, null)]
		[ViewVariables]
		public bool Snap = true;

		// Token: 0x04000BFE RID: 3070
		[DataField("precision", false, 1, false, false, null)]
		[ViewVariables]
		public float Precision = 10f;
	}
}
