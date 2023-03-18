using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Stacks
{
	// Token: 0x0200016D RID: 365
	[NetSerializable]
	[Serializable]
	public sealed class StackComponentState : ComponentState
	{
		// Token: 0x170000D0 RID: 208
		// (get) Token: 0x06000474 RID: 1140 RVA: 0x00011D22 File Offset: 0x0000FF22
		public int Count { get; }

		// Token: 0x170000D1 RID: 209
		// (get) Token: 0x06000475 RID: 1141 RVA: 0x00011D2A File Offset: 0x0000FF2A
		public int MaxCount { get; }

		// Token: 0x06000476 RID: 1142 RVA: 0x00011D32 File Offset: 0x0000FF32
		public StackComponentState(int count, int maxCount)
		{
			this.Count = count;
			this.MaxCount = maxCount;
		}
	}
}
