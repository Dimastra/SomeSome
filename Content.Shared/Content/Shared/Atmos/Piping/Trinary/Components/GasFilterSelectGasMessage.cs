using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Piping.Trinary.Components
{
	// Token: 0x020006B6 RID: 1718
	[NetSerializable]
	[Serializable]
	public sealed class GasFilterSelectGasMessage : BoundUserInterfaceMessage
	{
		// Token: 0x1700043F RID: 1087
		// (get) Token: 0x060014E9 RID: 5353 RVA: 0x000451FC File Offset: 0x000433FC
		public int? ID { get; }

		// Token: 0x060014EA RID: 5354 RVA: 0x00045204 File Offset: 0x00043404
		public GasFilterSelectGasMessage(int? id)
		{
			this.ID = id;
		}
	}
}
