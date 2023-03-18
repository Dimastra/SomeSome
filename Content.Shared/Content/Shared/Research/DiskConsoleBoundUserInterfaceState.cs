using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Research
{
	// Token: 0x020001FF RID: 511
	[NetSerializable]
	[Serializable]
	public sealed class DiskConsoleBoundUserInterfaceState : BoundUserInterfaceState
	{
		// Token: 0x060005A0 RID: 1440 RVA: 0x00014734 File Offset: 0x00012934
		public DiskConsoleBoundUserInterfaceState(int serverPoints, int pointCost, bool canPrint)
		{
			this.CanPrint = canPrint;
			this.PointCost = pointCost;
			this.ServerPoints = serverPoints;
		}

		// Token: 0x040005CB RID: 1483
		public bool CanPrint;

		// Token: 0x040005CC RID: 1484
		public int PointCost;

		// Token: 0x040005CD RID: 1485
		public int ServerPoints;
	}
}
