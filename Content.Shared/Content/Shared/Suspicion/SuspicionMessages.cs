using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Suspicion
{
	// Token: 0x020000F6 RID: 246
	public static class SuspicionMessages
	{
		// Token: 0x02000797 RID: 1943
		[NetSerializable]
		[Serializable]
		public sealed class SetSuspicionEndTimerMessage : EntityEventArgs
		{
			// Token: 0x040017A8 RID: 6056
			public TimeSpan? EndTime;
		}
	}
}
