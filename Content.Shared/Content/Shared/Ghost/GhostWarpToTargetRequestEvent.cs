using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Ghost
{
	// Token: 0x02000455 RID: 1109
	[NetSerializable]
	[Serializable]
	public sealed class GhostWarpToTargetRequestEvent : EntityEventArgs
	{
		// Token: 0x170002CD RID: 717
		// (get) Token: 0x06000D82 RID: 3458 RVA: 0x0002C8B2 File Offset: 0x0002AAB2
		public EntityUid Target { get; }

		// Token: 0x06000D83 RID: 3459 RVA: 0x0002C8BA File Offset: 0x0002AABA
		public GhostWarpToTargetRequestEvent(EntityUid target)
		{
			this.Target = target;
		}
	}
}
