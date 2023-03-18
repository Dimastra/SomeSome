using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Radiation.Events
{
	// Token: 0x02000229 RID: 553
	public sealed class OnIrradiatedEvent : EntityEventArgs
	{
		// Token: 0x1700012E RID: 302
		// (get) Token: 0x0600062A RID: 1578 RVA: 0x00015C6A File Offset: 0x00013E6A
		public float TotalRads
		{
			get
			{
				return this.RadsPerSecond * this.FrameTime;
			}
		}

		// Token: 0x0600062B RID: 1579 RVA: 0x00015C79 File Offset: 0x00013E79
		public OnIrradiatedEvent(float frameTime, float radsPerSecond)
		{
			this.FrameTime = frameTime;
			this.RadsPerSecond = radsPerSecond;
		}

		// Token: 0x04000629 RID: 1577
		public readonly float FrameTime;

		// Token: 0x0400062A RID: 1578
		public readonly float RadsPerSecond;
	}
}
