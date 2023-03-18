using System;
using Robust.Shared.Serialization;

namespace Content.Shared.GameTicking
{
	// Token: 0x0200046D RID: 1133
	[NetSerializable]
	[Serializable]
	public enum PlayerGameStatus : sbyte
	{
		// Token: 0x04000D13 RID: 3347
		NotReadyToPlay,
		// Token: 0x04000D14 RID: 3348
		ReadyToPlay,
		// Token: 0x04000D15 RID: 3349
		JoinedGame
	}
}
