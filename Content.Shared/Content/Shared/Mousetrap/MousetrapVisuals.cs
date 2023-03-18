using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Mousetrap
{
	// Token: 0x020002F6 RID: 758
	[NetSerializable]
	[Serializable]
	public enum MousetrapVisuals : byte
	{
		// Token: 0x040008A3 RID: 2211
		Visual,
		// Token: 0x040008A4 RID: 2212
		Armed,
		// Token: 0x040008A5 RID: 2213
		Unarmed
	}
}
