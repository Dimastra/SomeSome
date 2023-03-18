using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.GameObjects;

namespace Content.Shared.Chemistry.Reaction
{
	// Token: 0x020005ED RID: 1517
	public readonly struct AfterMixingEvent : IEquatable<AfterMixingEvent>
	{
		// Token: 0x06001257 RID: 4695 RVA: 0x0003BEA3 File Offset: 0x0003A0A3
		public AfterMixingEvent(EntityUid Mixed, EntityUid Mixer)
		{
			this.Mixed = Mixed;
			this.Mixer = Mixer;
		}

		// Token: 0x170003AD RID: 941
		// (get) Token: 0x06001258 RID: 4696 RVA: 0x0003BEB3 File Offset: 0x0003A0B3
		// (set) Token: 0x06001259 RID: 4697 RVA: 0x0003BEBB File Offset: 0x0003A0BB
		public EntityUid Mixed { get; set; }

		// Token: 0x170003AE RID: 942
		// (get) Token: 0x0600125A RID: 4698 RVA: 0x0003BEC4 File Offset: 0x0003A0C4
		// (set) Token: 0x0600125B RID: 4699 RVA: 0x0003BECC File Offset: 0x0003A0CC
		public EntityUid Mixer { get; set; }

		// Token: 0x0600125C RID: 4700 RVA: 0x0003BED8 File Offset: 0x0003A0D8
		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("AfterMixingEvent");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x0600125D RID: 4701 RVA: 0x0003BF24 File Offset: 0x0003A124
		[CompilerGenerated]
		private bool PrintMembers(StringBuilder builder)
		{
			builder.Append("Mixed = ");
			builder.Append(this.Mixed.ToString());
			builder.Append(", Mixer = ");
			builder.Append(this.Mixer.ToString());
			return true;
		}

		// Token: 0x0600125E RID: 4702 RVA: 0x0003BF80 File Offset: 0x0003A180
		[CompilerGenerated]
		public static bool operator !=(AfterMixingEvent left, AfterMixingEvent right)
		{
			return !(left == right);
		}

		// Token: 0x0600125F RID: 4703 RVA: 0x0003BF8C File Offset: 0x0003A18C
		[CompilerGenerated]
		public static bool operator ==(AfterMixingEvent left, AfterMixingEvent right)
		{
			return left.Equals(right);
		}

		// Token: 0x06001260 RID: 4704 RVA: 0x0003BF96 File Offset: 0x0003A196
		[CompilerGenerated]
		public override int GetHashCode()
		{
			return EqualityComparer<EntityUid>.Default.GetHashCode(this.<Mixed>k__BackingField) * -1521134295 + EqualityComparer<EntityUid>.Default.GetHashCode(this.<Mixer>k__BackingField);
		}

		// Token: 0x06001261 RID: 4705 RVA: 0x0003BFBF File Offset: 0x0003A1BF
		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			return obj is AfterMixingEvent && this.Equals((AfterMixingEvent)obj);
		}

		// Token: 0x06001262 RID: 4706 RVA: 0x0003BFD7 File Offset: 0x0003A1D7
		[CompilerGenerated]
		public bool Equals(AfterMixingEvent other)
		{
			return EqualityComparer<EntityUid>.Default.Equals(this.<Mixed>k__BackingField, other.<Mixed>k__BackingField) && EqualityComparer<EntityUid>.Default.Equals(this.<Mixer>k__BackingField, other.<Mixer>k__BackingField);
		}

		// Token: 0x06001263 RID: 4707 RVA: 0x0003C009 File Offset: 0x0003A209
		[CompilerGenerated]
		public void Deconstruct(out EntityUid Mixed, out EntityUid Mixer)
		{
			Mixed = this.Mixed;
			Mixer = this.Mixer;
		}
	}
}
