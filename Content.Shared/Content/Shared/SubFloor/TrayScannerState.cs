using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.SubFloor
{
	// Token: 0x02000109 RID: 265
	[NetSerializable]
	[Serializable]
	public sealed class TrayScannerState : ComponentState
	{
		// Token: 0x060002F2 RID: 754 RVA: 0x0000D240 File Offset: 0x0000B440
		public TrayScannerState(bool enabled)
		{
			this.Enabled = enabled;
		}

		// Token: 0x0400033C RID: 828
		public bool Enabled;
	}
}
