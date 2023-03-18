using System;
using System.Runtime.CompilerServices;
using Content.Shared.Construction.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Power.Components
{
	// Token: 0x020002BC RID: 700
	[RegisterComponent]
	public sealed class UpgradeBatteryComponent : Component
	{
		// Token: 0x0400084A RID: 2122
		[Nullable(1)]
		[DataField("machinePartPowerCapacity", false, 1, false, false, typeof(PrototypeIdSerializer<MachinePartPrototype>))]
		public string MachinePartPowerCapacity = "Capacitor";

		// Token: 0x0400084B RID: 2123
		[DataField("maxChargeMultiplier", false, 1, false, false, null)]
		public float MaxChargeMultiplier = 2f;

		// Token: 0x0400084C RID: 2124
		[DataField("baseMaxCharge", false, 1, false, false, null)]
		public float BaseMaxCharge = 8000000f;
	}
}
