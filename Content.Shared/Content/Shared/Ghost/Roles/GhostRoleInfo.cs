using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Serialization;

namespace Content.Shared.Ghost.Roles
{
	// Token: 0x02000459 RID: 1113
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public struct GhostRoleInfo
	{
		// Token: 0x170002CF RID: 719
		// (get) Token: 0x06000D88 RID: 3464 RVA: 0x0002C8F0 File Offset: 0x0002AAF0
		// (set) Token: 0x06000D89 RID: 3465 RVA: 0x0002C8F8 File Offset: 0x0002AAF8
		public uint Identifier { readonly get; set; }

		// Token: 0x170002D0 RID: 720
		// (get) Token: 0x06000D8A RID: 3466 RVA: 0x0002C901 File Offset: 0x0002AB01
		// (set) Token: 0x06000D8B RID: 3467 RVA: 0x0002C909 File Offset: 0x0002AB09
		public string Name { readonly get; set; }

		// Token: 0x170002D1 RID: 721
		// (get) Token: 0x06000D8C RID: 3468 RVA: 0x0002C912 File Offset: 0x0002AB12
		// (set) Token: 0x06000D8D RID: 3469 RVA: 0x0002C91A File Offset: 0x0002AB1A
		public string Description { readonly get; set; }

		// Token: 0x170002D2 RID: 722
		// (get) Token: 0x06000D8E RID: 3470 RVA: 0x0002C923 File Offset: 0x0002AB23
		// (set) Token: 0x06000D8F RID: 3471 RVA: 0x0002C92B File Offset: 0x0002AB2B
		public string Rules { readonly get; set; }
	}
}
