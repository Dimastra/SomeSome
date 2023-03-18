using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Warps
{
	// Token: 0x020000BC RID: 188
	[NullableContext(2)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class WarpPointComponent : Component
	{
		// Token: 0x17000067 RID: 103
		// (get) Token: 0x0600031B RID: 795 RVA: 0x00010CBE File Offset: 0x0000EEBE
		// (set) Token: 0x0600031C RID: 796 RVA: 0x00010CC6 File Offset: 0x0000EEC6
		[ViewVariables]
		[DataField("location", false, 1, false, false, null)]
		public string Location { get; set; }

		// Token: 0x04000217 RID: 535
		[DataField("follow", false, 1, false, false, null)]
		public readonly bool Follow;
	}
}
