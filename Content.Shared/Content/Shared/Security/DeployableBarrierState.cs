using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Security
{
	// Token: 0x020001D9 RID: 473
	[NetSerializable]
	[Serializable]
	public enum DeployableBarrierState : byte
	{
		// Token: 0x04000556 RID: 1366
		Idle,
		// Token: 0x04000557 RID: 1367
		Deployed
	}
}
