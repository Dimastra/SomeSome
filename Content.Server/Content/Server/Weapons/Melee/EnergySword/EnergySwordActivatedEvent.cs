using System;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.GameObjects;

namespace Content.Server.Weapons.Melee.EnergySword
{
	// Token: 0x020000B9 RID: 185
	[ByRefEvent]
	public readonly struct EnergySwordActivatedEvent : IEquatable<EnergySwordActivatedEvent>
	{
		// Token: 0x06000301 RID: 769 RVA: 0x000107D8 File Offset: 0x0000E9D8
		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("EnergySwordActivatedEvent");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x06000302 RID: 770 RVA: 0x00010824 File Offset: 0x0000EA24
		[CompilerGenerated]
		private bool PrintMembers(StringBuilder builder)
		{
			return false;
		}

		// Token: 0x06000303 RID: 771 RVA: 0x00010827 File Offset: 0x0000EA27
		[CompilerGenerated]
		public static bool operator !=(EnergySwordActivatedEvent left, EnergySwordActivatedEvent right)
		{
			return !(left == right);
		}

		// Token: 0x06000304 RID: 772 RVA: 0x00010833 File Offset: 0x0000EA33
		[CompilerGenerated]
		public static bool operator ==(EnergySwordActivatedEvent left, EnergySwordActivatedEvent right)
		{
			return left.Equals(right);
		}

		// Token: 0x06000305 RID: 773 RVA: 0x0001083D File Offset: 0x0000EA3D
		[CompilerGenerated]
		public override int GetHashCode()
		{
			return 0;
		}

		// Token: 0x06000306 RID: 774 RVA: 0x00010840 File Offset: 0x0000EA40
		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			return obj is EnergySwordActivatedEvent && this.Equals((EnergySwordActivatedEvent)obj);
		}

		// Token: 0x06000307 RID: 775 RVA: 0x00010858 File Offset: 0x0000EA58
		[CompilerGenerated]
		public bool Equals(EnergySwordActivatedEvent other)
		{
			return true;
		}
	}
}
