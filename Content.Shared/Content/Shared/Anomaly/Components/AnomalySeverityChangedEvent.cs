using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.GameObjects;

namespace Content.Shared.Anomaly.Components
{
	// Token: 0x02000710 RID: 1808
	[ByRefEvent]
	public readonly struct AnomalySeverityChangedEvent : IEquatable<AnomalySeverityChangedEvent>
	{
		// Token: 0x060015CA RID: 5578 RVA: 0x0004785F File Offset: 0x00045A5F
		public AnomalySeverityChangedEvent(EntityUid Anomaly, float Severity)
		{
			this.Anomaly = Anomaly;
			this.Severity = Severity;
		}

		// Token: 0x1700048D RID: 1165
		// (get) Token: 0x060015CB RID: 5579 RVA: 0x0004786F File Offset: 0x00045A6F
		// (set) Token: 0x060015CC RID: 5580 RVA: 0x00047877 File Offset: 0x00045A77
		public EntityUid Anomaly { get; set; }

		// Token: 0x1700048E RID: 1166
		// (get) Token: 0x060015CD RID: 5581 RVA: 0x00047880 File Offset: 0x00045A80
		// (set) Token: 0x060015CE RID: 5582 RVA: 0x00047888 File Offset: 0x00045A88
		public float Severity { get; set; }

		// Token: 0x060015CF RID: 5583 RVA: 0x00047894 File Offset: 0x00045A94
		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("AnomalySeverityChangedEvent");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x060015D0 RID: 5584 RVA: 0x000478E0 File Offset: 0x00045AE0
		[CompilerGenerated]
		private bool PrintMembers(StringBuilder builder)
		{
			builder.Append("Anomaly = ");
			builder.Append(this.Anomaly.ToString());
			builder.Append(", Severity = ");
			builder.Append(this.Severity.ToString());
			return true;
		}

		// Token: 0x060015D1 RID: 5585 RVA: 0x0004793C File Offset: 0x00045B3C
		[CompilerGenerated]
		public static bool operator !=(AnomalySeverityChangedEvent left, AnomalySeverityChangedEvent right)
		{
			return !(left == right);
		}

		// Token: 0x060015D2 RID: 5586 RVA: 0x00047948 File Offset: 0x00045B48
		[CompilerGenerated]
		public static bool operator ==(AnomalySeverityChangedEvent left, AnomalySeverityChangedEvent right)
		{
			return left.Equals(right);
		}

		// Token: 0x060015D3 RID: 5587 RVA: 0x00047952 File Offset: 0x00045B52
		[CompilerGenerated]
		public override int GetHashCode()
		{
			return EqualityComparer<EntityUid>.Default.GetHashCode(this.<Anomaly>k__BackingField) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.<Severity>k__BackingField);
		}

		// Token: 0x060015D4 RID: 5588 RVA: 0x0004797B File Offset: 0x00045B7B
		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			return obj is AnomalySeverityChangedEvent && this.Equals((AnomalySeverityChangedEvent)obj);
		}

		// Token: 0x060015D5 RID: 5589 RVA: 0x00047993 File Offset: 0x00045B93
		[CompilerGenerated]
		public bool Equals(AnomalySeverityChangedEvent other)
		{
			return EqualityComparer<EntityUid>.Default.Equals(this.<Anomaly>k__BackingField, other.<Anomaly>k__BackingField) && EqualityComparer<float>.Default.Equals(this.<Severity>k__BackingField, other.<Severity>k__BackingField);
		}

		// Token: 0x060015D6 RID: 5590 RVA: 0x000479C5 File Offset: 0x00045BC5
		[CompilerGenerated]
		public void Deconstruct(out EntityUid Anomaly, out float Severity)
		{
			Anomaly = this.Anomaly;
			Severity = this.Severity;
		}
	}
}
