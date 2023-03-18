using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Paper
{
	// Token: 0x020002A5 RID: 677
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class StampComponent : Component
	{
		// Token: 0x1700017D RID: 381
		// (get) Token: 0x06000793 RID: 1939 RVA: 0x00019B84 File Offset: 0x00017D84
		// (set) Token: 0x06000794 RID: 1940 RVA: 0x00019B8C File Offset: 0x00017D8C
		[DataField("stampedName", false, 1, false, false, null)]
		public string StampedName { get; set; } = "stamp-component-stamped-name-default";

		// Token: 0x1700017E RID: 382
		// (get) Token: 0x06000795 RID: 1941 RVA: 0x00019B95 File Offset: 0x00017D95
		// (set) Token: 0x06000796 RID: 1942 RVA: 0x00019B9D File Offset: 0x00017D9D
		[DataField("stampState", false, 1, false, false, null)]
		public string StampState { get; set; } = "paper_stamp-generic";
	}
}
