using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.GameObjects;

namespace Content.Shared.Storage.Components
{
	// Token: 0x0200013B RID: 315
	[ByRefEvent]
	public struct StorageOpenAttemptEvent : IEquatable<StorageOpenAttemptEvent>
	{
		// Token: 0x060003A9 RID: 937 RVA: 0x0000F5DB File Offset: 0x0000D7DB
		public StorageOpenAttemptEvent(bool Silent, bool Cancelled = false)
		{
			this.Silent = Silent;
			this.Cancelled = Cancelled;
		}

		// Token: 0x170000B3 RID: 179
		// (get) Token: 0x060003AA RID: 938 RVA: 0x0000F5EB File Offset: 0x0000D7EB
		// (set) Token: 0x060003AB RID: 939 RVA: 0x0000F5F3 File Offset: 0x0000D7F3
		public bool Silent { readonly get; set; }

		// Token: 0x170000B4 RID: 180
		// (get) Token: 0x060003AC RID: 940 RVA: 0x0000F5FC File Offset: 0x0000D7FC
		// (set) Token: 0x060003AD RID: 941 RVA: 0x0000F604 File Offset: 0x0000D804
		public bool Cancelled { readonly get; set; }

		// Token: 0x060003AE RID: 942 RVA: 0x0000F610 File Offset: 0x0000D810
		[CompilerGenerated]
		public override readonly string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("StorageOpenAttemptEvent");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x060003AF RID: 943 RVA: 0x0000F65C File Offset: 0x0000D85C
		[CompilerGenerated]
		private readonly bool PrintMembers(StringBuilder builder)
		{
			builder.Append("Silent = ");
			builder.Append(this.Silent.ToString());
			builder.Append(", Cancelled = ");
			builder.Append(this.Cancelled.ToString());
			return true;
		}

		// Token: 0x060003B0 RID: 944 RVA: 0x0000F6B8 File Offset: 0x0000D8B8
		[CompilerGenerated]
		public static bool operator !=(StorageOpenAttemptEvent left, StorageOpenAttemptEvent right)
		{
			return !(left == right);
		}

		// Token: 0x060003B1 RID: 945 RVA: 0x0000F6C4 File Offset: 0x0000D8C4
		[CompilerGenerated]
		public static bool operator ==(StorageOpenAttemptEvent left, StorageOpenAttemptEvent right)
		{
			return left.Equals(right);
		}

		// Token: 0x060003B2 RID: 946 RVA: 0x0000F6CE File Offset: 0x0000D8CE
		[CompilerGenerated]
		public override readonly int GetHashCode()
		{
			return EqualityComparer<bool>.Default.GetHashCode(this.<Silent>k__BackingField) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<Cancelled>k__BackingField);
		}

		// Token: 0x060003B3 RID: 947 RVA: 0x0000F6F7 File Offset: 0x0000D8F7
		[CompilerGenerated]
		public override readonly bool Equals(object obj)
		{
			return obj is StorageOpenAttemptEvent && this.Equals((StorageOpenAttemptEvent)obj);
		}

		// Token: 0x060003B4 RID: 948 RVA: 0x0000F70F File Offset: 0x0000D90F
		[CompilerGenerated]
		public readonly bool Equals(StorageOpenAttemptEvent other)
		{
			return EqualityComparer<bool>.Default.Equals(this.<Silent>k__BackingField, other.<Silent>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<Cancelled>k__BackingField, other.<Cancelled>k__BackingField);
		}

		// Token: 0x060003B5 RID: 949 RVA: 0x0000F741 File Offset: 0x0000D941
		[CompilerGenerated]
		public readonly void Deconstruct(out bool Silent, out bool Cancelled)
		{
			Silent = this.Silent;
			Cancelled = this.Cancelled;
		}
	}
}
