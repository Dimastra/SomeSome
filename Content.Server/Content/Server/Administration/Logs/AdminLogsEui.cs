using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Server.Administration.Managers;
using Content.Server.EUI;
using Content.Server.GameTicking;
using Content.Shared.Administration;
using Content.Shared.Administration.Logs;
using Content.Shared.CCVar;
using Content.Shared.Database;
using Content.Shared.Eui;
using Microsoft.Extensions.ObjectPool;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Utility;

namespace Content.Server.Administration.Logs
{
	// Token: 0x02000819 RID: 2073
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AdminLogsEui : BaseEui
	{
		// Token: 0x06002D76 RID: 11638 RVA: 0x000EF698 File Offset: 0x000ED898
		public AdminLogsEui()
		{
			IoCManager.InjectDependencies<AdminLogsEui>(this);
			this._sawmill = this._logManager.GetSawmill("admin.logs");
			this._configuration.OnValueChanged<int>(CCVars.AdminLogsClientBatchSize, new Action<int>(this.ClientBatchSizeChanged), true);
			this._filter = new LogFilter
			{
				CancellationToken = this._logSendCancellation.Token,
				Limit = new int?(this._clientBatchSize)
			};
		}

		// Token: 0x170006FD RID: 1789
		// (get) Token: 0x06002D77 RID: 11639 RVA: 0x000EF73F File Offset: 0x000ED93F
		public int CurrentRoundId
		{
			get
			{
				return EntitySystem.Get<GameTicker>().RoundId;
			}
		}

		// Token: 0x06002D78 RID: 11640 RVA: 0x000EF74C File Offset: 0x000ED94C
		public override void Opened()
		{
			AdminLogsEui.<Opened>d__14 <Opened>d__;
			<Opened>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Opened>d__.<>4__this = this;
			<Opened>d__.<>1__state = -1;
			<Opened>d__.<>t__builder.Start<AdminLogsEui.<Opened>d__14>(ref <Opened>d__);
		}

		// Token: 0x06002D79 RID: 11641 RVA: 0x000EF783 File Offset: 0x000ED983
		private void ClientBatchSizeChanged(int value)
		{
			this._clientBatchSize = value;
		}

		// Token: 0x06002D7A RID: 11642 RVA: 0x000EF78C File Offset: 0x000ED98C
		private void OnPermsChanged(AdminPermsChangedEventArgs args)
		{
			if (args.Player == base.Player && !this._adminManager.HasAdminFlag(base.Player, AdminFlags.Logs))
			{
				base.Close();
			}
		}

		// Token: 0x06002D7B RID: 11643 RVA: 0x000EF7BA File Offset: 0x000ED9BA
		public override EuiStateBase GetNewState()
		{
			if (this._isLoading)
			{
				return new AdminLogsEuiState(this.CurrentRoundId, new Dictionary<Guid, string>())
				{
					IsLoading = true
				};
			}
			return new AdminLogsEuiState(this.CurrentRoundId, this._players);
		}

		// Token: 0x06002D7C RID: 11644 RVA: 0x000EF7F0 File Offset: 0x000ED9F0
		public override void HandleMessage(EuiMessageBase msg)
		{
			AdminLogsEui.<HandleMessage>d__18 <HandleMessage>d__;
			<HandleMessage>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<HandleMessage>d__.<>4__this = this;
			<HandleMessage>d__.msg = msg;
			<HandleMessage>d__.<>1__state = -1;
			<HandleMessage>d__.<>t__builder.Start<AdminLogsEui.<HandleMessage>d__18>(ref <HandleMessage>d__);
		}

		// Token: 0x06002D7D RID: 11645 RVA: 0x000EF830 File Offset: 0x000EDA30
		[NullableContext(2)]
		public void SetLogFilter(string search = null, bool invertTypes = false, HashSet<LogType> types = null)
		{
			AdminLogsEuiMsg.SetLogFilter message = new AdminLogsEuiMsg.SetLogFilter(search, invertTypes, types);
			base.SendMessage(message);
		}

		// Token: 0x06002D7E RID: 11646 RVA: 0x000EF850 File Offset: 0x000EDA50
		private void SendLogs(bool replace)
		{
			AdminLogsEui.<SendLogs>d__20 <SendLogs>d__;
			<SendLogs>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<SendLogs>d__.<>4__this = this;
			<SendLogs>d__.replace = replace;
			<SendLogs>d__.<>1__state = -1;
			<SendLogs>d__.<>t__builder.Start<AdminLogsEui.<SendLogs>d__20>(ref <SendLogs>d__);
		}

		// Token: 0x06002D7F RID: 11647 RVA: 0x000EF890 File Offset: 0x000EDA90
		public override void Closed()
		{
			base.Closed();
			this._configuration.UnsubValueChanged<int>(CCVars.AdminLogsClientBatchSize, new Action<int>(this.ClientBatchSizeChanged));
			this._adminManager.OnPermsChanged -= this.OnPermsChanged;
			this._logSendCancellation.Cancel();
			this._logSendCancellation.Dispose();
		}

		// Token: 0x06002D80 RID: 11648 RVA: 0x000EF8EC File Offset: 0x000EDAEC
		private void LoadFromDb(int roundId)
		{
			AdminLogsEui.<LoadFromDb>d__22 <LoadFromDb>d__;
			<LoadFromDb>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<LoadFromDb>d__.<>4__this = this;
			<LoadFromDb>d__.roundId = roundId;
			<LoadFromDb>d__.<>1__state = -1;
			<LoadFromDb>d__.<>t__builder.Start<AdminLogsEui.<LoadFromDb>d__22>(ref <LoadFromDb>d__);
		}

		// Token: 0x04001C26 RID: 7206
		[Dependency]
		private readonly IAdminLogManager _adminLogs;

		// Token: 0x04001C27 RID: 7207
		[Dependency]
		private readonly IAdminManager _adminManager;

		// Token: 0x04001C28 RID: 7208
		[Dependency]
		private readonly ILogManager _logManager;

		// Token: 0x04001C29 RID: 7209
		[Dependency]
		private readonly IConfigurationManager _configuration;

		// Token: 0x04001C2A RID: 7210
		private readonly ISawmill _sawmill;

		// Token: 0x04001C2B RID: 7211
		private int _clientBatchSize;

		// Token: 0x04001C2C RID: 7212
		private bool _isLoading = true;

		// Token: 0x04001C2D RID: 7213
		private readonly Dictionary<Guid, string> _players = new Dictionary<Guid, string>();

		// Token: 0x04001C2E RID: 7214
		private CancellationTokenSource _logSendCancellation = new CancellationTokenSource();

		// Token: 0x04001C2F RID: 7215
		private LogFilter _filter;

		// Token: 0x04001C30 RID: 7216
		private DefaultObjectPool<List<SharedAdminLog>> _adminLogListPool = new DefaultObjectPool<List<SharedAdminLog>>(new ListPolicy<SharedAdminLog>());
	}
}
