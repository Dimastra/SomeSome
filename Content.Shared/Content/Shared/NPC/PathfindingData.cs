using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Serialization;

namespace Content.Shared.NPC
{
	// Token: 0x020002C9 RID: 713
	[NetSerializable]
	[Serializable]
	public struct PathfindingData : IEquatable<PathfindingData>
	{
		// Token: 0x17000188 RID: 392
		// (get) Token: 0x060007D5 RID: 2005 RVA: 0x0001A13B File Offset: 0x0001833B
		public bool IsFreeSpace
		{
			get
			{
				return this.Flags == PathfindingBreadcrumbFlag.None && this.Damage.Equals(0f);
			}
		}

		// Token: 0x060007D6 RID: 2006 RVA: 0x0001A157 File Offset: 0x00018357
		public PathfindingData(PathfindingBreadcrumbFlag flag, int layer, int mask, float damage)
		{
			this.Flags = flag;
			this.CollisionLayer = layer;
			this.CollisionMask = mask;
			this.Damage = damage;
		}

		// Token: 0x060007D7 RID: 2007 RVA: 0x0001A178 File Offset: 0x00018378
		public bool IsEquivalent(PathfindingData other)
		{
			return this.CollisionLayer.Equals(other.CollisionLayer) && this.CollisionMask.Equals(other.CollisionMask) && this.Flags.Equals(other.Flags);
		}

		// Token: 0x060007D8 RID: 2008 RVA: 0x0001A1CC File Offset: 0x000183CC
		public bool Equals(PathfindingData other)
		{
			return this.CollisionLayer.Equals(other.CollisionLayer) && this.CollisionMask.Equals(other.CollisionMask) && this.Flags.Equals(other.Flags) && this.Damage.Equals(other.Damage);
		}

		// Token: 0x060007D9 RID: 2009 RVA: 0x0001A230 File Offset: 0x00018430
		[NullableContext(2)]
		public override bool Equals(object obj)
		{
			if (obj is PathfindingData)
			{
				PathfindingData other = (PathfindingData)obj;
				return this.Equals(other);
			}
			return false;
		}

		// Token: 0x060007DA RID: 2010 RVA: 0x0001A255 File Offset: 0x00018455
		public override int GetHashCode()
		{
			return HashCode.Combine<int, int, int>((int)this.Flags, this.CollisionLayer, this.CollisionMask);
		}

		// Token: 0x040007FE RID: 2046
		public PathfindingBreadcrumbFlag Flags;

		// Token: 0x040007FF RID: 2047
		public int CollisionLayer;

		// Token: 0x04000800 RID: 2048
		public int CollisionMask;

		// Token: 0x04000801 RID: 2049
		public float Damage;
	}
}
