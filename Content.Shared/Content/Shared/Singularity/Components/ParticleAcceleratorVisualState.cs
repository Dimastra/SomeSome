using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Singularity.Components
{
	// Token: 0x020001AB RID: 427
	[NetSerializable]
	[Serializable]
	public enum ParticleAcceleratorVisualState
	{
		// Token: 0x040004D4 RID: 1236
		Unpowered,
		// Token: 0x040004D5 RID: 1237
		Powered,
		// Token: 0x040004D6 RID: 1238
		Level0,
		// Token: 0x040004D7 RID: 1239
		Level1,
		// Token: 0x040004D8 RID: 1240
		Level2,
		// Token: 0x040004D9 RID: 1241
		Level3
	}
}
