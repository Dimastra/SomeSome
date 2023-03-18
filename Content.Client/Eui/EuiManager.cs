using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Eui;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Reflection;

namespace Content.Client.Eui
{
	// Token: 0x0200032E RID: 814
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class EuiManager
	{
		// Token: 0x0600146C RID: 5228 RVA: 0x00077BF4 File Offset: 0x00075DF4
		public void Initialize()
		{
			this._net.RegisterNetMessage<MsgEuiCtl>(new ProcessMessage<MsgEuiCtl>(this.RxMsgCtl), 3);
			this._net.RegisterNetMessage<MsgEuiState>(new ProcessMessage<MsgEuiState>(this.RxMsgState), 3);
			this._net.RegisterNetMessage<MsgEuiMessage>(new ProcessMessage<MsgEuiMessage>(this.RxMsgMessage), 3);
			this._net.Disconnect += this.NetOnDisconnect;
		}

		// Token: 0x0600146D RID: 5229 RVA: 0x00077C60 File Offset: 0x00075E60
		private void NetOnDisconnect([Nullable(2)] object sender, NetDisconnectedArgs e)
		{
			foreach (KeyValuePair<uint, EuiManager.EuiData> keyValuePair in this._openUis)
			{
				keyValuePair.Value.Eui.Closed();
			}
			this._openUis.Clear();
		}

		// Token: 0x0600146E RID: 5230 RVA: 0x00077CC8 File Offset: 0x00075EC8
		private void RxMsgMessage(MsgEuiMessage message)
		{
			this._openUis[message.Id].Eui.HandleMessage(message.Message);
		}

		// Token: 0x0600146F RID: 5231 RVA: 0x00077CEB File Offset: 0x00075EEB
		private void RxMsgState(MsgEuiState message)
		{
			this._openUis[message.Id].Eui.HandleState(message.State);
		}

		// Token: 0x06001470 RID: 5232 RVA: 0x00077D10 File Offset: 0x00075F10
		private void RxMsgCtl(MsgEuiCtl message)
		{
			EuiManager.EuiData euiData;
			if (this._openUis.TryGetValue(message.Id, out euiData))
			{
				euiData.Eui.Closed();
				this._openUis.Remove(message.Id);
			}
			if (message.Type != MsgEuiCtl.CtlType.Open)
			{
				return;
			}
			Type type = this._refl.LooseGetType(message.OpenType);
			BaseEui baseEui = DynamicTypeFactoryExt.CreateInstance<BaseEui>(this._dtf, type);
			baseEui.Initialize(this, message.Id);
			baseEui.Opened();
			this._openUis.Add(message.Id, new EuiManager.EuiData(baseEui));
		}

		// Token: 0x04000A53 RID: 2643
		[Dependency]
		private readonly IClientNetManager _net;

		// Token: 0x04000A54 RID: 2644
		[Dependency]
		private readonly IReflectionManager _refl;

		// Token: 0x04000A55 RID: 2645
		[Dependency]
		private readonly IDynamicTypeFactory _dtf;

		// Token: 0x04000A56 RID: 2646
		private readonly Dictionary<uint, EuiManager.EuiData> _openUis = new Dictionary<uint, EuiManager.EuiData>();

		// Token: 0x0200032F RID: 815
		[Nullable(0)]
		private sealed class EuiData
		{
			// Token: 0x06001472 RID: 5234 RVA: 0x00077DB4 File Offset: 0x00075FB4
			public EuiData(BaseEui eui)
			{
				this.Eui = eui;
			}

			// Token: 0x04000A57 RID: 2647
			public readonly BaseEui Eui;
		}
	}
}
