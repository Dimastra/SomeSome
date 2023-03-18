using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Salvage
{
	// Token: 0x020001DC RID: 476
	[NetSerializable]
	[Serializable]
	public enum SalvageMagnetVisuals : byte
	{
		// Token: 0x0400055A RID: 1370
		ChargeState,
		// Token: 0x0400055B RID: 1371
		Ready,
		// Token: 0x0400055C RID: 1372
		ReadyBlinking,
		// Token: 0x0400055D RID: 1373
		Unready,
		// Token: 0x0400055E RID: 1374
		UnreadyBlinking
	}
}
