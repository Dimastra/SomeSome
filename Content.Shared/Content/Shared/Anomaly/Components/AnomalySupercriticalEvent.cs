using System;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.GameObjects;

namespace Content.Shared.Anomaly.Components
{
	// Token: 0x0200070E RID: 1806
	[ByRefEvent]
	public readonly struct AnomalySupercriticalEvent : IEquatable<AnomalySupercriticalEvent>
	{
		// Token: 0x060015B6 RID: 5558 RVA: 0x00047660 File Offset: 0x00045860
		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("AnomalySupercriticalEvent");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x060015B7 RID: 5559 RVA: 0x000476AC File Offset: 0x000458AC
		[CompilerGenerated]
		private bool PrintMembers(StringBuilder builder)
		{
			return false;
		}

		// Token: 0x060015B8 RID: 5560 RVA: 0x000476AF File Offset: 0x000458AF
		[CompilerGenerated]
		public static bool operator !=(AnomalySupercriticalEvent left, AnomalySupercriticalEvent right)
		{
			return !(left == right);
		}

		// Token: 0x060015B9 RID: 5561 RVA: 0x000476BB File Offset: 0x000458BB
		[CompilerGenerated]
		public static bool operator ==(AnomalySupercriticalEvent left, AnomalySupercriticalEvent right)
		{
			return left.Equals(right);
		}

		// Token: 0x060015BA RID: 5562 RVA: 0x000476C5 File Offset: 0x000458C5
		[CompilerGenerated]
		public override int GetHashCode()
		{
			return 0;
		}

		// Token: 0x060015BB RID: 5563 RVA: 0x000476C8 File Offset: 0x000458C8
		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			return obj is AnomalySupercriticalEvent && this.Equals((AnomalySupercriticalEvent)obj);
		}

		// Token: 0x060015BC RID: 5564 RVA: 0x000476E0 File Offset: 0x000458E0
		[CompilerGenerated]
		public bool Equals(AnomalySupercriticalEvent other)
		{
			return true;
		}
	}
}
