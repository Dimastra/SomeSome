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
	// Token: 0x020002BE RID: 702
	[RegisterComponent]
	public sealed class UpgradePowerSupplierComponent : Component
	{
		// Token: 0x04000851 RID: 2129
		[ViewVariables]
		public float BaseSupplyRate;

		// Token: 0x04000852 RID: 2130
		[Nullable(1)]
		[DataField("machinePartPowerSupply", false, 1, false, false, typeof(PrototypeIdSerializer<MachinePartPrototype>))]
		[ViewVariables]
		public string MachinePartPowerSupply = "Capacitor";

		// Token: 0x04000853 RID: 2131
		[DataField("powerSupplyMultiplier", false, 1, true, false, null)]
		[ViewVariables]
		public float PowerSupplyMultiplier = 1f;

		// Token: 0x04000854 RID: 2132
		[DataField("scaling", false, 1, true, false, null)]
		[ViewVariables]
		public MachineUpgradeScalingType Scaling;
	}
}
