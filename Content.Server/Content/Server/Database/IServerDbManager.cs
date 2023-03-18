using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Content.Server.Administration.Logs;
using Content.Shared.Administration.Logs;
using Content.Shared.Preferences;
using Robust.Shared.Maths;
using Robust.Shared.Network;

namespace Content.Server.Database
{
	// Token: 0x020005B7 RID: 1463
	[NullableContext(1)]
	public interface IServerDbManager
	{
		// Token: 0x06001EA3 RID: 7843
		void Init();

		// Token: 0x06001EA4 RID: 7844
		Task<PlayerPreferences> InitPrefsAsync(NetUserId userId, ICharacterProfile defaultProfile);

		// Token: 0x06001EA5 RID: 7845
		Task SaveSelectedCharacterIndexAsync(NetUserId userId, int index);

		// Token: 0x06001EA6 RID: 7846
		Task SaveCharacterSlotAsync(NetUserId userId, [Nullable(2)] ICharacterProfile profile, int slot);

		// Token: 0x06001EA7 RID: 7847
		Task SaveAdminOOCColorAsync(NetUserId userId, Color color);

		// Token: 0x06001EA8 RID: 7848
		Task DeleteSlotAndSetSelectedIndex(NetUserId userId, int deleteSlot, int newSlot);

		// Token: 0x06001EA9 RID: 7849
		[return: Nullable(new byte[]
		{
			1,
			2
		})]
		Task<PlayerPreferences> GetPlayerPreferencesAsync(NetUserId userId);

		// Token: 0x06001EAA RID: 7850
		Task AssignUserIdAsync(string name, NetUserId userId);

		// Token: 0x06001EAB RID: 7851
		Task<NetUserId?> GetAssignedUserIdAsync(string name);

		// Token: 0x06001EAC RID: 7852
		[return: Nullable(new byte[]
		{
			1,
			2
		})]
		Task<ServerBanDef> GetServerBanAsync(int id);

		// Token: 0x06001EAD RID: 7853
		[NullableContext(0)]
		[return: Nullable(new byte[]
		{
			1,
			2
		})]
		Task<ServerBanDef> GetServerBanAsync([Nullable(2)] IPAddress address, NetUserId? userId, ImmutableArray<byte>? hwId);

		// Token: 0x06001EAE RID: 7854
		[NullableContext(0)]
		[return: Nullable(1)]
		Task<List<ServerBanDef>> GetServerBansAsync([Nullable(2)] IPAddress address, NetUserId? userId, ImmutableArray<byte>? hwId, bool includeUnbanned = true);

		// Token: 0x06001EAF RID: 7855
		Task AddServerBanAsync(ServerBanDef serverBan);

		// Token: 0x06001EB0 RID: 7856
		Task AddServerUnbanAsync(ServerUnbanDef serverBan);

		// Token: 0x06001EB1 RID: 7857
		[return: Nullable(new byte[]
		{
			1,
			2
		})]
		Task<ServerRoleBanDef> GetServerRoleBanAsync(int id);

		// Token: 0x06001EB2 RID: 7858
		[NullableContext(0)]
		[return: Nullable(1)]
		Task<List<ServerRoleBanDef>> GetServerRoleBansAsync([Nullable(2)] IPAddress address, NetUserId? userId, ImmutableArray<byte>? hwId, bool includeUnbanned = true);

		// Token: 0x06001EB3 RID: 7859
		Task AddServerRoleBanAsync(ServerRoleBanDef serverBan);

		// Token: 0x06001EB4 RID: 7860
		Task AddServerRoleUnbanAsync(ServerRoleUnbanDef serverBan);

		// Token: 0x06001EB5 RID: 7861
		Task<List<PlayTime>> GetPlayTimes(Guid player);

		// Token: 0x06001EB6 RID: 7862
		Task UpdatePlayTimes(IReadOnlyCollection<PlayTimeUpdate> updates);

		// Token: 0x06001EB7 RID: 7863
		Task UpdatePlayerRecordAsync(NetUserId userId, string userName, IPAddress address, [Nullable(0)] ImmutableArray<byte> hwId);

		// Token: 0x06001EB8 RID: 7864
		[return: Nullable(new byte[]
		{
			1,
			2
		})]
		Task<PlayerRecord> GetPlayerRecordByUserName(string userName, CancellationToken cancel = default(CancellationToken));

		// Token: 0x06001EB9 RID: 7865
		[return: Nullable(new byte[]
		{
			1,
			2
		})]
		Task<PlayerRecord> GetPlayerRecordByUserId(NetUserId userId, CancellationToken cancel = default(CancellationToken));

