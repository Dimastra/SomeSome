using System;
using System.Runtime.CompilerServices;
using Content.Shared.Damage;
using Content.Shared.Tools;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Server.Borgs
{
	// Token: 0x0200009E RID: 158
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class BorgRepairComponent : Component
	{
		// Token: 0x040001CF RID: 463
		[ViewVariables]
		[DataField("qualityNeeded", false, 1, false, false, typeof(PrototypeIdSerializer<ToolQualityPrototype>))]
		public string QualityNeeded = "Welding";

		// Token: 0x040001D0 RID: 464
		[ViewVariables]
		[DataField("selfRepairPenalty", false, 1, false, false, null)]
		public float SelfRepairPenalty = 3f;

		// Token: 0x040001D1 RID: 465
		[DataField("damage", false, 1, true, false, null)]
		[ViewVariables]
		public DamageSpecifier Damage;
	}
}
