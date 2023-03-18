using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Piping.Unary.Components
{
	// Token: 0x020006AF RID: 1711
	[Flags]
	[NetSerializable]
	[Serializable]
	public enum VentPressureBound : sbyte
	{
		// Token: 0x040014E6 RID: 5350
		NoBound = 0,
		// Token: 0x040014E7 RID: 5351
		InternalBound = 1,
		// Token: 0x040014E8 RID: 5352
		ExternalBound = 2,
		// Token: 0x040014E9 RID: 5353
		Both = 3
	}
}
