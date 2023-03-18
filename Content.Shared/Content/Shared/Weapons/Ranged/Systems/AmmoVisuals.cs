using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Weapons.Ranged.Systems
{
	// Token: 0x0200004C RID: 76
	[NetSerializable]
	[Serializable]
	public enum AmmoVisuals : byte
	{
		// Token: 0x040000F2 RID: 242
		Spent,
		// Token: 0x040000F3 RID: 243
		AmmoCount,
		// Token: 0x040000F4 RID: 244
		AmmoMax,
		// Token: 0x040000F5 RID: 245
		HasAmmo,
		// Token: 0x040000F6 RID: 246
		MagLoaded,
		// Token: 0x040000F7 RID: 247
		InStun
	}
}
