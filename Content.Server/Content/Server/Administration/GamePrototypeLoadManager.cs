using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Managers;
using Content.Shared.Administration;
using Robust.Server.Player;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Replays;
using Robust.Shared.Serialization.Markdown.Mapping;

namespace Content.Server.Administration
{
	// Token: 0x020007FF RID: 2047
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GamePrototypeLoadManager : IGamePrototypeLoadManager
	{
		// Token: 0x170006E6 RID: 1766
		// (get) Token: 0x06002C3B RID: 11323 RVA: 0x000E7111 File Offset: 0x000E5311
		public IReadOnlyList<string> LoadedPrototypes
		{
			get
			{
				return this._loadedPrototypes;
			}
		}

		// Token: 0x06002C3C RID: 11324 RVA: 0x000E7119 File Offset: 0x000E5319
		public void Initialize()
		{
			this._netManager.RegisterNetMessage<GamePrototypeLoadMessage>(new ProcessMessage<GamePrototypeLoadMessage>(this.ClientLoadsPrototype), 3);
			this._netManager.Connected += this.NetManagerOnConnected;
		}

		// Token: 0x06002C3D RID: 11325 RVA: 0x000E714C File Offset: 0x000E534C
		private void OnStartReplayRecording([Nullable(new byte[]
		{
			0,
			1,
			1,
			1
		})] ValueTuple<MappingDataNode, List<object>> initReplayData)
		{
			foreach (string prototype in this._loadedPrototypes)
			{
				initReplayData.Item2.Add(new ReplayPrototypeUploadMsg
				{
					PrototypeData = prototype
				});
			}
		}

		// Token: 0x06002C3E RID: 11326 RVA: 0x000E71B0 File Offset: 0x000E53B0
		public void SendGamePrototype(string prototype)
		{
		}

		// Token: 0x06002C3F RID: 11327 RVA: 0x000E71B4 File Offset: 0x000E53B4
		private void ClientLoadsPrototype(GamePrototypeLoadMessage message)
		{
			IPlayerSession player = this._playerManager.GetSessionByChannel(message.MsgChannel);
			if (this._adminManager.IsAdmin(player, false) && this._adminManager.HasAdminFlag(player, AdminFlags.Query))
			{
				this.LoadPrototypeData(message.PrototypeData);
				Logger.InfoS("adminbus", "Loaded adminbus prototype data from " + player.Name + ".");
				return;
			}
			message.MsgChannel.Disconnect("Sent prototype message without permission!");
		}

		// Token: 0x06002C40 RID: 11328 RVA: 0x000E7234 File Offset: 0x000E5434
		private void LoadPrototypeData(string prototypeData)
		{
			this._loadedPrototypes.Add(prototypeData);
			this._replay.QueueReplayMessage(new ReplayPrototypeUploadMsg
			{
				PrototypeData = prototypeData
			});
			GamePrototypeLoadMessage msg = new GamePrototypeLoadMessage
			{
				PrototypeData = prototypeData
			};
			this._netManager.ServerSendToAll(msg);
			Dictionary<Type, HashSet<string>> changed = new Dictionary<Type, HashSet<string>>();
			this._prototypeManager.LoadString(prototypeData, true, changed);
			this._prototypeManager.ResolveResults();
			this._prototypeManager.ReloadPrototypes(changed);
			this._localizationManager.ReloadLocalizations();
		}

		// Token: 0x06002C41 RID: 11329 RVA: 0x000E72B4 File Offset: 0x000E54B4
		private void NetManagerOnConnected([Nullable(2)] object sender, NetChannelArgs e)
		{
			foreach (string prototype in this._loadedPrototypes)
			{
				GamePrototypeLoadMessage msg = new GamePrototypeLoadMessage
				{
					PrototypeData = prototype
				};
				e.Channel.SendMessage(msg);
			}
		}

		// Token: 0x04001B67 RID: 7015
		[Dependency]
		private readonly IReplayRecordingManager _replay;

		// Token: 0x04001B68 RID: 7016
		[Dependency]
		private readonly IServerNetManager _netManager;

		// Token: 0x04001B69 RID: 7017
		[Dependency]
		private readonly IAdminManager _adminManager;

		// Token: 0x04001B6A RID: 7018
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04001B6B RID: 7019
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04001B6C RID: 7020
		[Dependency]
		private readonly ILocalizationManager _localizationManager;

		// Token: 0x04001B6D RID: 7021
		private readonly List<string> _loadedPrototypes = new List<string>();
	}
}
