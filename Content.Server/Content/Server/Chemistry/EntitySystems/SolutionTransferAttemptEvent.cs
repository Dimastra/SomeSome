using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Server.Chemistry.EntitySystems
{
	// Token: 0x020006A1 RID: 1697
	[NullableContext(2)]
	[Nullable(0)]
	public sealed class SolutionTransferAttemptEvent : CancellableEntityEventArgs
	{
		// Token: 0x06002367 RID: 9063 RVA: 0x000B928A File Offset: 0x000B748A
		public SolutionTransferAttemptEvent(EntityUid from, EntityUid to)
		{
			this.From = from;
			this.To = to;
		}

		// Token: 0x1700053B RID: 1339
		// (get) Token: 0x06002368 RID: 9064 RVA: 0x000B92A0 File Offset: 0x000B74A0
		public EntityUid From { get; }

		// Token: 0x1700053C RID: 1340
		// (get) Token: 0x06002369 RID: 9065 RVA: 0x000B92A8 File Offset: 0x000B74A8
		public EntityUid To { get; }

		// Token: 0x1700053D RID: 1341
		// (get) Token: 0x0600236A RID: 9066 RVA: 0x000B92B0 File Offset: 0x000B74B0
		// (set) Token: 0x0600236B RID: 9067 RVA: 0x000B92B8 File Offset: 0x000B74B8
		public string CancelReason { get; private set; }

		// Token: 0x0600236C RID: 9068 RVA: 0x000B92C1 File Offset: 0x000B74C1
		[NullableContext(1)]
		public void Cancel(string reason)
		{
			base.Cancel();
			this.CancelReason = reason;
		}
	}
}
