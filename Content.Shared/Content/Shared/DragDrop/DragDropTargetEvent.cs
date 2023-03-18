using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.GameObjects;

namespace Content.Shared.DragDrop
{
	// Token: 0x020004DE RID: 1246
	[ByRefEvent]
	public struct DragDropTargetEvent : IEquatable<DragDropTargetEvent>
	{
		// Token: 0x06000F17 RID: 3863 RVA: 0x00030667 File Offset: 0x0002E867
		public DragDropTargetEvent(EntityUid User, EntityUid Dragged)
		{
			this.User = User;
			this.Dragged = Dragged;
			this.Handled = false;
		}

		// Token: 0x06000F18 RID: 3864 RVA: 0x00030680 File Offset: 0x0002E880
		[CompilerGenerated]
		public override readonly string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("DragDropTargetEvent");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x06000F19 RID: 3865 RVA: 0x000306CC File Offset: 0x0002E8CC
		[CompilerGenerated]
		private readonly bool PrintMembers(StringBuilder builder)
		{
			builder.Append("User = ");
			builder.Append(this.User.ToString());
			builder.Append(", Dragged = ");
			builder.Append(this.Dragged.ToString());
			builder.Append(", Handled = ");
			builder.Append(this.Handled.ToString());
			return true;
		}

		// Token: 0x06000F1A RID: 3866 RVA: 0x00030746 File Offset: 0x0002E946
		[CompilerGenerated]
		public static bool operator !=(DragDropTargetEvent left, DragDropTargetEvent right)
		{
			return !(left == right);
		}

		// Token: 0x06000F1B RID: 3867 RVA: 0x00030752 File Offset: 0x0002E952
		[CompilerGenerated]
		public static bool operator ==(DragDropTargetEvent left, DragDropTargetEvent right)
		{
			return left.Equals(right);
		}

		// Token: 0x06000F1C RID: 3868 RVA: 0x0003075C File Offset: 0x0002E95C
		[CompilerGenerated]
		public override readonly int GetHashCode()
		{
			return (EqualityComparer<EntityUid>.Default.GetHashCode(this.User) * -1521134295 + EqualityComparer<EntityUid>.Default.GetHashCode(this.Dragged)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.Handled);
		}

		// Token: 0x06000F1D RID: 3869 RVA: 0x0003079C File Offset: 0x0002E99C
		[CompilerGenerated]
		public override readonly bool Equals(object obj)
		{
			return obj is DragDropTargetEvent && this.Equals((DragDropTargetEvent)obj);
		}

		// Token: 0x06000F1E RID: 3870 RVA: 0x000307B4 File Offset: 0x0002E9B4
		[CompilerGenerated]
		public readonly bool Equals(DragDropTargetEvent other)
		{
			return EqualityComparer<EntityUid>.Default.Equals(this.User, other.User) && EqualityComparer<EntityUid>.Default.Equals(this.Dragged, other.Dragged) && EqualityComparer<bool>.Default.Equals(this.Handled, other.Handled);
		}

		// Token: 0x06000F1F RID: 3871 RVA: 0x00030809 File Offset: 0x0002EA09
		[CompilerGenerated]
		public readonly void Deconstruct(out EntityUid User, out EntityUid Dragged)
		{
			User = this.User;
			Dragged = this.Dragged;
		}

		// Token: 0x04000E27 RID: 3623
		public readonly EntityUid User;

		// Token: 0x04000E28 RID: 3624
		public readonly EntityUid Dragged;

		// Token: 0x04000E29 RID: 3625
		public bool Handled;
	}
}
