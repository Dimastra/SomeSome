using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Timing
{
	// Token: 0x020000C8 RID: 200
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class UseDelayComponent : Component
	{
		// Token: 0x17000068 RID: 104
		// (get) Token: 0x06000221 RID: 545 RVA: 0x0000AB1C File Offset: 0x00008D1C
		public bool ActiveDelay
		{
			get
			{
				CancellationTokenSource cancellationTokenSource = this.CancellationTokenSource;
				return cancellationTokenSource != null && !cancellationTokenSource.Token.IsCancellationRequested;
			}
		}

		// Token: 0x040002A5 RID: 677
		public TimeSpan LastUseTime;

		// Token: 0x040002A6 RID: 678
		public TimeSpan? DelayEndTime;

		// Token: 0x040002A7 RID: 679
		[DataField("delay", false, 1, false, false, null)]
		[ViewVariables]
		public TimeSpan Delay = TimeSpan.FromSeconds(1.0);

		// Token: 0x040002A8 RID: 680
		[DataField("remainingDelay", false, 1, false, false, null)]
		public TimeSpan? RemainingDelay;

		// Token: 0x040002A9 RID: 681
		[Nullable(2)]
		public CancellationTokenSource CancellationTokenSource;
	}
}
