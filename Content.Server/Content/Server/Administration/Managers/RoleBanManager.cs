using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Content.Server.Database;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;

namespace Content.Server.Administration.Managers
{
	// Token: 0x02000817 RID: 2071
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RoleBanManager
	{
		// Token: 0x06002D49 RID: 11593 RVA: 0x000EE6D6 File Offset: 0x000EC8D6
		public void Initialize()
		{
			this._playerManager.PlayerStatusChanged += this.OnPlayerStatusChanged;
		}

		// Token: 0x06002D4A RID: 11594 RVA: 0x000EE6F0 File Offset: 0x000EC8F0
		private void OnPlayerStatusChanged([Nullable(2)] object sender, SessionStatusEventArgs e)
		{
			RoleBanManager.<OnPlayerStatusChanged>d__7 <OnPlayerStatusChanged>d__;
			<OnPlayerStatusChanged>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<OnPlayerStatusChanged>d__.<>4__this = this;
			<OnPlayerStatusChanged>d__.e = e;
			<OnPlayerStatusChanged>d__.<>1__state = -1;
			<OnPlayerStatusChanged>d__.<>t__builder.Start<RoleBanManager.<OnPlayerStatusChanged>d__7>(ref <OnPlayerStatusChanged>d__);
		}

		// Token: 0x06002D4B RID: 11595 RVA: 0x000EE730 File Offset: 0x000EC930
		private Task<bool> AddRoleBan(ServerRoleBanDef banDef)
		{
			RoleBanManager.<AddRoleBan>d__8 <AddRoleBan>d__;
			<AddRoleBan>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<AddRoleBan>d__.<>4__this = this;
			<AddRoleBan>d__.banDef = banDef;
			<AddRoleBan>d__.<>1__state = -1;
			<AddRoleBan>d__.<>t__builder.Start<RoleBanManager.<AddRoleBan>d__8>(ref <AddRoleBan>d__);
			return <AddRoleBan>d__.<>t__builder.Task;
		}

		// Token: 0x06002D4C RID: 11596 RVA: 0x000EE77C File Offset: 0x000EC97C
		[return: Nullable(new byte[]
		{
			2,
			1
		})]
		public HashSet<string> GetRoleBans(NetUserId playerUserId)
		{
			HashSet<ServerRoleBanDef> roleBans;
			if (!this._cachedRoleBans.TryGetValue(playerUserId, out roleBans))
			{
				return null;
			}
			return (from banDef in roleBans
			select banDef.Role).ToHashSet<string>();
		}

		// Token: 0x06002D4D RID: 11597 RVA: 0x000EE7C8 File Offset: 0x000EC9C8
		[NullableContext(0)]
		[return: Nullable(1)]
		private Task CacheDbRoleBans(NetUserId userId, [Nullable(2)] IPAddress address = null, ImmutableArray<byte>? hwId = null)
		{
			RoleBanManager.<CacheDbRoleBans>d__10 <CacheDbRoleBans>d__;
			<CacheDbRoleBans>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<CacheDbRoleBans>d__.<>4__this = this;
			<CacheDbRoleBans>d__.userId = userId;
			<CacheDbRoleBans>d__.address = address;
			<CacheDbRoleBans>d__.hwId = hwId;
			<CacheDbRoleBans>d__.<>1__state = -1;
			<CacheDbRoleBans>d__.<>t__builder.Start<RoleBanManager.<CacheDbRoleBans>d__10>(ref <CacheDbRoleBans>d__);
			return <CacheDbRoleBans>d__.<>t__builder.Task;
		}

		// Token: 0x06002D4E RID: 11598 RVA: 0x000EE824 File Offset: 0x000ECA24
		public void Restart()
		{
			List<NetUserId> toRemove = new List<NetUserId>();
			foreach (NetUserId player in this._cachedRoleBans.Keys)
			{
				IPlayerSession playerSession;
				if (!this._playerManager.TryGetSessionById(player, ref playerSession))
				{
					toRemove.Add(player);
				}
			}
			foreach (NetUserId player2 in toRemove)
			{
				this._cachedRoleBans.Remove(player2);
			}
			foreach (KeyValuePair<NetUserId, HashSet<ServerRoleBanDef>> keyValuePair in this._cachedRoleBans)
			{
				NetUserId netUserId;
				HashSet<ServerRoleBanDef> hashSet;
				keyValuePair.Deconstruct(out netUserId, out hashSet);
				hashSet.RemoveWhere((ServerRoleBanDef ban) => DateTimeOffset.Now > ban.ExpirationTime);
			}
		}

		// Token: 0x06002D4F RID: 11599 RVA: 0x000EE948 File Offset: 0x000ECB48
		public void CreateJobBan(IConsoleShell shell, string target, string job, string reason, uint minutes)
		{
			RoleBanManager.<CreateJobBan>d__12 <CreateJobBan>d__;
			<CreateJobBan>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<CreateJobBan>d__.<>4__this = this;
			<CreateJobBan>d__.shell = shell;
			<CreateJobBan>d__.target = target;
			<CreateJobBan>d__.job = job;
			<CreateJobBan>d__.reason = reason;
			<CreateJobBan>d__.minutes = minutes;
			<CreateJobBan>d__.<>1__state = -1;
			<CreateJobBan>d__.<>t__builder.Start<RoleBanManager.<CreateJobBan>d__12>(ref <CreateJobBan>d__);
		}

		// Token: 0x06002D50 RID: 11600 RVA: 0x000EE9AC File Offset: 0x000ECBAC
		[return: Nullable(new byte[]
		{
			2,
			1
		})]
		public HashSet<string> GetJobBans(NetUserId playerUserId)
		{
			HashSet<ServerRoleBanDef> roleBans;
			if (!this._cachedRoleBans.TryGetValue(playerUserId, out roleBans))
			{
				return null;
			}
			return (from ban in roleBans
			where ban.Role.StartsWith("Job:", StringComparison.Ordinal)
			select ban).Select(delegate(ServerRoleBanDef ban)
			{
				string role = ban.Role;
				int length = "Job:".Length;
				return role.Substring(length, role.Length - length);
			}).ToHashSet<string>();
		}

		// Token: 0x06002D51 RID: 11601 RVA: 0x000EEA1C File Offset: 0x000ECC1C
		private void CreateRoleBan(IConsoleShell shell, string target, string role, string reason, uint minutes)
		{
			RoleBanManager.<CreateRoleBan>d__14 <CreateRoleBan>d__;
			<CreateRoleBan>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<CreateRoleBan>d__.<>4__this = this;
			<CreateRoleBan>d__.shell = shell;
			<CreateRoleBan>d__.target = target;
			<CreateRoleBan>d__.role = role;
			<CreateRoleBan>d__.reason = reason;
			<CreateRoleBan>d__.minutes = minutes;
			<CreateRoleBan>d__.<>1__state = -1;
			<CreateRoleBan>d__.<>t__builder.Start<RoleBanManager.<CreateRoleBan>d__14>(ref <CreateRoleBan>d__);
		}

		// Token: 0x04001BFD RID: 7165
		[Dependency]
		private readonly IServerDbManager _db;

		// Token: 0x04001BFE RID: 7166
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04001BFF RID: 7167
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04001C00 RID: 7168
		[Dependency]
		private readonly IPlayerLocator _playerLocator;

		// Token: 0x04001C01 RID: 7169
		private const string JobPrefix = "Job:";

		// Token: 0x04001C02 RID: 7170
		private readonly Dictionary<NetUserId, HashSet<ServerRoleBanDef>> _cachedRoleBans = new Dictionary<NetUserId, HashSet<ServerRoleBanDef>>();
	}
}
