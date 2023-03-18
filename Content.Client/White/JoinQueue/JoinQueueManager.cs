using System;
using System.Runtime.CompilerServices;
using Content.Shared.White.JoinQueue;
using Robust.Client.State;
using Robust.Shared.IoC;
using Robust.Shared.Network;

namespace Content.Client.White.JoinQueue
{
	// Token: 0x02000023 RID: 35
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class JoinQueueManager
	{
		// Token: 0x06000081 RID: 129 RVA: 0x00004D49 File Offset: 0x00002F49
		public void Initialize()
		{
			this._netManager.RegisterNetMessage<MsgQueueUpdate>(new ProcessMessage<MsgQueueUpdate>(this.OnQueueUpdate), 3);
		}

		// Token: 0x06000082 RID: 130 RVA: 0x00004D63 File Offset: 0x00002F63
		private void OnQueueUpdate(MsgQueueUpdate msg)
		{
			if (!(this._stateManager.CurrentState is QueueState))
			{
				this._stateManager.RequestStateChange<QueueState>();
			}
			((QueueState)this._stateManager.CurrentState).OnQueueUpdate(msg);
		}

		// Token: 0x0400004C RID: 76
		[Dependency]
		private readonly IClientNetManager _netManager;

		// Token: 0x0400004D RID: 77
		[Dependency]
		private readonly IStateManager _stateManager;
	}
}
