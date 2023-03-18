using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.GameObjects;

namespace Content.Shared.Gravity
{
	// Token: 0x02000445 RID: 1093
	[ByRefEvent]
	public readonly struct GravityChangedEvent : IEquatable<GravityChangedEvent>
	{
		// Token: 0x06000D3B RID: 3387 RVA: 0x0002BD31 File Offset: 0x00029F31
		public GravityChangedEvent(EntityUid ChangedGridIndex, bool HasGravity)
		{
			this.ChangedGridIndex = ChangedGridIndex;
			this.HasGravity = HasGravity;
		}

		// Token: 0x170002C0 RID: 704
		// (get) Token: 0x06000D3C RID: 3388 RVA: 0x0002BD41 File Offset: 0x00029F41
		// (set) Token: 0x06000D3D RID: 3389 RVA: 0x0002BD49 File Offset: 0x00029F49
		public EntityUid ChangedGridIndex { get; set; }

		// Token: 0x170002C1 RID: 705
		// (get) Token: 0x06000D3E RID: 3390 RVA: 0x0002BD52 File Offset: 0x00029F52
		// (set) Token: 0x06000D3F RID: 3391 RVA: 0x0002BD5A File Offset: 0x00029F5A
		public bool HasGravity { get; set; }

		// Token: 0x06000D40 RID: 3392 RVA: 0x0002BD64 File Offset: 0x00029F64
		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("GravityChangedEvent");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x06000D41 RID: 3393 RVA: 0x0002BDB0 File Offset: 0x00029FB0
		[CompilerGenerated]
		private bool PrintMembers(StringBuilder builder)
		{
			builder.Append("ChangedGridIndex = ");
			builder.Append(this.ChangedGridIndex.ToString());
			builder.Append(", HasGravity = ");
			builder.Append(this.HasGravity.ToString());
			return true;
		}

		// Token: 0x06000D42 RID: 3394 RVA: 0x0002BE0C File Offset: 0x0002A00C
		[CompilerGenerated]
		public static bool operator !=(GravityChangedEvent left, GravityChangedEvent right)
		{
			return !(left == right);
		}

		// Token: 0x06000D43 RID: 3395 RVA: 0x0002BE18 File Offset: 0x0002A018
		[CompilerGenerated]
		public static bool operator ==(GravityChangedEvent left, GravityChangedEvent right)
		{
			return left.Equals(right);
		}

		// Token: 0x06000D44 RID: 3396 RVA: 0x0002BE22 File Offset: 0x0002A022
		[CompilerGenerated]
		public override int GetHashCode()
		{
			return EqualityComparer<EntityUid>.Default.GetHashCode(this.<ChangedGridIndex>k__BackingField) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<HasGravity>k__BackingField);
		}

		// Token: 0x06000D45 RID: 3397 RVA: 0x0002BE4B File Offset: 0x0002A04B
		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			return obj is GravityChangedEvent && this.Equals((GravityChangedEvent)obj);
		}

		// Token: 0x06000D46 RID: 3398 RVA: 0x0002BE63 File Offset: 0x0002A063
		[CompilerGenerated]
		public bool Equals(GravityChangedEvent other)
		{
			return EqualityComparer<EntityUid>.Default.Equals(this.<ChangedGridIndex>k__BackingField, other.<ChangedGridIndex>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<HasGravity>k__BackingField, other.<HasGravity>k__BackingField);
		}

		// Token: 0x06000D47 RID: 3399 RVA: 0x0002BE95 File Offset: 0x0002A095
		[CompilerGenerated]
		public void Deconstruct(out EntityUid ChangedGridIndex, out bool HasGravity)
		{
			ChangedGridIndex = this.ChangedGridIndex;
			HasGravity = this.HasGravity;
		}
	}
}
