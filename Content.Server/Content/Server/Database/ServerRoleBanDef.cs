using System;
using System.Collections.Immutable;
using System.Net;
using System.Runtime.CompilerServices;
using Robust.Shared.Network;

namespace Content.Server.Database
{
	// Token: 0x020005BC RID: 1468
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ServerRoleBanDef
	{
		// Token: 0x17000492 RID: 1170
		// (get) Token: 0x06001F5B RID: 8027 RVA: 0x000A4F13 File Offset: 0x000A3113
		public int? Id { get; }

		// Token: 0x17000493 RID: 1171
		// (get) Token: 0x06001F5C RID: 8028 RVA: 0x000A4F1B File Offset: 0x000A311B
		public NetUserId? UserId { get; }

		// Token: 0x17000494 RID: 1172
		// (get) Token: 0x06001F5D RID: 8029 RVA: 0x000A4F23 File Offset: 0x000A3123
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
		public ValueTuple<IPAddress, int>? Address { [return: TupleElementNames(new string[]
		{
			"address",
			"cidrMask"
		})] [return: Nullable(new byte[]
		{
			0,
			1
		})] get; }

		// Token: 0x17000495 RID: 1173
		// (get) Token: 0x06001F5E RID: 8030 RVA: 0x000A4F2B File Offset: 0x000A312B
		[Nullable(0)]
		public ImmutableArray<byte>? HWId { [NullableContext(0)] get; }

		// Token: 0x17000496 RID: 1174
		// (get) Token: 0x06001F5F RID: 8031 RVA: 0x000A4F33 File Offset: 0x000A3133
		public DateTimeOffset BanTime { get; }

		// Token: 0x17000497 RID: 1175
		// (get) Token: 0x06001F60 RID: 8032 RVA: 0x000A4F3B File Offset: 0x000A313B
		public DateTimeOffset? ExpirationTime { get; }

		// Token: 0x17000498 RID: 1176
		// (get) Token: 0x06001F61 RID: 8033 RVA: 0x000A4F43 File Offset: 0x000A3143
		public string Reason { get; }

		// Token: 0x17000499 RID: 1177
		// (get) Token: 0x06001F62 RID: 8034 RVA: 0x000A4F4B File Offset: 0x000A314B
		public NetUserId? BanningAdmin { get; }

		// Token: 0x1700049A RID: 1178
		// (get) Token: 0x06001F63 RID: 8035 RVA: 0x000A4F53 File Offset: 0x000A3153
		[Nullable(2)]
		public ServerRoleUnbanDef Unban { [NullableContext(2)] get; }

		// Token: 0x1700049B RID: 1179
		// (get) Token: 0x06001F64 RID: 8036 RVA: 0x000A4F5B File Offset: 0x000A315B
		public string Role { get; }

		// Token: 0x06001F65 RID: 8037 RVA: 0x000A4F64 File Offset: 0x000A3164
		public ServerRoleBanDef(int? id, NetUserId? userId, [Nullable(new byte[]
		{
			0,
			1
		})] ValueTuple<IPAddress, int>? address, [Nullable(0)] ImmutableArray<byte>? hwId, DateTimeOffset banTime, DateTimeOffset? expirationTime, string reason, NetUserId? banningAdmin, [Nullable(2)] ServerRoleUnbanDef unban, string role)
		{
			if (userId == null && address == null && hwId == null)
			{
				throw new ArgumentException("Must have at least one of banned user, banned address or hardware ID");
			}
			if (address != null)
			{
				ValueTuple<IPAddress, int> addr = address.GetValueOrDefault();
				if (addr.Item1.IsIPv4MappedToIPv6)
				{
					address = new ValueTuple<IPAddress, int>?(new ValueTuple<IPAddress, int>(addr.Item1.MapToIPv4(), addr.Item2 - 96));
				}
			}
			this.Id = id;
			this.UserId = userId;
			this.Address = address;
			this.HWId = hwId;
			this.BanTime = banTime;
			this.ExpirationTime = expirationTime;
			this.Reason = reason;
			this.BanningAdmin = banningAdmin;
			this.Unban = unban;
			this.Role = role;
		}
	}
}
