using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.GameObjects;

namespace Content.Shared.Lock
{
	// Token: 0x0200035A RID: 858
	[ByRefEvent]
	public struct LockToggleAttemptEvent : IEquatable<LockToggleAttemptEvent>
	{
		// Token: 0x060009F9 RID: 2553 RVA: 0x00020856 File Offset: 0x0001EA56
		public LockToggleAttemptEvent(EntityUid User, bool Silent = false, bool Cancelled = false)
		{
			this.User = User;
			this.Silent = Silent;
			this.Cancelled = Cancelled;
		}

		// Token: 0x170001EC RID: 492
		// (get) Token: 0x060009FA RID: 2554 RVA: 0x0002086D File Offset: 0x0001EA6D
		// (set) Token: 0x060009FB RID: 2555 RVA: 0x00020875 File Offset: 0x0001EA75
		public EntityUid User { readonly get; set; }

		// Token: 0x170001ED RID: 493
		// (get) Token: 0x060009FC RID: 2556 RVA: 0x0002087E File Offset: 0x0001EA7E
		// (set) Token: 0x060009FD RID: 2557 RVA: 0x00020886 File Offset: 0x0001EA86
		public bool Silent { readonly get; set; }

		// Token: 0x170001EE RID: 494
		// (get) Token: 0x060009FE RID: 2558 RVA: 0x0002088F File Offset: 0x0001EA8F
		// (set) Token: 0x060009FF RID: 2559 RVA: 0x00020897 File Offset: 0x0001EA97
		public bool Cancelled { readonly get; set; }

		// Token: 0x06000A00 RID: 2560 RVA: 0x000208A0 File Offset: 0x0001EAA0
		[CompilerGenerated]
		public override readonly string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("LockToggleAttemptEvent");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x06000A01 RID: 2561 RVA: 0x000208EC File Offset: 0x0001EAEC
		[CompilerGenerated]
		private readonly bool PrintMembers(StringBuilder builder)
		{
			builder.Append("User = ");
			builder.Append(this.User.ToString());
			builder.Append(", Silent = ");
			builder.Append(this.Silent.ToString());
			builder.Append(", Cancelled = ");
			builder.Append(this.Cancelled.ToString());
			return true;
		}

		// Token: 0x06000A02 RID: 2562 RVA: 0x0002096F File Offset: 0x0001EB6F
		[CompilerGenerated]
		public static bool operator !=(LockToggleAttemptEvent left, LockToggleAttemptEvent right)
		{
			return !(left == right);
		}

		// Token: 0x06000A03 RID: 2563 RVA: 0x0002097B File Offset: 0x0001EB7B
		[CompilerGenerated]
		public static bool operator ==(LockToggleAttemptEvent left, LockToggleAttemptEvent right)
		{
			return left.Equals(right);
		}

		// Token: 0x06000A04 RID: 2564 RVA: 0x00020985 File Offset: 0x0001EB85
		[CompilerGenerated]
		public override readonly int GetHashCode()
		{
			return (EqualityComparer<EntityUid>.Default.GetHashCode(this.<User>k__BackingField) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<Silent>k__BackingField)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<Cancelled>k__BackingField);
		}

		// Token: 0x06000A05 RID: 2565 RVA: 0x000209C5 File Offset: 0x0001EBC5
		[CompilerGenerated]
		public override readonly bool Equals(object obj)
		{
			return obj is LockToggleAttemptEvent && this.Equals((LockToggleAttemptEvent)obj);
		}

		// Token: 0x06000A06 RID: 2566 RVA: 0x000209E0 File Offset: 0x0001EBE0
		[CompilerGenerated]
		public readonly bool Equals(LockToggleAttemptEvent other)
		{
			return EqualityComparer<EntityUid>.Default.Equals(this.<User>k__BackingField, other.<User>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<Silent>k__BackingField, other.<Silent>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<Cancelled>k__BackingField, other.<Cancelled>k__BackingField);
		}

		// Token: 0x06000A07 RID: 2567 RVA: 0x00020A35 File Offset: 0x0001EC35
		[CompilerGenerated]
		public readonly void Deconstruct(out EntityUid User, out bool Silent, out bool Cancelled)
		{
			User = this.User;
			Silent = this.Silent;
			Cancelled = this.Cancelled;
		}
	}
}
