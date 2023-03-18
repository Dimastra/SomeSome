using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Item
{
	// Token: 0x020003A1 RID: 929
	[NullableContext(2)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class ItemComponentState : ComponentState
	{
		// Token: 0x1700020D RID: 525
		// (get) Token: 0x06000AA6 RID: 2726 RVA: 0x00022C49 File Offset: 0x00020E49
		public int Size { get; }

		// Token: 0x1700020E RID: 526
		// (get) Token: 0x06000AA7 RID: 2727 RVA: 0x00022C51 File Offset: 0x00020E51
		public string HeldPrefix { get; }

		// Token: 0x06000AA8 RID: 2728 RVA: 0x00022C59 File Offset: 0x00020E59
		public ItemComponentState(int size, string heldPrefix)
		{
			this.Size = size;
			this.HeldPrefix = heldPrefix;
		}
	}
}
