using System;
using System.Runtime.CompilerServices;
using Content.Shared.Eui;
using Robust.Server.Player;
using Robust.Shared.IoC;
using Robust.Shared.Network;

namespace Content.Server.EUI
{
	// Token: 0x02000525 RID: 1317
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class BaseEui
	{
		// Token: 0x17000407 RID: 1031
		// (get) Token: 0x06001B5A RID: 7002 RVA: 0x00092948 File Offset: 0x00090B48
		// (set) Token: 0x06001B5B RID: 7003 RVA: 0x00092950 File Offset: 0x00090B50
		public IPlayerSession Player { get; private set; }

		// Token: 0x17000408 RID: 1032
		// (get) Token: 0x06001B5C RID: 7004 RVA: 0x00092959 File Offset: 0x00090B59
		// (set) Token: 0x06001B5D RID: 7005 RVA: 0x00092961 File Offset: 0x00090B61
		public bool IsShutDown { get; private set; }

		// Token: 0x17000409 RID: 1033
		// (get) Token: 0x06001B5E RID: 7006 RVA: 0x0009296A File Offset: 0x00090B6A
		// (set) Token: 0x06001B5F RID: 7007 RVA: 0x00092972 File Offset: 0x00090B72
		public EuiManager Manager { get; private set; }

		// Token: 0x1700040A RID: 1034
		// (get) Token: 0x06001B60 RID: 7008 RVA: 0x0009297B File Offset: 0x00090B7B
		// (set) Token: 0x06001B61 RID: 7009 RVA: 0x00092983 File Offset: 0x00090B83
		public uint Id { get; private set; }

		// Token: 0x06001B62 RID: 7010 RVA: 0x0009298C File Offset: 0x00090B8C
		public virtual void Opened()
		{
		}

		// Token: 0x06001B63 RID: 7011 RVA: 0x0009298E File Offset: 0x00090B8E
		public virtual void Closed()
		{
		}

		// Token: 0x06001B64 RID: 7012 RVA: 0x00092990 File Offset: 0x00090B90
		public virtual void HandleMessage(EuiMessageBase msg)
		{
		}

		// Token: 0x06001B65 RID: 7013 RVA: 0x00092992 File Offset: 0x00090B92
		public void StateDirty()
		{
			if (this._isStateDirty)
			{
				return;
			}
			this._isStateDirty = true;
			this.Manager.QueueStateUpdate(this);
		}

		// Token: 0x06001B66 RID: 7014 RVA: 0x000929B0 File Offset: 0x00090BB0
		public virtual EuiStateBase GetNewState()
		{
			throw new NotSupportedException();
		}

		// Token: 0x06001B67 RID: 7015 RVA: 0x000929B8 File Offset: 0x00090BB8
		public void SendMessage(EuiMessageBase message)
		{
			IoCManager.Resolve<IServerNetManager>().ServerSendMessage(new MsgEuiMessage
			{
				Id = this.Id,
				Message = message
			}, this.Player.ConnectedClient);
		}

		// Token: 0x06001B68 RID: 7016 RVA: 0x000929F4 File Offset: 0x00090BF4
		public void Close()
		{
			this.Manager.CloseEui(this);
		}

		// Token: 0x06001B69 RID: 7017 RVA: 0x00092A02 File Offset: 0x00090C02
		internal void Shutdown()
		{
			this.Closed();
			this.IsShutDown = true;
		}

		// Token: 0x06001B6A RID: 7018 RVA: 0x00092A14 File Offset: 0x00090C14
		internal void DoStateUpdate()
		{
			this._isStateDirty = false;
			EuiStateBase state = this.GetNewState();
			IoCManager.Resolve<IServerNetManager>().ServerSendMessage(new MsgEuiState
			{
				Id = this.Id,
				State = state
			}, this.Player.ConnectedClient);
		}

		// Token: 0x06001B6B RID: 7019 RVA: 0x00092A5E File Offset: 0x00090C5E
		internal void Initialize(EuiManager manager, IPlayerSession player, uint id)
		{
			this.Manager = manager;
			this.Player = player;
			this.Id = id;
			this.Opened();
		}

		// Token: 0x04001197 RID: 4503
		private bool _isStateDirty;
	}
}
