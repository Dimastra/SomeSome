using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Chemistry
{
	// Token: 0x020005DB RID: 1499
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class ReagentDispenserBoundUserInterfaceState : BoundUserInterfaceState
	{
		// Token: 0x0600120A RID: 4618 RVA: 0x0003B427 File Offset: 0x00039627
		public ReagentDispenserBoundUserInterfaceState([Nullable(2)] ContainerInfo outputContainer, List<string> inventory, ReagentDispenserDispenseAmount selectedDispenseAmount)
		{
			this.OutputContainer = outputContainer;
			this.Inventory = inventory;
			this.SelectedDispenseAmount = selectedDispenseAmount;
		}

		// Token: 0x040010F2 RID: 4338
		[Nullable(2)]
		public readonly ContainerInfo OutputContainer;

		// Token: 0x040010F3 RID: 4339
		public readonly List<string> Inventory;

		// Token: 0x040010F4 RID: 4340
		public readonly ReagentDispenserDispenseAmount SelectedDispenseAmount;
	}
}
