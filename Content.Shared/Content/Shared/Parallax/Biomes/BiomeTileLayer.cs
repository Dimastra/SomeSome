using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Maps;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.Parallax.Biomes
{
	// Token: 0x0200029F RID: 671
	public sealed class BiomeTileLayer : IBiomeLayer
	{
		// Token: 0x17000171 RID: 369
		// (get) Token: 0x06000778 RID: 1912 RVA: 0x000193E1 File Offset: 0x000175E1
		[DataField("threshold", false, 1, false, false, null)]
		public float Threshold { get; } = 0.5f;

		// Token: 0x17000172 RID: 370
		// (get) Token: 0x06000779 RID: 1913 RVA: 0x000193E9 File Offset: 0x000175E9
		[DataField("seedOffset", false, 1, false, false, null)]
		public int SeedOffset { get; }

		// Token: 0x17000173 RID: 371
		// (get) Token: 0x0600077A RID: 1914 RVA: 0x000193F1 File Offset: 0x000175F1
		[DataField("frequency", false, 1, false, false, null)]
		public float Frequency { get; } = 0.1f;

		// Token: 0x0400079D RID: 1949
		[Nullable(2)]
		[DataField("variants", false, 1, false, false, null)]
		public List<byte> Variants;

		// Token: 0x0400079E RID: 1950
		[Nullable(1)]
		[DataField("tile", false, 1, true, false, typeof(PrototypeIdSerializer<ContentTileDefinition>))]
		public string Tile = string.Empty;
	}
}
