using System;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.GameObjects;

namespace Content.Server.Weapons.Melee.EnergySword
{
	// Token: 0x020000BA RID: 186
	[ByRefEvent]
	public readonly struct EnergySwordDeactivatedEvent : IEquatable<EnergySwordDeactivatedEvent>
	{
		// Token: 0x06000309 RID: 777 RVA: 0x00010860 File Offset: 0x0000EA60
		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("EnergySwordDeactivatedEvent");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x0600030A RID: 778 RVA: 0x000108AC File Offset: 0x0000EAAC
		[CompilerGenerated]
		private bool PrintMembers(StringBuilder builder)
		{
			return false;
		}

		// Token: 0x0600030B RID: 779 RVA: 0x000108AF File Offset: 0x0000EAAF
		[CompilerGenerated]
		public static bool operator !=(EnergySwordDeactivatedEvent left, EnergySwordDeactivatedEvent right)
		{
			return !(left == right);
		}

		// Token: 0x0600030C RID: 780 RVA: 0x000108BB File Offset: 0x0000EABB
		[CompilerGenerated]
		public static bool operator ==(EnergySwordDeactivatedEvent left, EnergySwordDeactivatedEvent right)
		{
			return left.Equals(right);
		}

		// Token: 0x0600030D RID: 781 RVA: 0x000108C5 File Offset: 0x0000EAC5
		[CompilerGenerated]
		public override int GetHashCode()
		{
			return 0;
		}

		// Token: 0x0600030E RID: 782 RVA: 0x000108C8 File Offset: 0x0000EAC8
		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			return obj is EnergySwordDeactivatedEvent && this.Equals((EnergySwordDeactivatedEvent)obj);
		}

		// Token: 0x0600030F RID: 783 RVA: 0x000108E0 File Offset: 0x0000EAE0
		[CompilerGenerated]
		public bool Equals(EnergySwordDeactivatedEvent other)
		{
			return true;
		}
	}
}
