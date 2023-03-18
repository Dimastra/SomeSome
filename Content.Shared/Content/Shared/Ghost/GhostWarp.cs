using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Ghost
{
	// Token: 0x02000453 RID: 1107
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public struct GhostWarp
	{
		// Token: 0x06000D7C RID: 3452 RVA: 0x0002C86C File Offset: 0x0002AA6C
		public GhostWarp(EntityUid entity, string displayName, bool isWarpPoint)
		{
			this.Entity = entity;
			this.DisplayName = displayName;
			this.IsWarpPoint = isWarpPoint;
		}

		// Token: 0x170002C9 RID: 713
		// (get) Token: 0x06000D7D RID: 3453 RVA: 0x0002C883 File Offset: 0x0002AA83
		public readonly EntityUid Entity { get; }

		// Token: 0x170002CA RID: 714
		// (get) Token: 0x06000D7E RID: 3454 RVA: 0x0002C88B File Offset: 0x0002AA8B
		public readonly string DisplayName { get; }

		// Token: 0x170002CB RID: 715
		// (get) Token: 0x06000D7F RID: 3455 RVA: 0x0002C893 File Offset: 0x0002AA93
		public readonly bool IsWarpPoint { get; }
	}
}
