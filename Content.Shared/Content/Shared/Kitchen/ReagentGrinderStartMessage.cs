using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Kitchen
{
	// Token: 0x0200038B RID: 907
	[NetSerializable]
	[Serializable]
	public sealed class ReagentGrinderStartMessage : BoundUserInterfaceMessage
	{
		// Token: 0x06000A87 RID: 2695 RVA: 0x00022925 File Offset: 0x00020B25
		public ReagentGrinderStartMessage(GrinderProgram program)
		{
			this.Program = program;
		}

		// Token: 0x04000A6E RID: 2670
		public readonly GrinderProgram Program;
	}
}
