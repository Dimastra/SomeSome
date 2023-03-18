using System;
using System.Runtime.CompilerServices;
using Content.Shared.Tools;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Shared.PneumaticCannon
{
	// Token: 0x0200026A RID: 618
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class PneumaticCannonComponent : Component
	{
		// Token: 0x040006F4 RID: 1780
		public const string TankSlotId = "gas_tank";

		// Token: 0x040006F5 RID: 1781
		[ViewVariables]
		public PneumaticCannonPower Power = PneumaticCannonPower.Medium;

		// Token: 0x040006F6 RID: 1782
		[DataField("toolModifyPower", false, 1, false, false, typeof(PrototypeIdSerializer<ToolQualityPrototype>))]
		public string ToolModifyPower = "Anchoring";

		// Token: 0x040006F7 RID: 1783
		[DataField("highPowerStunTime", false, 1, false, false, null)]
		[ViewVariables]
		public float HighPowerStunTime = 3f;

		// Token: 0x040006F8 RID: 1784
		[DataField("gasUsage", false, 1, false, false, null)]
		[ViewVariables]
		public float GasUsage = 2f;

		// Token: 0x040006F9 RID: 1785
		[DataField("baseProjectileSpeed", false, 1, false, false, null)]
		public float BaseProjectileSpeed = 20f;
	}
}
