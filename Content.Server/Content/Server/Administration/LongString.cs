using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Content.Server.Administration
{
	// Token: 0x02000805 RID: 2053
	[NullableContext(1)]
	[Nullable(0)]
	public struct LongString : IEquatable<LongString>
	{
		// Token: 0x06002C7B RID: 11387 RVA: 0x000E81BD File Offset: 0x000E63BD
		public LongString(string String)
		{
			this.String = String;
		}

		// Token: 0x170006EF RID: 1775
		// (get) Token: 0x06002C7C RID: 11388 RVA: 0x000E81C6 File Offset: 0x000E63C6
		// (set) Token: 0x06002C7D RID: 11389 RVA: 0x000E81CE File Offset: 0x000E63CE
		public string String { readonly get; set; }

		// Token: 0x06002C7E RID: 11390 RVA: 0x000E81D7 File Offset: 0x000E63D7
		public static implicit operator string(LongString longString)
		{
			return longString.String;
		}

		// Token: 0x06002C7F RID: 11391 RVA: 0x000E81E0 File Offset: 0x000E63E0
		public static explicit operator LongString(string s)
		{
			return new LongString(s);
		}

		// Token: 0x06002C80 RID: 11392 RVA: 0x000E81E8 File Offset: 0x000E63E8
		[NullableContext(0)]
		[CompilerGenerated]
		public override readonly string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("LongString");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x06002C81 RID: 11393 RVA: 0x000E8234 File Offset: 0x000E6434
		[NullableContext(0)]
		[CompilerGenerated]
		private readonly bool PrintMembers(StringBuilder builder)
		{
			builder.Append("String = ");
			builder.Append(this.String);
			return true;
		}

		// Token: 0x06002C82 RID: 11394 RVA: 0x000E8250 File Offset: 0x000E6450
		[CompilerGenerated]
		public static bool operator !=(LongString left, LongString right)
		{
			return !(left == right);
		}

		// Token: 0x06002C83 RID: 11395 RVA: 0x000E825C File Offset: 0x000E645C
		[CompilerGenerated]
		public static bool operator ==(LongString left, LongString right)
		{
			return left.Equals(right);
		}

		// Token: 0x06002C84 RID: 11396 RVA: 0x000E8266 File Offset: 0x000E6466
		[CompilerGenerated]
		public override readonly int GetHashCode()
		{
			return EqualityComparer<string>.Default.GetHashCode(this.<String>k__BackingField);
		}

		// Token: 0x06002C85 RID: 11397 RVA: 0x000E8278 File Offset: 0x000E6478
		[NullableContext(0)]
		[CompilerGenerated]
		public override readonly bool Equals(object obj)
		{
			return obj is LongString && this.Equals((LongString)obj);
		}

		// Token: 0x06002C86 RID: 11398 RVA: 0x000E8290 File Offset: 0x000E6490
		[CompilerGenerated]
		public readonly bool Equals(LongString other)
		{
			return EqualityComparer<string>.Default.Equals(this.<String>k__BackingField, other.<String>k__BackingField);
		}

		// Token: 0x06002C87 RID: 11399 RVA: 0x000E82A8 File Offset: 0x000E64A8
		[CompilerGenerated]
		public readonly void Deconstruct(out string String)
		{
			String = this.String;
		}
	}
}
