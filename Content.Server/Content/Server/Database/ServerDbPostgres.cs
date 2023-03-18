using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Robust.Shared.Network;

namespace Content.Server.Database
{
	// Token: 0x020005BA RID: 1466
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ServerDbPostgres : ServerDbBase
	{
		// Token: 0x06001F27 RID: 7975 RVA: 0x000A3297 File Offset: 0x000A1497
		public ServerDbPostgres(DbContextOptions<PostgresServerDbContext> options)
		{
			this._options = options;
			this._dbReadyTask = Task.Run(delegate()
			{
				ServerDbPostgres.<<-ctor>b__2_0>d <<-ctor>b__2_0>d;
				<<-ctor>b__2_0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
				<<-ctor>b__2_0>d.<>4__this = this;
				<<-ctor>b__2_0>d.<>1__state = -1;
				<<-ctor>b__2_0>d.<>t__builder.Start<ServerDbPostgres.<<-ctor>b__2_0>d>(ref <<-ctor>b__2_0>d);
				return <<-ctor>b__2_0>d.<>t__builder.Task;
			});
		}

		// Token: 0x06001F28 RID: 7976 RVA: 0x000A32C0 File Offset: 0x000A14C0
		[return: Nullable(new byte[]
		{
			1,
			2
		})]
		public override Task<ServerBanDef> GetServerBanAsync(int id)
		{
			ServerDbPostgres.<GetServerBanAsync>d__3 <GetServerBanAsync>d__;
			<GetServerBanAsync>d__.<>t__builder = AsyncTaskMethodBuilder<ServerBanDef>.Create();
			<GetServerBanAsync>d__.<>4__this = this;
			<GetServerBanAsync>d__.id = id;
			<GetServerBanAsync>d__.<>1__state = -1;
			<GetServerBanAsync>d__.<>t__builder.Start<ServerDbPostgres.<GetServerBanAsync>d__3>(ref <GetServerBanAsync>d__);
			return <GetServerBanAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06001F29 RID: 7977 RVA: 0x000A330C File Offset: 0x000A150C
		[NullableContext(0)]
		[return: Nullable(new byte[]
		{
			1,
			2
		})]
		public override Task<ServerBanDef> GetServerBanAsync([Nullable(2)] IPAddress address, NetUserId? userId, ImmutableArray<byte>? hwId)
		{
			ServerDbPostgres.<GetServerBanAsync>d__4 <GetServerBanAsync>d__;
			<GetServerBanAsync>d__.<>t__builder = AsyncTaskMethodBuilder<ServerBanDef>.Create();
			<GetServerBanAsync>d__.<>4__this = this;
			<GetServerBanAsync>d__.address = address;
			<GetServerBanAsync>d__.userId = userId;
			<GetServerBanAsync>d__.hwId = hwId;
			<GetServerBanAsync>d__.<>1__state = -1;
			<GetServerBanAsync>d__.<>t__builder.Start<ServerDbPostgres.<GetServerBanAsync>d__4>(ref <GetServerBanAsync>d__);
			return <GetServerBanAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06001F2A RID: 7978 RVA: 0x000A3368 File Offset: 0x000A1568
		[NullableContext(0)]
		[return: Nullable(1)]
		public override Task<List<ServerBanDef>> GetServerBansAsync([Nullable(2)] IPAddress address, NetUserId? userId, ImmutableArray<byte>? hwId, bool includeUnbanned)
		{
			ServerDbPostgres.<GetServerBansAsync>d__5 <GetServerBansAsync>d__;
			<GetServerBansAsync>d__.<>t__builder = AsyncTaskMethodBuilder<List<ServerBanDef>>.Create();
			<GetServerBansAsync>d__.<>4__this = this;
			<GetServerBansAsync>d__.address = address;
			<GetServerBansAsync>d__.userId = userId;
			<GetServerBansAsync>d__.hwId = hwId;
			<GetServerBansAsync>d__.includeUnbanned = includeUnbanned;
			<GetServerBansAsync>d__.<>1__state = -1;
			<GetServerBansAsync>d__.<>t__builder.Start<ServerDbPostgres.<GetServerBansAsync>d__5>(ref <GetServerBansAsync>d__);
			return <GetServerBansAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06001F2B RID: 7979 RVA: 0x000A33CC File Offset: 0x000A15CC
		private static IQueryable<ServerBan> MakeBanLookupQuery([Nullable(2)] IPAddress address, NetUserId? userId, [Nullable(0)] ImmutableArray<byte>? hwId, ServerDbPostgres.DbGuardImpl db, bool includeUnbanned)
		{
			IQueryable<ServerBan> query = null;
			if (userId != null)
			{
				NetUserId uid = userId.GetValueOrDefault();
				IQueryable<ServerBan> newQ = from b in db.PgDbContext.Ban.Include((ServerBan p) => p.Unban)
				where b.UserId == (Guid?)uid.UserId
				select b;
				query = ((query == null) ? newQ : query.Union(newQ));
			}
			if (address != null)
			{
				IQueryable<ServerBan> newQ2 = from b in db.PgDbContext.Ban.Include((ServerBan p) => p.Unban)
				where b.Address != null && EF.Functions.ContainsOrEqual(b.Address.Value, address)
				select b;
				query = ((query == null) ? newQ2 : query.Union(newQ2));
			}
			if (hwId != null && hwId.Value.Length > 0)
			{
				IQueryable<ServerBan> newQ3 = from b in db.PgDbContext.Ban.Include((ServerBan p) => p.Unban)
				where b.HWId.SequenceEqual(hwId.Value.ToArray<byte>())
				select b;
				query = ((query == null) ? newQ3 : query.Union(newQ3));
			}
			if (!includeUnbanned)
			{
				query = ((query != null) ? (from p in query
				where p.Unban == null && (p.ExpirationTime == null || p.ExpirationTime.Value > DateTime.Now)
				select p) : null);
			}
			return query.Distinct<ServerBan>();
		}

		// Token: 0x06001F2C RID: 7980 RVA: 0x000A38A0 File Offset: 0x000A1AA0
		[NullableContext(2)]
		private static ServerBanDef ConvertBan(ServerBan ban)
		{
			if (ban == null)
			{
				return null;
			}
			NetUserId? uid = null;
			Guid? guid2 = ban.UserId;
			if (guid2 != null)
			{
				Guid guid = guid2.GetValueOrDefault();
				uid = new NetUserId?(new NetUserId(guid));
			}
			NetUserId? aUid = null;
			guid2 = ban.BanningAdmin;
			if (guid2 != null)
			{
				Guid aGuid = guid2.GetValueOrDefault();
				aUid = new NetUserId?(new NetUserId(aGuid));
			}
			ServerUnbanDef unbanDef = ServerDbPostgres.ConvertUnban(ban.Unban);
			int? id = new int?(ban.Id);
			NetUserId? userId = uid;
			ValueTuple<IPAddress, int>? address = ban.Address;
			ImmutableArray<byte>? hwId = (ban.HWId == null) ? null : new ImmutableArray<byte>?(ImmutableArray.Create<byte>(ban.HWId));
			DateTimeOffset banTime = ban.BanTime;
			DateTime? expirationTime = ban.ExpirationTime;
			return new ServerBanDef(id, userId, address, hwId, banTime, (expirationTime != null) ? new DateTimeOffset?(expirationTime.GetValueOrDefault()) : null, ban.Reason, aUid, unbanDef);
		}

		// Token: 0x06001F2D RID: 7981 RVA: 0x000A3998 File Offset: 0x000A1B98
		[NullableContext(2)]
		private static ServerUnbanDef ConvertUnban(ServerUnban unban)
		{
			if (unban == null)
			{
				return null;
			}
			NetUserId? aUid = null;
			Guid? unbanningAdmin = unban.UnbanningAdmin;
			if (unbanningAdmin != null)
			{
				Guid aGuid = unbanningAdmin.GetValueOrDefault();
				aUid = new NetUserId?(new NetUserId(aGuid));
			}
			return new ServerUnbanDef(unban.Id, aUid, unban.UnbanTime);
		}

		// Token: 0x06001F2E RID: 7982 RVA: 0x000A39F0 File Offset: 0x000A1BF0
		public override Task AddServerBanAsync(ServerBanDef serverBan)
		{
			ServerDbPostgres.<AddServerBanAsync>d__9 <AddServerBanAsync>d__;
			<AddServerBanAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<AddServerBanAsync>d__.<>4__this = this;
			<AddServerBanAsync>d__.serverBan = serverBan;
			<AddServerBanAsync>d__.<>1__state = -1;
			<AddServerBanAsync>d__.<>t__builder.Start<ServerDbPostgres.<AddServerBanAsync>d__9>(ref <AddServerBanAsync>d__);
			return <AddServerBanAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06001F2F RID: 7983 RVA: 0x000A3A3C File Offset: 0x000A1C3C
		public override Task AddServerUnbanAsync(ServerUnbanDef serverUnban)
		{
			ServerDbPostgres.<AddServerUnbanAsync>d__10 <AddServerUnbanAsync>d__;
			<AddServerUnbanAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<AddServerUnbanAsync>d__.<>4__this = this;
			<AddServerUnbanAsync>d__.serverUnban = serverUnban;
			<AddServerUnbanAsync>d__.<>1__state = -1;
			<AddServerUnbanAsync>d__.<>t__builder.Start<ServerDbPostgres.<AddServerUnbanAsync>d__10>(ref <AddServerUnbanAsync>d__);
			return <AddServerUnbanAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06001F30 RID: 7984 RVA: 0x000A3A88 File Offset: 0x000A1C88
		[return: Nullable(new byte[]
		{
			1,
			2
		})]
		public override Task<ServerRoleBanDef> GetServerRoleBanAsync(int id)
		{
			ServerDbPostgres.<GetServerRoleBanAsync>d__11 <GetServerRoleBanAsync>d__;
			<GetServerRoleBanAsync>d__.<>t__builder = AsyncTaskMethodBuilder<ServerRoleBanDef>.Create();
			<GetServerRoleBanAsync>d__.<>4__this = this;
			<GetServerRoleBanAsync>d__.id = id;
			<GetServerRoleBanAsync>d__.<>1__state = -1;
			<GetServerRoleBanAsync>d__.<>t__builder.Start<ServerDbPostgres.<GetServerRoleBanAsync>d__11>(ref <GetServerRoleBanAsync>d__);
			return <GetServerRoleBanAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06001F31 RID: 7985 RVA: 0x000A3AD4 File Offset: 0x000A1CD4
		[NullableContext(0)]
		[return: Nullable(1)]
		public override Task<List<ServerRoleBanDef>> GetServerRoleBansAsync([Nullable(2)] IPAddress address, NetUserId? userId, ImmutableArray<byte>? hwId, bool includeUnbanned)
		{
			ServerDbPostgres.<GetServerRoleBansAsync>d__12 <GetServerRoleBansAsync>d__;
			<GetServerRoleBansAsync>d__.<>t__builder = AsyncTaskMethodBuilder<List<ServerRoleBanDef>>.Create();
			<GetServerRoleBansAsync>d__.<>4__this = this;
			<GetServerRoleBansAsync>d__.address = address;
			<GetServerRoleBansAsync>d__.userId = userId;
			<GetServerRoleBansAsync>d__.hwId = hwId;
			<GetServerRoleBansAsync>d__.includeUnbanned = includeUnbanned;
			<GetServerRoleBansAsync>d__.<>1__state = -1;
			<GetServerRoleBansAsync>d__.<>t__builder.Start<ServerDbPostgres.<GetServerRoleBansAsync>d__12>(ref <GetServerRoleBansAsync>d__);
			return <GetServerRoleBansAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06001F32 RID: 7986 RVA: 0x000A3B38 File Offset: 0x000A1D38
		private static Task<List<ServerRoleBanDef>> QueryRoleBans(IQueryable<ServerRoleBan> query)
		{
			ServerDbPostgres.<QueryRoleBans>d__13 <QueryRoleBans>d__;
			<QueryRoleBans>d__.<>t__builder = AsyncTaskMethodBuilder<List<ServerRoleBanDef>>.Create();
			<QueryRoleBans>d__.query = query;
			<QueryRoleBans>d__.<>1__state = -1;
			<QueryRoleBans>d__.<>t__builder.Start<ServerDbPostgres.<QueryRoleBans>d__13>(ref <QueryRoleBans>d__);
			return <QueryRoleBans>d__.<>t__builder.Task;
		}

		// Token: 0x06001F33 RID: 7987 RVA: 0x000A3B7C File Offset: 0x000A1D7C
		private static IQueryable<ServerRoleBan> MakeRoleBanLookupQuery([Nullable(2)] IPAddress address, NetUserId? userId, [Nullable(0)] ImmutableArray<byte>? hwId, ServerDbPostgres.DbGuardImpl db, bool includeUnbanned)
		{
			IQueryable<ServerRoleBan> query = null;
			if (userId != null)
			{
				NetUserId uid = userId.GetValueOrDefault();
				IQueryable<ServerRoleBan> newQ = from b in db.PgDbContext.RoleBan.Include((ServerRoleBan p) => p.Unban)
				where b.UserId == (Guid?)uid.UserId
				select b;
				query = ((query == null) ? newQ : query.Union(newQ));
			}
			if (address != null)
			{
				IQueryable<ServerRoleBan> newQ2 = from b in db.PgDbContext.RoleBan.Include((ServerRoleBan p) => p.Unban)
				where b.Address != null && EF.Functions.ContainsOrEqual(b.Address.Value, address)
				select b;
				query = ((query == null) ? newQ2 : query.Union(newQ2));
			}
			if (hwId != null && hwId.Value.Length > 0)
			{
				IQueryable<ServerRoleBan> newQ3 = from b in db.PgDbContext.RoleBan.Include((ServerRoleBan p) => p.Unban)
				where b.HWId.SequenceEqual(hwId.Value.ToArray<byte>())
				select b;
				query = ((query == null) ? newQ3 : query.Union(newQ3));
			}
			if (!includeUnbanned)
			{
				query = ((query != null) ? (from p in query
				where p.Unban == null && (p.ExpirationTime == null || p.ExpirationTime.Value > DateTime.Now)
				select p) : null);
			}
			return query.Distinct<ServerRoleBan>();
		}

		// Token: 0x06001F34 RID: 7988 RVA: 0x000A4050 File Offset: 0x000A2250
		[NullableContext(2)]
		private static ServerRoleBanDef ConvertRoleBan(ServerRoleBan ban)
		{
			if (ban == null)
			{
				return null;
			}
			NetUserId? uid = null;
			Guid? guid2 = ban.UserId;
			if (guid2 != null)
			{
				Guid guid = guid2.GetValueOrDefault();
				uid = new NetUserId?(new NetUserId(guid));
			}
			NetUserId? aUid = null;
			guid2 = ban.BanningAdmin;
			if (guid2 != null)
			{
				Guid aGuid = guid2.GetValueOrDefault();
				aUid = new NetUserId?(new NetUserId(aGuid));
			}
			ServerRoleUnbanDef unbanDef = ServerDbPostgres.ConvertRoleUnban(ban.Unban);
			int? id = new int?(ban.Id);
			NetUserId? userId = uid;
			ValueTuple<IPAddress, int>? address = ban.Address;
			ImmutableArray<byte>? hwId = (ban.HWId == null) ? null : new ImmutableArray<byte>?(ImmutableArray.Create<byte>(ban.HWId));
			DateTimeOffset banTime = ban.BanTime;
			DateTime? expirationTime = ban.ExpirationTime;
			return new ServerRoleBanDef(id, userId, address, hwId, banTime, (expirationTime != null) ? new DateTimeOffset?(expirationTime.GetValueOrDefault()) : null, ban.Reason, aUid, unbanDef, ban.RoleId);
		}

		// Token: 0x06001F35 RID: 7989 RVA: 0x000A4150 File Offset: 0x000A2350
		[NullableContext(2)]
		private static ServerRoleUnbanDef ConvertRoleUnban(ServerRoleUnban unban)
		{
			if (unban == null)
			{
				return null;
			}
			NetUserId? aUid = null;
			Guid? unbanningAdmin = unban.UnbanningAdmin;
			if (unbanningAdmin != null)
			{
				Guid aGuid = unbanningAdmin.GetValueOrDefault();
				aUid = new NetUserId?(new NetUserId(aGuid));
			}
			return new ServerRoleUnbanDef(unban.Id, aUid, unban.UnbanTime);
		}

		// Token: 0x06001F36 RID: 7990 RVA: 0x000A41A8 File Offset: 0x000A23A8
		public override Task AddServerRoleBanAsync(ServerRoleBanDef serverRoleBan)
		{
			ServerDbPostgres.<AddServerRoleBanAsync>d__17 <AddServerRoleBanAsync>d__;
			<AddServerRoleBanAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<AddServerRoleBanAsync>d__.<>4__this = this;
			<AddServerRoleBanAsync>d__.serverRoleBan = serverRoleBan;
			<AddServerRoleBanAsync>d__.<>1__state = -1;
			<AddServerRoleBanAsync>d__.<>t__builder.Start<ServerDbPostgres.<AddServerRoleBanAsync>d__17>(ref <AddServerRoleBanAsync>d__);
			return <AddServerRoleBanAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06001F37 RID: 7991 RVA: 0x000A41F4 File Offset: 0x000A23F4
		public override Task AddServerRoleUnbanAsync(ServerRoleUnbanDef serverRoleUnban)
		{
			ServerDbPostgres.<AddServerRoleUnbanAsync>d__18 <AddServerRoleUnbanAsync>d__;
			<AddServerRoleUnbanAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<AddServerRoleUnbanAsync>d__.<>4__this = this;
			<AddServerRoleUnbanAsync>d__.serverRoleUnban = serverRoleUnban;
			<AddServerRoleUnbanAsync>d__.<>1__state = -1;
			<AddServerRoleUnbanAsync>d__.<>t__builder.Start<ServerDbPostgres.<AddServerRoleUnbanAsync>d__18>(ref <AddServerRoleUnbanAsync>d__);
			return <AddServerRoleUnbanAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06001F38 RID: 7992 RVA: 0x000A4240 File Offset: 0x000A2440
		protected override PlayerRecord MakePlayerRecord(Player record)
		{
			NetUserId userId = new NetUserId(record.UserId);
			DateTimeOffset firstSeenTime = new DateTimeOffset(record.FirstSeenTime);
			string lastSeenUserName = record.LastSeenUserName;
			DateTimeOffset lastSeenTime = new DateTimeOffset(record.LastSeenTime);
			IPAddress lastSeenAddress = record.LastSeenAddress;
			byte[] lastSeenHWId = record.LastSeenHWId;
			return new PlayerRecord(userId, firstSeenTime, lastSeenUserName, lastSeenTime, lastSeenAddress, (lastSeenHWId != null) ? new ImmutableArray<byte>?(lastSeenHWId.ToImmutableArray<byte>()) : null);
		}

		// Token: 0x06001F39 RID: 7993 RVA: 0x000A42A0 File Offset: 0x000A24A0
		public override Task<int> AddConnectionLogAsync(NetUserId userId, string userName, IPAddress address, [Nullable(0)] ImmutableArray<byte> hwId, ConnectionDenyReason? denied)
		{
			ServerDbPostgres.<AddConnectionLogAsync>d__20 <AddConnectionLogAsync>d__;
			<AddConnectionLogAsync>d__.<>t__builder = AsyncTaskMethodBuilder<int>.Create();
			<AddConnectionLogAsync>d__.<>4__this = this;
			<AddConnectionLogAsync>d__.userId = userId;
			<AddConnectionLogAsync>d__.userName = userName;
			<AddConnectionLogAsync>d__.address = address;
			<AddConnectionLogAsync>d__.hwId = hwId;
			<AddConnectionLogAsync>d__.denied = denied;
			<AddConnectionLogAsync>d__.<>1__state = -1;
			<AddConnectionLogAsync>d__.<>t__builder.Start<ServerDbPostgres.<AddConnectionLogAsync>d__20>(ref <AddConnectionLogAsync>d__);
			return <AddConnectionLogAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06001F3A RID: 7994 RVA: 0x000A4310 File Offset: 0x000A2510
		[return: TupleElementNames(new string[]
		{
			"admins",
			null,
			null,
			"lastUserName"
		})]
		[return: Nullable(new byte[]
		{
			1,
			0,
			1,
			0,
			1,
			2,
			1,
			1
		})]
		public override Task<ValueTuple<ValueTuple<Admin, string>[], AdminRank[]>> GetAllAdminAndRanksAsync(CancellationToken cancel)
		{
			ServerDbPostgres.<GetAllAdminAndRanksAsync>d__21 <GetAllAdminAndRanksAsync>d__;
			<GetAllAdminAndRanksAsync>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<ValueTuple<Admin, string>[], AdminRank[]>>.Create();
			<GetAllAdminAndRanksAsync>d__.<>4__this = this;
			<GetAllAdminAndRanksAsync>d__.cancel = cancel;
			<GetAllAdminAndRanksAsync>d__.<>1__state = -1;
			<GetAllAdminAndRanksAsync>d__.<>t__builder.Start<ServerDbPostgres.<GetAllAdminAndRanksAsync>d__21>(ref <GetAllAdminAndRanksAsync>d__);
			return <GetAllAdminAndRanksAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06001F3B RID: 7995 RVA: 0x000A435C File Offset: 0x000A255C
		private Task<ServerDbPostgres.DbGuardImpl> GetDbImpl()
		{
			ServerDbPostgres.<GetDbImpl>d__22 <GetDbImpl>d__;
			<GetDbImpl>d__.<>t__builder = AsyncTaskMethodBuilder<ServerDbPostgres.DbGuardImpl>.Create();
			<GetDbImpl>d__.<>4__this = this;
			<GetDbImpl>d__.<>1__state = -1;
			<GetDbImpl>d__.<>t__builder.Start<ServerDbPostgres.<GetDbImpl>d__22>(ref <GetDbImpl>d__);
			return <GetDbImpl>d__.<>t__builder.Task;
		}

		// Token: 0x06001F3C RID: 7996 RVA: 0x000A43A0 File Offset: 0x000A25A0
		protected override Task<ServerDbBase.DbGuard> GetDb()
		{
			ServerDbPostgres.<GetDb>d__23 <GetDb>d__;
			<GetDb>d__.<>t__builder = AsyncTaskMethodBuilder<ServerDbBase.DbGuard>.Create();
			<GetDb>d__.<>4__this = this;
			<GetDb>d__.<>1__state = -1;
			<GetDb>d__.<>t__builder.Start<ServerDbPostgres.<GetDb>d__23>(ref <GetDb>d__);
			return <GetDb>d__.<>t__builder.Task;
		}

		// Token: 0x04001370 RID: 4976
		private readonly DbContextOptions<PostgresServerDbContext> _options;

		// Token: 0x04001371 RID: 4977
		private readonly Task _dbReadyTask;

		// Token: 0x02000A7D RID: 2685
		[Nullable(0)]
		private sealed class DbGuardImpl : ServerDbBase.DbGuard
		{
			// Token: 0x0600351F RID: 13599 RVA: 0x001160A6 File Offset: 0x001142A6
			public DbGuardImpl(PostgresServerDbContext dbC)
			{
				this.PgDbContext = dbC;
			}

			// Token: 0x17000839 RID: 2105
			// (get) Token: 0x06003520 RID: 13600 RVA: 0x001160B5 File Offset: 0x001142B5
			public PostgresServerDbContext PgDbContext { get; }

			// Token: 0x1700083A RID: 2106
			// (get) Token: 0x06003521 RID: 13601 RVA: 0x001160BD File Offset: 0x001142BD
			public override ServerDbContext DbContext
			{
				get
				{
					return this.PgDbContext;
				}
			}

			// Token: 0x06003522 RID: 13602 RVA: 0x001160C5 File Offset: 0x001142C5
			public override ValueTask DisposeAsync()
			{
				return this.DbContext.DisposeAsync();
			}
		}
	}
}
