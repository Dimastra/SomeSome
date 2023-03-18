using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Visuals
{
	// Token: 0x0200069B RID: 1691
	[NetSerializable]
	[Serializable]
	public enum VentPumpState : byte
	{
		// Token: 0x040014AA RID: 5290
		Off,
		// Token: 0x040014AB RID: 5291
		In,
		// Token: 0x040014AC RID: 5292
		Out,
		// Token: 0x040014AD RID: 5293
		Welded
	}
}
