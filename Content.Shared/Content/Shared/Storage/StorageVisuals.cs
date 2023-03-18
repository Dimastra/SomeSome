using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Storage
{
	// Token: 0x0200012E RID: 302
	[NetSerializable]
	[Serializable]
	public enum StorageVisuals : byte
	{
		// Token: 0x04000394 RID: 916
		Open,
		// Token: 0x04000395 RID: 917
		HasContents,
		// Token: 0x04000396 RID: 918
		CanLock,
		// Token: 0x04000397 RID: 919
		Locked
	}
}
