using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.AirlockPainter.Prototypes
{
	// Token: 0x0200072A RID: 1834
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("AirlockGroup", 1)]
	public sealed class AirlockGroupPrototype : IPrototype
	{
		// Token: 0x170004A5 RID: 1189
		// (get) Token: 0x0600163D RID: 5693 RVA: 0x00048C3E File Offset: 0x00046E3E
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x04001688 RID: 5768
		[DataField("stylePaths", false, 1, false, false, null)]
		public Dictionary<string, string> StylePaths;

		// Token: 0x04001689 RID: 5769
		[DataField("iconPriority", false, 1, false, false, null)]
		public int IconPriority;
	}
}
