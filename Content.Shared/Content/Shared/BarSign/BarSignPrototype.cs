using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.BarSign
{
	// Token: 0x0200067E RID: 1662
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("barSign", 1)]
	public sealed class BarSignPrototype : IPrototype
	{
		// Token: 0x17000409 RID: 1033
		// (get) Token: 0x06001451 RID: 5201 RVA: 0x0004405F File Offset: 0x0004225F
		[ViewVariables]
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x1700040A RID: 1034
		// (get) Token: 0x06001452 RID: 5202 RVA: 0x00044067 File Offset: 0x00042267
		// (set) Token: 0x06001453 RID: 5203 RVA: 0x0004406F File Offset: 0x0004226F
		[DataField("icon", false, 1, false, false, null)]
		public string Icon { get; private set; } = string.Empty;

		// Token: 0x1700040B RID: 1035
		// (get) Token: 0x06001454 RID: 5204 RVA: 0x00044078 File Offset: 0x00042278
		// (set) Token: 0x06001455 RID: 5205 RVA: 0x00044080 File Offset: 0x00042280
		[DataField("name", false, 1, false, false, null)]
		public string Name { get; set; } = "";

		// Token: 0x1700040C RID: 1036
		// (get) Token: 0x06001456 RID: 5206 RVA: 0x00044089 File Offset: 0x00042289
		// (set) Token: 0x06001457 RID: 5207 RVA: 0x00044091 File Offset: 0x00042291
		[DataField("description", false, 1, false, false, null)]
		public string Description { get; set; } = "";

		// Token: 0x1700040D RID: 1037
		// (get) Token: 0x06001458 RID: 5208 RVA: 0x0004409A File Offset: 0x0004229A
		// (set) Token: 0x06001459 RID: 5209 RVA: 0x000440A2 File Offset: 0x000422A2
		[DataField("renameArea", false, 1, false, false, null)]
		public bool RenameArea { get; private set; } = true;

		// Token: 0x1700040E RID: 1038
		// (get) Token: 0x0600145A RID: 5210 RVA: 0x000440AB File Offset: 0x000422AB
		// (set) Token: 0x0600145B RID: 5211 RVA: 0x000440B3 File Offset: 0x000422B3
		[DataField("hidden", false, 1, false, false, null)]
		public bool Hidden { get; private set; }
	}
}
