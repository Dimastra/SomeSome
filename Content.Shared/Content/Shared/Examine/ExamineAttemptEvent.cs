using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Examine
{
	// Token: 0x020004AD RID: 1197
	public sealed class ExamineAttemptEvent : CancellableEntityEventArgs
	{
		// Token: 0x06000E85 RID: 3717 RVA: 0x0002EE89 File Offset: 0x0002D089
		public ExamineAttemptEvent(EntityUid examiner)
		{
			this.Examiner = examiner;
		}

		// Token: 0x04000DAC RID: 3500
		public readonly EntityUid Examiner;
	}
}
