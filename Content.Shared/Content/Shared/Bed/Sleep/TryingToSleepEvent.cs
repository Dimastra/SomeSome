using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.GameObjects;

namespace Content.Shared.Bed.Sleep
{
	// Token: 0x02000675 RID: 1653
	[ByRefEvent]
	public struct TryingToSleepEvent : IEquatable<TryingToSleepEvent>
	{
		// Token: 0x0600143C RID: 5180 RVA: 0x00043E35 File Offset: 0x00042035
		public TryingToSleepEvent(EntityUid uid, bool Cancelled = false)
		{
			this.uid = uid;
			this.Cancelled = Cancelled;
		}

		// Token: 0x17000407 RID: 1031
		// (get) Token: 0x0600143D RID: 5181 RVA: 0x00043E45 File Offset: 0x00042045
		// (set) Token: 0x0600143E RID: 5182 RVA: 0x00043E4D File Offset: 0x0004204D
		public EntityUid uid { readonly get; set; }

		// Token: 0x17000408 RID: 1032
		// (get) Token: 0x0600143F RID: 5183 RVA: 0x00043E56 File Offset: 0x00042056
		// (set) Token: 0x06001440 RID: 5184 RVA: 0x00043E5E File Offset: 0x0004205E
		public bool Cancelled { readonly get; set; }

		// Token: 0x06001441 RID: 5185 RVA: 0x00043E68 File Offset: 0x00042068
		[CompilerGenerated]
		public override readonly string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("TryingToSleepEvent");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x06001442 RID: 5186 RVA: 0x00043EB4 File Offset: 0x000420B4
		[CompilerGenerated]
		private readonly bool PrintMembers(StringBuilder builder)
		{
			builder.Append("uid = ");
			builder.Append(this.uid.ToString());
			builder.Append(", Cancelled = ");
			builder.Append(this.Cancelled.ToString());
			return true;
		}

		// Token: 0x06001443 RID: 5187 RVA: 0x00043F10 File Offset: 0x00042110
		[CompilerGenerated]
		public static bool operator !=(TryingToSleepEvent left, TryingToSleepEvent right)
		{
			return !(left == right);
		}

		// Token: 0x06001444 RID: 5188 RVA: 0x00043F1C File Offset: 0x0004211C
		[CompilerGenerated]
		public static bool operator ==(TryingToSleepEvent left, TryingToSleepEvent right)
		{
			return left.Equals(right);
		}

		// Token: 0x06001445 RID: 5189 RVA: 0x00043F26 File Offset: 0x00042126
		[CompilerGenerated]
		public override readonly int GetHashCode()
		{
			return EqualityComparer<EntityUid>.Default.GetHashCode(this.<uid>k__BackingField) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<Cancelled>k__BackingField);
		}

		// Token: 0x06001446 RID: 5190 RVA: 0x00043F4F File Offset: 0x0004214F
		[CompilerGenerated]
		public override readonly bool Equals(object obj)
		{
			return obj is TryingToSleepEvent && this.Equals((TryingToSleepEvent)obj);
		}

		// Token: 0x06001447 RID: 5191 RVA: 0x00043F67 File Offset: 0x00042167
		[CompilerGenerated]
		public readonly bool Equals(TryingToSleepEvent other)
		{
			return EqualityComparer<EntityUid>.Default.Equals(this.<uid>k__BackingField, other.<uid>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<Cancelled>k__BackingField, other.<Cancelled>k__BackingField);
		}

		// Token: 0x06001448 RID: 5192 RVA: 0x00043F99 File Offset: 0x00042199
		[CompilerGenerated]
		public readonly void Deconstruct(out EntityUid uid, out bool Cancelled)
		{
			uid = this.uid;
			Cancelled = this.Cancelled;
		}
	}
}
