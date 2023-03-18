using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Anomaly.Components
{
	// Token: 0x0200070C RID: 1804
	[NetSerializable]
	[Serializable]
	public sealed class AnomalyComponentState : ComponentState
	{
		// Token: 0x060015A8 RID: 5544 RVA: 0x000474C3 File Offset: 0x000456C3
		public AnomalyComponentState(float severity, float stability, float health, TimeSpan nextPulseTime)
		{
			this.Severity = severity;
			this.Stability = stability;
			this.Health = health;
			this.NextPulseTime = nextPulseTime;
		}

		// Token: 0x0400161D RID: 5661
		public float Severity;

		// Token: 0x0400161E RID: 5662
		public float Stability;

		// Token: 0x0400161F RID: 5663
		public float Health;

		// Token: 0x04001620 RID: 5664
		public TimeSpan NextPulseTime;
	}
}
