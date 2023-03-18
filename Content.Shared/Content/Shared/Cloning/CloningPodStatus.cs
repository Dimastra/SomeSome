using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Cloning
{
	// Token: 0x020005BC RID: 1468
	[NetSerializable]
	[Serializable]
	public enum CloningPodStatus : byte
	{
		// Token: 0x0400108C RID: 4236
		Idle,
		// Token: 0x0400108D RID: 4237
		Cloning,
		// Token: 0x0400108E RID: 4238
		Gore,
		// Token: 0x0400108F RID: 4239
		NoMind
	}
}
