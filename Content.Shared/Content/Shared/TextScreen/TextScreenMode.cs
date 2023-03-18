using System;
using Robust.Shared.Serialization;

namespace Content.Shared.TextScreen
{
	// Token: 0x020000DB RID: 219
	[NetSerializable]
	[Serializable]
	public enum TextScreenMode : byte
	{
		// Token: 0x040002D1 RID: 721
		Text,
		// Token: 0x040002D2 RID: 722
		Timer
	}
}
