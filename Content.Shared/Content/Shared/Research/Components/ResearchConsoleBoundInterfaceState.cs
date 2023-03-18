using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Research.Components
{
	// Token: 0x02000213 RID: 531
	[NetSerializable]
	[Serializable]
	public sealed class ResearchConsoleBoundInterfaceState : BoundUserInterfaceState
	{
		// Token: 0x060005F0 RID: 1520 RVA: 0x00015042 File Offset: 0x00013242
		public ResearchConsoleBoundInterfaceState(int points, int pointsPerSecond)
		{
			this.Points = points;
			this.PointsPerSecond = pointsPerSecond;
		}

		// Token: 0x040005F7 RID: 1527
		public int Points;

		// Token: 0x040005F8 RID: 1528
		public int PointsPerSecond;
	}
}
