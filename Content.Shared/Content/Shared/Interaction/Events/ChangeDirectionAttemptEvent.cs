using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Interaction.Events
{
	// Token: 0x020003D3 RID: 979
	public sealed class ChangeDirectionAttemptEvent : CancellableEntityEventArgs
	{
		// Token: 0x06000B7A RID: 2938 RVA: 0x000261C8 File Offset: 0x000243C8
		public ChangeDirectionAttemptEvent(EntityUid uid)
		{
			this.Uid = uid;
		}

		// Token: 0x1700023E RID: 574
		// (get) Token: 0x06000B7B RID: 2939 RVA: 0x000261D7 File Offset: 0x000243D7
		public EntityUid Uid { get; }
	}
}
