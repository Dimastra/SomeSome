using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Content.Client.UserInterface.Controls
{
	// Token: 0x020000DB RID: 219
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class ListData : IEquatable<ListData>
	{
		// Token: 0x17000106 RID: 262
		// (get) Token: 0x0600061D RID: 1565 RVA: 0x00021478 File Offset: 0x0001F678
		[CompilerGenerated]
		protected virtual Type EqualityContract
		{
			[CompilerGenerated]
			get
			{
				return typeof(ListData);
			}
		}

		// Token: 0x0600061E RID: 1566 RVA: 0x00021484 File Offset: 0x0001F684
		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("ListData");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x0600061F RID: 1567 RVA: 0x00003C59 File Offset: 0x00001E59
		[CompilerGenerated]
		protected virtual bool PrintMembers(StringBuilder builder)
		{
			return false;
		}

		// Token: 0x06000620 RID: 1568 RVA: 0x000214D0 File Offset: 0x0001F6D0
		[NullableContext(2)]
		[CompilerGenerated]
		public static bool operator !=(ListData left, ListData right)
		{
			return !(left == right);
		}

		// Token: 0x06000621 RID: 1569 RVA: 0x000214DC File Offset: 0x0001F6DC
		[NullableContext(2)]
		[CompilerGenerated]
		public static bool operator ==(ListData left, ListData right)
		{
			return left == right || (left != null && left.Equals(right));
		}

		// Token: 0x06000622 RID: 1570 RVA: 0x000214F0 File Offset: 0x0001F6F0
		[CompilerGenerated]
		public override int GetHashCode()
		{
			return EqualityComparer<Type>.Default.GetHashCode(this.EqualityContract);
		}

		// Token: 0x06000623 RID: 1571 RVA: 0x00021502 File Offset: 0x0001F702
		[NullableContext(2)]
		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			return this.Equals(obj as ListData);
		}

		// Token: 0x06000624 RID: 1572 RVA: 0x00021510 File Offset: 0x0001F710
		[NullableContext(2)]
		[CompilerGenerated]
		public virtual bool Equals(ListData other)
		{
			return this == other || (other != null && this.EqualityContract == other.EqualityContract);
		}

		// Token: 0x06000626 RID: 1574 RVA: 0x00004569 File Offset: 0x00002769
		[CompilerGenerated]
		protected ListData(ListData original)
		{
		}

		// Token: 0x06000627 RID: 1575 RVA: 0x00004569 File Offset: 0x00002769
		protected ListData()
		{
		}
	}
}
