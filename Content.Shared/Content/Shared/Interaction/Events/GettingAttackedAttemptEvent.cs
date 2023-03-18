using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.GameObjects;

namespace Content.Shared.Interaction.Events
{
	// Token: 0x020003D6 RID: 982
	[ByRefEvent]
	public struct GettingAttackedAttemptEvent : IEquatable<GettingAttackedAttemptEvent>
	{
		// Token: 0x06000B7F RID: 2943 RVA: 0x00026205 File Offset: 0x00024405
		public GettingAttackedAttemptEvent(bool Cancelled)
		{
			this.Cancelled = Cancelled;
		}

		// Token: 0x17000240 RID: 576
		// (get) Token: 0x06000B80 RID: 2944 RVA: 0x0002620E File Offset: 0x0002440E
		// (set) Token: 0x06000B81 RID: 2945 RVA: 0x00026216 File Offset: 0x00024416
		public bool Cancelled { readonly get; set; }

		// Token: 0x06000B82 RID: 2946 RVA: 0x00026220 File Offset: 0x00024420
		[CompilerGenerated]
		public override readonly string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("GettingAttackedAttemptEvent");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x06000B83 RID: 2947 RVA: 0x0002626C File Offset: 0x0002446C
		[CompilerGenerated]
		private readonly bool PrintMembers(StringBuilder builder)
		{
			builder.Append("Cancelled = ");
			builder.Append(this.Cancelled.ToString());
			return true;
		}

		// Token: 0x06000B84 RID: 2948 RVA: 0x000262A1 File Offset: 0x000244A1
		[CompilerGenerated]
		public static bool operator !=(GettingAttackedAttemptEvent left, GettingAttackedAttemptEvent right)
		{
			return !(left == right);
		}

		// Token: 0x06000B85 RID: 2949 RVA: 0x000262AD File Offset: 0x000244AD
		[CompilerGenerated]
		public static bool operator ==(GettingAttackedAttemptEvent left, GettingAttackedAttemptEvent right)
		{
			return left.Equals(right);
		}

		// Token: 0x06000B86 RID: 2950 RVA: 0x000262B7 File Offset: 0x000244B7
		[CompilerGenerated]
		public override readonly int GetHashCode()
		{
			return EqualityComparer<bool>.Default.GetHashCode(this.<Cancelled>k__BackingField);
		}

		// Token: 0x06000B87 RID: 2951 RVA: 0x000262C9 File Offset: 0x000244C9
		[CompilerGenerated]
		public override readonly bool Equals(object obj)
		{
			return obj is GettingAttackedAttemptEvent && this.Equals((GettingAttackedAttemptEvent)obj);
		}

		// Token: 0x06000B88 RID: 2952 RVA: 0x000262E1 File Offset: 0x000244E1
		[CompilerGenerated]
		public readonly bool Equals(GettingAttackedAttemptEvent other)
		{
			return EqualityComparer<bool>.Default.Equals(this.<Cancelled>k__BackingField, other.<Cancelled>k__BackingField);
		}

		// Token: 0x06000B89 RID: 2953 RVA: 0x000262F9 File Offset: 0x000244F9
		[CompilerGenerated]
		public readonly void Deconstruct(out bool Cancelled)
		{
			Cancelled = this.Cancelled;
		}
	}
}
