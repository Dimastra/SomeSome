using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.GameObjects;

namespace Content.Shared.DragDrop
{
	// Token: 0x020004DB RID: 1243
	[ByRefEvent]
	public struct CanDropDraggedEvent : IEquatable<CanDropDraggedEvent>
	{
		// Token: 0x06000EFC RID: 3836 RVA: 0x00030063 File Offset: 0x0002E263
		public CanDropDraggedEvent(EntityUid User, EntityUid Target)
		{
			this.User = User;
			this.Target = Target;
			this.Handled = false;
			this.CanDrop = false;
		}

		// Token: 0x06000EFD RID: 3837 RVA: 0x00030084 File Offset: 0x0002E284
		[CompilerGenerated]
		public override readonly string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("CanDropDraggedEvent");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x06000EFE RID: 3838 RVA: 0x000300D0 File Offset: 0x0002E2D0
		[CompilerGenerated]
		private readonly bool PrintMembers(StringBuilder builder)
		{
			builder.Append("User = ");
			builder.Append(this.User.ToString());
			builder.Append(", Target = ");
			builder.Append(this.Target.ToString());
			builder.Append(", Handled = ");
			builder.Append(this.Handled.ToString());
			builder.Append(", CanDrop = ");
			builder.Append(this.CanDrop.ToString());
			return true;
		}

		// Token: 0x06000EFF RID: 3839 RVA: 0x0003016E File Offset: 0x0002E36E
		[CompilerGenerated]
		public static bool operator !=(CanDropDraggedEvent left, CanDropDraggedEvent right)
		{
			return !(left == right);
		}

		// Token: 0x06000F00 RID: 3840 RVA: 0x0003017A File Offset: 0x0002E37A
		[CompilerGenerated]
		public static bool operator ==(CanDropDraggedEvent left, CanDropDraggedEvent right)
		{
			return left.Equals(right);
		}

		// Token: 0x06000F01 RID: 3841 RVA: 0x00030184 File Offset: 0x0002E384
		[CompilerGenerated]
		public override readonly int GetHashCode()
		{
			return ((EqualityComparer<EntityUid>.Default.GetHashCode(this.User) * -1521134295 + EqualityComparer<EntityUid>.Default.GetHashCode(this.Target)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.Handled)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.CanDrop);
		}

		// Token: 0x06000F02 RID: 3842 RVA: 0x000301E6 File Offset: 0x0002E3E6
		[CompilerGenerated]
		public override readonly bool Equals(object obj)
		{
			return obj is CanDropDraggedEvent && this.Equals((CanDropDraggedEvent)obj);
		}

		// Token: 0x06000F03 RID: 3843 RVA: 0x00030200 File Offset: 0x0002E400
		[CompilerGenerated]
		public readonly bool Equals(CanDropDraggedEvent other)
		{
			return EqualityComparer<EntityUid>.Default.Equals(this.User, other.User) && EqualityComparer<EntityUid>.Default.Equals(this.Target, other.Target) && EqualityComparer<bool>.Default.Equals(this.Handled, other.Handled) && EqualityComparer<bool>.Default.Equals(this.CanDrop, other.CanDrop);
		}

		// Token: 0x06000F04 RID: 3844 RVA: 0x0003026D File Offset: 0x0002E46D
		[CompilerGenerated]
		public readonly void Deconstruct(out EntityUid User, out EntityUid Target)
		{
			User = this.User;
			Target = this.Target;
		}

		// Token: 0x04000E1C RID: 3612
		public readonly EntityUid User;

		// Token: 0x04000E1D RID: 3613
		public readonly EntityUid Target;

		// Token: 0x04000E1E RID: 3614
		public bool Handled;

		// Token: 0x04000E1F RID: 3615
		public bool CanDrop;
	}
}
