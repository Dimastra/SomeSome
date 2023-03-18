using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Research.Prototypes
{
	// Token: 0x02000203 RID: 515
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Prototype("technology", 1)]
	[Serializable]
	public sealed class TechnologyPrototype : IPrototype
	{
		// Token: 0x17000116 RID: 278
		// (get) Token: 0x060005B2 RID: 1458 RVA: 0x00014A56 File Offset: 0x00012C56
		[ViewVariables]
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x17000117 RID: 279
		// (get) Token: 0x060005B3 RID: 1459 RVA: 0x00014A5E File Offset: 0x00012C5E
		// (set) Token: 0x060005B4 RID: 1460 RVA: 0x00014A66 File Offset: 0x00012C66
		[Nullable(2)]
		[DataField("name", false, 1, false, false, null)]
		public string Name { [NullableContext(2)] get; [NullableContext(2)] private set; }

		// Token: 0x17000118 RID: 280
		// (get) Token: 0x060005B5 RID: 1461 RVA: 0x00014A6F File Offset: 0x00012C6F
		[DataField("icon", false, 1, false, false, null)]
		public SpriteSpecifier Icon { get; } = SpriteSpecifier.Invalid;

		// Token: 0x17000119 RID: 281
		// (get) Token: 0x060005B6 RID: 1462 RVA: 0x00014A77 File Offset: 0x00012C77
		// (set) Token: 0x060005B7 RID: 1463 RVA: 0x00014A7F File Offset: 0x00012C7F
		[DataField("description", false, 1, false, false, null)]
		public string Description { get; private set; } = "";

		// Token: 0x1700011A RID: 282
		// (get) Token: 0x060005B8 RID: 1464 RVA: 0x00014A88 File Offset: 0x00012C88
		[DataField("requiredPoints", false, 1, false, false, null)]
		public int RequiredPoints { get; }

		// Token: 0x1700011B RID: 283
		// (get) Token: 0x060005B9 RID: 1465 RVA: 0x00014A90 File Offset: 0x00012C90
		[DataField("requiredTechnologies", false, 1, false, false, typeof(PrototypeIdListSerializer<TechnologyPrototype>))]
		public List<string> RequiredTechnologies { get; } = new List<string>();

		// Token: 0x1700011C RID: 284
		// (get) Token: 0x060005BA RID: 1466 RVA: 0x00014A98 File Offset: 0x00012C98
		[DataField("unlockedRecipes", false, 1, false, false, typeof(PrototypeIdListSerializer<LatheRecipePrototype>))]
		public List<string> UnlockedRecipes { get; } = new List<string>();
	}
}
