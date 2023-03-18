using System;
using Robust.Shared.Map;

namespace Content.Server.NPC.Pathfinding
{
	// Token: 0x0200033D RID: 829
	public struct PathPortal
	{
		// Token: 0x0600117D RID: 4477 RVA: 0x0005C485 File Offset: 0x0005A685
		public PathPortal(int handle, EntityCoordinates coordsA, EntityCoordinates coordsB)
		{
			this.Handle = handle;
			this.CoordinatesA = coordsA;
			this.CoordinatesB = coordsB;
		}

		// Token: 0x0600117E RID: 4478 RVA: 0x0005C49C File Offset: 0x0005A69C
		public override int GetHashCode()
		{
			return this.Handle;
		}

		// Token: 0x04000A66 RID: 2662
		public readonly int Handle;

		// Token: 0x04000A67 RID: 2663
		public readonly EntityCoordinates CoordinatesA;

		// Token: 0x04000A68 RID: 2664
		public readonly EntityCoordinates CoordinatesB;
	}
}
