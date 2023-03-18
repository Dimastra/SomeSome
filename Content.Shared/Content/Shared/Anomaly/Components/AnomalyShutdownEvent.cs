using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.GameObjects;

namespace Content.Shared.Anomaly.Components
{
	// Token: 0x0200070F RID: 1807
	[ByRefEvent]
	public readonly struct AnomalyShutdownEvent : IEquatable<AnomalyShutdownEvent>
	{
		// Token: 0x060015BD RID: 5565 RVA: 0x000476E3 File Offset: 0x000458E3
		public AnomalyShutdownEvent(EntityUid Anomaly, bool Supercritical)
		{
			this.Anomaly = Anomaly;
			this.Supercritical = Supercritical;
		}

		// Token: 0x1700048B RID: 1163
		// (get) Token: 0x060015BE RID: 5566 RVA: 0x000476F3 File Offset: 0x000458F3
		// (set) Token: 0x060015BF RID: 5567 RVA: 0x000476FB File Offset: 0x000458FB
		public EntityUid Anomaly { get; set; }

		// Token: 0x1700048C RID: 1164
		// (get) Token: 0x060015C0 RID: 5568 RVA: 0x00047704 File Offset: 0x00045904
		// (set) Token: 0x060015C1 RID: 5569 RVA: 0x0004770C File Offset: 0x0004590C
		public bool Supercritical { get; set; }

		// Token: 0x060015C2 RID: 5570 RVA: 0x00047718 File Offset: 0x00045918
		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("AnomalyShutdownEvent");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x060015C3 RID: 5571 RVA: 0x00047764 File Offset: 0x00045964
		[CompilerGenerated]
		private bool PrintMembers(StringBuilder builder)
		{
			builder.Append("Anomaly = ");
			builder.Append(this.Anomaly.ToString());
			builder.Append(", Supercritical = ");
			builder.Append(this.Supercritical.ToString());
			return true;
		}

		// Token: 0x060015C4 RID: 5572 RVA: 0x000477C0 File Offset: 0x000459C0
		[CompilerGenerated]
		public static bool operator !=(AnomalyShutdownEvent left, AnomalyShutdownEvent right)
		{
			return !(left == right);
		}

		// Token: 0x060015C5 RID: 5573 RVA: 0x000477CC File Offset: 0x000459CC
		[CompilerGenerated]
		public static bool operator ==(AnomalyShutdownEvent left, AnomalyShutdownEvent right)
		{
			return left.Equals(right);
		}

		// Token: 0x060015C6 RID: 5574 RVA: 0x000477D6 File Offset: 0x000459D6
		[CompilerGenerated]
		public override int GetHashCode()
		{
			return EqualityComparer<EntityUid>.Default.GetHashCode(this.<Anomaly>k__BackingField) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<Supercritical>k__BackingField);
		}

		// Token: 0x060015C7 RID: 5575 RVA: 0x000477FF File Offset: 0x000459FF
		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			return obj is AnomalyShutdownEvent && this.Equals((AnomalyShutdownEvent)obj);
		}

		// Token: 0x060015C8 RID: 5576 RVA: 0x00047817 File Offset: 0x00045A17
		[CompilerGenerated]
		public bool Equals(AnomalyShutdownEvent other)
		{
			return EqualityComparer<EntityUid>.Default.Equals(this.<Anomaly>k__BackingField, other.<Anomaly>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<Supercritical>k__BackingField, other.<Supercritical>k__BackingField);
		}

		// Token: 0x060015C9 RID: 5577 RVA: 0x00047849 File Offset: 0x00045A49
		[CompilerGenerated]
		public void Deconstruct(out EntityUid Anomaly, out bool Supercritical)
		{
			Anomaly = this.Anomaly;
			Supercritical = this.Supercritical;
		}
	}
}
