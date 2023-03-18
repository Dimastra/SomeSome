using System;
using System.Runtime.CompilerServices;
using Content.Shared.Mining;
using Content.Shared.Random;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Server.Mining.Components
{
	// Token: 0x02000009 RID: 9
	[NullableContext(2)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class OreVeinComponent : Component
	{
		// Token: 0x04000005 RID: 5
		[DataField("oreChance", false, 1, false, false, null)]
		public float OreChance = 0.1f;

		// Token: 0x04000006 RID: 6
		[DataField("oreRarityPrototypeId", false, 1, false, false, typeof(PrototypeIdSerializer<WeightedRandomPrototype>))]
		public string OreRarityPrototypeId;

		// Token: 0x04000007 RID: 7
		[DataField("currentOre", false, 1, false, false, typeof(PrototypeIdSerializer<OrePrototype>))]
		[ViewVariables]
		public string CurrentOre;
	}
}
