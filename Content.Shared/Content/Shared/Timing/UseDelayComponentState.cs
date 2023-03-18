using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Timing
{
	// Token: 0x020000C9 RID: 201
	[NetSerializable]
	[Serializable]
	public sealed class UseDelayComponentState : ComponentState
	{
		// Token: 0x06000223 RID: 547 RVA: 0x0000AB62 File Offset: 0x00008D62
		public UseDelayComponentState(TimeSpan lastUseTime, TimeSpan delay, TimeSpan? delayEndTime)
		{
			this.LastUseTime = lastUseTime;
			this.Delay = delay;
			this.DelayEndTime = delayEndTime;
		}

		// Token: 0x040002AA RID: 682
		public readonly TimeSpan LastUseTime;

		// Token: 0x040002AB RID: 683
		public readonly TimeSpan Delay;

		// Token: 0x040002AC RID: 684
		public readonly TimeSpan? DelayEndTime;
	}
}
