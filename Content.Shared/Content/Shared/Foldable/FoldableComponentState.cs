using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Foldable
{
	// Token: 0x0200047E RID: 1150
	[NetSerializable]
	[Serializable]
	public sealed class FoldableComponentState : ComponentState
	{
		// Token: 0x06000DE1 RID: 3553 RVA: 0x0002D45B File Offset: 0x0002B65B
		public FoldableComponentState(bool isFolded)
		{
			this.IsFolded = isFolded;
		}

		// Token: 0x04000D30 RID: 3376
		public readonly bool IsFolded;
	}
}
