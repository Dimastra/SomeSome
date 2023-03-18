using System;
using System.Runtime.CompilerServices;
using Content.Shared.Whitelist;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Storage.Components
{
	// Token: 0x02000142 RID: 322
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	[Serializable]
	public sealed class SharedMapLayerData
	{
		// Token: 0x170000B8 RID: 184
		// (get) Token: 0x060003E3 RID: 995 RVA: 0x0000FB2F File Offset: 0x0000DD2F
		// (set) Token: 0x060003E4 RID: 996 RVA: 0x0000FB37 File Offset: 0x0000DD37
		[DataField("whitelist", false, 1, true, true, null)]
		public EntityWhitelist ServerWhitelist { get; set; } = new EntityWhitelist();

		// Token: 0x040003C1 RID: 961
		public string Layer = string.Empty;

		// Token: 0x040003C3 RID: 963
		[DataField("minCount", false, 1, false, false, null)]
		public int MinCount = 1;

		// Token: 0x040003C4 RID: 964
		[DataField("maxCount", false, 1, false, false, null)]
		public int MaxCount = int.MaxValue;
	}
}
