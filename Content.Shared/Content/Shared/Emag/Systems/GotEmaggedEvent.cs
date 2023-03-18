using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.GameObjects;

namespace Content.Shared.Emag.Systems
{
	// Token: 0x020004C7 RID: 1223
	[ByRefEvent]
	public struct GotEmaggedEvent : IEquatable<GotEmaggedEvent>
	{
		// Token: 0x06000EC8 RID: 3784 RVA: 0x0002FAA8 File Offset: 0x0002DCA8
		public GotEmaggedEvent(EntityUid UserUid, bool Handled = false, bool Repeatable = false)
		{
			this.UserUid = UserUid;
			this.Handled = Handled;
			this.Repeatable = Repeatable;
		}

		// Token: 0x1700030C RID: 780
		// (get) Token: 0x06000EC9 RID: 3785 RVA: 0x0002FABF File Offset: 0x0002DCBF
		// (set) Token: 0x06000ECA RID: 3786 RVA: 0x0002FAC7 File Offset: 0x0002DCC7
		public EntityUid UserUid { readonly get; set; }

		// Token: 0x1700030D RID: 781
		// (get) Token: 0x06000ECB RID: 3787 RVA: 0x0002FAD0 File Offset: 0x0002DCD0
		// (set) Token: 0x06000ECC RID: 3788 RVA: 0x0002FAD8 File Offset: 0x0002DCD8
		public bool Handled { readonly get; set; }

		// Token: 0x1700030E RID: 782
		// (get) Token: 0x06000ECD RID: 3789 RVA: 0x0002FAE1 File Offset: 0x0002DCE1
		// (set) Token: 0x06000ECE RID: 3790 RVA: 0x0002FAE9 File Offset: 0x0002DCE9
		public bool Repeatable { readonly get; set; }

		// Token: 0x06000ECF RID: 3791 RVA: 0x0002FAF4 File Offset: 0x0002DCF4
		[CompilerGenerated]
		public override readonly string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("GotEmaggedEvent");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x06000ED0 RID: 3792 RVA: 0x0002FB40 File Offset: 0x0002DD40
		[CompilerGenerated]
		private readonly bool PrintMembers(StringBuilder builder)
		{
			builder.Append("UserUid = ");
			builder.Append(this.UserUid.ToString());
			builder.Append(", Handled = ");
			builder.Append(this.Handled.ToString());
			builder.Append(", Repeatable = ");
			builder.Append(this.Repeatable.ToString());
			return true;
		}

		// Token: 0x06000ED1 RID: 3793 RVA: 0x0002FBC3 File Offset: 0x0002DDC3
		[CompilerGenerated]
		public static bool operator !=(GotEmaggedEvent left, GotEmaggedEvent right)
		{
			return !(left == right);
		}

		// Token: 0x06000ED2 RID: 3794 RVA: 0x0002FBCF File Offset: 0x0002DDCF
		[CompilerGenerated]
		public static bool operator ==(GotEmaggedEvent left, GotEmaggedEvent right)
		{
			return left.Equals(right);
		}

		// Token: 0x06000ED3 RID: 3795 RVA: 0x0002FBD9 File Offset: 0x0002DDD9
		[CompilerGenerated]
		public override readonly int GetHashCode()
		{
			return (EqualityComparer<EntityUid>.Default.GetHashCode(this.<UserUid>k__BackingField) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<Handled>k__BackingField)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<Repeatable>k__BackingField);
		}

		// Token: 0x06000ED4 RID: 3796 RVA: 0x0002FC19 File Offset: 0x0002DE19
		[CompilerGenerated]
		public override readonly bool Equals(object obj)
		{
			return obj is GotEmaggedEvent && this.Equals((GotEmaggedEvent)obj);
		}

		// Token: 0x06000ED5 RID: 3797 RVA: 0x0002FC34 File Offset: 0x0002DE34
		[CompilerGenerated]
		public readonly bool Equals(GotEmaggedEvent other)
		{
			return EqualityComparer<EntityUid>.Default.Equals(this.<UserUid>k__BackingField, other.<UserUid>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<Handled>k__BackingField, other.<Handled>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<Repeatable>k__BackingField, other.<Repeatable>k__BackingField);
		}

		// Token: 0x06000ED6 RID: 3798 RVA: 0x0002FC89 File Offset: 0x0002DE89
		[CompilerGenerated]
		public readonly void Deconstruct(out EntityUid UserUid, out bool Handled, out bool Repeatable)
		{
			UserUid = this.UserUid;
			Handled = this.Handled;
			Repeatable = this.Repeatable;
		}
	}
}
