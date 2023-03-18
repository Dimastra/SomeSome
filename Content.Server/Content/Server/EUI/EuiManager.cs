using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Eui;
using Robust.Server.Player;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Network;

namespace Content.Server.EUI
{
	// Token: 0x02000526 RID: 1318
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class EuiManager : IPostInjectInit
	{
		// Token: 0x06001B6D RID: 7021 RVA: 0x00092A83 File Offset: 0x00090C83
		void IPostInjectInit.PostInject()
		{
			this._players.PlayerStatusChanged += this.PlayerStatusChanged;
		}

		// Token: 0x06001B6E RID: 7022 RVA: 0x00092A9C File Offset: 0x00090C9C
		public void Initialize()
		{
			this._net.RegisterNetMessage<MsgEuiCtl>(null, 3);
			this._net.RegisterNetMessage<MsgEuiState>(null, 3);
			this._net.RegisterNetMessage<MsgEuiMessage>(new ProcessMessage<MsgEuiMessage>(this.RxMsgMessage), 3);
		}

		// Token: 0x06001B6F RID: 7023 RVA: 0x00092AD0 File Offset: 0x00090CD0
		public void SendUpdates()
		{
			ValueTuple<IPlayerSession, uint> tuple;
			while (this._stateUpdateQueue.TryDequeue(out tuple))
			{
				ValueTuple<IPlayerSession, uint> valueTuple = tuple;
				IPlayerSession player = valueTuple.Item1;
				uint id = valueTuple.Item2;
				EuiManager.PlayerEuiData plyDat;
				BaseEui ui;
				if (this._playerData.TryGetValue(player, out plyDat) && plyDat.OpenUIs.TryGetValue(id, out ui))
				{
					ui.DoStateUpdate();
				}
			}
		}

		// Token: 0x06001B70 RID: 7024 RVA: 0x00092B24 File Offset: 0x00090D24
		public void OpenEui(BaseEui eui, IPlayerSession player)
		{
			if (eui.Id != 0U)
			{
				throw new ArgumentException("That EUI is already open!");
			}
			EuiManager.PlayerEuiData playerEuiData = this._playerData[player];
			uint nextId = playerEuiData.NextId;
			playerEuiData.NextId = nextId + 1U;
			uint newId = nextId;
			eui.Initialize(this, player, newId);
			playerEuiData.OpenUIs.Add(newId, eui);
			MsgEuiCtl msg = new MsgEuiCtl();
			msg.Id = newId;
			msg.Type = MsgEuiCtl.CtlType.Open;
			msg.OpenType = eui.GetType().Name;
			this._net.ServerSendMessage(msg, player.ConnectedClient);
		}

		// Token: 0x06001B71 RID: 7025 RVA: 0x00092BB0 File Offset: 0x00090DB0
		public void CloseEui(BaseEui eui)
		{
			eui.Shutdown();
			this._playerData[eui.Player].OpenUIs.Remove(eui.Id);
			MsgEuiCtl msg = new MsgEuiCtl();
			msg.Id = eui.Id;
			msg.Type = MsgEuiCtl.CtlType.Close;
			this._net.ServerSendMessage(msg, eui.Player.ConnectedClient);
		}

		// Token: 0x06001B72 RID: 7026 RVA: 0x00092C18 File Offset: 0x00090E18
		private void RxMsgMessage(MsgEuiMessage message)
		{
			IPlayerSession ply;
			if (!this._players.TryGetSessionByChannel(message.MsgChannel, ref ply))
			{
				return;
			}
			EuiManager.PlayerEuiData dat;
			if (!this._playerData.TryGetValue(ply, out dat))
			{
				return;
			}
			BaseEui eui;
			if (!dat.OpenUIs.TryGetValue(message.Id, out eui))
			{
				string text = "eui";
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(49, 2);
				defaultInterpolatedStringHandler.AppendLiteral("Got EUI message from player ");
				defaultInterpolatedStringHandler.AppendFormatted<IPlayerSession>(ply);
				defaultInterpolatedStringHandler.AppendLiteral(" for non-existing UI ");
				defaultInterpolatedStringHandler.AppendFormatted<uint>(message.Id);
				Logger.WarningS(text, defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			eui.HandleMessage(message.Message);
		}

		// Token: 0x06001B73 RID: 7027 RVA: 0x00092CB8 File Offset: 0x00090EB8
		private void PlayerStatusChanged([Nullable(2)] object sender, SessionStatusEventArgs e)
		{
			if (e.NewStatus == 2)
			{
				this._playerData.Add(e.Session, new EuiManager.PlayerEuiData());
				return;
			}
			EuiManager.PlayerEuiData plyDat;
			if (e.NewStatus == 4 && this._playerData.TryGetValue(e.Session, out plyDat))
			{
				foreach (BaseEui baseEui in plyDat.OpenUIs.Values)
				{
					baseEui.Closed();
				}
				this._playerData.Remove(e.Session);
			}
		}

		// Token: 0x06001B74 RID: 7028 RVA: 0x00092D60 File Offset: 0x00090F60
		public void QueueStateUpdate(BaseEui eui)
		{
			this._stateUpdateQueue.Enqueue(new ValueTuple<IPlayerSession, uint>(eui.Player, eui.Id));
		}

		// Token: 0x0400119C RID: 4508
		[Dependency]
		private readonly IPlayerManager _players;

		// Token: 0x0400119D RID: 4509
		[Dependency]
		private readonly IServerNetManager _net;

		// Token: 0x0400119E RID: 4510
		private readonly Dictionary<IPlayerSession, EuiManager.PlayerEuiData> _playerData = new Dictionary<IPlayerSession, EuiManager.PlayerEuiData>();

		// Token: 0x0400119F RID: 4511
		[TupleElementNames(new string[]
		{
			"player",
			"id"
		})]
		[Nullable(new byte[]
		{
			1,
			0,
			1
		})]
		private readonly Queue<ValueTuple<IPlayerSession, uint>> _stateUpdateQueue = new Queue<ValueTuple<IPlayerSession, uint>>();

		// Token: 0x02000A0E RID: 2574
		[NullableContext(0)]
		private sealed class PlayerEuiData
		{
			// Token: 0x0400232C RID: 9004
			public uint NextId = 1U;

			// Token: 0x0400232D RID: 9005
			[Nullable(1)]
			public readonly Dictionary<uint, BaseEui> OpenUIs = new Dictionary<uint, BaseEui>();
		}
	}
}
