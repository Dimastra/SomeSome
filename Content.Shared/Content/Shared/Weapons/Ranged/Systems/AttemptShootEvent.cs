using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.GameObjects;

namespace Content.Shared.Weapons.Ranged.Systems
{
	// Token: 0x02000049 RID: 73
	[ByRefEvent]
	public struct AttemptShootEvent : IEquatable<AttemptShootEvent>
	{
		// Token: 0x0600010B RID: 267 RVA: 0x00006A4F File Offset: 0x00004C4F
		public AttemptShootEvent(EntityUid User, bool Cancelled = false)
		{
			this.User = User;
			this.Cancelled = Cancelled;
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x0600010C RID: 268 RVA: 0x00006A5F File Offset: 0x00004C5F
		// (set) Token: 0x0600010D RID: 269 RVA: 0x00006A67 File Offset: 0x00004C67
		public EntityUid User { readonly get; set; }

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x0600010E RID: 270 RVA: 0x00006A70 File Offset: 0x00004C70
		// (set) Token: 0x0600010F RID: 271 RVA: 0x00006A78 File Offset: 0x00004C78
		public bool Cancelled { readonly get; set; }

		// Token: 0x06000110 RID: 272 RVA: 0x00006A84 File Offset: 0x00004C84
		[CompilerGenerated]
		public override readonly string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("AttemptShootEvent");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x06000111 RID: 273 RVA: 0x00006AD0 File Offset: 0x00004CD0
		[CompilerGenerated]
		private readonly bool PrintMembers(StringBuilder builder)
		{
			builder.Append("User = ");
			builder.Append(this.User.ToString());
			builder.Append(", Cancelled = ");
			builder.Append(this.Cancelled.ToString());
			return true;
		}

		// Token: 0x06000112 RID: 274 RVA: 0x00006B2C File Offset: 0x00004D2C
		[CompilerGenerated]
		public static bool operator !=(AttemptShootEvent left, AttemptShootEvent right)
		{
			return !(left == right);
		}

		// Token: 0x06000113 RID: 275 RVA: 0x00006B38 File Offset: 0x00004D38
		[CompilerGenerated]
		public static bool operator ==(AttemptShootEvent left, AttemptShootEvent right)
		{
			return left.Equals(right);
		}

		// Token: 0x06000114 RID: 276 RVA: 0x00006B42 File Offset: 0x00004D42
		[CompilerGenerated]
		public override readonly int GetHashCode()
		{
			return EqualityComparer<EntityUid>.Default.GetHashCode(this.<User>k__BackingField) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<Cancelled>k__BackingField);
		}

		// Token: 0x06000115 RID: 277 RVA: 0x00006B6B File Offset: 0x00004D6B
		[CompilerGenerated]
		public override readonly bool Equals(object obj)
		{
			return obj is AttemptShootEvent && this.Equals((AttemptShootEvent)obj);
		}

		// Token: 0x06000116 RID: 278 RVA: 0x00006B83 File Offset: 0x00004D83
		[CompilerGenerated]
		public readonly bool Equals(AttemptShootEvent other)
		{
			return EqualityComparer<EntityUid>.Default.Equals(this.<User>k__BackingField, other.<User>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<Cancelled>k__BackingField, other.<Cancelled>k__BackingField);
		}

		// Token: 0x06000117 RID: 279 RVA: 0x00006BB5 File Offset: 0x00004DB5
		[CompilerGenerated]
		public readonly void Deconstruct(out EntityUid User, out bool Cancelled)
		{
			User = this.User;
			Cancelled = this.Cancelled;
		}
	}
}
