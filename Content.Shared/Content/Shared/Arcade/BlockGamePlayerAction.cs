using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Arcade
{
	// Token: 0x020006ED RID: 1773
	[NetSerializable]
	[Serializable]
	public enum BlockGamePlayerAction
	{
		// Token: 0x0400158F RID: 5519
		NewGame,
		// Token: 0x04001590 RID: 5520
		StartLeft,
		// Token: 0x04001591 RID: 5521
		EndLeft,
		// Token: 0x04001592 RID: 5522
		StartRight,
		// Token: 0x04001593 RID: 5523
		EndRight,
		// Token: 0x04001594 RID: 5524
		Rotate,
		// Token: 0x04001595 RID: 5525
		CounterRotate,
		// Token: 0x04001596 RID: 5526
		SoftdropStart,
		// Token: 0x04001597 RID: 5527
		SoftdropEnd,
		// Token: 0x04001598 RID: 5528
		Harddrop,
		// Token: 0x04001599 RID: 5529
		Pause,
		// Token: 0x0400159A RID: 5530
		Unpause,
		// Token: 0x0400159B RID: 5531
		Hold,
		// Token: 0x0400159C RID: 5532
		ShowHighscores
	}
}
