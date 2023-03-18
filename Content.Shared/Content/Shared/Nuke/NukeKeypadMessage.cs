using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Nuke
{
	// Token: 0x020002B9 RID: 697
	[NetSerializable]
	[Serializable]
	public sealed class NukeKeypadMessage : BoundUserInterfaceMessage
	{
		// Token: 0x060007C2 RID: 1986 RVA: 0x00019FC0 File Offset: 0x000181C0
		public NukeKeypadMessage(int value)
		{
			this.Value = value;
		}

		// Token: 0x040007DB RID: 2011
		public int Value;
	}
}
