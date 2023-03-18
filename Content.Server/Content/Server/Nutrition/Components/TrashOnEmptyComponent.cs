using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Nutrition.Components
{
	// Token: 0x02000321 RID: 801
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class TrashOnEmptyComponent : Component
	{
		// Token: 0x1700026E RID: 622
		// (get) Token: 0x06001093 RID: 4243 RVA: 0x0005542F File Offset: 0x0005362F
		// (set) Token: 0x06001094 RID: 4244 RVA: 0x00055437 File Offset: 0x00053637
		[DataField("solution", false, 1, false, false, null)]
		public string Solution { get; set; } = string.Empty;
	}
}
