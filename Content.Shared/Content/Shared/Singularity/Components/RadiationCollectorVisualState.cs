using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Singularity.Components
{
	// Token: 0x020001B5 RID: 437
	[NetSerializable]
	[Serializable]
	public enum RadiationCollectorVisualState
	{
		// Token: 0x040004FD RID: 1277
		Active,
		// Token: 0x040004FE RID: 1278
		Activating,
		// Token: 0x040004FF RID: 1279
		Deactivating,
		// Token: 0x04000500 RID: 1280
		Deactive
	}
}
