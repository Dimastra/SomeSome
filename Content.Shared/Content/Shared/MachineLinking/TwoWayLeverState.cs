using System;
using Robust.Shared.Serialization;

namespace Content.Shared.MachineLinking
{
	// Token: 0x02000350 RID: 848
	[NetSerializable]
	[Serializable]
	public enum TwoWayLeverState : byte
	{
		// Token: 0x040009AA RID: 2474
		Middle,
		// Token: 0x040009AB RID: 2475
		Right,
		// Token: 0x040009AC RID: 2476
		Left
	}
}
