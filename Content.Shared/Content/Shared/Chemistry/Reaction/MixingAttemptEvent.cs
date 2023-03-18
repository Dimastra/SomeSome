using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.GameObjects;

namespace Content.Shared.Chemistry.Reaction
{
	// Token: 0x020005EC RID: 1516
	[ByRefEvent]
	public struct MixingAttemptEvent : IEquatable<MixingAttemptEvent>
	{
		// Token: 0x0600124A RID: 4682 RVA: 0x0003BD27 File Offset: 0x00039F27
		public MixingAttemptEvent(EntityUid Mixed, bool Cancelled = false)
		{
			this.Mixed = Mixed;
			this.Cancelled = Cancelled;
		}

		// Token: 0x170003AB RID: 939
		// (get) Token: 0x0600124B RID: 4683 RVA: 0x0003BD37 File Offset: 0x00039F37
		// (set) Token: 0x0600124C RID: 4684 RVA: 0x0003BD3F File Offset: 0x00039F3F
		public EntityUid Mixed { readonly get; set; }

		// Token: 0x170003AC RID: 940
		// (get) Token: 0x0600124D RID: 4685 RVA: 0x0003BD48 File Offset: 0x00039F48
		// (set) Token: 0x0600124E RID: 4686 RVA: 0x0003BD50 File Offset: 0x00039F50
		public bool Cancelled { readonly get; set; }

		// Token: 0x0600124F RID: 4687 RVA: 0x0003BD5C File Offset: 0x00039F5C
		[CompilerGenerated]
		public override readonly string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("MixingAttemptEvent");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x06001250 RID: 4688 RVA: 0x0003BDA8 File Offset: 0x00039FA8
		[CompilerGenerated]
		private readonly bool PrintMembers(StringBuilder builder)
		{
			builder.Append("Mixed = ");
			builder.Append(this.Mixed.ToString());
			builder.Append(", Cancelled = ");
			builder.Append(this.Cancelled.ToString());
			return true;
		}

		// Token: 0x06001251 RID: 4689 RVA: 0x0003BE04 File Offset: 0x0003A004
		[CompilerGenerated]
		public static bool operator !=(MixingAttemptEvent left, MixingAttemptEvent right)
		{
			return !(left == right);
		}

		// Token: 0x06001252 RID: 4690 RVA: 0x0003BE10 File Offset: 0x0003A010
		[CompilerGenerated]
		public static bool operator ==(MixingAttemptEvent left, MixingAttemptEvent right)
		{
			return left.Equals(right);
		}

		// Token: 0x06001253 RID: 4691 RVA: 0x0003BE1A File Offset: 0x0003A01A
		[CompilerGenerated]
		public override readonly int GetHashCode()
		{
			return EqualityComparer<EntityUid>.Default.GetHashCode(this.<Mixed>k__BackingField) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<Cancelled>k__BackingField);
		}

		// Token: 0x06001254 RID: 4692 RVA: 0x0003BE43 File Offset: 0x0003A043
		[CompilerGenerated]
		public override readonly bool Equals(object obj)
		{
			return obj is MixingAttemptEvent && this.Equals((MixingAttemptEvent)obj);
		}

		// Token: 0x06001255 RID: 4693 RVA: 0x0003BE5B File Offset: 0x0003A05B
		[CompilerGenerated]
		public readonly bool Equals(MixingAttemptEvent other)
		{
			return EqualityComparer<EntityUid>.Default.Equals(this.<Mixed>k__BackingField, other.<Mixed>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<Cancelled>k__BackingField, other.<Cancelled>k__BackingField);
		}

		// Token: 0x06001256 RID: 4694 RVA: 0x0003BE8D File Offset: 0x0003A08D
		[CompilerGenerated]
		public readonly void Deconstruct(out EntityUid Mixed, out bool Cancelled)
		{
			Mixed = this.Mixed;
			Cancelled = this.Cancelled;
		}
	}
}
