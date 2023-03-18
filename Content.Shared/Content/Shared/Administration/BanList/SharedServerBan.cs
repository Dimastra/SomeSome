using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration.BanList
{
	// Token: 0x02000758 RID: 1880
	[NullableContext(2)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class SharedServerBan : IEquatable<SharedServerBan>
	{
		// Token: 0x060016F6 RID: 5878 RVA: 0x0004A6A4 File Offset: 0x000488A4
		public SharedServerBan(int? Id, NetUserId? UserId, [TupleElementNames(new string[]
		{
			"address",
			"cidrMask"
		})] [Nullable(new byte[]
		{
			0,
			1
		})] ValueTuple<string, int>? Address, string HWId, DateTime BanTime, DateTime? ExpirationTime, [Nullable(1)] string Reason, string BanningAdminName, SharedServerUnban Unban)
		{
			this.Id = Id;
			this.UserId = UserId;
			this.Address = Address;
			this.HWId = HWId;
			this.BanTime = BanTime;
			this.ExpirationTime = ExpirationTime;
			this.Reason = Reason;
			this.BanningAdminName = BanningAdminName;
			this.Unban = Unban;
			base..ctor();
		}

		// Token: 0x170004D1 RID: 1233
		// (get) Token: 0x060016F7 RID: 5879 RVA: 0x0004A6FC File Offset: 0x000488FC
		[Nullable(1)]
		[CompilerGenerated]
		private Type EqualityContract
		{
			[NullableContext(1)]
			[CompilerGenerated]
			get
			{
				return typeof(SharedServerBan);
			}
		}

		// Token: 0x170004D2 RID: 1234
		// (get) Token: 0x060016F8 RID: 5880 RVA: 0x0004A708 File Offset: 0x00048908
		// (set) Token: 0x060016F9 RID: 5881 RVA: 0x0004A710 File Offset: 0x00048910
		public int? Id { get; set; }

		// Token: 0x170004D3 RID: 1235
		// (get) Token: 0x060016FA RID: 5882 RVA: 0x0004A719 File Offset: 0x00048919
		// (set) Token: 0x060016FB RID: 5883 RVA: 0x0004A721 File Offset: 0x00048921
		public NetUserId? UserId { get; set; }

		// Token: 0x170004D4 RID: 1236
		// (get) Token: 0x060016FC RID: 5884 RVA: 0x0004A72A File Offset: 0x0004892A
		// (set) Token: 0x060016FD RID: 5885 RVA: 0x0004A732 File Offset: 0x00048932
		[TupleElementNames(new string[]
		{
			"address",
			"cidrMask"
		})]
		[Nullable(new byte[]
		{
			0,
			1
		})]
		public ValueTuple<string, int>? Address { [return: TupleElementNames(new string[]
		{
			"address",
			"cidrMask"
		})] [return: Nullable(new byte[]
		{
			0,
			1
		})] get; [param: TupleElementNames(new string[]
		{
			"address",
			"cidrMask"
		})] [param: Nullable(new byte[]
		{
			0,
			1
		})] set; }

		// Token: 0x170004D5 RID: 1237
		// (get) Token: 0x060016FE RID: 5886 RVA: 0x0004A73B File Offset: 0x0004893B
		// (set) Token: 0x060016FF RID: 5887 RVA: 0x0004A743 File Offset: 0x00048943
		public string HWId { get; set; }

		// Token: 0x170004D6 RID: 1238
		// (get) Token: 0x06001700 RID: 5888 RVA: 0x0004A74C File Offset: 0x0004894C
		// (set) Token: 0x06001701 RID: 5889 RVA: 0x0004A754 File Offset: 0x00048954
		public DateTime BanTime { get; set; }

		// Token: 0x170004D7 RID: 1239
		// (get) Token: 0x06001702 RID: 5890 RVA: 0x0004A75D File Offset: 0x0004895D
		// (set) Token: 0x06001703 RID: 5891 RVA: 0x0004A765 File Offset: 0x00048965
		public DateTime? ExpirationTime { get; set; }

		// Token: 0x170004D8 RID: 1240
		// (get) Token: 0x06001704 RID: 5892 RVA: 0x0004A76E File Offset: 0x0004896E
		// (set) Token: 0x06001705 RID: 5893 RVA: 0x0004A776 File Offset: 0x00048976
		[Nullable(1)]
		public string Reason { [NullableContext(1)] get; [NullableContext(1)] set; }

		// Token: 0x170004D9 RID: 1241
		// (get) Token: 0x06001706 RID: 5894 RVA: 0x0004A77F File Offset: 0x0004897F
		// (set) Token: 0x06001707 RID: 5895 RVA: 0x0004A787 File Offset: 0x00048987
		public string BanningAdminName { get; set; }

		// Token: 0x170004DA RID: 1242
		// (get) Token: 0x06001708 RID: 5896 RVA: 0x0004A790 File Offset: 0x00048990
		// (set) Token: 0x06001709 RID: 5897 RVA: 0x0004A798 File Offset: 0x00048998
		public SharedServerUnban Unban { get; set; }

		// Token: 0x0600170A RID: 5898 RVA: 0x0004A7A4 File Offset: 0x000489A4
		[NullableContext(1)]
		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("SharedServerBan");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x0600170B RID: 5899 RVA: 0x0004A7F0 File Offset: 0x000489F0
		[NullableContext(1)]
		[CompilerGenerated]
		private bool PrintMembers(StringBuilder builder)
		{
			RuntimeHelpers.EnsureSufficientExecutionStack();
			builder.Append("Id = ");
			builder.Append(this.Id.ToString());
			builder.Append(", UserId = ");
			builder.Append(this.UserId.ToString());
			builder.Append(", Address = ");
			builder.Append(this.Address.ToString());
			builder.Append(", HWId = ");
			builder.Append(this.HWId);
			builder.Append(", BanTime = ");
			builder.Append(this.BanTime.ToString());
			builder.Append(", ExpirationTime = ");
			builder.Append(this.ExpirationTime.ToString());
			builder.Append(", Reason = ");
			builder.Append(this.Reason);
			builder.Append(", BanningAdminName = ");
			builder.Append(this.BanningAdminName);
			builder.Append(", Unban = ");
			builder.Append(this.Unban);
			return true;
		}

		// Token: 0x0600170C RID: 5900 RVA: 0x0004A92B File Offset: 0x00048B2B
		[CompilerGenerated]
		public static bool operator !=(SharedServerBan left, SharedServerBan right)
		{
			return !(left == right);
		}

		// Token: 0x0600170D RID: 5901 RVA: 0x0004A937 File Offset: 0x00048B37
		[CompilerGenerated]
		public static bool operator ==(SharedServerBan left, SharedServerBan right)
		{
			return left == right || (left != null && left.Equals(right));
		}

		// Token: 0x0600170E RID: 5902 RVA: 0x0004A94C File Offset: 0x00048B4C
		[CompilerGenerated]
		public override int GetHashCode()
		{
			return ((((((((EqualityComparer<Type>.Default.GetHashCode(this.EqualityContract) * -1521134295 + EqualityComparer<int?>.Default.GetHashCode(this.<Id>k__BackingField)) * -1521134295 + EqualityComparer<NetUserId?>.Default.GetHashCode(this.<UserId>k__BackingField)) * -1521134295 + EqualityComparer<ValueTuple<string, int>?>.Default.GetHashCode(this.<Address>k__BackingField)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.<HWId>k__BackingField)) * -1521134295 + EqualityComparer<DateTime>.Default.GetHashCode(this.<BanTime>k__BackingField)) * -1521134295 + EqualityComparer<DateTime?>.Default.GetHashCode(this.<ExpirationTime>k__BackingField)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.<Reason>k__BackingField)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.<BanningAdminName>k__BackingField)) * -1521134295 + EqualityComparer<SharedServerUnban>.Default.GetHashCode(this.<Unban>k__BackingField);
		}

		// Token: 0x0600170F RID: 5903 RVA: 0x0004AA38 File Offset: 0x00048C38
		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			return this.Equals(obj as SharedServerBan);
		}

		// Token: 0x06001710 RID: 5904 RVA: 0x0004AA48 File Offset: 0x00048C48
		[CompilerGenerated]
		public bool Equals(SharedServerBan other)
		{
			return this == other || (other != null && this.EqualityContract == other.EqualityContract && EqualityComparer<int?>.Default.Equals(this.<Id>k__BackingField, other.<Id>k__BackingField) && EqualityComparer<NetUserId?>.Default.Equals(this.<UserId>k__BackingField, other.<UserId>k__BackingField) && EqualityComparer<ValueTuple<string, int>?>.Default.Equals(this.<Address>k__BackingField, other.<Address>k__BackingField) && EqualityComparer<string>.Default.Equals(this.<HWId>k__BackingField, other.<HWId>k__BackingField) && EqualityComparer<DateTime>.Default.Equals(this.<BanTime>k__BackingField, other.<BanTime>k__BackingField) && EqualityComparer<DateTime?>.Default.Equals(this.<ExpirationTime>k__BackingField, other.<ExpirationTime>k__BackingField) && EqualityComparer<string>.Default.Equals(this.<Reason>k__BackingField, other.<Reason>k__BackingField) && EqualityComparer<string>.Default.Equals(this.<BanningAdminName>k__BackingField, other.<BanningAdminName>k__BackingField) && EqualityComparer<SharedServerUnban>.Default.Equals(this.<Unban>k__BackingField, other.<Unban>k__BackingField));
		}

		// Token: 0x06001712 RID: 5906 RVA: 0x0004AB64 File Offset: 0x00048D64
		[CompilerGenerated]
		private SharedServerBan([Nullable(1)] SharedServerBan original)
		{
			this.Id = original.<Id>k__BackingField;
			this.UserId = original.<UserId>k__BackingField;
			this.Address = original.<Address>k__BackingField;
			this.HWId = original.<HWId>k__BackingField;
			this.BanTime = original.<BanTime>k__BackingField;
			this.ExpirationTime = original.<ExpirationTime>k__BackingField;
			this.Reason = original.<Reason>k__BackingField;
			this.BanningAdminName = original.<BanningAdminName>k__BackingField;
			this.Unban = original.<Unban>k__BackingField;
		}

		// Token: 0x06001713 RID: 5907 RVA: 0x0004ABE4 File Offset: 0x00048DE4
		[CompilerGenerated]
		public void Deconstruct(out int? Id, out NetUserId? UserId, [TupleElementNames(new string[]
		{
			"address",
			"cidrMask"
		})] [Nullable(new byte[]
		{
			0,
			1
		})] out ValueTuple<string, int>? Address, out string HWId, out DateTime BanTime, out DateTime? ExpirationTime, [Nullable(1)] out string Reason, out string BanningAdminName, out SharedServerUnban Unban)
		{
			Id = this.Id;
			UserId = this.UserId;
			Address = this.Address;
			HWId = this.HWId;
			BanTime = this.BanTime;
			ExpirationTime = this.ExpirationTime;
			Reason = this.Reason;
			BanningAdminName = this.BanningAdminName;
			Unban = this.Unban;
		}
	}
}
