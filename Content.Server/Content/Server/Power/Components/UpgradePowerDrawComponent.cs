using System;
using System.Runtime.CompilerServices;
using Content.Server.Construction.Components;
using Content.Shared.Construction.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Server.Power.Components
{
	// Token: 0x020002BD RID: 701
	[RegisterComponent]
	public sealed class UpgradePowerDrawComponent : Component
	{
		// Token: 0x0400084D RID: 2125
		[ViewVariables]
		public float BaseLoad;

		// Token: 0x0400084E RID: 2126
		[Nullable(1)]
		[DataField("machinePartPowerDraw", false, 1, false, false, typeof(PrototypeIdSerializer<MachinePartPrototype>))]
		[ViewVariables]
		public string MachinePartPowerDraw = "Capacitor";

		// Token: 0x0400084F RID: 2127
		[DataField("powerDrawMultiplier", false, 1, true, false, null)]
		[ViewVariables]
		public float PowerDrawMultiplier = 1f;

		// Token: 0x04000850 RID: 2128
		[DataField("scaling", false, 1, true, false, null)]
		[ViewVariables]
		public MachineUpgradeScalingType Scaling;
	}
}
