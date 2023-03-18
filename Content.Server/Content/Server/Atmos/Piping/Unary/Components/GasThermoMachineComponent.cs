using System;
using System.Runtime.CompilerServices;
using Content.Shared.Atmos.Piping.Unary.Components;
using Content.Shared.Construction.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Server.Atmos.Piping.Unary.Components
{
	// Token: 0x02000753 RID: 1875
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class GasThermoMachineComponent : Component
	{
		// Token: 0x0400188B RID: 6283
		[DataField("inlet", false, 1, false, false, null)]
		public string InletName = "pipe";

		// Token: 0x0400188C RID: 6284
		[ViewVariables]
		[DataField("enabled", false, 1, false, false, null)]
		public bool Enabled = true;

		// Token: 0x0400188D RID: 6285
		[ViewVariables]
		public float HeatCapacity = 10000f;

		// Token: 0x0400188E RID: 6286
		[DataField("baseHeatCapacity", false, 1, false, false, null)]
		public float BaseHeatCapacity = 5000f;

		// Token: 0x0400188F RID: 6287
		[DataField("targetTemperature", false, 1, false, false, null)]
		[ViewVariables]
		public float TargetTemperature = 293.15f;

		// Token: 0x04001890 RID: 6288
		[DataField("mode", false, 1, false, false, null)]
		public ThermoMachineMode Mode;

		// Token: 0x04001891 RID: 6289
		[ViewVariables]
		public float MinTemperature;

		// Token: 0x04001892 RID: 6290
		[ViewVariables]
		public float MaxTemperature;

		// Token: 0x04001893 RID: 6291
		[DataField("baseMinTemperature", false, 1, false, false, null)]
		[ViewVariables]
		public float BaseMinTemperature = 96.625f;

		// Token: 0x04001894 RID: 6292
		[DataField("baseMaxTemperature", false, 1, false, false, null)]
		[ViewVariables]
		public float BaseMaxTemperature = 293.15f;

		// Token: 0x04001895 RID: 6293
		[DataField("minTemperatureDelta", false, 1, false, false, null)]
		[ViewVariables]
		public float MinTemperatureDelta = 23.475f;

		// Token: 0x04001896 RID: 6294
		[DataField("maxTemperatureDelta", false, 1, false, false, null)]
		[ViewVariables]
		public float MaxTemperatureDelta = 300f;

		// Token: 0x04001897 RID: 6295
		[DataField("machinePartHeatCapacity", false, 1, false, false, typeof(PrototypeIdSerializer<MachinePartPrototype>))]
		public string MachinePartHeatCapacity = "MatterBin";

		// Token: 0x04001898 RID: 6296
		[DataField("machinePartTemperature", false, 1, false, false, typeof(PrototypeIdSerializer<MachinePartPrototype>))]
		public string MachinePartTemperature = "Laser";
	}
}
