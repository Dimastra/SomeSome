using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Examine
{
	// Token: 0x020004AF RID: 1199
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	public sealed class ExamineGroup
	{
		// Token: 0x04000DAE RID: 3502
		[Nullable(2)]
		[DataField("title", false, 1, false, false, null)]
		[ViewVariables]
		public string Title;

		// Token: 0x04000DAF RID: 3503
		[DataField("entries", false, 1, false, false, null)]
		public List<ExamineEntry> Entries = new List<ExamineEntry>();

		// Token: 0x04000DB0 RID: 3504
		[DataField("components", false, 1, false, false, null)]
		public List<string> Components = new List<string>();

		// Token: 0x04000DB1 RID: 3505
		[DataField("icon", false, 1, false, false, null)]
		public string Icon = "/Textures/Interface/examine-star.png";

		// Token: 0x04000DB2 RID: 3506
		[DataField("contextText", false, 1, false, false, null)]
		public string ContextText = string.Empty;

		// Token: 0x04000DB3 RID: 3507
		[DataField("hoverMessage", false, 1, false, false, null)]
		public string HoverMessage = string.Empty;
	}
}
