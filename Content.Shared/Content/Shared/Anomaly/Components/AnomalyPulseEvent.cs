using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.GameObjects;

namespace Content.Shared.Anomaly.Components
{
	// Token: 0x0200070D RID: 1805
	[ByRefEvent]
	public readonly struct AnomalyPulseEvent : IEquatable<AnomalyPulseEvent>
	{
		// Token: 0x060015A9 RID: 5545 RVA: 0x000474E8 File Offset: 0x000456E8
		public AnomalyPulseEvent(float Stability, float Severity)
		{
			this.Stability = Stability;
			this.Severity = Severity;
		}

		// Token: 0x17000489 RID: 1161
		// (get) Token: 0x060015AA RID: 5546 RVA: 0x000474F8 File Offset: 0x000456F8
		// (set) Token: 0x060015AB RID: 5547 RVA: 0x00047500 File Offset: 0x00045700
		public float Stability { get; set; }

		// Token: 0x1700048A RID: 1162
		// (get) Token: 0x060015AC RID: 5548 RVA: 0x00047509 File Offset: 0x00045709
		// (set) Token: 0x060015AD RID: 5549 RVA: 0x00047511 File Offset: 0x00045711
		public float Severity { get; set; }

		// Token: 0x060015AE RID: 5550 RVA: 0x0004751C File Offset: 0x0004571C
		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("AnomalyPulseEvent");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x060015AF RID: 5551 RVA: 0x00047568 File Offset: 0x00045768
		[CompilerGenerated]
		private bool PrintMembers(StringBuilder builder)
		{
			builder.Append("Stability = ");
			builder.Append(this.Stability.ToString());
			builder.Append(", Severity = ");
			builder.Append(this.Severity.ToString());
			return true;
		}

		// Token: 0x060015B0 RID: 5552 RVA: 0x000475C4 File Offset: 0x000457C4
		[CompilerGenerated]
		public static bool operator !=(AnomalyPulseEvent left, AnomalyPulseEvent right)
		{
			return !(left == right);
		}

		// Token: 0x060015B1 RID: 5553 RVA: 0x000475D0 File Offset: 0x000457D0
		[CompilerGenerated]
		public static bool operator ==(AnomalyPulseEvent left, AnomalyPulseEvent right)
		{
			return left.Equals(right);
		}

		// Token: 0x060015B2 RID: 5554 RVA: 0x000475DA File Offset: 0x000457DA
		[CompilerGenerated]
		public override int GetHashCode()
		{
			return EqualityComparer<float>.Default.GetHashCode(this.<Stability>k__BackingField) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.<Severity>k__BackingField);
		}

		// Token: 0x060015B3 RID: 5555 RVA: 0x00047603 File Offset: 0x00045803
		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			return obj is AnomalyPulseEvent && this.Equals((AnomalyPulseEvent)obj);
		}

		// Token: 0x060015B4 RID: 5556 RVA: 0x0004761B File Offset: 0x0004581B
		[CompilerGenerated]
		public bool Equals(AnomalyPulseEvent other)
		{
			return EqualityComparer<float>.Default.Equals(this.<Stability>k__BackingField, other.<Stability>k__BackingField) && EqualityComparer<float>.Default.Equals(this.<Severity>k__BackingField, other.<Severity>k__BackingField);
		}

		// Token: 0x060015B5 RID: 5557 RVA: 0x0004764D File Offset: 0x0004584D
		[CompilerGenerated]
		public void Deconstruct(out float Stability, out float Severity)
		{
			Stability = this.Stability;
			Severity = this.Severity;
		}
	}
}
