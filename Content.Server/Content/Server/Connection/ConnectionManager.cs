using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Content.Server.Database;
using Content.Server.White.Sponsors;
using Robust.Server.Player;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Network;

namespace Content.Server.Connection
{
	// Token: 0x0200062D RID: 1581
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ConnectionManager : IConnectionManager
	{
		// Token: 0x060021A9 RID: 8617 RVA: 0x000AF634 File Offset: 0x000AD834
		public void Initialize()
		{
			this._netMgr.Connecting += this.NetMgrOnConnecting;
			this._netMgr.AssignUserIdCallback = new Func<string, Task<NetUserId?>>(this.AssignUserIdCallback);
		}

		// Token: 0x060021AA RID: 8618 RVA: 0x000AF664 File Offset: 0x000AD864
		private Task NetMgrOnConnecting(NetConnectingArgs e)
		{
			ConnectionManager.<NetMgrOnConnecting>d__7 <NetMgrOnConnecting>d__;
			<NetMgrOnConnecting>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<NetMgrOnConnecting>d__.<>4__this = this;
			<NetMgrOnConnecting>d__.e = e;
			<NetMgrOnConnecting>d__.<>1__state = -1;
			<NetMgrOnConnecting>d__.<>t__builder.Start<ConnectionManager.<NetMgrOnConnecting>d__7>(ref <NetMgrOnConnecting>d__);
			return <NetMgrOnConnecting>d__.<>t__builder.Task;
		}

		// Token: 0x060021AB RID: 8619 RVA: 0x000AF6B0 File Offset: 0x000AD8B0
		[return: TupleElementNames(new string[]
		{
			null,
			null,
			"bansHit"
		})]
		[return: Nullable(new byte[]
		{
			1,
			0,
			1,
			2,
			1
		})]
		private Task<ValueTuple<ConnectionDenyReason, string, List<ServerBanDef>>?> ShouldDeny(NetConnectingArgs e)
		{
			ConnectionManager.<ShouldDeny>d__8 <ShouldDeny>d__;
			<ShouldDeny>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<ConnectionDenyReason, string, List<ServerBanDef>>?>.Create();
			<ShouldDeny>d__.<>4__this = this;
			<ShouldDeny>d__.e = e;
			<ShouldDeny>d__.<>1__state = -1;
			<ShouldDeny>d__.<>t__builder.Start<ConnectionManager.<ShouldDeny>d__8>(ref <ShouldDeny>d__);
			return <ShouldDeny>d__.<>t__builder.Task;
		}

		// Token: 0x060021AC RID: 8620 RVA: 0x000AF6FC File Offset: 0x000AD8FC
		private Task<NetUserId?> AssignUserIdCallback(string name)
		{
			ConnectionManager.<AssignUserIdCallback>d__9 <AssignUserIdCallback>d__;
			<AssignUserIdCallback>d__.<>t__builder = AsyncTaskMethodBuilder<NetUserId?>.Create();
			<AssignUserIdCallback>d__.<>4__this = this;
			<AssignUserIdCallback>d__.name = name;
			<AssignUserIdCallback>d__.<>1__state = -1;
			<AssignUserIdCallback>d__.<>t__builder.Start<ConnectionManager.<AssignUserIdCallback>d__9>(ref <AssignUserIdCallback>d__);
			return <AssignUserIdCallback>d__.<>t__builder.Task;
		}

		// Token: 0x060021AD RID: 8621 RVA: 0x000AF748 File Offset: 0x000AD948
		public Task<bool> HavePrivilegedJoin(NetUserId userId)
		{
			ConnectionManager.<HavePrivilegedJoin>d__10 <HavePrivilegedJoin>d__;
			<HavePrivilegedJoin>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<HavePrivilegedJoin>d__.<>4__this = this;
			<HavePrivilegedJoin>d__.userId = userId;
			<HavePrivilegedJoin>d__.<>1__state = -1;
			<HavePrivilegedJoin>d__.<>t__builder.Start<ConnectionManager.<HavePrivilegedJoin>d__10>(ref <HavePrivilegedJoin>d__);
			return <HavePrivilegedJoin>d__.<>t__builder.Task;
		}

		// Token: 0x0400149C RID: 5276
		[Dependency]
		private readonly IServerDbManager _dbManager;

		// Token: 0x0400149D RID: 5277
		[Dependency]
		private readonly IPlayerManager _plyMgr;

		// Token: 0x0400149E RID: 5278
		[Dependency]
		private readonly IServerNetManager _netMgr;

		// Token: 0x0400149F RID: 5279
		[Dependency]
		private readonly IServerDbManager _db;

		// Token: 0x040014A0 RID: 5280
		[Dependency]
		private readonly IConfigurationManager _cfg;

		// Token: 0x040014A1 RID: 5281
		[Dependency]
		private readonly SponsorsManager _sponsorsManager;
	}
}
