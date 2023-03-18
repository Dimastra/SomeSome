using System;

namespace Content.Shared.Weapons.Ranged.Components
{
	// Token: 0x02000064 RID: 100
	[Flags]
	public enum SelectiveFire : byte
	{
		// Token: 0x0400013C RID: 316
		Invalid = 0,
		// Token: 0x0400013D RID: 317
		SemiAuto = 1,
		// Token: 0x0400013E RID: 318
		Burst = 2,
		// Token: 0x0400013F RID: 319
		FullAuto = 4
	}
}
