using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Content.Server.Administration.Logs;
using Content.Shared.Administration.Logs;
using Content.Shared.CCVar;
using Content.Shared.Preferences;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Prometheus;
using Robust.Shared.Configuration;
using Robust.Shared.ContentPack;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Maths;
using Robust.Shared.Network;

namespace Content.Server.Database
{
	// Token: 0x020005B8 RID: 1464
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ServerDbManager : IServerDbManager
	{
		// Token: 0x06001ED9 RID: 7897 RVA: 0x000A25D8 File Offset: 0x000A07D8
		public void Init()
		{
			this._msLogProvider = new ServerDbManager.LoggingProvider(this._logMgr);
			this._msLoggerFactory = LoggerFactory.Create(delegate(ILoggingBuilder builder)
			{
				builder.AddProvider(this._msLogProvider);
			});
			string engine = this._cfg.GetCVar<string>(CCVars.DatabaseEngine).ToLower();
			if (engine == "sqlite")
			{
				DbContextOptions<SqliteServerDbContext> sqliteOptions = this.CreateSqliteOptions();
				this._db = new ServerDbSqlite(sqliteOptions);
				return;
			}
			if (!(engine == "postgres"))
			{
				throw new InvalidDataException("Unknown database engine " + engine + ".");
			}
			DbContextOptions<PostgresServerDbContext> pgOptions = this.CreatePostgresOptions();
			this._db = new ServerDbPostgres(pgOptions);
		}

		// Token: 0x06001EDA RID: 7898 RVA: 0x000A267C File Offset: 0x000A087C
		public Task<PlayerPreferences> InitPrefsAsync(NetUserId userId, ICharacterProfile defaultProfile)
		{
			ServerDbManager.DbWriteOpsMetric.Inc(1.0);
			return this._db.InitPrefsAsync(userId, defaultProfile);
		}

		// Token: 0x06001EDB RID: 7899 RVA: 0x000A269E File Offset: 0x000A089E
		public Task SaveSelectedCharacterIndexAsync(NetUserId userId, int index)
		{
			ServerDbManager.DbWriteOpsMetric.Inc(1.0);
			return this._db.SaveSelectedCharacterIndexAsync(userId, index);
		}

		// Token: 0x06001EDC RID: 7900 RVA: 0x000A26C0 File Offset: 0x000A08C0
		public Task SaveCharacterSlotAsync(NetUserId userId, [Nullable(2)] ICharacterProfile profile, int slot)
		{
			ServerDbManager.DbWriteOpsMetric.Inc(1.0);
			return this._db.SaveCharacterSlotAsync(userId, profile, slot);
		}

		// Token: 0x06001EDD RID: 7901 RVA: 0x000A26E3 File Offset: 0x000A08E3
		public Task DeleteSlotAndSetSelectedIndex(NetUserId userId, int deleteSlot, int newSlot)
		{
			ServerDbManager.DbWriteOpsMetric.Inc(1.0);
			return this._db.DeleteSlotAndSetSelectedIndex(userId, deleteSlot, newSlot);
		}

		// Token: 0x06001EDE RID: 7902 RVA: 0x000A2706 File Offset: 0x000A0906
		public Task SaveAdminOOCColorAsync(NetUserId userId, Color color)
		{
			ServerDbManager.DbWriteOpsMetric.Inc(1.0);
			return this._db.SaveAdminOOCColorAsync(userId, color);
		}

		// Token: 0x06001EDF RID: 7903 RVA: 0x000A2728 File Offset: 0x000A0928
		[return: Nullable(new byte[]
		{
			1,
			2
		})]
		public Task<PlayerPreferences> GetPlayerPreferencesAsync(NetUserId userId)
		{
			ServerDbManager.DbReadOpsMetric.Inc(1.0);
			return this._db.GetPlayerPreferencesAsync(userId);
		}

		// Token: 0x06001EE0 RID: 7904 RVA: 0x000A2749 File Offset: 0x000A0949
		public Task AssignUserIdAsync(string name, NetUserId userId)
		{
			ServerDbManager.DbWriteOpsMetric.Inc(1.0);
			return this._db.AssignUserIdAsync(name, userId);
		}

		// Token: 0x06001EE1 RID: 7905 RVA: 0x000A276B File Offset: 0x000A096B
		public Task<NetUserId?> GetAssignedUserIdAsync(string name)
		{
			ServerDbManager.DbReadOpsMetric.Inc(1.0);
			return this._db.GetAssignedUserIdAsync(name);
		}

		// Token: 0x06001EE2 RID: 7906 RVA: 0x000A278C File Offset: 0x000A098C
		[return: Nullable(new byte[]
		{
			1,
			2
		})]
		public Task<ServerBanDef> GetServerBanAsync(int id)
		{
			ServerDbManager.DbReadOpsMetric.Inc(1.0);
			return this._db.GetServerBanAsync(id);
		}

		// Token: 0x06001EE3 RID: 7907 RVA: 0x000A27AD File Offset: 0x000A09AD
		[NullableContext(0)]
		[return: Nullable(new byte[]
		{
			1,
			2
		})]
		public Task<ServerBanDef> GetServerBanAsync([Nullable(2)] IPAddress address, NetUserId? userId, ImmutableArray<byte>? hwId)
		{
			ServerDbManager.DbReadOpsMetric.Inc(1.0);
			return this._db.GetServerBanAsync(address, userId, hwId);
		}

		// Token: 0x06001EE4 RID: 7908 RVA: 0x000A27D0 File Offset: 0x000A09D0
		[NullableContext(0)]
		[return: Nullable(1)]
		public Task<List<ServerBanDef>> GetServerBansAsync([Nullable(2)] IPAddress address, NetUserId? userId, ImmutableArray<byte>? hwId, bool includeUnbanned = true)
		{
			ServerDbManager.DbReadOpsMetric.Inc(1.0);
			return this._db.GetServerBansAsync(address, userId, hwId, includeUnbanned);
		}

		// Token: 0x06001EE5 RID: 7909 RVA: 0x000A27F5 File Offset: 0x000A09F5
		public Task AddServerBanAsync(ServerBanDef serverBan)
		{
			ServerDbManager.DbWriteOpsMetric.Inc(1.0);
			return this._db.AddServerBanAsync(serverBan);
		}

		// Token: 0x06001EE6 RID: 7910 RVA: 0x000A2816 File Offset: 0x000A0A16
		public Task AddServerUnbanAsync(ServerUnbanDef serverUnban)
		{
			ServerDbManager.DbWriteOpsMetric.Inc(1.0);
			return this._db.AddServerUnbanAsync(serverUnban);
		}

		// Token: 0x06001EE7 RID: 7911 RVA: 0x000A2837 File Offset: 0x000A0A37
		[return: Nullable(new byte[]
		{
			1,
			2
		})]
		public Task<ServerRoleBanDef> GetServerRoleBanAsync(int id)
		{
			ServerDbManager.DbReadOpsMetric.Inc(1.0);
			return this._db.GetServerRoleBanAsync(id);
		}

		// Token: 0x06001EE8 RID: 7912 RVA: 0x000A2858 File Offset: 0x000A0A58
		[NullableContext(0)]
		[return: Nullable(1)]
		public Task<List<ServerRoleBanDef>> GetServerRoleBansAsync([Nullable(2)] IPAddress address, NetUserId? userId, ImmutableArray<byte>? hwId, bool includeUnbanned = true)
		{
			ServerDbManager.DbReadOpsMetric.Inc(1.0);
			return this._db.GetServerRoleBansAsync(address, userId, hwId, includeUnbanned);
		}

		// Token: 0x06001EE9 RID: 7913 RVA: 0x000A287D File Offset: 0x000A0A7D
		public Task AddServerRoleBanAsync(ServerRoleBanDef serverRoleBan)
		{
			ServerDbManager.DbWriteOpsMetric.Inc(1.0);
			return this._db.AddServerRoleBanAsync(serverRoleBan);
		}

		// Token: 0x06001EEA RID: 7914 RVA: 0x000A289E File Offset: 0x000A0A9E
		public Task AddServerRoleUnbanAsync(ServerRoleUnbanDef serverRoleUnban)
		{
			ServerDbManager.DbWriteOpsMetric.Inc(1.0);
			return this._db.AddServerRoleUnbanAsync(serverRoleUnban);
		}

		// Token: 0x06001EEB RID: 7915 RVA: 0x000A28BF File Offset: 0x000A0ABF
		public Task<List<PlayTime>> GetPlayTimes(Guid player)
		{
			ServerDbManager.DbReadOpsMetric.Inc(1.0);
			return this._db.GetPlayTimes(player);
		}

		// Token: 0x06001EEC RID: 7916 RVA: 0x000A28E0 File Offset: 0x000A0AE0
		public Task UpdatePlayTimes(IReadOnlyCollection<PlayTimeUpdate> updates)
		{
			ServerDbManager.DbWriteOpsMetric.Inc(1.0);
			return this._db.UpdatePlayTimes(updates);
		}

		// Token: 0x06001EED RID: 7917 RVA: 0x000A2901 File Offset: 0x000A0B01
		public Task UpdatePlayerRecordAsync(NetUserId userId, string userName, IPAddress address, [Nullable(0)] ImmutableArray<byte> hwId)
		{
			ServerDbManager.DbWriteOpsMetric.Inc(1.0);
			return this._db.UpdatePlayerRecord(userId, userName, address, hwId);
		}

		// Token: 0x06001EEE RID: 7918 RVA: 0x000A2926 File Offset: 0x000A0B26
		[return: Nullable(new byte[]
		{
			1,
			2
		})]
		public Task<PlayerRecord> GetPlayerRecordByUserName(string userName, CancellationToken cancel = default(CancellationToken))
		{
			ServerDbManager.DbReadOpsMetric.Inc(1.0);
			return this._db.GetPlayerRecordByUserName(userName, cancel);
		}

		// Token: 0x06001EEF RID: 7919 RVA: 0x000A2948 File Offset: 0x000A0B48
		[return: Nullable(new byte[]
		{
			1,
			2
		})]
		public Task<PlayerRecord> GetPlayerRecordByUserId(NetUserId userId, CancellationToken cancel = default(CancellationToken))
		{
			ServerDbManager.DbReadOpsMetric.Inc(1.0);
			return this._db.GetPlayerRecordByUserId(userId, cancel);
		}

		// Token: 0x06001EF0 RID: 7920 RVA: 0x000A296A File Offset: 0x000A0B6A
		public Task<int> AddConnectionLogAsync(NetUserId userId, string userName, IPAddress address, [Nullable(0)] ImmutableArray<byte> hwId, ConnectionDenyReason? denied)
		{
			ServerDbManager.DbWriteOpsMetric.Inc(1.0);
			return this._db.AddConnectionLogAsync(userId, userName, address, hwId, denied);
		}

		// Token: 0x06001EF1 RID: 7921 RVA: 0x000A2991 File Offset: 0x000A0B91
		public Task AddServerBanHitsAsync(int connection, IEnumerable<ServerBanDef> bans)
		{
			ServerDbManager.DbWriteOpsMetric.Inc(1.0);
			return this._db.AddServerBanHitsAsync(connection, bans);
		}

		// Token: 0x06001EF2 RID: 7922 RVA: 0x000A29B3 File Offset: 0x000A0BB3
		[return: Nullable(new byte[]
		{
			1,
			2
		})]
		public Task<Admin> GetAdminDataForAsync(NetUserId userId, CancellationToken cancel = default(CancellationToken))
		{
			ServerDbManager.DbReadOpsMetric.Inc(1.0);
			return this._db.GetAdminDataForAsync(userId, cancel);
		}

		// Token: 0x06001EF3 RID: 7923 RVA: 0x000A29D5 File Offset: 0x000A0BD5
		[return: Nullable(new byte[]
		{
			1,
			2
		})]
		public Task<AdminRank> GetAdminRankAsync(int id, CancellationToken cancel = default(CancellationToken))
		{
			ServerDbManager.DbReadOpsMetric.Inc(1.0);
			return this._db.GetAdminRankDataForAsync(id, cancel);
		}

		// Token: 0x06001EF4 RID: 7924 RVA: 0x000A29F7 File Offset: 0x000A0BF7
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
		public Task<ValueTuple<ValueTuple<Admin, string>[], AdminRank[]>> GetAllAdminAndRanksAsync(CancellationToken cancel = default(CancellationToken))
		{
			ServerDbManager.DbReadOpsMetric.Inc(1.0);
			return this._db.GetAllAdminAndRanksAsync(cancel);
		}

		// Token: 0x06001EF5 RID: 7925 RVA: 0x000A2A18 File Offset: 0x000A0C18
		public Task RemoveAdminAsync(NetUserId userId, CancellationToken cancel = default(CancellationToken))
		{
			ServerDbManager.DbWriteOpsMetric.Inc(1.0);
			return this._db.RemoveAdminAsync(userId, cancel);
		}

		// Token: 0x06001EF6 RID: 7926 RVA: 0x000A2A3A File Offset: 0x000A0C3A
		public Task AddAdminAsync(Admin admin, CancellationToken cancel = default(CancellationToken))
		{
			ServerDbManager.DbWriteOpsMetric.Inc(1.0);
			return this._db.AddAdminAsync(admin, cancel);
		}

		// Token: 0x06001EF7 RID: 7927 RVA: 0x000A2A5C File Offset: 0x000A0C5C
		public Task UpdateAdminAsync(Admin admin, CancellationToken cancel = default(CancellationToken))
		{
			ServerDbManager.DbWriteOpsMetric.Inc(1.0);
			return this._db.UpdateAdminAsync(admin, cancel);
		}

		// Token: 0x06001EF8 RID: 7928 RVA: 0x000A2A7E File Offset: 0x000A0C7E
		public Task RemoveAdminRankAsync(int rankId, CancellationToken cancel = default(CancellationToken))
		{
			ServerDbManager.DbWriteOpsMetric.Inc(1.0);
			return this._db.RemoveAdminRankAsync(rankId, cancel);
		}

		// Token: 0x06001EF9 RID: 7929 RVA: 0x000A2AA0 File Offset: 0x000A0CA0
		public Task AddAdminRankAsync(AdminRank rank, CancellationToken cancel = default(CancellationToken))
		{
			ServerDbManager.DbWriteOpsMetric.Inc(1.0);
			return this._db.AddAdminRankAsync(rank, cancel);
		}

		// Token: 0x06001EFA RID: 7930 RVA: 0x000A2AC2 File Offset: 0x000A0CC2
		public Task<int> AddNewRound(Server server, params Guid[] playerIds)
		{
			ServerDbManager.DbWriteOpsMetric.Inc(1.0);
			return this._db.AddNewRound(server, playerIds);
		}

		// Token: 0x06001EFB RID: 7931 RVA: 0x000A2AE4 File Offset: 0x000A0CE4
		public Task<Round> GetRound(int id)
		{
			ServerDbManager.DbReadOpsMetric.Inc(1.0);
			return this._db.GetRound(id);
		}

		// Token: 0x06001EFC RID: 7932 RVA: 0x000A2B05 File Offset: 0x000A0D05
		public Task AddRoundPlayers(int id, params Guid[] playerIds)
		{
			ServerDbManager.DbWriteOpsMetric.Inc(1.0);
			return this._db.AddRoundPlayers(id, playerIds);
		}

		// Token: 0x06001EFD RID: 7933 RVA: 0x000A2B27 File Offset: 0x000A0D27
		public Task UpdateAdminRankAsync(AdminRank rank, CancellationToken cancel = default(CancellationToken))
		{
			ServerDbManager.DbWriteOpsMetric.Inc(1.0);
			return this._db.UpdateAdminRankAsync(rank, cancel);
		}

		// Token: 0x06001EFE RID: 7934 RVA: 0x000A2B4C File Offset: 0x000A0D4C
		public Task<Server> AddOrGetServer(string serverName)
		{
			ServerDbManager.<AddOrGetServer>d__45 <AddOrGetServer>d__;
			<AddOrGetServer>d__.<>t__builder = AsyncTaskMethodBuilder<Server>.Create();
			<AddOrGetServer>d__.<>4__this = this;
			<AddOrGetServer>d__.serverName = serverName;
			<AddOrGetServer>d__.<>1__state = -1;
			<AddOrGetServer>d__.<>t__builder.Start<ServerDbManager.<AddOrGetServer>d__45>(ref <AddOrGetServer>d__);
			return <AddOrGetServer>d__.<>t__builder.Task;
		}

		// Token: 0x06001EFF RID: 7935 RVA: 0x000A2B97 File Offset: 0x000A0D97
		public Task AddAdminLogs(List<QueuedLog> logs)
		{
			ServerDbManager.DbWriteOpsMetric.Inc(1.0);
			return this._db.AddAdminLogs(logs);
		}

		// Token: 0x06001F00 RID: 7936 RVA: 0x000A2BB8 File Offset: 0x000A0DB8
		public IAsyncEnumerable<string> GetAdminLogMessages([Nullable(2)] LogFilter filter = null)
		{
			ServerDbManager.DbReadOpsMetric.Inc(1.0);
			return this._db.GetAdminLogMessages(filter);
		}

		// Token: 0x06001F01 RID: 7937 RVA: 0x000A2BD9 File Offset: 0x000A0DD9
		public IAsyncEnumerable<SharedAdminLog> GetAdminLogs([Nullable(2)] LogFilter filter = null)
		{
			ServerDbManager.DbReadOpsMetric.Inc(1.0);
			return this._db.GetAdminLogs(filter);
		}

		// Token: 0x06001F02 RID: 7938 RVA: 0x000A2BFA File Offset: 0x000A0DFA
		public IAsyncEnumerable<JsonDocument> GetAdminLogsJson([Nullable(2)] LogFilter filter = null)
		{
			ServerDbManager.DbReadOpsMetric.Inc(1.0);
			return this._db.GetAdminLogsJson(filter);
		}

		// Token: 0x06001F03 RID: 7939 RVA: 0x000A2C1B File Offset: 0x000A0E1B
		public Task<bool> GetWhitelistStatusAsync(NetUserId player)
		{
			ServerDbManager.DbReadOpsMetric.Inc(1.0);
			return this._db.GetWhitelistStatusAsync(player);
		}

		// Token: 0x06001F04 RID: 7940 RVA: 0x000A2C3C File Offset: 0x000A0E3C
		public Task AddToWhitelistAsync(NetUserId player)
		{
			ServerDbManager.DbWriteOpsMetric.Inc(1.0);
			return this._db.AddToWhitelistAsync(player);
		}

		// Token: 0x06001F05 RID: 7941 RVA: 0x000A2C5D File Offset: 0x000A0E5D
		public Task RemoveFromWhitelistAsync(NetUserId player)
		{
			ServerDbManager.DbWriteOpsMetric.Inc(1.0);
			return this._db.RemoveFromWhitelistAsync(player);
		}

		// Token: 0x06001F06 RID: 7942 RVA: 0x000A2C7E File Offset: 0x000A0E7E
		public Task AddUploadedResourceLogAsync(NetUserId user, DateTime date, string path, byte[] data)
		{
			ServerDbManager.DbWriteOpsMetric.Inc(1.0);
			return this._db.AddUploadedResourceLogAsync(user, date, path, data);
		}

		// Token: 0x06001F07 RID: 7943 RVA: 0x000A2CA3 File Offset: 0x000A0EA3
		public Task PurgeUploadedResourceLogAsync(int days)
		{
			ServerDbManager.DbWriteOpsMetric.Inc(1.0);
			return this._db.PurgeUploadedResourceLogAsync(days);
		}

		// Token: 0x06001F08 RID: 7944 RVA: 0x000A2CC4 File Offset: 0x000A0EC4
		public Task<DateTime?> GetLastReadRules(NetUserId player)
		{
			ServerDbManager.DbReadOpsMetric.Inc(1.0);
			return this._db.GetLastReadRules(player);
		}

		// Token: 0x06001F09 RID: 7945 RVA: 0x000A2CE5 File Offset: 0x000A0EE5
		public Task SetLastReadRules(NetUserId player, DateTime time)
		{
			ServerDbManager.DbWriteOpsMetric.Inc(1.0);
			return this._db.SetLastReadRules(player, time);
		}

		// Token: 0x06001F0A RID: 7946 RVA: 0x000A2D08 File Offset: 0x000A0F08
		public Task<int> AddAdminNote(int? roundId, Guid player, string message, Guid createdBy, DateTime createdAt)
		{
			ServerDbManager.DbWriteOpsMetric.Inc(1.0);
			AdminNote note = new AdminNote
			{
				RoundId = roundId,
				CreatedById = createdBy,
				LastEditedById = createdBy,
				PlayerUserId = player,
				Message = message,
				CreatedAt = createdAt,
				LastEditedAt = createdAt
			};
			return this._db.AddAdminNote(note);
		}

		// Token: 0x06001F0B RID: 7947 RVA: 0x000A2D6F File Offset: 0x000A0F6F
		[return: Nullable(new byte[]
		{
			1,
			2
		})]
		public Task<AdminNote> GetAdminNote(int id)
		{
			ServerDbManager.DbReadOpsMetric.Inc(1.0);
			return this._db.GetAdminNote(id);
		}

		// Token: 0x06001F0C RID: 7948 RVA: 0x000A2D90 File Offset: 0x000A0F90
		public Task<List<AdminNote>> GetAdminNotes(Guid player)
		{
			ServerDbManager.DbReadOpsMetric.Inc(1.0);
			return this._db.GetAdminNotes(player);
		}

		// Token: 0x06001F0D RID: 7949 RVA: 0x000A2DB1 File Offset: 0x000A0FB1
		public Task DeleteAdminNote(int id, Guid deletedBy, DateTime deletedAt)
		{
			ServerDbManager.DbWriteOpsMetric.Inc(1.0);
			return this._db.DeleteAdminNote(id, deletedBy, deletedAt);
		}

		// Token: 0x06001F0E RID: 7950 RVA: 0x000A2DD4 File Offset: 0x000A0FD4
		public Task EditAdminNote(int id, string message, Guid editedBy, DateTime editedAt)
		{
			ServerDbManager.DbWriteOpsMetric.Inc(1.0);
			return this._db.EditAdminNote(id, message, editedBy, editedAt);
		}

		// Token: 0x06001F0F RID: 7951 RVA: 0x000A2DFC File Offset: 0x000A0FFC
		private DbContextOptions<PostgresServerDbContext> CreatePostgresOptions()
		{
			string host = this._cfg.GetCVar<string>(CCVars.DatabasePgHost);
			int port = this._cfg.GetCVar<int>(CCVars.DatabasePgPort);
			string db = this._cfg.GetCVar<string>(CCVars.DatabasePgDatabase);
			string user = this._cfg.GetCVar<string>(CCVars.DatabasePgUsername);
			string pass = this._cfg.GetCVar<string>(CCVars.DatabasePgPassword);
			DbContextOptionsBuilder<PostgresServerDbContext> builder = new DbContextOptionsBuilder<PostgresServerDbContext>();
			string connectionString = new NpgsqlConnectionStringBuilder
			{
				Host = host,
				Port = port,
				Database = db,
				Username = user,
				Password = pass
			}.ConnectionString;
			string text = "db.manager";
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(19, 3);
			defaultInterpolatedStringHandler.AppendLiteral("Using Postgres \"");
			defaultInterpolatedStringHandler.AppendFormatted(host);
			defaultInterpolatedStringHandler.AppendLiteral(":");
			defaultInterpolatedStringHandler.AppendFormatted<int>(port);
			defaultInterpolatedStringHandler.AppendLiteral("/");
			defaultInterpolatedStringHandler.AppendFormatted(db);
			defaultInterpolatedStringHandler.AppendLiteral("\"");
			Logger.DebugS(text, defaultInterpolatedStringHandler.ToStringAndClear());
			builder.UseNpgsql(connectionString, null);
			this.SetupLogging(builder);
			return builder.Options;
		}

		// Token: 0x06001F10 RID: 7952 RVA: 0x000A2F14 File Offset: 0x000A1114
		private DbContextOptions<SqliteServerDbContext> CreateSqliteOptions()
		{
			DbContextOptionsBuilder<SqliteServerDbContext> builder = new DbContextOptionsBuilder<SqliteServerDbContext>();
			string configPreferencesDbPath = this._cfg.GetCVar<string>(CCVars.DatabaseSqliteDbPath);
			SqliteConnection connection;
			if (this._res.UserData.RootDir != null)
			{
				string finalPreferencesDbPath = Path.Combine(this._res.UserData.RootDir, configPreferencesDbPath);
				Logger.DebugS("db.manager", "Using SQLite DB \"" + finalPreferencesDbPath + "\"");
				connection = new SqliteConnection("Data Source=" + finalPreferencesDbPath);
			}
			else
			{
				Logger.DebugS("db.manager", "Using in-memory SQLite DB");
				connection = new SqliteConnection("Data Source=:memory:");
				connection.Open();
			}
			builder.UseSqlite(connection, null);
			this.SetupLogging(builder);
			return builder.Options;
		}

		// Token: 0x06001F11 RID: 7953 RVA: 0x000A2FC7 File Offset: 0x000A11C7
		private void SetupLogging(DbContextOptionsBuilder builder)
		{
			builder.UseLoggerFactory(this._msLoggerFactory);
		}

		// Token: 0x04001365 RID: 4965
		public static readonly Counter DbReadOpsMetric = Metrics.CreateCounter("db_read_ops", "Amount of read operations processed by the database manager.", null);

		// Token: 0x04001366 RID: 4966
		public static readonly Counter DbWriteOpsMetric = Metrics.CreateCounter("db_write_ops", "Amount of write operations processed by the database manager.", null);

		// Token: 0x04001367 RID: 4967
		[Dependency]
		private readonly IConfigurationManager _cfg;

		// Token: 0x04001368 RID: 4968
		[Dependency]
		private readonly IResourceManager _res;

		// Token: 0x04001369 RID: 4969
		[Dependency]
		private readonly ILogManager _logMgr;

		// Token: 0x0400136A RID: 4970
		private ServerDbBase _db;

		// Token: 0x0400136B RID: 4971
		private ServerDbManager.LoggingProvider _msLogProvider;

		// Token: 0x0400136C RID: 4972
		private ILoggerFactory _msLoggerFactory;

		// Token: 0x02000A7A RID: 2682
		[NullableContext(0)]
		private sealed class LoggingProvider : ILoggerProvider, IDisposable
		{
			// Token: 0x06003516 RID: 13590 RVA: 0x00115EEE File Offset: 0x001140EE
			[NullableContext(1)]
			public LoggingProvider(ILogManager logManager)
			{
				this._logManager = logManager;
			}

			// Token: 0x06003517 RID: 13591 RVA: 0x00115EFD File Offset: 0x001140FD
			public void Dispose()
			{
			}

			// Token: 0x06003518 RID: 13592 RVA: 0x00115EFF File Offset: 0x001140FF
			[NullableContext(1)]
			public ILogger CreateLogger(string categoryName)
			{
				return new ServerDbManager.MSLogger(this._logManager.GetSawmill("db.ef"));
			}

			// Token: 0x040025ED RID: 9709
			[Nullable(1)]
			private readonly ILogManager _logManager;
		}

		// Token: 0x02000A7B RID: 2683
		[Nullable(0)]
		private sealed class MSLogger : ILogger
		{
			// Token: 0x06003519 RID: 13593 RVA: 0x00115F16 File Offset: 0x00114116
			public MSLogger(ISawmill sawmill)
			{
				this._sawmill = sawmill;
			}

			// Token: 0x0600351A RID: 13594 RVA: 0x00115F28 File Offset: 0x00114128
			[NullableContext(2)]
			public void Log<TState>(LogLevel logLevel, EventId eventId, [Nullable(1)] TState state, Exception exception, [Nullable(new byte[]
			{
				1,
				1,
				2,
				1
			})] Func<TState, Exception, string> formatter)
			{
				LogLevel logLevel2;
				switch (logLevel)
				{
				case LogLevel.Trace:
					logLevel2 = 1;
					break;
				case LogLevel.Debug:
					logLevel2 = 1;
					break;
				case LogLevel.Information:
					logLevel2 = 1;
					break;
				case LogLevel.Warning:
					logLevel2 = 3;
					break;
				case LogLevel.Error:
					logLevel2 = 4;
					break;
				case LogLevel.Critical:
					logLevel2 = 5;
					break;
				case LogLevel.None:
					logLevel2 = 1;
					break;
				default:
					logLevel2 = 1;
					break;
				}
				LogLevel lvl = logLevel2;
				this._sawmill.Log(lvl, formatter(state, exception));
			}

			// Token: 0x0600351B RID: 13595 RVA: 0x00115F8F File Offset: 0x0011418F
			public bool IsEnabled(LogLevel logLevel)
			{
				return true;
			}

			// Token: 0x0600351C RID: 13596 RVA: 0x00115F92 File Offset: 0x00114192
			public IDisposable BeginScope<[Nullable(2)] TState>(TState state)
			{
				return null;
			}

			// Token: 0x040025EE RID: 9710
			private readonly ISawmill _sawmill;
		}
	}
}
