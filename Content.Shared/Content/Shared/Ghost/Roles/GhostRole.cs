using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Ghost.Roles
{
	// Token: 0x02000460 RID: 1120
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class GhostRole
	{
		// Token: 0x170002D7 RID: 727
		// (get) Token: 0x06000D9A RID: 3482 RVA: 0x0002C9A0 File Offset: 0x0002ABA0
		// (set) Token: 0x06000D9B RID: 3483 RVA: 0x0002C9A8 File Offset: 0x0002ABA8
		public string Name { get; set; } = string.Empty;

		// Token: 0x170002D8 RID: 728
		// (get) Token: 0x06000D9C RID: 3484 RVA: 0x0002C9B1 File Offset: 0x0002ABB1
		// (set) Token: 0x06000D9D RID: 3485 RVA: 0x0002C9B9 File Offset: 0x0002ABB9
		public string Description { get; set; } = string.Empty;

		// Token: 0x04000CF9 RID: 3321
		public EntityUid Id;
	}
}
