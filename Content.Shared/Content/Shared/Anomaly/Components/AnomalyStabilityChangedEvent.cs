using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.GameObjects;

namespace Content.Shared.Anomaly.Components
{
	// Token: 0x02000711 RID: 1809
	[ByRefEvent]
	public readonly struct AnomalyStabilityChangedEvent : IEquatable<AnomalyStabilityChangedEvent>
	{
		// Token: 0x060015D7 RID: 5591 RVA: 0x000479DB File Offset: 0x00045BDB
		public AnomalyStabilityChangedEvent(EntityUid Anomaly, float Stability)
		{
			this.Anomaly = Anomaly;
			this.Stability = Stability;
		}

		// Token: 0x1700048F RID: 1167
		// (get) Token: 0x060015D8 RID: 5592 RVA: 0x000479EB File Offset: 0x00045BEB
		// (set) Token: 0x060015D9 RID: 5593 RVA: 0x000479F3 File Offset: 0x00045BF3
		public EntityUid Anomaly { get; set; }

		// Token: 0x17000490 RID: 1168
		// (get) Token: 0x060015DA RID: 5594 RVA: 0x000479FC File Offset: 0x00045BFC
		// (set) Token: 0x060015DB RID: 5595 RVA: 0x00047A04 File Offset: 0x00045C04
		public float Stability { get; set; }

		// Token: 0x060015DC RID: 5596 RVA: 0x00047A10 File Offset: 0x00045C10
		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("AnomalyStabilityChangedEvent");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x060015DD RID: 5597 RVA: 0x00047A5C File Offset: 0x00045C5C
		[CompilerGenerated]
		private bool PrintMembers(StringBuilder builder)
		{
			builder.Append("Anomaly = ");
			builder.Append(this.Anomaly.ToString());
			builder.Append(", Stability = ");
			builder.Append(this.Stability.ToString());
			return true;
		}

		// Token: 0x060015DE RID: 5598 RVA: 0x00047AB8 File Offset: 0x00045CB8
		[CompilerGenerated]
		public static bool operator !=(AnomalyStabilityChangedEvent left, AnomalyStabilityChangedEvent right)
		{
			return !(left == right);
		}

		// Token: 0x060015DF RID: 5599 RVA: 0x00047AC4 File Offset: 0x00045CC4
		[CompilerGenerated]
		public static bool operator ==(AnomalyStabilityChangedEvent left, AnomalyStabilityChangedEvent right)
		{
			return left.Equals(right);
		}

		// Token: 0x060015E0 RID: 5600 RVA: 0x00047ACE File Offset: 0x00045CCE
		[CompilerGenerated]
		public override int GetHashCode()
		{
			return EqualityComparer<EntityUid>.Default.GetHashCode(this.<Anomaly>k__BackingField) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.<Stability>k__BackingField);
		}

		// Token: 0x060015E1 RID: 5601 RVA: 0x00047AF7 File Offset: 0x00045CF7
		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			return obj is AnomalyStabilityChangedEvent && this.Equals((AnomalyStabilityChangedEvent)obj);
		}

		// Token: 0x060015E2 RID: 5602 RVA: 0x00047B0F File Offset: 0x00045D0F
		[CompilerGenerated]
		public bool Equals(AnomalyStabilityChangedEvent other)
		{
			return EqualityComparer<EntityUid>.Default.Equals(this.<Anomaly>k__BackingField, other.<Anomaly>k__BackingField) && EqualityComparer<float>.Default.Equals(this.<Stability>k__BackingField, other.<Stability>k__BackingField);
		}

		// Token: 0x060015E3 RID: 5603 RVA: 0x00047B41 File Offset: 0x00045D41
		[CompilerGenerated]
		public void Deconstruct(out EntityUid Anomaly, out float Stability)
		{
			Anomaly = this.Anomaly;
			Stability = this.Stability;
		}
	}
}
