using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Content.Server.Administration.Logs;
using Content.Server.IP;
using Content.Shared.CCVar;
using Microsoft.EntityFrameworkCore;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Network;

namespace Content.Server.Database
{
	// Token: 0x020005BB RID: 1467
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ServerDbSqlite : ServerDbBase
	{
		// Token: 0x06001F3E RID: 7998 RVA: 0x000A4428 File Offset: 0x000A2628
		public ServerDbSqlite(DbContextOptions<SqliteServerDbContext> options)
		{
			this._prefsCtx = new SqliteServerDbContext(options);
			IConfigurationManager configurationManager = IoCManager.Resolve<IConfigurationManager>();
			if (configurationManager.GetCVar<bool>(CCVars.DatabaseSynchronous))
			{
				this._prefsCtx.Database.Migrate();
				this._dbReadyTask = Task.CompletedTask;
			}
			else
			{
				this._dbReadyTask = Task.Run(delegate()
				{
					this._prefsCtx.Database.Migrate();
				});
			}
			configurationManager.OnValueChanged<int>(CCVars.DatabaseSqliteDelay, delegate(int v)
			{
				this._msDelay = v;
			}, true);
		}

		// Token: 0x06001F3F RID: 7999 RVA: 0x000A44B4 File Offset: 0x000A26B4
		[return: Nullable(new byte[]
		{
			1,
			2
		})]
		public override Task<ServerBanDef> GetServerBanAsync(int id)
		{
			ServerDbSqlite.<GetServerBanAsync>d__5 <GetServerBanAsync>d__;
			<GetServerBanAsync>d__.<>t__builder = AsyncTaskMethodBuilder<ServerBanDef>.Create();
			<GetServerBanAsync>d__.<>4__this = this;
			<GetServerBanAsync>d__.id = id;
			<GetServerBanAsync>d__.<>1__state = -1;
			<GetServerBanAsync>d__.<>t__builder.Start<ServerDbSqlite.<GetServerBanAsync>d__5>(ref <GetServerBanAsync>d__);
			return <GetServerBanAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06001F40 RID: 8000 RVA: 0x000A4500 File Offset: 0x000A2700
		[NullableContext(0)]
		[return: Nullable(new byte[]
		{
			1,
			2
		})]
		public override Task<ServerBanDef> GetServerBanAsync([Nullable(2)] IPAddress address, NetUserId? userId, ImmutableArray<byte>? hwId)
		{
			ServerDbSqlite.<GetServerBanAsync>d__6 <GetServerBanAsync>d__;
			<GetServerBanAsync>d__.<>t__builder = AsyncTaskMethodBuilder<ServerBanDef>.Create();
			<GetServerBanAsync>d__.<>4__this = this;
			<GetServerBanAsync>d__.address = address;
			<GetServerBanAsync>d__.userId = userId;
			<GetServerBanAsync>d__.hwId = hwId;
			<GetServerBanAsync>d__.<>1__state = -1;
			<GetServerBanAsync>d__.<>t__builder.Start<ServerDbSqlite.<GetServerBanAsync>d__6>(ref <GetServerBanAsync>d__);
			return <GetServerBanAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06001F41 RID: 8001 RVA: 0x000A455C File Offset: 0x000A275C
		[NullableContext(0)]
		[return: Nullable(1)]
		public override Task<List<ServerBanDef>> GetServerBansAsync([Nullable(2)] IPAddress address, NetUserId? userId, ImmutableArray<byte>? hwId, bool includeUnbanned)
		{
			ServerDbSqlite.<GetServerBansAsync>d__7 <GetServerBansAsync>d__;
			<GetServerBansAsync>d__.<>t__builder = AsyncTaskMethodBuilder<List<ServerBanDef>>.Create();
			<GetServerBansAsync>d__.<>4__this = this;
			<GetServerBansAsync>d__.address = address;
			<GetServerBansAsync>d__.userId = userId;
			<GetServerBansAsync>d__.hwId = hwId;
			<GetServerBansAsync>d__.includeUnbanned = includeUnbanned;
			<GetServerBansAsync>d__.<>1__state = -1;
			<GetServerBansAsync>d__.<>t__builder.Start<ServerDbSqlite.<GetServerBansAsync>d__7>(ref <GetServerBansAsync>d__);
			return <GetServerBansAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06001F42 RID: 8002 RVA: 0x000A45C0 File Offset: 0x000A27C0
		private static Task<List<ServerBan>> GetAllBans(SqliteServerDbContext db, bool includeUnbanned)
		{
			ServerDbSqlite.<GetAllBans>d__8 <GetAllBans>d__;
			<GetAllBans>d__.<>t__builder = AsyncTaskMethodBuilder<List<ServerBan>>.Create();
			<GetAllBans>d__.db = db;
			<GetAllBans>d__.includeUnbanned = includeUnbanned;
			<GetAllBans>d__.<>1__state = -1;
			<GetAllBans>d__.<>t__builder.Start<ServerDbSqlite.<GetAllBans>d__8>(ref <GetAllBans>d__);
			return <GetAllBans>d__.<>t__builder.Task;
		}

		// Token: 0x06001F43 RID: 8003 RVA: 0x000A460C File Offset: 0x000A280C
		[NullableContext(0)]
		private static bool BanMatches([Nullable(1)] ServerBan ban, [Nullable(2)] IPAddress address, NetUserId? userId, ImmutableArray<byte>? hwId)
		{
			if (address != null && ban.Address != null && address.IsInSubnet(ban.Address.Value))
			{
				return true;
			}
			if (userId != null)
			{
				NetUserId id = userId.GetValueOrDefault();
				Guid? userId2 = ban.UserId;
				Guid userId3 = id.UserId;
				if (userId2 != null && (userId2 == null || userId2.GetValueOrDefault() == userId3))
				{
					return true;
				}
			}
			if (hwId != null)
			{
				ImmutableArray<byte> hwIdVar = hwId.GetValueOrDefault();
				if (hwIdVar.Length > 0 && hwIdVar.AsSpan().SequenceEqual(ban.HWId))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001F44 RID: 8004 RVA: 0x000A46C8 File Offset: 0x000A28C8
		public override Task AddServerBanAsync(ServerBanDef serverBan)
		{
			ServerDbSqlite.<AddServerBanAsync>d__10 <AddServerBanAsync>d__;
			<AddServerBanAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<AddServerBanAsync>d__.<>4__this = this;
			<AddServerBanAsync>d__.serverBan = serverBan;
			<AddServerBanAsync>d__.<>1__state = -1;
			<AddServerBanAsync>d__.<>t__builder.Start<ServerDbSqlite.<AddServerBanAsync>d__10>(ref <AddServerBanAsync>d__);
			return <AddServerBanAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06001F45 RID: 8005 RVA: 0x000A4714 File Offset: 0x000A2914
		public override Task AddServerUnbanAsync(ServerUnbanDef serverUnban)
		{
			ServerDbSqlite.<AddServerUnbanAsync>d__11 <AddServerUnbanAsync>d__;
			<AddServerUnbanAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<AddServerUnbanAsync>d__.<>4__this = this;
			<AddServerUnbanAsync>d__.serverUnban = serverUnban;
			<AddServerUnbanAsync>d__.<>1__state = -1;
			<AddServerUnbanAsync>d__.<>t__builder.Start<ServerDbSqlite.<AddServerUnbanAsync>d__11>(ref <AddServerUnbanAsync>d__);
			return <AddServerUnbanAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06001F46 RID: 8006 RVA: 0x000A4760 File Offset: 0x000A2960
		[return: Nullable(new byte[]
		{
			1,
			2
		})]
		public override Task<ServerRoleBanDef> GetServerRoleBanAsync(int id)
		{
			ServerDbSqlite.<GetServerRoleBanAsync>d__12 <GetServerRoleBanAsync>d__;
			<GetServerRoleBanAsync>d__.<>t__builder = AsyncTaskMethodBuilder<ServerRoleBanDef>.Create();
			<GetServerRoleBanAsync>d__.<>4__this = this;
			<GetServerRoleBanAsync>d__.id = id;
			<GetServerRoleBanAsync>d__.<>1__state = -1;
			<GetServerRoleBanAsync>d__.<>t__builder.Start<ServerDbSqlite.<GetServerRoleBanAsync>d__12>(ref <GetServerRoleBanAsync>d__);
			return <GetServerRoleBanAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06001F47 RID: 8007 RVA: 0x000A47AC File Offset: 0x000A29AC
		[NullableContext(0)]
		[return: Nullable(1)]
		public override Task<List<ServerRoleBanDef>> GetServerRoleBansAsync([Nullable(2)] IPAddress address, NetUserId? userId, ImmutableArray<byte>? hwId, bool includeUnbanned)
		{
			ServerDbSqlite.<GetServerRoleBansAsync>d__13 <GetServerRoleBansAsync>d__;
			<GetServerRoleBansAsync>d__.<>t__builder = AsyncTaskMethodBuilder<List<ServerRoleBanDef>>.Create();
			<GetServerRoleBansAsync>d__.<>4__this = this;
			<GetServerRoleBansAsync>d__.address = address;
			<GetServerRoleBansAsync>d__.userId = userId;
			<GetServerRoleBansAsync>d__.hwId = hwId;
			<GetServerRoleBansAsync>d__.includeUnbanned = includeUnbanned;
			<GetServerRoleBansAsync>d__.<>1__state = -1;
			<GetServerRoleBansAsync>d__.<>t__builder.Start<ServerDbSqlite.<GetServerRoleBansAsync>d__13>(ref <GetServerRoleBansAsync>d__);
			return <GetServerRoleBansAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06001F48 RID: 8008 RVA: 0x000A4810 File Offset: 0x000A2A10
		private static Task<List<ServerRoleBan>> GetAllRoleBans(SqliteServerDbContext db, bool includeUnbanned)
		{
			ServerDbSqlite.<GetAllRoleBans>d__14 <GetAllRoleBans>d__;
			<GetAllRoleBans>d__.<>t__builder = AsyncTaskMethodBuilder<List<ServerRoleBan>>.Create();
			<GetAllRoleBans>d__.db = db;
			<GetAllRoleBans>d__.includeUnbanned = includeUnbanned;
			<GetAllRoleBans>d__.<>1__state = -1;
			<GetAllRoleBans>d__.<>t__builder.Start<ServerDbSqlite.<GetAllRoleBans>d__14>(ref <GetAllRoleBans>d__);
			return <GetAllRoleBans>d__.<>t__builder.Task;
		}

		// Token: 0x06001F49 RID: 8009 RVA: 0x000A485C File Offset: 0x000A2A5C
		[NullableContext(0)]
		private static bool RoleBanMatches([Nullable(1)] ServerRoleBan ban, [Nullable(2)] IPAddress address, NetUserId? userId, ImmutableArray<byte>? hwId)
		{
			if (address != null && ban.Address != null && address.IsInSubnet(ban.Address.Value))
			{
				return true;
			}
			if (userId != null)
			{
				NetUserId id = userId.GetValueOrDefault();
				Guid? userId2 = ban.UserId;
				Guid userId3 = id.UserId;
				if (userId2 != null && (userId2 == null || userId2.GetValueOrDefault() == userId3))
				{
					return true;
				}
			}
			if (hwId != null)
			{
				ImmutableArray<byte> hwIdVar = hwId.GetValueOrDefault();
				if (hwIdVar.Length > 0 && hwIdVar.AsSpan().SequenceEqual(ban.HWId))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001F4A RID: 8010 RVA: 0x000A4918 File Offset: 0x000A2B18
		public override Task AddServerRoleBanAsync(ServerRoleBanDef serverBan)
		{
			ServerDbSqlite.<AddServerRoleBanAsync>d__16 <AddServerRoleBanAsync>d__;
			<AddServerRoleBanAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<AddServerRoleBanAsync>d__.<>4__this = this;
			<AddServerRoleBanAsync>d__.serverBan = serverBan;
			<AddServerRoleBanAsync>d__.<>1__state = -1;
			<AddServerRoleBanAsync>d__.<>t__builder.Start<ServerDbSqlite.<AddServerRoleBanAsync>d__16>(ref <AddServerRoleBanAsync>d__);
			return <AddServerRoleBanAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06001F4B RID: 8011 RVA: 0x000A4964 File Offset: 0x000A2B64
		public override Task AddServerRoleUnbanAsync(ServerRoleUnbanDef serverUnban)
		{
			ServerDbSqlite.<AddServerRoleUnbanAsync>d__17 <AddServerRoleUnbanAsync>d__;
			<AddServerRoleUnbanAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<AddServerRoleUnbanAsync>d__.<>4__this = this;
			<AddServerRoleUnbanAsync>d__.serverUnban = serverUnban;
			<AddServerRoleUnbanAsync>d__.<>1__state = -1;
			<AddServerRoleUnbanAsync>d__.<>t__builder.Start<ServerDbSqlite.<AddServerRoleUnbanAsync>d__17>(ref <AddServerRoleUnbanAsync>d__);
			return <AddServerRoleUnbanAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06001F4C RID: 8012 RVA: 0x000A49B0 File Offset: 0x000A2BB0
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
			ServerRoleUnbanDef unban = ServerDbSqlite.ConvertRoleUnban(ban.Unban);
			int? id = new int?(ban.Id);
			NetUserId? userId = uid;
			ValueTuple<IPAddress, int>? address = ban.Address;
			ImmutableArray<byte>? hwId = (ban.HWId == null) ? null : new ImmutableArray<byte>?(ImmutableArray.Create<byte>(ban.HWId));
			DateTimeOffset banTime = ban.BanTime;
			DateTime? expirationTime = ban.ExpirationTime;
			return new ServerRoleBanDef(id, userId, address, hwId, banTime, (expirationTime != null) ? new DateTimeOffset?(expirationTime.GetValueOrDefault()) : null, ban.Reason, aUid, unban, ban.RoleId);
		}

		// Token: 0x06001F4D RID: 8013 RVA: 0x000A4AB0 File Offset: 0x000A2CB0
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

		// Token: 0x06001F4E RID: 8014 RVA: 0x000A4B08 File Offset: 0x000A2D08
		protected override PlayerRecord MakePlayerRecord(Player record)
		{
			NetUserId userId = new NetUserId(record.UserId);
			DateTimeOffset firstSeenTime = new DateTimeOffset(record.FirstSeenTime, TimeSpan.Zero);
			string lastSeenUserName = record.LastSeenUserName;
			DateTimeOffset lastSeenTime = new DateTimeOffset(record.LastSeenTime, TimeSpan.Zero);
			IPAddress lastSeenAddress = record.LastSeenAddress;
			byte[] lastSeenHWId = record.LastSeenHWId;
			return new PlayerRecord(userId, firstSeenTime, lastSeenUserName, lastSeenTime, lastSeenAddress, (lastSeenHWId != null) ? new ImmutableArray<byte>?(lastSeenHWId.ToImmutableArray<byte>()) : null);
		}

		// Token: 0x06001F4F RID: 8015 RVA: 0x000A4B70 File Offset: 0x000A2D70
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
			ServerUnbanDef unban = ServerDbSqlite.ConvertUnban(ban.Unban);
			int? id = new int?(ban.Id);
			NetUserId? userId = uid;
			ValueTuple<IPAddress, int>? address = ban.Address;
			ImmutableArray<byte>? hwId = (ban.HWId == null) ? null : new ImmutableArray<byte>?(ImmutableArray.Create<byte>(ban.HWId));
			DateTimeOffset banTime = ban.BanTime;
			DateTime? expirationTime = ban.ExpirationTime;
			return new ServerBanDef(id, userId, address, hwId, banTime, (expirationTime != null) ? new DateTimeOffset?(expirationTime.GetValueOrDefault()) : null, ban.Reason, aUid, unban);
		}

		// Token: 0x06001F50 RID: 8016 RVA: 0x000A4C68 File Offset: 0x000A2E68
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

		// Token: 0x06001F51 RID: 8017 RVA: 0x000A4CC0 File Offset: 0x000A2EC0
		public override Task<int> AddConnectionLogAsync(NetUserId userId, string userName, IPAddress address, [Nullable(0)] ImmutableArray<byte> hwId, ConnectionDenyReason? denied)
		{
			ServerDbSqlite.<AddConnectionLogAsync>d__23 <AddConnectionLogAsync>d__;
			<AddConnectionLogAsync>d__.<>t__builder = AsyncTaskMethodBuilder<int>.Create();
			<AddConnectionLogAsync>d__.<>4__this = this;
			<AddConnectionLogAsync>d__.userId = userId;
			<AddConnectionLogAsync>d__.userName = userName;
			<AddConnectionLogAsync>d__.address = address;
			<AddConnectionLogAsync>d__.hwId = hwId;
			<AddConnectionLogAsync>d__.denied = denied;
			<AddConnectionLogAsync>d__.<>1__state = -1;
			<AddConnectionLogAsync>d__.<>t__builder.Start<ServerDbSqlite.<AddConnectionLogAsync>d__23>(ref <AddConnectionLogAsync>d__);
			return <AddConnectionLogAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06001F52 RID: 8018 RVA: 0x000A4D30 File Offset: 0x000A2F30
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
			ServerDbSqlite.<GetAllAdminAndRanksAsync>d__24 <GetAllAdminAndRanksAsync>d__;
			<GetAllAdminAndRanksAsync>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<ValueTuple<Admin, string>[], AdminRank[]>>.Create();
			<GetAllAdminAndRanksAsync>d__.<>4__this = this;
			<GetAllAdminAndRanksAsync>d__.cancel = cancel;
			<GetAllAdminAndRanksAsync>d__.<>1__state = -1;
			<GetAllAdminAndRanksAsync>d__.<>t__builder.Start<ServerDbSqlite.<GetAllAdminAndRanksAsync>d__24>(ref <GetAllAdminAndRanksAsync>d__);
			return <GetAllAdminAndRanksAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06001F53 RID: 8019 RVA: 0x000A4D7C File Offset: 0x000A2F7C
		public override Task<int> AddNewRound(Server server, params Guid[] playerIds)
		{
			ServerDbSqlite.<AddNewRound>d__25 <AddNewRound>d__;
			<AddNewRound>d__.<>t__builder = AsyncTaskMethodBuilder<int>.Create();
			<AddNewRound>d__.<>4__this = this;
			<AddNewRound>d__.server = server;
			<AddNewRound>d__.playerIds = playerIds;
			<AddNewRound>d__.<>1__state = -1;
			<AddNewRound>d__.<>t__builder.Start<ServerDbSqlite.<AddNewRound>d__25>(ref <AddNewRound>d__);
			return <AddNewRound>d__.<>t__builder.Task;
		}

		// Token: 0x06001F54 RID: 8020 RVA: 0x000A4DD0 File Offset: 0x000A2FD0
		public override Task AddAdminLogs(List<QueuedLog> logs)
		{
			ServerDbSqlite.<AddAdminLogs>d__26 <AddAdminLogs>d__;
			<AddAdminLogs>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<AddAdminLogs>d__.<>4__this = this;
			<AddAdminLogs>d__.logs = logs;
			<AddAdminLogs>d__.<>1__state = -1;
			<AddAdminLogs>d__.<>t__builder.Start<ServerDbSqlite.<AddAdminLogs>d__26>(ref <AddAdminLogs>d__);
			return <AddAdminLogs>d__.<>t__builder.Task;
		}

		// Token: 0x06001F55 RID: 8021 RVA: 0x000A4E1C File Offset: 0x000A301C
		public override Task<int> AddAdminNote(AdminNote note)
		{
			ServerDbSqlite.<AddAdminNote>d__27 <AddAdminNote>d__;
			<AddAdminNote>d__.<>t__builder = AsyncTaskMethodBuilder<int>.Create();
			<AddAdminNote>d__.<>4__this = this;
			<AddAdminNote>d__.note = note;
			<AddAdminNote>d__.<>1__state = -1;
			<AddAdminNote>d__.<>t__builder.Start<ServerDbSqlite.<AddAdminNote>d__27>(ref <AddAdminNote>d__);
			return <AddAdminNote>d__.<>t__builder.Task;
		}

		// Token: 0x06001F56 RID: 8022 RVA: 0x000A4E68 File Offset: 0x000A3068
		private Task<ServerDbSqlite.DbGuardImpl> GetDbImpl()
		{
			ServerDbSqlite.<GetDbImpl>d__28 <GetDbImpl>d__;
			<GetDbImpl>d__.<>t__builder = AsyncTaskMethodBuilder<ServerDbSqlite.DbGuardImpl>.Create();
			<GetDbImpl>d__.<>4__this = this;
			<GetDbImpl>d__.<>1__state = -1;
			<GetDbImpl>d__.<>t__builder.Start<ServerDbSqlite.<GetDbImpl>d__28>(ref <GetDbImpl>d__);
			return <GetDbImpl>d__.<>t__builder.Task;
		}

		// Token: 0x06001F57 RID: 8023 RVA: 0x000A4EAC File Offset: 0x000A30AC
		protected override Task<ServerDbBase.DbGuard> GetDb()
		{
			ServerDbSqlite.<GetDb>d__29 <GetDb>d__;
			<GetDb>d__.<>t__builder = AsyncTaskMethodBuilder<ServerDbBase.DbGuard>.Create();
			<GetDb>d__.<>4__this = this;
			<GetDb>d__.<>1__state = -1;
			<GetDb>d__.<>t__builder.Start<ServerDbSqlite.<GetDb>d__29>(ref <GetDb>d__);
			return <GetDb>d__.<>t__builder.Task;
		}

		// Token: 0x04001372 RID: 4978
		private readonly SemaphoreSlim _prefsSemaphore = new SemaphoreSlim(1, 1);

		// Token: 0x04001373 RID: 4979
		private readonly Task _dbReadyTask;

		// Token: 0x04001374 RID: 4980
		private readonly SqliteServerDbContext _prefsCtx;

		// Token: 0x04001375 RID: 4981
		private int _msDelay;

		// Token: 0x02000A92 RID: 2706
		[Nullable(0)]
		private sealed class DbGuardImpl : ServerDbBase.DbGuard
		{
			// Token: 0x06003548 RID: 13640 RVA: 0x00118E22 File Offset: 0x00117022
			public DbGuardImpl(ServerDbSqlite db)
			{
				this._db = db;
			}

			// Token: 0x1700083B RID: 2107
			// (get) Token: 0x06003549 RID: 13641 RVA: 0x00118E31 File Offset: 0x00117031
			public override ServerDbContext DbContext
			{
				get
				{
					return this._db._prefsCtx;
				}
			}

			// Token: 0x1700083C RID: 2108
			// (get) Token: 0x0600354A RID: 13642 RVA: 0x00118E3E File Offset: 0x0011703E
			public SqliteServerDbContext SqliteDbContext
			{
				get
				{
					return this._db._prefsCtx;
				}
			}

			// Token: 0x0600354B RID: 13643 RVA: 0x00118E4C File Offset: 0x0011704C
			public override ValueTask DisposeAsync()
			{
				this._db._prefsSemaphore.Release();
				return default(ValueTask);
			}

			// Token: 0x040026A0 RID: 9888
			private readonly ServerDbSqlite _db;
		}

		// Token: 0x02000A93 RID: 2707
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x040026A1 RID: 9889
			[Nullable(0)]
			public static Func<ServerBan, ServerBanDef> <0>__ConvertBan;

			// Token: 0x040026A2 RID: 9890
			[Nullable(0)]
			public static Func<ServerRoleBan, ServerRoleBanDef> <1>__ConvertRoleBan;
		}
	}
}
