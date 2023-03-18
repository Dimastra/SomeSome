using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.GameObjects;

namespace Content.Shared.Materials
{
	// Token: 0x02000335 RID: 821
	[ByRefEvent]
	public struct GetMaterialWhitelistEvent : IEquatable<GetMaterialWhitelistEvent>
	{
		// Token: 0x06000976 RID: 2422 RVA: 0x0001F8E3 File Offset: 0x0001DAE3
		public GetMaterialWhitelistEvent(EntityUid Storage)
		{
			this.Storage = Storage;
			this.Whitelist = new List<string>();
		}

		// Token: 0x06000977 RID: 2423 RVA: 0x0001F8F8 File Offset: 0x0001DAF8
		[CompilerGenerated]
		public override readonly string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("GetMaterialWhitelistEvent");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x06000978 RID: 2424 RVA: 0x0001F944 File Offset: 0x0001DB44
		[CompilerGenerated]
		private readonly bool PrintMembers(StringBuilder builder)
		{
			builder.Append("Storage = ");
			builder.Append(this.Storage.ToString());
			builder.Append(", Whitelist = ");
			builder.Append(this.Whitelist);
			return true;
		}

		// Token: 0x06000979 RID: 2425 RVA: 0x0001F984 File Offset: 0x0001DB84
		[CompilerGenerated]
		public static bool operator !=(GetMaterialWhitelistEvent left, GetMaterialWhitelistEvent right)
		{
			return !(left == right);
		}

		// Token: 0x0600097A RID: 2426 RVA: 0x0001F990 File Offset: 0x0001DB90
		[CompilerGenerated]
		public static bool operator ==(GetMaterialWhitelistEvent left, GetMaterialWhitelistEvent right)
		{
			return left.Equals(right);
		}

		// Token: 0x0600097B RID: 2427 RVA: 0x0001F99A File Offset: 0x0001DB9A
		[CompilerGenerated]
		public override readonly int GetHashCode()
		{
			return EqualityComparer<EntityUid>.Default.GetHashCode(this.Storage) * -1521134295 + EqualityComparer<List<string>>.Default.GetHashCode(this.Whitelist);
		}

		// Token: 0x0600097C RID: 2428 RVA: 0x0001F9C3 File Offset: 0x0001DBC3
		[CompilerGenerated]
		public override readonly bool Equals(object obj)
		{
			return obj is GetMaterialWhitelistEvent && this.Equals((GetMaterialWhitelistEvent)obj);
		}

		// Token: 0x0600097D RID: 2429 RVA: 0x0001F9DB File Offset: 0x0001DBDB
		[CompilerGenerated]
		public readonly bool Equals(GetMaterialWhitelistEvent other)
		{
			return EqualityComparer<EntityUid>.Default.Equals(this.Storage, other.Storage) && EqualityComparer<List<string>>.Default.Equals(this.Whitelist, other.Whitelist);
		}

		// Token: 0x0600097E RID: 2430 RVA: 0x0001FA0D File Offset: 0x0001DC0D
		[CompilerGenerated]
		public readonly void Deconstruct(out EntityUid Storage)
		{
			Storage = this.Storage;
		}

		// Token: 0x0400095B RID: 2395
		public readonly EntityUid Storage;

		// Token: 0x0400095C RID: 2396
		[Nullable(1)]
		public List<string> Whitelist;
	}
}
