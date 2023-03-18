using System;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.GameObjects;

namespace Content.Server.Store.Components
{
	// Token: 0x0200015C RID: 348
	[ByRefEvent]
	public readonly struct StoreRemovedEvent : IEquatable<StoreRemovedEvent>
	{
		// Token: 0x0600069F RID: 1695 RVA: 0x00020098 File Offset: 0x0001E298
		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("StoreRemovedEvent");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x060006A0 RID: 1696 RVA: 0x000200E4 File Offset: 0x0001E2E4
		[CompilerGenerated]
		private bool PrintMembers(StringBuilder builder)
		{
			return false;
		}

		// Token: 0x060006A1 RID: 1697 RVA: 0x000200E7 File Offset: 0x0001E2E7
		[CompilerGenerated]
		public static bool operator !=(StoreRemovedEvent left, StoreRemovedEvent right)
		{
			return !(left == right);
		}

		// Token: 0x060006A2 RID: 1698 RVA: 0x000200F3 File Offset: 0x0001E2F3
		[CompilerGenerated]
		public static bool operator ==(StoreRemovedEvent left, StoreRemovedEvent right)
		{
			return left.Equals(right);
		}

		// Token: 0x060006A3 RID: 1699 RVA: 0x000200FD File Offset: 0x0001E2FD
		[CompilerGenerated]
		public override int GetHashCode()
		{
			return 0;
		}

		// Token: 0x060006A4 RID: 1700 RVA: 0x00020100 File Offset: 0x0001E300
		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			return obj is StoreRemovedEvent && this.Equals((StoreRemovedEvent)obj);
		}

		// Token: 0x060006A5 RID: 1701 RVA: 0x00020118 File Offset: 0x0001E318
		[CompilerGenerated]
		public bool Equals(StoreRemovedEvent other)
		{
			return true;
		}
	}
}
