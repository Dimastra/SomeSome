using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Dragon
{
	// Token: 0x020004D8 RID: 1240
	[NetSerializable]
	[Serializable]
	public enum DragonRiftState : byte
	{
		// Token: 0x04000E16 RID: 3606
		Charging,
		// Token: 0x04000E17 RID: 3607
		AlmostFinished,
		// Token: 0x04000E18 RID: 3608
		Finished
	}
}
