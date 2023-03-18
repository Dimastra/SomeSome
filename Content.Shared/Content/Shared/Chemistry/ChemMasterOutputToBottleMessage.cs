using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Chemistry
{
	// Token: 0x020005CF RID: 1487
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class ChemMasterOutputToBottleMessage : BoundUserInterfaceMessage
	{
		// Token: 0x06001202 RID: 4610 RVA: 0x0003B34C File Offset: 0x0003954C
		public ChemMasterOutputToBottleMessage(uint dosage, string label)
		{
			this.Dosage = dosage;
			this.Label = label;
		}

		// Token: 0x040010CB RID: 4299
		public readonly uint Dosage;

		// Token: 0x040010CC RID: 4300
		public readonly string Label;
	}
}
