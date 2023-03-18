using System;
using System.Runtime.CompilerServices;
using Content.Shared.Damage;
using Content.Shared.Tools;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Server.Repairable
{
	// Token: 0x02000244 RID: 580
	[RegisterComponent]
	public sealed class RepairableComponent : Component
	{
		// Token: 0x0400071D RID: 1821
		[Nullable(2)]
		[ViewVariables]
		[DataField("damage", false, 1, false, false, null)]
		public DamageSpecifier Damage;

		// Token: 0x0400071E RID: 1822
		[ViewVariables]
		[DataField("fuelCost", false, 1, false, false, null)]
		public int FuelCost = 5;

		// Token: 0x0400071F RID: 1823
		[Nullable(1)]
		[ViewVariables]
		[DataField("qualityNeeded", false, 1, false, false, typeof(PrototypeIdSerializer<ToolQualityPrototype>))]
		public string QualityNeeded = "Welding";

		// Token: 0x04000720 RID: 1824
		[ViewVariables]
		[DataField("doAfterDelay", false, 1, false, false, null)]
		public int DoAfterDelay = 1;

		// Token: 0x04000721 RID: 1825
		[ViewVariables]
		[DataField("selfRepairPenalty", false, 1, false, false, null)]
		public float SelfRepairPenalty = 3f;
	}
}
