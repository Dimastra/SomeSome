using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Components
{
	// Token: 0x020006E9 RID: 1769
	[NetSerializable]
	[Serializable]
	public sealed class GasTankBoundUserInterfaceState : BoundUserInterfaceState
	{
		// Token: 0x17000485 RID: 1157
		// (get) Token: 0x0600156D RID: 5485 RVA: 0x000460EF File Offset: 0x000442EF
		// (set) Token: 0x0600156E RID: 5486 RVA: 0x000460F7 File Offset: 0x000442F7
		public float TankPressure { get; set; }

		// Token: 0x17000486 RID: 1158
		// (get) Token: 0x0600156F RID: 5487 RVA: 0x00046100 File Offset: 0x00044300
		// (set) Token: 0x06001570 RID: 5488 RVA: 0x00046108 File Offset: 0x00044308
		public float? OutputPressure { get; set; }

		// Token: 0x17000487 RID: 1159
		// (get) Token: 0x06001571 RID: 5489 RVA: 0x00046111 File Offset: 0x00044311
		// (set) Token: 0x06001572 RID: 5490 RVA: 0x00046119 File Offset: 0x00044319
		public bool InternalsConnected { get; set; }

		// Token: 0x17000488 RID: 1160
		// (get) Token: 0x06001573 RID: 5491 RVA: 0x00046122 File Offset: 0x00044322
		// (set) Token: 0x06001574 RID: 5492 RVA: 0x0004612A File Offset: 0x0004432A
		public bool CanConnectInternals { get; set; }
	}
}
