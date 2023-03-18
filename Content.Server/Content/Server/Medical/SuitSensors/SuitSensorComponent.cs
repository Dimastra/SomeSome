using System;
using System.Runtime.CompilerServices;
using Content.Shared.Medical.SuitSensor;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Medical.SuitSensors
{
	// Token: 0x020003B5 RID: 949
	[NullableContext(2)]
	[Nullable(0)]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(SuitSensorSystem)
	})]
	public sealed class SuitSensorComponent : Component
	{
		// Token: 0x04000BE6 RID: 3046
		[DataField("randomMode", false, 1, false, false, null)]
		public bool RandomMode = true;

		// Token: 0x04000BE7 RID: 3047
		[DataField("controlsLocked", false, 1, false, false, null)]
		public bool ControlsLocked;

		// Token: 0x04000BE8 RID: 3048
		[DataField("mode", false, 1, false, false, null)]
		public SuitSensorMode Mode;

		// Token: 0x04000BE9 RID: 3049
		[Nullable(1)]
		[DataField("activationSlot", false, 1, false, false, null)]
		public string ActivationSlot = "jumpsuit";

		// Token: 0x04000BEA RID: 3050
		[DataField("activationContainer", false, 1, false, false, null)]
		public string ActivationContainer;

		// Token: 0x04000BEB RID: 3051
		[DataField("updateRate", false, 1, false, false, null)]
		public TimeSpan UpdateRate = TimeSpan.FromSeconds(2.0);

		// Token: 0x04000BEC RID: 3052
		[ViewVariables]
		public EntityUid? User;

		// Token: 0x04000BED RID: 3053
		public TimeSpan LastUpdate = TimeSpan.Zero;

		// Token: 0x04000BEE RID: 3054
		[DataField("station", false, 1, false, false, null)]
		public EntityUid? StationId;

		// Token: 0x04000BEF RID: 3055
		[DataField("server", false, 1, false, false, null)]
		public string ConnectedServer;
	}
}
