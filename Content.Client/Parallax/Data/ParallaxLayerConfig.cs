using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Client.Parallax.Data
{
	// Token: 0x020001E9 RID: 489
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	public sealed class ParallaxLayerConfig
	{
		// Token: 0x17000278 RID: 632
		// (get) Token: 0x06000C83 RID: 3203 RVA: 0x000494AD File Offset: 0x000476AD
		// (set) Token: 0x06000C84 RID: 3204 RVA: 0x000494B5 File Offset: 0x000476B5
		[DataField("texture", false, 1, true, false, null)]
		public IParallaxTextureSource Texture { get; set; }

		// Token: 0x17000279 RID: 633
		// (get) Token: 0x06000C85 RID: 3205 RVA: 0x000494BE File Offset: 0x000476BE
		// (set) Token: 0x06000C86 RID: 3206 RVA: 0x000494C6 File Offset: 0x000476C6
		[DataField("scale", false, 1, false, false, null)]
		public Vector2 Scale { get; set; } = Vector2.One;

		// Token: 0x1700027A RID: 634
		// (get) Token: 0x06000C87 RID: 3207 RVA: 0x000494CF File Offset: 0x000476CF
		// (set) Token: 0x06000C88 RID: 3208 RVA: 0x000494D7 File Offset: 0x000476D7
		[DataField("tiled", false, 1, false, false, null)]
		public bool Tiled { get; set; } = true;

		// Token: 0x1700027B RID: 635
		// (get) Token: 0x06000C89 RID: 3209 RVA: 0x000494E0 File Offset: 0x000476E0
		// (set) Token: 0x06000C8A RID: 3210 RVA: 0x000494E8 File Offset: 0x000476E8
		[DataField("controlHomePosition", false, 1, false, false, null)]
		public Vector2 ControlHomePosition { get; set; }

		// Token: 0x1700027C RID: 636
		// (get) Token: 0x06000C8B RID: 3211 RVA: 0x000494F1 File Offset: 0x000476F1
		// (set) Token: 0x06000C8C RID: 3212 RVA: 0x000494F9 File Offset: 0x000476F9
		[DataField("worldHomePosition", false, 1, false, false, null)]
		public Vector2 WorldHomePosition { get; set; }

		// Token: 0x1700027D RID: 637
		// (get) Token: 0x06000C8D RID: 3213 RVA: 0x00049502 File Offset: 0x00047702
		// (set) Token: 0x06000C8E RID: 3214 RVA: 0x0004950A File Offset: 0x0004770A
		[DataField("worldAdjustPosition", false, 1, false, false, null)]
		public Vector2 WorldAdjustPosition { get; set; }

		// Token: 0x1700027E RID: 638
		// (get) Token: 0x06000C8F RID: 3215 RVA: 0x00049513 File Offset: 0x00047713
		// (set) Token: 0x06000C90 RID: 3216 RVA: 0x0004951B File Offset: 0x0004771B
		[DataField("slowness", false, 1, false, false, null)]
		public float Slowness { get; set; } = 0.5f;

		// Token: 0x04000659 RID: 1625
		[DataField("scrolling", false, 1, false, false, null)]
		public Vector2 Scrolling = Vector2.Zero;

		// Token: 0x0400065A RID: 1626
		[Nullable(2)]
		[DataField("shader", false, 1, false, false, null)]
		public string Shader = "unshaded";
	}
}
