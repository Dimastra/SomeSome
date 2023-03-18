using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.GameObjects;

namespace Content.Shared.Storage.Components
{
	// Token: 0x0200013A RID: 314
	[ByRefEvent]
	public struct StoreMobInItemContainerAttemptEvent : IEquatable<StoreMobInItemContainerAttemptEvent>
	{
		// Token: 0x0600039C RID: 924 RVA: 0x0000F463 File Offset: 0x0000D663
		public StoreMobInItemContainerAttemptEvent(bool Handled, bool Cancelled = false)
		{
			this.Handled = Handled;
			this.Cancelled = Cancelled;
		}

		// Token: 0x170000B1 RID: 177
		// (get) Token: 0x0600039D RID: 925 RVA: 0x0000F473 File Offset: 0x0000D673
		// (set) Token: 0x0600039E RID: 926 RVA: 0x0000F47B File Offset: 0x0000D67B
		public bool Handled { readonly get; set; }

		// Token: 0x170000B2 RID: 178
		// (get) Token: 0x0600039F RID: 927 RVA: 0x0000F484 File Offset: 0x0000D684
		// (set) Token: 0x060003A0 RID: 928 RVA: 0x0000F48C File Offset: 0x0000D68C
		public bool Cancelled { readonly get; set; }

		// Token: 0x060003A1 RID: 929 RVA: 0x0000F498 File Offset: 0x0000D698
		[CompilerGenerated]
		public override readonly string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("StoreMobInItemContainerAttemptEvent");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x060003A2 RID: 930 RVA: 0x0000F4E4 File Offset: 0x0000D6E4
		[CompilerGenerated]
		private readonly bool PrintMembers(StringBuilder builder)
		{
			builder.Append("Handled = ");
			builder.Append(this.Handled.ToString());
			builder.Append(", Cancelled = ");
			builder.Append(this.Cancelled.ToString());
			return true;
		}

		// Token: 0x060003A3 RID: 931 RVA: 0x0000F540 File Offset: 0x0000D740
		[CompilerGenerated]
		public static bool operator !=(StoreMobInItemContainerAttemptEvent left, StoreMobInItemContainerAttemptEvent right)
		{
			return !(left == right);
		}

		// Token: 0x060003A4 RID: 932 RVA: 0x0000F54C File Offset: 0x0000D74C
		[CompilerGenerated]
		public static bool operator ==(StoreMobInItemContainerAttemptEvent left, StoreMobInItemContainerAttemptEvent right)
		{
			return left.Equals(right);
		}

		// Token: 0x060003A5 RID: 933 RVA: 0x0000F556 File Offset: 0x0000D756
		[CompilerGenerated]
		public override readonly int GetHashCode()
		{
			return EqualityComparer<bool>.Default.GetHashCode(this.<Handled>k__BackingField) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<Cancelled>k__BackingField);
		}

		// Token: 0x060003A6 RID: 934 RVA: 0x0000F57F File Offset: 0x0000D77F
		[CompilerGenerated]
		public override readonly bool Equals(object obj)
		{
			return obj is StoreMobInItemContainerAttemptEvent && this.Equals((StoreMobInItemContainerAttemptEvent)obj);
		}

		// Token: 0x060003A7 RID: 935 RVA: 0x0000F597 File Offset: 0x0000D797
		[CompilerGenerated]
		public readonly bool Equals(StoreMobInItemContainerAttemptEvent other)
		{
			return EqualityComparer<bool>.Default.Equals(this.<Handled>k__BackingField, other.<Handled>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<Cancelled>k__BackingField, other.<Cancelled>k__BackingField);
		}

		// Token: 0x060003A8 RID: 936 RVA: 0x0000F5C9 File Offset: 0x0000D7C9
		[CompilerGenerated]
		public readonly void Deconstruct(out bool Handled, out bool Cancelled)
		{
			Handled = this.Handled;
			Cancelled = this.Cancelled;
		}
	}
}
