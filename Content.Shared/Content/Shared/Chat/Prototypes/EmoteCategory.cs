using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Chat.Prototypes
{
	// Token: 0x0200060D RID: 1549
	[Flags]
	[NetSerializable]
	[Serializable]
	public enum EmoteCategory : byte
	{
		// Token: 0x040011B4 RID: 4532
		Invalid = 0,
		// Token: 0x040011B5 RID: 4533
		Vocal = 1,
		// Token: 0x040011B6 RID: 4534
		Gesture = 2,
		// Token: 0x040011B7 RID: 4535
		General = 255
	}
}
