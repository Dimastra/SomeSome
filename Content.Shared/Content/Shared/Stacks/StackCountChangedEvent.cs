using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Stacks
{
	// Token: 0x0200016B RID: 363
	public sealed class StackCountChangedEvent : EntityEventArgs
	{
		// Token: 0x06000466 RID: 1126 RVA: 0x00011C96 File Offset: 0x0000FE96
		public StackCountChangedEvent(int oldCount, int newCount)
		{
			this.OldCount = oldCount;
			this.NewCount = newCount;
		}

		// Token: 0x04000426 RID: 1062
		public int OldCount;

		// Token: 0x04000427 RID: 1063
		public int NewCount;
	}
}
