using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.GameObjects;

namespace Content.Shared.Storage.Components
{
	// Token: 0x0200013E RID: 318
	[ByRefEvent]
	public struct StorageCloseAttemptEvent : IEquatable<StorageCloseAttemptEvent>
	{
		// Token: 0x060003C4 RID: 964 RVA: 0x0000F85B File Offset: 0x0000DA5B
		public StorageCloseAttemptEvent(bool Cancelled = false)
		{
			this.Cancelled = Cancelled;
		}

		// Token: 0x170000B5 RID: 181
		// (get) Token: 0x060003C5 RID: 965 RVA: 0x0000F864 File Offset: 0x0000DA64
		// (set) Token: 0x060003C6 RID: 966 RVA: 0x0000F86C File Offset: 0x0000DA6C
		public bool Cancelled { readonly get; set; }

		// Token: 0x060003C7 RID: 967 RVA: 0x0000F878 File Offset: 0x0000DA78
		[CompilerGenerated]
		public override readonly string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("StorageCloseAttemptEvent");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x060003C8 RID: 968 RVA: 0x0000F8C4 File Offset: 0x0000DAC4
		[CompilerGenerated]
		private readonly bool PrintMembers(StringBuilder builder)
		{
			builder.Append("Cancelled = ");
			builder.Append(this.Cancelled.ToString());
			return true;
		}

		// Token: 0x060003C9 RID: 969 RVA: 0x0000F8F9 File Offset: 0x0000DAF9
		[CompilerGenerated]
		public static bool operator !=(StorageCloseAttemptEvent left, StorageCloseAttemptEvent right)
		{
			return !(left == right);
		}

		// Token: 0x060003CA RID: 970 RVA: 0x0000F905 File Offset: 0x0000DB05
		[CompilerGenerated]
		public static bool operator ==(StorageCloseAttemptEvent left, StorageCloseAttemptEvent right)
		{
			return left.Equals(right);
		}

		// Token: 0x060003CB RID: 971 RVA: 0x0000F90F File Offset: 0x0000DB0F
		[CompilerGenerated]
		public override readonly int GetHashCode()
		{
			return EqualityComparer<bool>.Default.GetHashCode(this.<Cancelled>k__BackingField);
		}

		// Token: 0x060003CC RID: 972 RVA: 0x0000F921 File Offset: 0x0000DB21
		[CompilerGenerated]
		public override readonly bool Equals(object obj)
		{
			return obj is StorageCloseAttemptEvent && this.Equals((StorageCloseAttemptEvent)obj);
		}

		// Token: 0x060003CD RID: 973 RVA: 0x0000F939 File Offset: 0x0000DB39
		[CompilerGenerated]
		public readonly bool Equals(StorageCloseAttemptEvent other)
		{
			return EqualityComparer<bool>.Default.Equals(this.<Cancelled>k__BackingField, other.<Cancelled>k__BackingField);
		}

		// Token: 0x060003CE RID: 974 RVA: 0x0000F951 File Offset: 0x0000DB51
		[CompilerGenerated]
		public readonly void Deconstruct(out bool Cancelled)
		{
			Cancelled = this.Cancelled;
		}
	}
}
