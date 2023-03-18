using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Stacks
{
	// Token: 0x0200016F RID: 367
	[NetSerializable]
	[Serializable]
	public enum StackVisuals : byte
	{
		// Token: 0x04000436 RID: 1078
		Actual,
		// Token: 0x04000437 RID: 1079
		MaxCount,
		// Token: 0x04000438 RID: 1080
		Hide
	}
}
