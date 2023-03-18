using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.GameObjects;

namespace Content.Shared.DragDrop
{
	// Token: 0x020004DD RID: 1245
	[ByRefEvent]
	public struct DragDropDraggedEvent : IEquatable<DragDropDraggedEvent>
	{
		// Token: 0x06000F0E RID: 3854 RVA: 0x000304AB File Offset: 0x0002E6AB
		public DragDropDraggedEvent(EntityUid User, EntityUid Target)
		{
			this.User = User;
			this.Target = Target;
			this.Handled = false;
		}

		// Token: 0x06000F0F RID: 3855 RVA: 0x000304C4 File Offset: 0x0002E6C4
		[CompilerGenerated]
		public override readonly string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("DragDropDraggedEvent");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x06000F10 RID: 3856 RVA: 0x00030510 File Offset: 0x0002E710
		[CompilerGenerated]
		private readonly bool PrintMembers(StringBuilder builder)
		{
			builder.Append("User = ");
			builder.Append(this.User.ToString());
			builder.Append(", Target = ");
			builder.Append(this.Target.ToString());
			builder.Append(", Handled = ");
			builder.Append(this.Handled.ToString());
			return true;
		}

		// Token: 0x06000F11 RID: 3857 RVA: 0x0003058A File Offset: 0x0002E78A
		[CompilerGenerated]
		public static bool operator !=(DragDropDraggedEvent left, DragDropDraggedEvent right)
		{
			return !(left == right);
		}

		// Token: 0x06000F12 RID: 3858 RVA: 0x00030596 File Offset: 0x0002E796
		[CompilerGenerated]
		public static bool operator ==(DragDropDraggedEvent left, DragDropDraggedEvent right)
		{
			return left.Equals(right);
		}

		// Token: 0x06000F13 RID: 3859 RVA: 0x000305A0 File Offset: 0x0002E7A0
		[CompilerGenerated]
		public override readonly int GetHashCode()
		{
			return (EqualityComparer<EntityUid>.Default.GetHashCode(this.User) * -1521134295 + EqualityComparer<EntityUid>.Default.GetHashCode(this.Target)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.Handled);
		}

		// Token: 0x06000F14 RID: 3860 RVA: 0x000305E0 File Offset: 0x0002E7E0
		[CompilerGenerated]
		public override readonly bool Equals(object obj)
		{
			return obj is DragDropDraggedEvent && this.Equals((DragDropDraggedEvent)obj);
		}

		// Token: 0x06000F15 RID: 3861 RVA: 0x000305F8 File Offset: 0x0002E7F8
		[CompilerGenerated]
		public readonly bool Equals(DragDropDraggedEvent other)
		{
			return EqualityComparer<EntityUid>.Default.Equals(this.User, other.User) && EqualityComparer<EntityUid>.Default.Equals(this.Target, other.Target) && EqualityComparer<bool>.Default.Equals(this.Handled, other.Handled);
		}

		// Token: 0x06000F16 RID: 3862 RVA: 0x0003064D File Offset: 0x0002E84D
		[CompilerGenerated]
		public readonly void Deconstruct(out EntityUid User, out EntityUid Target)
		{
			User = this.User;
			Target = this.Target;
		}

		// Token: 0x04000E24 RID: 3620
		public readonly EntityUid User;

		// Token: 0x04000E25 RID: 3621
		public readonly EntityUid Target;

		// Token: 0x04000E26 RID: 3622
		public bool Handled;
	}
}
