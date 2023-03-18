using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.Network;

namespace Content.Server.Administration
{
	// Token: 0x02000801 RID: 2049
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class LocatedPlayerData : IEquatable<LocatedPlayerData>
	{
		// Token: 0x06002C52 RID: 11346 RVA: 0x000E759C File Offset: 0x000E579C
		[NullableContext(0)]
		public LocatedPlayerData(NetUserId UserId, [Nullable(2)] IPAddress LastAddress, ImmutableArray<byte>? LastHWId, [Nullable(1)] string Username)
		{
			this.UserId = UserId;
			this.LastAddress = LastAddress;
			this.LastHWId = LastHWId;
			this.Username = Username;
			base..ctor();
		}

		// Token: 0x170006EA RID: 1770
		// (get) Token: 0x06002C53 RID: 11347 RVA: 0x000E75C1 File Offset: 0x000E57C1
		[CompilerGenerated]
		private Type EqualityContract
		{
			[CompilerGenerated]
			get
			{
				return typeof(LocatedPlayerData);
			}
		}

		// Token: 0x170006EB RID: 1771
		// (get) Token: 0x06002C54 RID: 11348 RVA: 0x000E75CD File Offset: 0x000E57CD
		// (set) Token: 0x06002C55 RID: 11349 RVA: 0x000E75D5 File Offset: 0x000E57D5
		public NetUserId UserId { get; set; }

		// Token: 0x170006EC RID: 1772
		// (get) Token: 0x06002C56 RID: 11350 RVA: 0x000E75DE File Offset: 0x000E57DE
		// (set) Token: 0x06002C57 RID: 11351 RVA: 0x000E75E6 File Offset: 0x000E57E6
		[Nullable(2)]
		public IPAddress LastAddress { [NullableContext(2)] get; [NullableContext(2)] set; }

		// Token: 0x170006ED RID: 1773
		// (get) Token: 0x06002C58 RID: 11352 RVA: 0x000E75EF File Offset: 0x000E57EF
		// (set) Token: 0x06002C59 RID: 11353 RVA: 0x000E75F7 File Offset: 0x000E57F7
		[Nullable(0)]
		public ImmutableArray<byte>? LastHWId { [NullableContext(0)] get; [NullableContext(0)] set; }

		// Token: 0x170006EE RID: 1774
		// (get) Token: 0x06002C5A RID: 11354 RVA: 0x000E7600 File Offset: 0x000E5800
		// (set) Token: 0x06002C5B RID: 11355 RVA: 0x000E7608 File Offset: 0x000E5808
		public string Username { get; set; }

		// Token: 0x06002C5C RID: 11356 RVA: 0x000E7614 File Offset: 0x000E5814
		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("LocatedPlayerData");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x06002C5D RID: 11357 RVA: 0x000E7660 File Offset: 0x000E5860
		[CompilerGenerated]
		private bool PrintMembers(StringBuilder builder)
		{
			RuntimeHelpers.EnsureSufficientExecutionStack();
			builder.Append("UserId = ");
			builder.Append(this.UserId.ToString());
			builder.Append(", LastAddress = ");
			builder.Append(this.LastAddress);
			builder.Append(", LastHWId = ");
			builder.Append(this.LastHWId.ToString());
			builder.Append(", Username = ");
			builder.Append(this.Username);
			return true;
		}

		// Token: 0x06002C5E RID: 11358 RVA: 0x000E76F3 File Offset: 0x000E58F3
		[NullableContext(2)]
		[CompilerGenerated]
		public static bool operator !=(LocatedPlayerData left, LocatedPlayerData right)
		{
			return !(left == right);
		}

		// Token: 0x06002C5F RID: 11359 RVA: 0x000E76FF File Offset: 0x000E58FF
		[NullableContext(2)]
		[CompilerGenerated]
		public static bool operator ==(LocatedPlayerData left, LocatedPlayerData right)
		{
			return left == right || (left != null && left.Equals(right));
		}

		// Token: 0x06002C60 RID: 11360 RVA: 0x000E7714 File Offset: 0x000E5914
		[CompilerGenerated]
		public override int GetHashCode()
		{
			return (((EqualityComparer<Type>.Default.GetHashCode(this.EqualityContract) * -1521134295 + EqualityComparer<NetUserId>.Default.GetHashCode(this.<UserId>k__BackingField)) * -1521134295 + EqualityComparer<IPAddress>.Default.GetHashCode(this.<LastAddress>k__BackingField)) * -1521134295 + EqualityComparer<ImmutableArray<byte>?>.Default.GetHashCode(this.<LastHWId>k__BackingField)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.<Username>k__BackingField);
		}

		// Token: 0x06002C61 RID: 11361 RVA: 0x000E778D File Offset: 0x000E598D
		[NullableContext(2)]
		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			return this.Equals(obj as LocatedPlayerData);
		}

		// Token: 0x06002C62 RID: 11362 RVA: 0x000E779C File Offset: 0x000E599C
		[NullableContext(2)]
		[CompilerGenerated]
		public bool Equals(LocatedPlayerData other)
		{
			return this == other || (other != null && this.EqualityContract == other.EqualityContract && EqualityComparer<NetUserId>.Default.Equals(this.<UserId>k__BackingField, other.<UserId>k__BackingField) && EqualityComparer<IPAddress>.Default.Equals(this.<LastAddress>k__BackingField, other.<LastAddress>k__BackingField) && EqualityComparer<ImmutableArray<byte>?>.Default.Equals(this.<LastHWId>k__BackingField, other.<LastHWId>k__BackingField) && EqualityComparer<string>.Default.Equals(this.<Username>k__BackingField, other.<Username>k__BackingField));
		}

		// Token: 0x06002C64 RID: 11364 RVA: 0x000E782D File Offset: 0x000E5A2D
		[CompilerGenerated]
		private LocatedPlayerData(LocatedPlayerData original)
		{
			this.UserId = original.<UserId>k__BackingField;
			this.LastAddress = original.<LastAddress>k__BackingField;
			this.LastHWId = original.<LastHWId>k__BackingField;
			this.Username = original.<Username>k__BackingField;
		}

		// Token: 0x06002C65 RID: 11365 RVA: 0x000E7865 File Offset: 0x000E5A65
		[NullableContext(0)]
		[CompilerGenerated]
		public void Deconstruct(out NetUserId UserId, [Nullable(2)] out IPAddress LastAddress, out ImmutableArray<byte>? LastHWId, [Nullable(1)] out string Username)
		{
			UserId = this.UserId;
			LastAddress = this.LastAddress;
			LastHWId = this.LastHWId;
			Username = this.Username;
		}
	}
}
