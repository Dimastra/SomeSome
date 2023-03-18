using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Humanoid.Markings
{
	// Token: 0x0200041B RID: 1051
	[NetSerializable]
	[Serializable]
	public enum MarkingCategories : byte
	{
		// Token: 0x04000C6E RID: 3182
		Hair,
		// Token: 0x04000C6F RID: 3183
		FacialHair,
		// Token: 0x04000C70 RID: 3184
		Head,
		// Token: 0x04000C71 RID: 3185
		HeadTop,
		// Token: 0x04000C72 RID: 3186
		HeadSide,
		// Token: 0x04000C73 RID: 3187
		Snout,
		// Token: 0x04000C74 RID: 3188
		Chest,
		// Token: 0x04000C75 RID: 3189
		Arms,
		// Token: 0x04000C76 RID: 3190
		Legs,
		// Token: 0x04000C77 RID: 3191
		Tail,
		// Token: 0x04000C78 RID: 3192
		Overlay
	}
}
