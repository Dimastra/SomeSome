using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Flash
{
	// Token: 0x0200048A RID: 1162
	[NetSerializable]
	[Serializable]
	public sealed class FlashableComponentState : ComponentState
	{
		// Token: 0x170002F1 RID: 753
		// (get) Token: 0x06000DF6 RID: 3574 RVA: 0x0002D6F7 File Offset: 0x0002B8F7
		public float Duration { get; }

		// Token: 0x170002F2 RID: 754
		// (get) Token: 0x06000DF7 RID: 3575 RVA: 0x0002D6FF File Offset: 0x0002B8FF
		public TimeSpan Time { get; }

		// Token: 0x06000DF8 RID: 3576 RVA: 0x0002D707 File Offset: 0x0002B907
		public FlashableComponentState(float duration, TimeSpan time)
		{
			this.Duration = duration;
			this.Time = time;
		}
	}
}
