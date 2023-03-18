using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.GameObjects;

namespace Content.Shared.Anomaly.Components
{
	// Token: 0x02000712 RID: 1810
	[ByRefEvent]
	public readonly struct AnomalyHealthChangedEvent : IEquatable<AnomalyHealthChangedEvent>
	{
		// Token: 0x060015E4 RID: 5604 RVA: 0x00047B57 File Offset: 0x00045D57
		public AnomalyHealthChangedEvent(EntityUid Anomaly, float Health)
		{
			this.Anomaly = Anomaly;
			this.Health = Health;
		}

		// Token: 0x17000491 RID: 1169
		// (get) Token: 0x060015E5 RID: 5605 RVA: 0x00047B67 File Offset: 0x00045D67
		// (set) Token: 0x060015E6 RID: 5606 RVA: 0x00047B6F File Offset: 0x00045D6F
		public EntityUid Anomaly { get; set; }

		// Token: 0x17000492 RID: 1170
		// (get) Token: 0x060015E7 RID: 5607 RVA: 0x00047B78 File Offset: 0x00045D78
		// (set) Token: 0x060015E8 RID: 5608 RVA: 0x00047B80 File Offset: 0x00045D80
		public float Health { get; set; }

		// Token: 0x060015E9 RID: 5609 RVA: 0x00047B8C File Offset: 0x00045D8C
		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("AnomalyHealthChangedEvent");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x060015EA RID: 5610 RVA: 0x00047BD8 File Offset: 0x00045DD8
		[CompilerGenerated]
		private bool PrintMembers(StringBuilder builder)
		{
			builder.Append("Anomaly = ");
			builder.Append(this.Anomaly.ToString());
			builder.Append(", Health = ");
			builder.Append(this.Health.ToString());
			return true;
		}

		// Token: 0x060015EB RID: 5611 RVA: 0x00047C34 File Offset: 0x00045E34
		[CompilerGenerated]
		public static bool operator !=(AnomalyHealthChangedEvent left, AnomalyHealthChangedEvent right)
		{
			return !(left == right);
		}

		// Token: 0x060015EC RID: 5612 RVA: 0x00047C40 File Offset: 0x00045E40
		[CompilerGenerated]
		public static bool operator ==(AnomalyHealthChangedEvent left, AnomalyHealthChangedEvent right)
		{
			return left.Equals(right);
		}

		// Token: 0x060015ED RID: 5613 RVA: 0x00047C4A File Offset: 0x00045E4A
		[CompilerGenerated]
		public override int GetHashCode()
		{
			return EqualityComparer<EntityUid>.Default.GetHashCode(this.<Anomaly>k__BackingField) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.<Health>k__BackingField);
		}

		// Token: 0x060015EE RID: 5614 RVA: 0x00047C73 File Offset: 0x00045E73
		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			return obj is AnomalyHealthChangedEvent && this.Equals((AnomalyHealthChangedEvent)obj);
		}

		// Token: 0x060015EF RID: 5615 RVA: 0x00047C8B File Offset: 0x00045E8B
		[CompilerGenerated]
		public bool Equals(AnomalyHealthChangedEvent other)
		{
			return EqualityComparer<EntityUid>.Default.Equals(this.<Anomaly>k__BackingField, other.<Anomaly>k__BackingField) && EqualityComparer<float>.Default.Equals(this.<Health>k__BackingField, other.<Health>k__BackingField);
		}

		// Token: 0x060015F0 RID: 5616 RVA: 0x00047CBD File Offset: 0x00045EBD
		[CompilerGenerated]
		public void Deconstruct(out EntityUid Anomaly, out float Health)
		{
			Anomaly = this.Anomaly;
			Health = this.Health;
		}
	}
}
