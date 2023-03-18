using System;
using System.Runtime.CompilerServices;
using Content.Shared.Eui;
using Robust.Shared.IoC;
using Robust.Shared.Network;

namespace Content.Client.Eui
{
	// Token: 0x0200032D RID: 813
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class BaseEui
	{
		// Token: 0x1700042D RID: 1069
		// (get) Token: 0x06001461 RID: 5217 RVA: 0x00077B7B File Offset: 0x00075D7B
		// (set) Token: 0x06001462 RID: 5218 RVA: 0x00077B83 File Offset: 0x00075D83
		public EuiManager Manager { get; private set; }

		// Token: 0x1700042E RID: 1070
		// (get) Token: 0x06001463 RID: 5219 RVA: 0x00077B8C File Offset: 0x00075D8C
		// (set) Token: 0x06001464 RID: 5220 RVA: 0x00077B94 File Offset: 0x00075D94
		public uint Id { get; private set; }

		// Token: 0x06001465 RID: 5221 RVA: 0x00077B9D File Offset: 0x00075D9D
		protected BaseEui()
		{
			IoCManager.InjectDependencies<BaseEui>(this);
		}

		// Token: 0x06001466 RID: 5222 RVA: 0x00077BAC File Offset: 0x00075DAC
		internal void Initialize(EuiManager mgr, uint id)
		{
			this.Manager = mgr;
			this.Id = id;
		}

		// Token: 0x06001467 RID: 5223 RVA: 0x0001B008 File Offset: 0x00019208
		public virtual void Opened()
		{
		}

		// Token: 0x06001468 RID: 5224 RVA: 0x0001B008 File Offset: 0x00019208
		public virtual void Closed()
		{
		}

		// Token: 0x06001469 RID: 5225 RVA: 0x0001B008 File Offset: 0x00019208
		public virtual void HandleState(EuiStateBase state)
		{
		}

		// Token: 0x0600146A RID: 5226 RVA: 0x0001B008 File Offset: 0x00019208
		public virtual void HandleMessage(EuiMessageBase msg)
		{
		}

		// Token: 0x0600146B RID: 5227 RVA: 0x00077BBC File Offset: 0x00075DBC
		protected void SendMessage(EuiMessageBase msg)
		{
			MsgEuiMessage msgEuiMessage = this._netManager.CreateNetMessage<MsgEuiMessage>();
			msgEuiMessage.Id = this.Id;
			msgEuiMessage.Message = msg;
			this._netManager.ClientSendMessage(msgEuiMessage);
		}

		// Token: 0x04000A50 RID: 2640
		[Dependency]
		private readonly IClientNetManager _netManager;
	}
}
