using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.NPC
{
	// Token: 0x020002C8 RID: 712
	[NetSerializable]
	[Serializable]
	public struct PathfindingBreadcrumb : IEquatable<PathfindingBreadcrumb>
	{
		// Token: 0x060007CF RID: 1999 RVA: 0x0001A07C File Offset: 0x0001827C
		public PathfindingBreadcrumb(Vector2i coordinates, int layer, int mask, float damage, PathfindingBreadcrumbFlag flags = PathfindingBreadcrumbFlag.None)
		{
			this.Coordinates = coordinates;
			this.Data = new PathfindingData(flags, layer, mask, damage);
		}

		// Token: 0x060007D0 RID: 2000 RVA: 0x0001A096 File Offset: 0x00018296
		public bool Equivalent(PathfindingBreadcrumb other)
		{
			return this.Data.Equals(other.Data);
		}

		// Token: 0x060007D1 RID: 2001 RVA: 0x0001A0A9 File Offset: 0x000182A9
		public bool Equals(PathfindingBreadcrumb other)
		{
			return this.Coordinates.Equals(other.Coordinates) && this.Data.Equals(other.Data);
		}

		// Token: 0x060007D2 RID: 2002 RVA: 0x0001A0D4 File Offset: 0x000182D4
		[NullableContext(2)]
		public override bool Equals(object obj)
		{
			if (obj is PathfindingBreadcrumb)
			{
				PathfindingBreadcrumb other = (PathfindingBreadcrumb)obj;
				return this.Equals(other);
			}
			return false;
		}

		// Token: 0x060007D3 RID: 2003 RVA: 0x0001A0F9 File Offset: 0x000182F9
		public override int GetHashCode()
		{
			return HashCode.Combine<Vector2i, PathfindingData>(this.Coordinates, this.Data);
		}

		// Token: 0x040007FB RID: 2043
		public Vector2i Coordinates;

		// Token: 0x040007FC RID: 2044
		public PathfindingData Data;

		// Token: 0x040007FD RID: 2045
		public static readonly PathfindingBreadcrumb Invalid = new PathfindingBreadcrumb
		{
			Data = new PathfindingData(PathfindingBreadcrumbFlag.None, -1, -1, 0f)
		};
	}
}
