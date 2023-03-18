using System;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.GameObjects;

namespace Content.Shared.Storage.Components
{
	// Token: 0x0200013C RID: 316
	[ByRefEvent]
	public readonly struct StorageBeforeOpenEvent : IEquatable<StorageBeforeOpenEvent>
	{
		// Token: 0x060003B6 RID: 950 RVA: 0x0000F754 File Offset: 0x0000D954
		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("StorageBeforeOpenEvent");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x060003B7 RID: 951 RVA: 0x0000F7A0 File Offset: 0x0000D9A0
		[CompilerGenerated]
		private bool PrintMembers(StringBuilder builder)
		{
			return false;
		}

		// Token: 0x060003B8 RID: 952 RVA: 0x0000F7A3 File Offset: 0x0000D9A3
		[CompilerGenerated]
		public static bool operator !=(StorageBeforeOpenEvent left, StorageBeforeOpenEvent right)
		{
			return !(left == right);
		}

		// Token: 0x060003B9 RID: 953 RVA: 0x0000F7AF File Offset: 0x0000D9AF
		[CompilerGenerated]
		public static bool operator ==(StorageBeforeOpenEvent left, StorageBeforeOpenEvent right)
		{
			return left.Equals(right);
		}

		// Token: 0x060003BA RID: 954 RVA: 0x0000F7B9 File Offset: 0x0000D9B9
		[CompilerGenerated]
		public override int GetHashCode()
		{
			return 0;
		}

		// Token: 0x060003BB RID: 955 RVA: 0x0000F7BC File Offset: 0x0000D9BC
		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			return obj is StorageBeforeOpenEvent && this.Equals((StorageBeforeOpenEvent)obj);
		}

		// Token: 0x060003BC RID: 956 RVA: 0x0000F7D4 File Offset: 0x0000D9D4
		[CompilerGenerated]
		public bool Equals(StorageBeforeOpenEvent other)
		{
			return true;
		}
	}
}
