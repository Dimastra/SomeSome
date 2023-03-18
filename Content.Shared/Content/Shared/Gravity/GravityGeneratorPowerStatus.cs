using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Gravity
{
	// Token: 0x0200044C RID: 1100
	[NetSerializable]
	[Serializable]
	public enum GravityGeneratorPowerStatus : byte
	{
		// Token: 0x04000CDC RID: 3292
		Off,
		// Token: 0x04000CDD RID: 3293
		Discharging,
		// Token: 0x04000CDE RID: 3294
		Charging,
		// Token: 0x04000CDF RID: 3295
		FullyCharged
	}
}
