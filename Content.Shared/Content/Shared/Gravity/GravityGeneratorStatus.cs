using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Gravity
{
	// Token: 0x0200044B RID: 1099
	[NetSerializable]
	[Serializable]
	public enum GravityGeneratorStatus
	{
		// Token: 0x04000CD7 RID: 3287
		Broken,
		// Token: 0x04000CD8 RID: 3288
		Unpowered,
		// Token: 0x04000CD9 RID: 3289
		Off,
		// Token: 0x04000CDA RID: 3290
		On
	}
}
