using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Ghost
{
	// Token: 0x02000454 RID: 1108
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class GhostWarpsResponseEvent : EntityEventArgs
	{
		// Token: 0x06000D80 RID: 3456 RVA: 0x0002C89B File Offset: 0x0002AA9B
		public GhostWarpsResponseEvent(List<GhostWarp> warps)
		{
			this.Warps = warps;
		}

		// Token: 0x170002CC RID: 716
		// (get) Token: 0x06000D81 RID: 3457 RVA: 0x0002C8AA File Offset: 0x0002AAAA
		public List<GhostWarp> Warps { get; }
	}
}
