using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Singularity.Components
{
	// Token: 0x020001AE RID: 430
	[NetSerializable]
	[Serializable]
	public enum ParticleAcceleratorWireStatus
	{
		// Token: 0x040004E4 RID: 1252
		Power,
		// Token: 0x040004E5 RID: 1253
		Keyboard,
		// Token: 0x040004E6 RID: 1254
		Limiter,
		// Token: 0x040004E7 RID: 1255
		Strength
	}
}
