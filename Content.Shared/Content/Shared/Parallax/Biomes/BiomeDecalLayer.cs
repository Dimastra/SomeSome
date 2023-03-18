using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Decals;
using Content.Shared.Maps;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;

namespace Content.Shared.Parallax.Biomes
{
	// Token: 0x020002A1 RID: 673
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class BiomeDecalLayer : IBiomeWorldLayer, IBiomeLayer
	{
		// Token: 0x17000175 RID: 373
		// (get) Token: 0x0600077D RID: 1917 RVA: 0x00019422 File Offset: 0x00017622
		[DataField("allowedTiles", false, 1, false, false, typeof(PrototypeIdListSerializer<ContentTileDefinition>))]
		public List<string> AllowedTiles { get; } = new List<string>();

		// Token: 0x17000176 RID: 374
		// (get) Token: 0x0600077E RID: 1918 RVA: 0x0001942A File Offset: 0x0001762A
		[DataField("seedOffset", false, 1, false, false, null)]
		public int SeedOffset { get; }

		// Token: 0x17000177 RID: 375
		// (get) Token: 0x0600077F RID: 1919 RVA: 0x00019432 File Offset: 0x00017632
		[DataField("frequency", false, 1, false, false, null)]
		public float Frequency { get; } = 0.25f;

		// Token: 0x17000178 RID: 376
		// (get) Token: 0x06000780 RID: 1920 RVA: 0x0001943A File Offset: 0x0001763A
		[DataField("threshold", false, 1, false, false, null)]
		public float Threshold { get; } = 0.8f;

		// Token: 0x040007A0 RID: 1952
		[DataField("divisions", false, 1, false, false, null)]
		public float Divisions = 1f;

		// Token: 0x040007A4 RID: 1956
		[DataField("decals", false, 1, true, false, typeof(PrototypeIdListSerializer<DecalPrototype>))]
		public List<string> Decals = new List<string>();
	}
}
