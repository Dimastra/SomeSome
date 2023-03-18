using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Shared.Decals
{
	// Token: 0x02000527 RID: 1319
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("decal", 1)]
	public sealed class DecalPrototype : IPrototype
	{
		// Token: 0x1700032D RID: 813
		// (get) Token: 0x06000FF8 RID: 4088 RVA: 0x00033730 File Offset: 0x00031930
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x1700032E RID: 814
		// (get) Token: 0x06000FF9 RID: 4089 RVA: 0x00033738 File Offset: 0x00031938
		[DataField("sprite", false, 1, false, false, null)]
		public SpriteSpecifier Sprite { get; } = SpriteSpecifier.Invalid;

		// Token: 0x04000F21 RID: 3873
		[DataField("tags", false, 1, false, false, null)]
		public List<string> Tags = new List<string>();

		// Token: 0x04000F22 RID: 3874
		[DataField("showMenu", false, 1, false, false, null)]
		public bool ShowMenu = true;

		// Token: 0x04000F23 RID: 3875
		[DataField("snapCardinals", false, 1, false, false, null)]
		public bool SnapCardinals;
	}
}
