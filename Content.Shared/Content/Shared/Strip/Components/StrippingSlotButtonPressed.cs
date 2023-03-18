using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Strip.Components
{
	// Token: 0x02000116 RID: 278
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class StrippingSlotButtonPressed : BoundUserInterfaceMessage
	{
		// Token: 0x06000335 RID: 821 RVA: 0x0000E0D1 File Offset: 0x0000C2D1
		public StrippingSlotButtonPressed(string slot, bool isHand)
		{
			this.Slot = slot;
			this.IsHand = isHand;
		}

		// Token: 0x0400035D RID: 861
		public readonly string Slot;

		// Token: 0x0400035E RID: 862
		public readonly bool IsHand;
	}
}
