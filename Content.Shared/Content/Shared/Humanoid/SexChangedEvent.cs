using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Content.Shared.Humanoid
{
	// Token: 0x0200040C RID: 1036
	public struct SexChangedEvent : IEquatable<SexChangedEvent>
	{
		// Token: 0x06000C23 RID: 3107 RVA: 0x000283D3 File Offset: 0x000265D3
		public SexChangedEvent(Sex OldSex, Sex NewSex)
		{
			this.OldSex = OldSex;
			this.NewSex = NewSex;
		}

		// Token: 0x17000268 RID: 616
		// (get) Token: 0x06000C24 RID: 3108 RVA: 0x000283E3 File Offset: 0x000265E3
		// (set) Token: 0x06000C25 RID: 3109 RVA: 0x000283EB File Offset: 0x000265EB
		public Sex OldSex { readonly get; set; }

		// Token: 0x17000269 RID: 617
		// (get) Token: 0x06000C26 RID: 3110 RVA: 0x000283F4 File Offset: 0x000265F4
		// (set) Token: 0x06000C27 RID: 3111 RVA: 0x000283FC File Offset: 0x000265FC
		public Sex NewSex { readonly get; set; }

		// Token: 0x06000C28 RID: 3112 RVA: 0x00028408 File Offset: 0x00026608
		[CompilerGenerated]
		public override readonly string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("SexChangedEvent");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x06000C29 RID: 3113 RVA: 0x00028454 File Offset: 0x00026654
		[CompilerGenerated]
		private readonly bool PrintMembers(StringBuilder builder)
		{
			builder.Append("OldSex = ");
			builder.Append(this.OldSex.ToString());
			builder.Append(", NewSex = ");
			builder.Append(this.NewSex.ToString());
			return true;
		}

		// Token: 0x06000C2A RID: 3114 RVA: 0x000284B0 File Offset: 0x000266B0
		[CompilerGenerated]
		public static bool operator !=(SexChangedEvent left, SexChangedEvent right)
		{
			return !(left == right);
		}

		// Token: 0x06000C2B RID: 3115 RVA: 0x000284BC File Offset: 0x000266BC
		[CompilerGenerated]
		public static bool operator ==(SexChangedEvent left, SexChangedEvent right)
		{
			return left.Equals(right);
		}

		// Token: 0x06000C2C RID: 3116 RVA: 0x000284C6 File Offset: 0x000266C6
		[CompilerGenerated]
		public override readonly int GetHashCode()
		{
			return EqualityComparer<Sex>.Default.GetHashCode(this.<OldSex>k__BackingField) * -1521134295 + EqualityComparer<Sex>.Default.GetHashCode(this.<NewSex>k__BackingField);
		}

		// Token: 0x06000C2D RID: 3117 RVA: 0x000284EF File Offset: 0x000266EF
		[CompilerGenerated]
		public override readonly bool Equals(object obj)
		{
			return obj is SexChangedEvent && this.Equals((SexChangedEvent)obj);
		}

		// Token: 0x06000C2E RID: 3118 RVA: 0x00028507 File Offset: 0x00026707
		[CompilerGenerated]
		public readonly bool Equals(SexChangedEvent other)
		{
			return EqualityComparer<Sex>.Default.Equals(this.<OldSex>k__BackingField, other.<OldSex>k__BackingField) && EqualityComparer<Sex>.Default.Equals(this.<NewSex>k__BackingField, other.<NewSex>k__BackingField);
		}

		// Token: 0x06000C2F RID: 3119 RVA: 0x00028539 File Offset: 0x00026739
		[CompilerGenerated]
		public readonly void Deconstruct(out Sex OldSex, out Sex NewSex)
		{
			OldSex = this.OldSex;
			NewSex = this.NewSex;
		}
	}
}
