using System;
using System.Collections.Immutable;
using System.Net;
using System.Runtime.CompilerServices;
using Content.Shared.CCVar;
using Robust.Shared.Localization;
using Robust.Shared.Network;

namespace Content.Server.Database
{
	// Token: 0x020005B5 RID: 1461
	public sealed class ServerBanDef
	{
		// Token: 0x17000484 RID: 1156
		// (get) Token: 0x06001E5B RID: 7771 RVA: 0x000A111D File Offset: 0x0009F31D
		public int? Id { get; }

		// Token: 0x17000485 RID: 1157
		// (get) Token: 0x06001E5C RID: 7772 RVA: 0x000A1125 File Offset: 0x0009F325
		public NetUserId? UserId { get; }

		// Token: 0x17000486 RID: 1158
		// (get) Token: 0x06001E5D RID: 7773 RVA: 0x000A112D File Offset: 0x0009F32D
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

		// Token: 0x17000487 RID: 1159
		// (get) Token: 0x06001E5E RID: 7774 RVA: 0x000A1135 File Offset: 0x0009F335
		public ImmutableArray<byte>? HWId { get; }

		// Token: 0x17000488 RID: 1160
		// (get) Token: 0x06001E5F RID: 7775 RVA: 0x000A113D File Offset: 0x0009F33D
		public DateTimeOffset BanTime { get; }

		// Token: 0x17000489 RID: 1161
		// (get) Token: 0x06001E60 RID: 7776 RVA: 0x000A1145 File Offset: 0x0009F345
		public DateTimeOffset? ExpirationTime { get; }

		// Token: 0x1700048A RID: 1162
		// (get) Token: 0x06001E61 RID: 7777 RVA: 0x000A114D File Offset: 0x0009F34D
		[Nullable(1)]
		public string Reason { [NullableContext(1)] get; }

		// Token: 0x1700048B RID: 1163
		// (get) Token: 0x06001E62 RID: 7778 RVA: 0x000A1155 File Offset: 0x0009F355
		public NetUserId? BanningAdmin { get; }

		// Token: 0x1700048C RID: 1164
		// (get) Token: 0x06001E63 RID: 7779 RVA: 0x000A115D File Offset: 0x0009F35D
		[Nullable(2)]
		public ServerUnbanDef Unban { [NullableContext(2)] get; }

		// Token: 0x06001E64 RID: 7780 RVA: 0x000A1168 File Offset: 0x0009F368
		public ServerBanDef(int? id, NetUserId? userId, [Nullable(new byte[]
		{
			0,
			1
		})] ValueTuple<IPAddress, int>? address, ImmutableArray<byte>? hwId, DateTimeOffset banTime, DateTimeOffset? expirationTime, [Nullable(1)] string reason, NetUserId? banningAdmin, [Nullable(2)] ServerUnbanDef unban)
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
		}

		// Token: 0x1700048D RID: 1165
		// (get) Token: 0x06001E65 RID: 7781 RVA: 0x000A1230 File Offset: 0x0009F430
		[Nullable(1)]
		public string DisconnectMessage
		{
			[NullableContext(1)]
			get
			{
				string expires = Loc.GetString("ban-banned-permanent", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("discord", CCVars.InfoLinksDiscord)
				});
				DateTimeOffset? expirationTime = this.ExpirationTime;
				if (expirationTime != null)
				{
					DateTimeOffset expireTime = expirationTime.GetValueOrDefault();
					TimeSpan duration = expireTime - this.BanTime;
					DateTimeOffset utc = expireTime.ToUniversalTime();
					expires = Loc.GetString("ban-expires", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("duration", duration.TotalMinutes.ToString("N0")),
						new ValueTuple<string, object>("time", utc.ToString("f"))
					});
				}
				return string.Concat(new string[]
				{
					Loc.GetString("ban-banned-1"),
					"\n",
					Loc.GetString("ban-banned-2", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("reason", this.Reason)
					}),
					"\n",
					expires
				});
			}
		}
	}
}
