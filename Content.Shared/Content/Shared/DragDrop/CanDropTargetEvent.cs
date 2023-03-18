using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.GameObjects;

namespace Content.Shared.DragDrop
{
	// Token: 0x020004DC RID: 1244
	[ByRefEvent]
	public struct CanDropTargetEvent : IEquatable<CanDropTargetEvent>
	{
		// Token: 0x06000F05 RID: 3845 RVA: 0x00030287 File Offset: 0x0002E487
		public CanDropTargetEvent(EntityUid User, EntityUid Dragged)
		{
			this.User = User;
			this.Dragged = Dragged;
			this.Handled = false;
			this.CanDrop = false;
		}

		// Token: 0x06000F06 RID: 3846 RVA: 0x000302A8 File Offset: 0x0002E4A8
		[CompilerGenerated]
		public override readonly string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("CanDropTargetEvent");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x06000F07 RID: 3847 RVA: 0x000302F4 File Offset: 0x0002E4F4
		[CompilerGenerated]
		private readonly bool PrintMembers(StringBuilder builder)
		{
			builder.Append("User = ");
			builder.Append(this.User.ToString());
			builder.Append(", Dragged = ");
			builder.Append(this.Dragged.ToString());
			builder.Append(", Handled = ");
			builder.Append(this.Handled.ToString());
			builder.Append(", CanDrop = ");
			builder.Append(this.CanDrop.ToString());
			return true;
		}

		// Token: 0x06000F08 RID: 3848 RVA: 0x00030392 File Offset: 0x0002E592
		[CompilerGenerated]
		public static bool operator !=(CanDropTargetEvent left, CanDropTargetEvent right)
		{
			return !(left == right);
		}

		// Token: 0x06000F09 RID: 3849 RVA: 0x0003039E File Offset: 0x0002E59E
		[CompilerGenerated]
		public static bool operator ==(CanDropTargetEvent left, CanDropTargetEvent right)
		{
			return left.Equals(right);
		}

		// Token: 0x06000F0A RID: 3850 RVA: 0x000303A8 File Offset: 0x0002E5A8
		[CompilerGenerated]
		public override readonly int GetHashCode()
		{
			return ((EqualityComparer<EntityUid>.Default.GetHashCode(this.User) * -1521134295 + EqualityComparer<EntityUid>.Default.GetHashCode(this.Dragged)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.Handled)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.CanDrop);
		}

		// Token: 0x06000F0B RID: 3851 RVA: 0x0003040A File Offset: 0x0002E60A
		[CompilerGenerated]
		public override readonly bool Equals(object obj)
		{
			return obj is CanDropTargetEvent && this.Equals((CanDropTargetEvent)obj);
		}

		// Token: 0x06000F0C RID: 3852 RVA: 0x00030424 File Offset: 0x0002E624
		[CompilerGenerated]
		public readonly bool Equals(CanDropTargetEvent other)
		{
			return EqualityComparer<EntityUid>.Default.Equals(this.User, other.User) && EqualityComparer<EntityUid>.Default.Equals(this.Dragged, other.Dragged) && EqualityComparer<bool>.Default.Equals(this.Handled, other.Handled) && EqualityComparer<bool>.Default.Equals(this.CanDrop, other.CanDrop);
		}

		// Token: 0x06000F0D RID: 3853 RVA: 0x00030491 File Offset: 0x0002E691
		[CompilerGenerated]
		public readonly void Deconstruct(out EntityUid User, out EntityUid Dragged)
		{
			User = this.User;
			Dragged = this.Dragged;
		}

		// Token: 0x04000E20 RID: 3616
		public readonly EntityUid User;

		// Token: 0x04000E21 RID: 3617
		public readonly EntityUid Dragged;

		// Token: 0x04000E22 RID: 3618
		public bool Handled;

		// Token: 0x04000E23 RID: 3619
		public bool CanDrop;
	}
}
