using System;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.GameObjects;

namespace Content.Shared.Storage.Components
{
	// Token: 0x02000140 RID: 320
	[ByRefEvent]
	public readonly struct StorageAfterCloseEvent : IEquatable<StorageAfterCloseEvent>
	{
		// Token: 0x060003DC RID: 988 RVA: 0x0000FAAC File Offset: 0x0000DCAC
		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("StorageAfterCloseEvent");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x060003DD RID: 989 RVA: 0x0000FAF8 File Offset: 0x0000DCF8
		[CompilerGenerated]
		private bool PrintMembers(StringBuilder builder)
		{
			return false;
		}

		// Token: 0x060003DE RID: 990 RVA: 0x0000FAFB File Offset: 0x0000DCFB
		[CompilerGenerated]
		public static bool operator !=(StorageAfterCloseEvent left, StorageAfterCloseEvent right)
		{
			return !(left == right);
		}

		// Token: 0x060003DF RID: 991 RVA: 0x0000FB07 File Offset: 0x0000DD07
		[CompilerGenerated]
		public static bool operator ==(StorageAfterCloseEvent left, StorageAfterCloseEvent right)
		{
			return left.Equals(right);
		}

		// Token: 0x060003E0 RID: 992 RVA: 0x0000FB11 File Offset: 0x0000DD11
		[CompilerGenerated]
		public override int GetHashCode()
		{
			return 0;
		}

		// Token: 0x060003E1 RID: 993 RVA: 0x0000FB14 File Offset: 0x0000DD14
		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			return obj is StorageAfterCloseEvent && this.Equals((StorageAfterCloseEvent)obj);
		}

		// Token: 0x060003E2 RID: 994 RVA: 0x0000FB2C File Offset: 0x0000DD2C
		[CompilerGenerated]
		public bool Equals(StorageAfterCloseEvent other)
		{
			return true;
		}
	}
}
