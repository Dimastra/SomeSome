using System;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.GameObjects;

namespace Content.Shared.Climbing.Events
{
	// Token: 0x020005C7 RID: 1479
	[ByRefEvent]
	public readonly struct EndClimbEvent : IEquatable<EndClimbEvent>
	{
		// Token: 0x060011EB RID: 4587 RVA: 0x0003ABDC File Offset: 0x00038DDC
		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("EndClimbEvent");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x060011EC RID: 4588 RVA: 0x0003AC28 File Offset: 0x00038E28
		[CompilerGenerated]
		private bool PrintMembers(StringBuilder builder)
		{
			return false;
		}

		// Token: 0x060011ED RID: 4589 RVA: 0x0003AC2B File Offset: 0x00038E2B
		[CompilerGenerated]
		public static bool operator !=(EndClimbEvent left, EndClimbEvent right)
		{
			return !(left == right);
		}

		// Token: 0x060011EE RID: 4590 RVA: 0x0003AC37 File Offset: 0x00038E37
		[CompilerGenerated]
		public static bool operator ==(EndClimbEvent left, EndClimbEvent right)
		{
			return left.Equals(right);
		}

		// Token: 0x060011EF RID: 4591 RVA: 0x0003AC41 File Offset: 0x00038E41
		[CompilerGenerated]
		public override int GetHashCode()
		{
			return 0;
		}

		// Token: 0x060011F0 RID: 4592 RVA: 0x0003AC44 File Offset: 0x00038E44
		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			return obj is EndClimbEvent && this.Equals((EndClimbEvent)obj);
		}

		// Token: 0x060011F1 RID: 4593 RVA: 0x0003AC5C File Offset: 0x00038E5C
		[CompilerGenerated]
		public bool Equals(EndClimbEvent other)
		{
			return true;
		}
	}
}
