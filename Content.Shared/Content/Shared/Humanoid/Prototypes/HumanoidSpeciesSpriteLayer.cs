using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Shared.Humanoid.Prototypes
{
	// Token: 0x02000416 RID: 1046
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("humanoidBaseSprite", 1)]
	public sealed class HumanoidSpeciesSpriteLayer : IPrototype
	{
		// Token: 0x17000277 RID: 631
		// (get) Token: 0x06000C54 RID: 3156 RVA: 0x00028B76 File Offset: 0x00026D76
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x17000278 RID: 632
		// (get) Token: 0x06000C55 RID: 3157 RVA: 0x00028B7E File Offset: 0x00026D7E
		[Nullable(2)]
		[DataField("baseSprite", false, 1, false, false, null)]
		public SpriteSpecifier BaseSprite { [NullableContext(2)] get; }

		// Token: 0x17000279 RID: 633
		// (get) Token: 0x06000C56 RID: 3158 RVA: 0x00028B86 File Offset: 0x00026D86
		[DataField("layerAlpha", false, 1, false, false, null)]
		public float LayerAlpha { get; } = 1f;

		// Token: 0x1700027A RID: 634
		// (get) Token: 0x06000C57 RID: 3159 RVA: 0x00028B8E File Offset: 0x00026D8E
		[DataField("allowsMarkings", false, 1, false, false, null)]
		public bool AllowsMarkings { get; } = 1;

		// Token: 0x1700027B RID: 635
		// (get) Token: 0x06000C58 RID: 3160 RVA: 0x00028B96 File Offset: 0x00026D96
		[DataField("matchSkin", false, 1, false, false, null)]
		public bool MatchSkin { get; } = 1;

		// Token: 0x1700027C RID: 636
		// (get) Token: 0x06000C59 RID: 3161 RVA: 0x00028B9E File Offset: 0x00026D9E
		[DataField("markingsMatchSkin", false, 1, false, false, null)]
		public bool MarkingsMatchSkin { get; }
	}
}
