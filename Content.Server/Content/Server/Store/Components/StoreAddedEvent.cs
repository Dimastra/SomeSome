using System;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.GameObjects;

namespace Content.Server.Store.Components
{
	// Token: 0x0200015B RID: 347
	[ByRefEvent]
	public readonly struct StoreAddedEvent : IEquatable<StoreAddedEvent>
	{
		// Token: 0x06000698 RID: 1688 RVA: 0x00020014 File Offset: 0x0001E214
		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("StoreAddedEvent");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x06000699 RID: 1689 RVA: 0x00020060 File Offset: 0x0001E260
		[CompilerGenerated]
		private bool PrintMembers(StringBuilder builder)
		{
			return false;
		}

		// Token: 0x0600069A RID: 1690 RVA: 0x00020063 File Offset: 0x0001E263
		[CompilerGenerated]
		public static bool operator !=(StoreAddedEvent left, StoreAddedEvent right)
		{
			return !(left == right);
		}

		// Token: 0x0600069B RID: 1691 RVA: 0x0002006F File Offset: 0x0001E26F
		[CompilerGenerated]
		public static bool operator ==(StoreAddedEvent left, StoreAddedEvent right)
		{
			return left.Equals(right);
		}

		// Token: 0x0600069C RID: 1692 RVA: 0x00020079 File Offset: 0x0001E279
		[CompilerGenerated]
		public override int GetHashCode()
		{
			return 0;
		}

		// Token: 0x0600069D RID: 1693 RVA: 0x0002007C File Offset: 0x0001E27C
		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			return obj is StoreAddedEvent && this.Equals((StoreAddedEvent)obj);
		}

		// Token: 0x0600069E RID: 1694 RVA: 0x00020094 File Offset: 0x0001E294
		[CompilerGenerated]
		public bool Equals(StoreAddedEvent other)
		{
			return true;
		}
	}
}
