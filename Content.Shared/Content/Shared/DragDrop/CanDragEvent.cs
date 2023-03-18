using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.GameObjects;

namespace Content.Shared.DragDrop
{
	// Token: 0x020004DA RID: 1242
	[ByRefEvent]
	public struct CanDragEvent : IEquatable<CanDragEvent>
	{
		// Token: 0x06000EF5 RID: 3829 RVA: 0x0002FF98 File Offset: 0x0002E198
		[CompilerGenerated]
		public override readonly string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("CanDragEvent");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x06000EF6 RID: 3830 RVA: 0x0002FFE4 File Offset: 0x0002E1E4
		[CompilerGenerated]
		private readonly bool PrintMembers(StringBuilder builder)
		{
			builder.Append("Handled = ");
			builder.Append(this.Handled.ToString());
			return true;
		}

		// Token: 0x06000EF7 RID: 3831 RVA: 0x0003000B File Offset: 0x0002E20B
		[CompilerGenerated]
		public static bool operator !=(CanDragEvent left, CanDragEvent right)
		{
			return !(left == right);
		}

		// Token: 0x06000EF8 RID: 3832 RVA: 0x00030017 File Offset: 0x0002E217
		[CompilerGenerated]
		public static bool operator ==(CanDragEvent left, CanDragEvent right)
		{
			return left.Equals(right);
		}

		// Token: 0x06000EF9 RID: 3833 RVA: 0x00030021 File Offset: 0x0002E221
		[CompilerGenerated]
		public override readonly int GetHashCode()
		{
			return EqualityComparer<bool>.Default.GetHashCode(this.Handled);
		}

		// Token: 0x06000EFA RID: 3834 RVA: 0x00030033 File Offset: 0x0002E233
		[CompilerGenerated]
		public override readonly bool Equals(object obj)
		{
			return obj is CanDragEvent && this.Equals((CanDragEvent)obj);
		}

		// Token: 0x06000EFB RID: 3835 RVA: 0x0003004B File Offset: 0x0002E24B
		[CompilerGenerated]
		public readonly bool Equals(CanDragEvent other)
		{
			return EqualityComparer<bool>.Default.Equals(this.Handled, other.Handled);
		}

		// Token: 0x04000E1B RID: 3611
		public bool Handled;
	}
}
