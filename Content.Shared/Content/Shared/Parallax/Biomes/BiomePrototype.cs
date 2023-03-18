using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Parallax.Biomes
{
	// Token: 0x0200029D RID: 669
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("biome", 1)]
	public sealed class BiomePrototype : IPrototype
	{
		// Token: 0x1700016D RID: 365
		// (get) Token: 0x06000773 RID: 1907 RVA: 0x000193C6 File Offset: 0x000175C6
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x04000799 RID: 1945
		[DataField("layers", false, 1, false, false, null)]
		public List<IBiomeLayer> Layers = new List<IBiomeLayer>();
	}
}
