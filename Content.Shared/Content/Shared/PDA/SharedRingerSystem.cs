using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.PDA
{
	// Token: 0x0200028D RID: 653
	public abstract class SharedRingerSystem : EntitySystem
	{
		// Token: 0x0400076E RID: 1902
		public const int RingtoneLength = 4;

		// Token: 0x0400076F RID: 1903
		public const int NoteTempo = 300;

		// Token: 0x04000770 RID: 1904
		public const float NoteDelay = 0.2f;
	}
}
