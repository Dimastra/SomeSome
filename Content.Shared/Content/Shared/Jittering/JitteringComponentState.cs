using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Jittering
{
	// Token: 0x0200039E RID: 926
	[NetSerializable]
	[Serializable]
	public sealed class JitteringComponentState : ComponentState
	{
		// Token: 0x1700020B RID: 523
		// (get) Token: 0x06000A9B RID: 2715 RVA: 0x00022A7B File Offset: 0x00020C7B
		public float Amplitude { get; }

		// Token: 0x1700020C RID: 524
		// (get) Token: 0x06000A9C RID: 2716 RVA: 0x00022A83 File Offset: 0x00020C83
		public float Frequency { get; }

		// Token: 0x06000A9D RID: 2717 RVA: 0x00022A8B File Offset: 0x00020C8B
		public JitteringComponentState(float amplitude, float frequency)
		{
			this.Amplitude = amplitude;
			this.Frequency = frequency;
		}
	}
}
