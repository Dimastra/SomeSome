using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Piping.Trinary.Components
{
	// Token: 0x020006B3 RID: 1715
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class GasFilterBoundUserInterfaceState : BoundUserInterfaceState
	{
		// Token: 0x17000439 RID: 1081
		// (get) Token: 0x060014E0 RID: 5344 RVA: 0x00045189 File Offset: 0x00043389
		public string FilterLabel { get; }

		// Token: 0x1700043A RID: 1082
		// (get) Token: 0x060014E1 RID: 5345 RVA: 0x00045191 File Offset: 0x00043391
		public float TransferRate { get; }

		// Token: 0x1700043B RID: 1083
		// (get) Token: 0x060014E2 RID: 5346 RVA: 0x00045199 File Offset: 0x00043399
		public bool Enabled { get; }

		// Token: 0x1700043C RID: 1084
		// (get) Token: 0x060014E3 RID: 5347 RVA: 0x000451A1 File Offset: 0x000433A1
		public Gas? FilteredGas { get; }

		// Token: 0x060014E4 RID: 5348 RVA: 0x000451A9 File Offset: 0x000433A9
		public GasFilterBoundUserInterfaceState(string filterLabel, float transferRate, bool enabled, Gas? filteredGas)
		{
			this.FilterLabel = filterLabel;
			this.TransferRate = transferRate;
			this.Enabled = enabled;
			this.FilteredGas = filteredGas;
		}
	}
}
