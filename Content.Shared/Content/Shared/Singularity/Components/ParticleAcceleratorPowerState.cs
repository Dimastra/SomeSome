using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Singularity.Components
{
	// Token: 0x020001AC RID: 428
	[NetSerializable]
	[Serializable]
	public enum ParticleAcceleratorPowerState
	{
		// Token: 0x040004DB RID: 1243
		Standby = 1,
		// Token: 0x040004DC RID: 1244
		Level0,
		// Token: 0x040004DD RID: 1245
		Level1,
		// Token: 0x040004DE RID: 1246
		Level2,
		// Token: 0x040004DF RID: 1247
		Level3
	}
}
