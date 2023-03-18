using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Eye.Blinding
{
	// Token: 0x0200049A RID: 1178
	[NetSerializable]
	[Serializable]
	public sealed class BlurryVisionComponentState : ComponentState
	{
		// Token: 0x06000E56 RID: 3670 RVA: 0x0002E110 File Offset: 0x0002C310
		public BlurryVisionComponentState(float magnitude)
		{
			this.Magnitude = magnitude;
		}

		// Token: 0x04000D6E RID: 3438
		public float Magnitude;
	}
}
