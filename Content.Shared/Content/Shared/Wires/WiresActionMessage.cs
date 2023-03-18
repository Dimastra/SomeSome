using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Wires
{
	// Token: 0x02000023 RID: 35
	[NetSerializable]
	[Serializable]
	public sealed class WiresActionMessage : BoundUserInterfaceMessage
	{
		// Token: 0x06000027 RID: 39 RVA: 0x00002464 File Offset: 0x00000664
		public WiresActionMessage(int id, WiresAction action)
		{
			this.Id = id;
			this.Action = action;
		}

		// Token: 0x04000048 RID: 72
		public readonly int Id;

		// Token: 0x04000049 RID: 73
		public readonly WiresAction Action;
	}
}
