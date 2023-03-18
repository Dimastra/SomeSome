using System;
using Robust.Shared.Serialization;

namespace Content.Shared.TextScreen
{
	// Token: 0x020000DA RID: 218
	[NetSerializable]
	[Serializable]
	public enum TextScreenVisuals : byte
	{
		// Token: 0x040002CC RID: 716
		On,
		// Token: 0x040002CD RID: 717
		Mode,
		// Token: 0x040002CE RID: 718
		ScreenText,
		// Token: 0x040002CF RID: 719
		TargetTime
	}
}
