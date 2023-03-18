using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Piping.Binary.Components
{
	// Token: 0x020006C9 RID: 1737
	[NetSerializable]
	[Serializable]
	public sealed class GasVolumePumpChangeTransferRateMessage : BoundUserInterfaceMessage
	{
		// Token: 0x1700045B RID: 1115
		// (get) Token: 0x06001513 RID: 5395 RVA: 0x00045425 File Offset: 0x00043625
		public float TransferRate { get; }

		// Token: 0x06001514 RID: 5396 RVA: 0x0004542D File Offset: 0x0004362D
		public GasVolumePumpChangeTransferRateMessage(float transferRate)
		{
			this.TransferRate = transferRate;
		}
	}
}
