using System;
using System.Runtime.CompilerServices;
using Content.Shared.Inventory;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Clothing.Components
{
	// Token: 0x020005B2 RID: 1458
	[NullableContext(2)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class ChameleonBoundUserInterfaceState : BoundUserInterfaceState
	{
		// Token: 0x060011D3 RID: 4563 RVA: 0x0003A873 File Offset: 0x00038A73
		public ChameleonBoundUserInterfaceState(SlotFlags slot, string selectedId)
		{
			this.Slot = slot;
			this.SelectedId = selectedId;
		}

		// Token: 0x04001069 RID: 4201
		public readonly SlotFlags Slot;

		// Token: 0x0400106A RID: 4202
		public readonly string SelectedId;
	}
}
