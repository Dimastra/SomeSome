using System;
using Robust.Shared.GameObjects;

namespace Content.Server.Actions.Events
{
	// Token: 0x02000876 RID: 2166
	public sealed class DisarmAttemptEvent : CancellableEntityEventArgs
	{
		// Token: 0x06002F52 RID: 12114 RVA: 0x000F4D7E File Offset: 0x000F2F7E
		public DisarmAttemptEvent(EntityUid targetUid, EntityUid disarmerUid, EntityUid? targetItemInHandUid = null)
		{
			this.TargetUid = targetUid;
			this.DisarmerUid = disarmerUid;
			this.TargetItemInHandUid = targetItemInHandUid;
		}

		// Token: 0x04001C76 RID: 7286
		public readonly EntityUid TargetUid;

		// Token: 0x04001C77 RID: 7287
		public readonly EntityUid DisarmerUid;

		// Token: 0x04001C78 RID: 7288
		public readonly EntityUid? TargetItemInHandUid;
	}
}
