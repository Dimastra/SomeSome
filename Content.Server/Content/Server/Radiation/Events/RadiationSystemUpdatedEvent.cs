using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Content.Server.Radiation.Events
{
	// Token: 0x02000266 RID: 614
	public struct RadiationSystemUpdatedEvent : IEquatable<RadiationSystemUpdatedEvent>
	{
		// Token: 0x06000C42 RID: 3138 RVA: 0x000407C0 File Offset: 0x0003E9C0
		[CompilerGenerated]
		public override readonly string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("RadiationSystemUpdatedEvent");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x06000C43 RID: 3139 RVA: 0x0004080C File Offset: 0x0003EA0C
		[CompilerGenerated]
		private readonly bool PrintMembers(StringBuilder builder)
		{
			return false;
		}

		// Token: 0x06000C44 RID: 3140 RVA: 0x0004080F File Offset: 0x0003EA0F
		[CompilerGenerated]
		public static bool operator !=(RadiationSystemUpdatedEvent left, RadiationSystemUpdatedEvent right)
		{
			return !(left == right);
		}

		// Token: 0x06000C45 RID: 3141 RVA: 0x0004081B File Offset: 0x0003EA1B
		[CompilerGenerated]
		public static bool operator ==(RadiationSystemUpdatedEvent left, RadiationSystemUpdatedEvent right)
		{
			return left.Equals(right);
		}

		// Token: 0x06000C46 RID: 3142 RVA: 0x00040825 File Offset: 0x0003EA25
		[CompilerGenerated]
		public override readonly int GetHashCode()
		{
			return 0;
		}

		// Token: 0x06000C47 RID: 3143 RVA: 0x00040828 File Offset: 0x0003EA28
		[CompilerGenerated]
		public override readonly bool Equals(object obj)
		{
			return obj is RadiationSystemUpdatedEvent && this.Equals((RadiationSystemUpdatedEvent)obj);
		}

		// Token: 0x06000C48 RID: 3144 RVA: 0x00040840 File Offset: 0x0003EA40
		[CompilerGenerated]
		public readonly bool Equals(RadiationSystemUpdatedEvent other)
		{
			return true;
		}
	}
}
