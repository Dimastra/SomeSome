using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Maps;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;

namespace Content.Shared.Parallax.Biomes
{
	// Token: 0x020002A2 RID: 674
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class BiomeEntityLayer : IBiomeWorldLayer, IBiomeLayer
	{
		// Token: 0x17000179 RID: 377
		// (get) Token: 0x06000782 RID: 1922 RVA: 0x00019481 File Offset: 0x00017681
		[DataField("allowedTiles", false, 1, false, false, typeof(PrototypeIdListSerializer<ContentTileDefinition>))]
		public List<string> AllowedTiles { get; } = new List<string>();

		// Token: 0x1700017A RID: 378
		// (get) Token: 0x06000783 RID: 1923 RVA: 0x00019489 File Offset: 0x00017689
		[DataField("threshold", false, 1, false, false, null)]
		public float Threshold { get; } = 0.5f;

		// Token: 0x1700017B RID: 379
		// (get) Token: 0x06000784 RID: 1924 RVA: 0x00019491 File Offset: 0x00017691
		[DataField("seedOffset", false, 1, false, false, null)]
		public int SeedOffset { get; }

		// Token: 0x1700017C RID: 380
		// (get) Token: 0x06000785 RID: 1925 RVA: 0x00019499 File Offset: 0x00017699
		[DataField("frequency", false, 1, false, false, null)]
		public float Frequency { get; } = 0.1f;

		// Token: 0x040007A9 RID: 1961
		[DataField("entities", false, 1, true, false, typeof(PrototypeIdListSerializer<EntityPrototype>))]
		public List<string> Entities = new List<string>();
	}
}
