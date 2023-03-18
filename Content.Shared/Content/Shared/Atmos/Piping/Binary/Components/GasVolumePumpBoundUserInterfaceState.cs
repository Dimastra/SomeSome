using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Piping.Binary.Components
{
	// Token: 0x020006C7 RID: 1735
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class GasVolumePumpBoundUserInterfaceState : BoundUserInterfaceState
	{
		// Token: 0x17000457 RID: 1111
		// (get) Token: 0x0600150D RID: 5389 RVA: 0x000453D9 File Offset: 0x000435D9
		public string PumpLabel { get; }

		// Token: 0x17000458 RID: 1112
		// (get) Token: 0x0600150E RID: 5390 RVA: 0x000453E1 File Offset: 0x000435E1
		public float TransferRate { get; }

		// Token: 0x17000459 RID: 1113
		// (get) Token: 0x0600150F RID: 5391 RVA: 0x000453E9 File Offset: 0x000435E9
		public bool Enabled { get; }

		// Token: 0x06001510 RID: 5392 RVA: 0x000453F1 File Offset: 0x000435F1
		public GasVolumePumpBoundUserInterfaceState(string pumpLabel, float transferRate, bool enabled)
		{
			this.PumpLabel = pumpLabel;
			this.TransferRate = transferRate;
			this.Enabled = enabled;
		}
	}
}
