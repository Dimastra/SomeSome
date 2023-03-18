using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Kitchen
{
	// Token: 0x0200038E RID: 910
	[NetSerializable]
	[Serializable]
	public sealed class ReagentGrinderWorkStartedMessage : BoundUserInterfaceMessage
	{
		// Token: 0x06000A8A RID: 2698 RVA: 0x0002294B File Offset: 0x00020B4B
		public ReagentGrinderWorkStartedMessage(GrinderProgram grinderProgram)
		{
			this.GrinderProgram = grinderProgram;
		}

		// Token: 0x04000A70 RID: 2672
		public GrinderProgram GrinderProgram;
	}
}
