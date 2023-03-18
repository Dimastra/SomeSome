using System;
using Robust.Shared.Serialization;

namespace Content.Shared.DoAfter
{
	// Token: 0x020004F7 RID: 1271
	[NetSerializable]
	[Serializable]
	public enum DoAfterStatus : byte
	{
		// Token: 0x04000E9F RID: 3743
		Running,
		// Token: 0x04000EA0 RID: 3744
		Cancelled,
		// Token: 0x04000EA1 RID: 3745
		Finished
	}
}
