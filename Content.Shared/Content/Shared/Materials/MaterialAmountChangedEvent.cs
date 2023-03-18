using System;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.GameObjects;

namespace Content.Shared.Materials
{
	// Token: 0x02000334 RID: 820
	[ByRefEvent]
	public readonly struct MaterialAmountChangedEvent : IEquatable<MaterialAmountChangedEvent>
	{
		// Token: 0x0600096F RID: 2415 RVA: 0x0001F860 File Offset: 0x0001DA60
		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("MaterialAmountChangedEvent");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x06000970 RID: 2416 RVA: 0x0001F8AC File Offset: 0x0001DAAC
		[CompilerGenerated]
		private bool PrintMembers(StringBuilder builder)
		{
			return false;
		}

		// Token: 0x06000971 RID: 2417 RVA: 0x0001F8AF File Offset: 0x0001DAAF
		[CompilerGenerated]
		public static bool operator !=(MaterialAmountChangedEvent left, MaterialAmountChangedEvent right)
		{
			return !(left == right);
		}

		// Token: 0x06000972 RID: 2418 RVA: 0x0001F8BB File Offset: 0x0001DABB
		[CompilerGenerated]
		public static bool operator ==(MaterialAmountChangedEvent left, MaterialAmountChangedEvent right)
		{
			return left.Equals(right);
		}

		// Token: 0x06000973 RID: 2419 RVA: 0x0001F8C5 File Offset: 0x0001DAC5
		[CompilerGenerated]
		public override int GetHashCode()
		{
			return 0;
		}

		// Token: 0x06000974 RID: 2420 RVA: 0x0001F8C8 File Offset: 0x0001DAC8
		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			return obj is MaterialAmountChangedEvent && this.Equals((MaterialAmountChangedEvent)obj);
		}

		// Token: 0x06000975 RID: 2421 RVA: 0x0001F8E0 File Offset: 0x0001DAE0
		[CompilerGenerated]
		public bool Equals(MaterialAmountChangedEvent other)
		{
			return true;
		}
	}
}
