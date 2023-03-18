using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.GameObjects;

namespace Content.Shared.Storage.Components
{
	// Token: 0x02000139 RID: 313
	[ByRefEvent]
	public struct InsertIntoEntityStorageAttemptEvent : IEquatable<InsertIntoEntityStorageAttemptEvent>
	{
		// Token: 0x06000391 RID: 913 RVA: 0x0000F366 File Offset: 0x0000D566
		public InsertIntoEntityStorageAttemptEvent(bool Cancelled = false)
		{
			this.Cancelled = Cancelled;
		}

		// Token: 0x170000B0 RID: 176
		// (get) Token: 0x06000392 RID: 914 RVA: 0x0000F36F File Offset: 0x0000D56F
		// (set) Token: 0x06000393 RID: 915 RVA: 0x0000F377 File Offset: 0x0000D577
		public bool Cancelled { readonly get; set; }

		// Token: 0x06000394 RID: 916 RVA: 0x0000F380 File Offset: 0x0000D580
		[CompilerGenerated]
		public override readonly string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("InsertIntoEntityStorageAttemptEvent");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x06000395 RID: 917 RVA: 0x0000F3CC File Offset: 0x0000D5CC
		[CompilerGenerated]
		private readonly bool PrintMembers(StringBuilder builder)
		{
			builder.Append("Cancelled = ");
			builder.Append(this.Cancelled.ToString());
			return true;
		}

		// Token: 0x06000396 RID: 918 RVA: 0x0000F401 File Offset: 0x0000D601
		[CompilerGenerated]
		public static bool operator !=(InsertIntoEntityStorageAttemptEvent left, InsertIntoEntityStorageAttemptEvent right)
		{
			return !(left == right);
		}

		// Token: 0x06000397 RID: 919 RVA: 0x0000F40D File Offset: 0x0000D60D
		[CompilerGenerated]
		public static bool operator ==(InsertIntoEntityStorageAttemptEvent left, InsertIntoEntityStorageAttemptEvent right)
		{
			return left.Equals(right);
		}

		// Token: 0x06000398 RID: 920 RVA: 0x0000F417 File Offset: 0x0000D617
		[CompilerGenerated]
		public override readonly int GetHashCode()
		{
			return EqualityComparer<bool>.Default.GetHashCode(this.<Cancelled>k__BackingField);
		}

		// Token: 0x06000399 RID: 921 RVA: 0x0000F429 File Offset: 0x0000D629
		[CompilerGenerated]
		public override readonly bool Equals(object obj)
		{
			return obj is InsertIntoEntityStorageAttemptEvent && this.Equals((InsertIntoEntityStorageAttemptEvent)obj);
		}

		// Token: 0x0600039A RID: 922 RVA: 0x0000F441 File Offset: 0x0000D641
		[CompilerGenerated]
		public readonly bool Equals(InsertIntoEntityStorageAttemptEvent other)
		{
			return EqualityComparer<bool>.Default.Equals(this.<Cancelled>k__BackingField, other.<Cancelled>k__BackingField);
		}

		// Token: 0x0600039B RID: 923 RVA: 0x0000F459 File Offset: 0x0000D659
		[CompilerGenerated]
		public readonly void Deconstruct(out bool Cancelled)
		{
			Cancelled = this.Cancelled;
		}
	}
}
