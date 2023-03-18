using System;
using System.Runtime.CompilerServices;
using Content.Server.Power.EntitySystems;
using Content.Shared.Tools;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Power.Components
{
	// Token: 0x020002AF RID: 687
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(CableSystem)
	})]
	public sealed class CableComponent : Component
	{
		// Token: 0x04000831 RID: 2097
		[DataField("cableDroppedOnCutPrototype", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public readonly string CableDroppedOnCutPrototype = "CableHVStack1";

		// Token: 0x04000832 RID: 2098
		[DataField("cuttingQuality", false, 1, false, false, typeof(PrototypeIdSerializer<ToolQualityPrototype>))]
		public string CuttingQuality = "Cutting";

		// Token: 0x04000833 RID: 2099
		[DataField("cableType", false, 1, false, false, null)]
		public CableType CableType;

		// Token: 0x04000834 RID: 2100
		[DataField("cuttingDelay", false, 1, false, false, null)]
		public float CuttingDelay = 0.25f;
	}
}
