using System;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.GameObjects;

namespace Content.Shared.Research.Components
{
	// Token: 0x02000215 RID: 533
	[ByRefEvent]
	public readonly struct TechnologyDatabaseModifiedEvent : IEquatable<TechnologyDatabaseModifiedEvent>
	{
		// Token: 0x060005F2 RID: 1522 RVA: 0x00015078 File Offset: 0x00013278
		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("TechnologyDatabaseModifiedEvent");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x060005F3 RID: 1523 RVA: 0x000150C4 File Offset: 0x000132C4
		[CompilerGenerated]
		private bool PrintMembers(StringBuilder builder)
		{
			return false;
		}

		// Token: 0x060005F4 RID: 1524 RVA: 0x000150C7 File Offset: 0x000132C7
		[CompilerGenerated]
		public static bool operator !=(TechnologyDatabaseModifiedEvent left, TechnologyDatabaseModifiedEvent right)
		{
			return !(left == right);
		}

		// Token: 0x060005F5 RID: 1525 RVA: 0x000150D3 File Offset: 0x000132D3
		[CompilerGenerated]
		public static bool operator ==(TechnologyDatabaseModifiedEvent left, TechnologyDatabaseModifiedEvent right)
		{
			return left.Equals(right);
		}

		// Token: 0x060005F6 RID: 1526 RVA: 0x000150DD File Offset: 0x000132DD
		[CompilerGenerated]
		public override int GetHashCode()
		{
			return 0;
		}

		// Token: 0x060005F7 RID: 1527 RVA: 0x000150E0 File Offset: 0x000132E0
		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			return obj is TechnologyDatabaseModifiedEvent && this.Equals((TechnologyDatabaseModifiedEvent)obj);
		}

		// Token: 0x060005F8 RID: 1528 RVA: 0x000150F8 File Offset: 0x000132F8
		[CompilerGenerated]
		public bool Equals(TechnologyDatabaseModifiedEvent other)
		{
			return true;
		}
	}
}
