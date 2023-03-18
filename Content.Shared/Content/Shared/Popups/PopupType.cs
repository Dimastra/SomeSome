using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Popups
{
	// Token: 0x02000262 RID: 610
	[NetSerializable]
	[Serializable]
	public enum PopupType : byte
	{
		// Token: 0x040006D7 RID: 1751
		Small,
		// Token: 0x040006D8 RID: 1752
		SmallCaution,
		// Token: 0x040006D9 RID: 1753
		Medium,
		// Token: 0x040006DA RID: 1754
		MediumCaution,
		// Token: 0x040006DB RID: 1755
		Large,
		// Token: 0x040006DC RID: 1756
		LargeCaution
	}
}
