using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Doors
{
	// Token: 0x020004E0 RID: 1248
	[NetSerializable]
	[Serializable]
	public enum AirlockWireStatus
	{
		// Token: 0x04000E2B RID: 3627
		PowerIndicator,
		// Token: 0x04000E2C RID: 3628
		BoltIndicator,
		// Token: 0x04000E2D RID: 3629
		BoltLightIndicator,
		// Token: 0x04000E2E RID: 3630
		AIControlIndicator,
		// Token: 0x04000E2F RID: 3631
		TimingIndicator,
		// Token: 0x04000E30 RID: 3632
		SafetyIndicator
	}
}
