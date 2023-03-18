using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Client.Parallax.Data
{
	// Token: 0x020001EA RID: 490
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("parallax", 1)]
	public sealed class ParallaxPrototype : IPrototype
	{
		// Token: 0x1700027F RID: 639
		// (get) Token: 0x06000C92 RID: 3218 RVA: 0x0004955F File Offset: 0x0004775F
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x17000280 RID: 640
		// (get) Token: 0x06000C93 RID: 3219 RVA: 0x00049567 File Offset: 0x00047767
		[DataField("layers", false, 1, false, false, null)]
		public List<ParallaxLayerConfig> Layers { get; } = new List<ParallaxLayerConfig>();

		// Token: 0x17000281 RID: 641
		// (get) Token: 0x06000C94 RID: 3220 RVA: 0x0004956F File Offset: 0x0004776F
		[DataField("layersLQ", false, 1, false, false, null)]
		public List<ParallaxLayerConfig> LayersLQ { get; } = new List<ParallaxLayerConfig>();

		// Token: 0x17000282 RID: 642
		// (get) Token: 0x06000C95 RID: 3221 RVA: 0x00049577 File Offset: 0x00047777
		[DataField("layersLQUseHQ", false, 1, false, false, null)]
		public bool LayersLQUseHQ { get; } = 1;
	}
}
