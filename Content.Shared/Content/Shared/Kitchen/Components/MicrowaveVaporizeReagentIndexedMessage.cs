using System;
using Content.Shared.Chemistry.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Kitchen.Components
{
	// Token: 0x02000398 RID: 920
	[NetSerializable]
	[Serializable]
	public sealed class MicrowaveVaporizeReagentIndexedMessage : BoundUserInterfaceMessage
	{
		// Token: 0x06000A91 RID: 2705 RVA: 0x000229F6 File Offset: 0x00020BF6
		public MicrowaveVaporizeReagentIndexedMessage(Solution.ReagentQuantity reagentQuantity)
		{
			this.ReagentQuantity = reagentQuantity;
		}

		// Token: 0x04000A82 RID: 2690
		public Solution.ReagentQuantity ReagentQuantity;
	}
}
