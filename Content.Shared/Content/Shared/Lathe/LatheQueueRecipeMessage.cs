using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Lathe
{
	// Token: 0x0200037D RID: 893
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class LatheQueueRecipeMessage : BoundUserInterfaceMessage
	{
		// Token: 0x06000A67 RID: 2663 RVA: 0x0002254A File Offset: 0x0002074A
		public LatheQueueRecipeMessage(string id, int quantity)
		{
			this.ID = id;
			this.Quantity = quantity;
		}

		// Token: 0x04000A50 RID: 2640
		public readonly string ID;

		// Token: 0x04000A51 RID: 2641
		public readonly int Quantity;
	}
}
