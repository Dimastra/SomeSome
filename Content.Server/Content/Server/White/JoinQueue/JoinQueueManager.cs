using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Server.Connection;
using Content.Shared.CCVar;
using Content.Shared.White.JoinQueue;
using Robust.Server.Player;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Timing;

namespace Content.Server.White.JoinQueue
{
	// Token: 0x02000099 RID: 153
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class JoinQueueManager
	{
		// Token: 0x1700004D RID: 77
		// (get) Token: 0x06000260 RID: 608 RVA: 0x0000CECE File Offset: 0x0000B0CE
		public int PlayerInQueueCount
		{
			get
			{
				return this._queue.Count;
			}
		}

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x06000261 RID: 609 RVA: 0x0000CEDB File Offset: 0x0000B0DB
		public int ActualPlayersCount
		{
			get
			{
				return this._playerManager.PlayerCount - this.PlayerInQueueCount;
			}
		}

		// Token: 0x06000262 RID: 610 RVA: 0x0000CEF0 File Offset: 0x0000B0F0
		public void Initialize()
		{
			this._netManager.RegisterNetMessage<MsgQueueUpdate>(null, 3);
			this._cfg.OnValueChanged<bool>(CCVars.QueueEnabled, new Action<bool>(this.OnQueueCVarChanged), true);
			this._playerManager.PlayerStatusChanged += this.OnPlayerStatusChanged;
		}

		// Token: 0x06000263 RID: 611 RVA: 0x0000CF40 File Offset: 0x0000B140
		private void OnQueueCVarChanged(bool value)
		{
			this._isEnabled = value;
			if (!value)
			{
				foreach (IPlayerSession playerSession in this._queue)
				{
					playerSession.ConnectedClient.Disconnect("Queue was disabled");
				}
			}
		}

		// Token: 0x06000264 RID: 612 RVA: 0x0000CFA4 File Offset: 0x0000B1A4
		private void OnPlayerStatusChanged([Nullable(2)] object sender, SessionStatusEventArgs e)
		{
			JoinQueueManager.<OnPlayerStatusChanged>d__12 <OnPlayerStatusChanged>d__;
			<OnPlayerStatusChanged>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<OnPlayerStatusChanged>d__.<>4__this = this;
			<OnPlayerStatusChanged>d__.e = e;
			<OnPlayerStatusChanged>d__.<>1__state = -1;
			<OnPlayerStatusChanged>d__.<>t__builder.Start<JoinQueueManager.<OnPlayerStatusChanged>d__12>(ref <OnPlayerStatusChanged>d__);
		}

		// Token: 0x06000265 RID: 613 RVA: 0x0000CFE4 File Offset: 0x0000B1E4
		private void ProcessQueue(bool isDisconnect)
		{
			int players = this.ActualPlayersCount;
			if (isDisconnect)
			{
				players--;
			}
			bool haveFreeSlot = players < this._cfg.GetCVar<int>(CCVars.SoftMaxPlayers);
			bool queueContains = this._queue.Count > 0;
			if ((!this._isEnabled || haveFreeSlot) && queueContains)
			{
				IPlayerSession session = this._queue.First<IPlayerSession>();
				this._queue.Remove(session);
				this.SendToGame(session);
			}
			this.SendUpdateMessages();
		}

		// Token: 0x06000266 RID: 614 RVA: 0x0000D058 File Offset: 0x0000B258
		private void SendUpdateMessages()
		{
			for (int i = 0; i < this._queue.Count; i++)
			{
				this._queue[i].ConnectedClient.SendMessage(new MsgQueueUpdate
				{
					Total = this._queue.Count,
					Position = i + 1
				});
			}
		}

		// Token: 0x06000267 RID: 615 RVA: 0x0000D0B0 File Offset: 0x0000B2B0
		private void SendToGame(IPlayerSession s)
		{
			Timer.Spawn(0, new Action(s.JoinGame), default(CancellationToken));
		}

		// Token: 0x040001BF RID: 447
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x040001C0 RID: 448
		[Dependency]
		private readonly IConnectionManager _connectionManager;

		// Token: 0x040001C1 RID: 449
		[Dependency]
		private readonly IConfigurationManager _cfg;

		// Token: 0x040001C2 RID: 450
		[Dependency]
		private readonly IServerNetManager _netManager;

		// Token: 0x040001C3 RID: 451
		private readonly List<IPlayerSession> _queue = new List<IPlayerSession>();

		// Token: 0x040001C4 RID: 452
		private bool _isEnabled;
	}
}
