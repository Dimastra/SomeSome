using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.GameObjects;

namespace Content.Shared.Mech
{
	// Token: 0x02000320 RID: 800
	[ByRefEvent]
	public struct AttemptRemoveMechEquipmentEvent : IEquatable<AttemptRemoveMechEquipmentEvent>
	{
		// Token: 0x0600092A RID: 2346 RVA: 0x0001EA05 File Offset: 0x0001CC05
		public AttemptRemoveMechEquipmentEvent()
		{
			this.Cancelled = false;
		}

		// Token: 0x0600092B RID: 2347 RVA: 0x0001EA10 File Offset: 0x0001CC10
		[CompilerGenerated]
		public override readonly string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("AttemptRemoveMechEquipmentEvent");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x0600092C RID: 2348 RVA: 0x0001EA5C File Offset: 0x0001CC5C
		[CompilerGenerated]
		private readonly bool PrintMembers(StringBuilder builder)
		{
			builder.Append("Cancelled = ");
			builder.Append(this.Cancelled.ToString());
			return true;
		}

		// Token: 0x0600092D RID: 2349 RVA: 0x0001EA83 File Offset: 0x0001CC83
		[CompilerGenerated]
		public static bool operator !=(AttemptRemoveMechEquipmentEvent left, AttemptRemoveMechEquipmentEvent right)
		{
			return !(left == right);
		}

		// Token: 0x0600092E RID: 2350 RVA: 0x0001EA8F File Offset: 0x0001CC8F
		[CompilerGenerated]
		public static bool operator ==(AttemptRemoveMechEquipmentEvent left, AttemptRemoveMechEquipmentEvent right)
		{
			return left.Equals(right);
		}

		// Token: 0x0600092F RID: 2351 RVA: 0x0001EA99 File Offset: 0x0001CC99
		[CompilerGenerated]
		public override readonly int GetHashCode()
		{
			return EqualityComparer<bool>.Default.GetHashCode(this.Cancelled);
		}

		// Token: 0x06000930 RID: 2352 RVA: 0x0001EAAB File Offset: 0x0001CCAB
		[CompilerGenerated]
		public override readonly bool Equals(object obj)
		{
			return obj is AttemptRemoveMechEquipmentEvent && this.Equals((AttemptRemoveMechEquipmentEvent)obj);
		}

		// Token: 0x06000931 RID: 2353 RVA: 0x0001EAC3 File Offset: 0x0001CCC3
		[CompilerGenerated]
		public readonly bool Equals(AttemptRemoveMechEquipmentEvent other)
		{
			return EqualityComparer<bool>.Default.Equals(this.Cancelled, other.Cancelled);
		}

		// Token: 0x04000917 RID: 2327
		public bool Cancelled;
	}
}
