using System;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.GameObjects;

namespace Content.Shared.Storage.Components
{
	// Token: 0x0200013D RID: 317
	[ByRefEvent]
	public readonly struct StorageAfterOpenEvent : IEquatable<StorageAfterOpenEvent>
	{
		// Token: 0x060003BD RID: 957 RVA: 0x0000F7D8 File Offset: 0x0000D9D8
		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("StorageAfterOpenEvent");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x060003BE RID: 958 RVA: 0x0000F824 File Offset: 0x0000DA24
		[CompilerGenerated]
		private bool PrintMembers(StringBuilder builder)
		{
			return false;
		}

		// Token: 0x060003BF RID: 959 RVA: 0x0000F827 File Offset: 0x0000DA27
		[CompilerGenerated]
		public static bool operator !=(StorageAfterOpenEvent left, StorageAfterOpenEvent right)
		{
			return !(left == right);
		}

		// Token: 0x060003C0 RID: 960 RVA: 0x0000F833 File Offset: 0x0000DA33
		[CompilerGenerated]
		public static bool operator ==(StorageAfterOpenEvent left, StorageAfterOpenEvent right)
		{
			return left.Equals(right);
		}

		// Token: 0x060003C1 RID: 961 RVA: 0x0000F83D File Offset: 0x0000DA3D
		[CompilerGenerated]
		public override int GetHashCode()
		{
			return 0;
		}

		// Token: 0x060003C2 RID: 962 RVA: 0x0000F840 File Offset: 0x0000DA40
		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			return obj is StorageAfterOpenEvent && this.Equals((StorageAfterOpenEvent)obj);
		}

		// Token: 0x060003C3 RID: 963 RVA: 0x0000F858 File Offset: 0x0000DA58
		[CompilerGenerated]
		public bool Equals(StorageAfterOpenEvent other)
		{
			return true;
		}
	}
}
