using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Chemistry
{
	// Token: 0x020005CC RID: 1484
	[NetSerializable]
	[Serializable]
	public sealed class ChemMasterSetPillTypeMessage : BoundUserInterfaceMessage
	{
		// Token: 0x060011FF RID: 4607 RVA: 0x0003B303 File Offset: 0x00039503
		public ChemMasterSetPillTypeMessage(uint pillType)
		{
			this.PillType = pillType;
		}

		// Token: 0x040010C4 RID: 4292
		public readonly uint PillType;
	}
}
