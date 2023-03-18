using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Chemistry
{
	// Token: 0x020005CB RID: 1483
	[NetSerializable]
	[Serializable]
	public sealed class ChemMasterSetModeMessage : BoundUserInterfaceMessage
	{
		// Token: 0x060011FE RID: 4606 RVA: 0x0003B2F4 File Offset: 0x000394F4
		public ChemMasterSetModeMessage(ChemMasterMode mode)
		{
			this.ChemMasterMode = mode;
		}

		// Token: 0x040010C3 RID: 4291
		public readonly ChemMasterMode ChemMasterMode;
	}
}
