using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Piping.Unary.Components
{
	// Token: 0x020006AE RID: 1710
	[NetSerializable]
	[Serializable]
	public enum VentPumpDirection : sbyte
	{
		// Token: 0x040014E3 RID: 5347
		Siphoning,
		// Token: 0x040014E4 RID: 5348
		Releasing
	}
}