		// Token: 0x06001EBA RID: 7866
		Task<int> AddConnectionLogAsync(NetUserId userId, string userName, IPAddress address, [Nullable(0)] ImmutableArray<byte> hwId, ConnectionDenyReason? denied);

		// Token: 0x06001EBB RID: 7867
		Task AddServerBanHitsAsync(int connection, IEnumerable<ServerBanDef> bans);

		// Token: 0x06001EBC RID: 7868
		[return: Nullable(new byte[]
		{
			1,
			2
		})]
		Task<Admin> GetAdminDataForAsync(NetUserId userId, CancellationToken cancel = default(CancellationToken));

		// Token: 0x06001EBD RID: 7869
		[return: Nullable(new byte[]
		{
			1,
			2
		})]
		Task<AdminRank> GetAdminRankAsync(int id, CancellationToken cancel = default(CancellationToken));

		// Token: 0x06001EBE RID: 7870
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
		Task<ValueTuple<ValueTuple<Admin, string>[], AdminRank[]>> GetAllAdminAndRanksAsync(CancellationToken cancel = default(CancellationToken));

		// Token: 0x06001EBF RID: 7871
		Task RemoveAdminAsync(NetUserId userId, CancellationToken cancel = default(CancellationToken));

		// Token: 0x06001EC0 RID: 7872
		Task AddAdminAsync(Admin admin, CancellationToken cancel = default(CancellationToken));

		// Token: 0x06001EC1 RID: 7873
		Task UpdateAdminAsync(Admin admin, CancellationToken cancel = default(CancellationToken));

		// Token: 0x06001EC2 RID: 7874
		Task RemoveAdminRankAsync(int rankId, CancellationToken cancel = default(CancellationToken));

		// Token: 0x06001EC3 RID: 7875
		Task AddAdminRankAsync(AdminRank rank, CancellationToken cancel = default(CancellationToken));

		// Token: 0x06001EC4 RID: 7876
		Task UpdateAdminRankAsync(AdminRank rank, CancellationToken cancel = default(CancellationToken));

		// Token: 0x06001EC5 RID: 7877
		Task<int> AddNewRound(Server server, params Guid[] playerIds);

		// Token: 0x06001EC6 RID: 7878
		Task<Round> GetRound(int id);

		// Token: 0x06001EC7 RID: 7879
		Task AddRoundPlayers(int id, params Guid[] playerIds);

		// Token: 0x06001EC8 RID: 7880
		Task<Server> AddOrGetServer(string serverName);

		// Token: 0x06001EC9 RID: 7881
		Task AddAdminLogs(List<QueuedLog> logs);

		// Token: 0x06001ECA RID: 7882
		IAsyncEnumerable<string> GetAdminLogMessages([Nullable(2)] LogFilter filter = null);

		// Token: 0x06001ECB RID: 7883
		IAsyncEnumerable<SharedAdminLog> GetAdminLogs([Nullable(2)] LogFilter filter = null);

		// Token: 0x06001ECC RID: 7884
		IAsyncEnumerable<JsonDocument> GetAdminLogsJson([Nullable(2)] LogFilter filter = null);

		// Token: 0x06001ECD RID: 7885
		Task<bool> GetWhitelistStatusAsync(NetUserId player);

		// Token: 0x06001ECE RID: 7886
		Task AddToWhitelistAsync(NetUserId player);

		// Token: 0x06001ECF RID: 7887
		Task RemoveFromWhitelistAsync(NetUserId player);

		// Token: 0x06001ED0 RID: 7888
		Task AddUploadedResourceLogAsync(NetUserId user, DateTime date, string path, byte[] data);

		// Token: 0x06001ED1 RID: 7889
		Task PurgeUploadedResourceLogAsync(int days);

		// Token: 0x06001ED2 RID: 7890
		Task<DateTime?> GetLastReadRules(NetUserId player);

		// Token: 0x06001ED3 RID: 7891
		Task SetLastReadRules(NetUserId player, DateTime time);

		// Token: 0x06001ED4 RID: 7892
		Task<int> AddAdminNote(int? roundId, Guid player, string message, Guid createdBy, DateTime createdAt);

		// Token: 0x06001ED5 RID: 7893
		[return: Nullable(new byte[]
		{
			1,
			2
		})]
		Task<AdminNote> GetAdminNote(int id);

		// Token: 0x06001ED6 RID: 7894
		Task<List<AdminNote>> GetAdminNotes(Guid player);

		// Token: 0x06001ED7 RID: 7895
		Task DeleteAdminNote(int id, Guid deletedBy, DateTime deletedAt);

		// Token: 0x06001ED8 RID: 7896
		Task EditAdminNote(int id, string message, Guid editedBy, DateTime editedAt);
	}
}
