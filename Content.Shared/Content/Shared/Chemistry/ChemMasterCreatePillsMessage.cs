using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Chemistry
{
	// Token: 0x020005CE RID: 1486
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class ChemMasterCreatePillsMessage : BoundUserInterfaceMessage
	{
		// Token: 0x06001201 RID: 4609 RVA: 0x0003B32F File Offset: 0x0003952F
		public ChemMasterCreatePillsMessage(uint dosage, uint number, string label)
		{
			this.Dosage = dosage;
			this.Number = number;
			this.Label = label;
		}

		// Token: 0x040010C8 RID: 4296
		public readonly uint Dosage;

		// Token: 0x040010C9 RID: 4297
		public readonly uint Number;

		// Token: 0x040010CA RID: 4298
		public readonly string Label;
	}
}
