using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Chemistry
{
	// Token: 0x020005D8 RID: 1496
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class ReagentDispenserDispenseReagentMessage : BoundUserInterfaceMessage
	{
		// Token: 0x06001208 RID: 4616 RVA: 0x0003B410 File Offset: 0x00039610
		public ReagentDispenserDispenseReagentMessage(string reagentId)
		{
			this.ReagentId = reagentId;
		}

		// Token: 0x040010E7 RID: 4327
		public readonly string ReagentId;
	}
}
