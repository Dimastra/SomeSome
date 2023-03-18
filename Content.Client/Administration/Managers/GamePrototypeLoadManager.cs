using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Administration;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;

namespace Content.Client.Administration.Managers
{
	// Token: 0x020004E5 RID: 1253
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GamePrototypeLoadManager : IGamePrototypeLoadManager
	{
		// Token: 0x06001FEA RID: 8170 RVA: 0x000B9E95 File Offset: 0x000B8095
		public void Initialize()
		{
			this._netManager.RegisterNetMessage<GamePrototypeLoadMessage>(new ProcessMessage<GamePrototypeLoadMessage>(this.LoadGamePrototype), 3);
		}

		// Token: 0x06001FEB RID: 8171 RVA: 0x000B9EB0 File Offset: 0x000B80B0
		private void LoadGamePrototype(GamePrototypeLoadMessage message)
		{
			Dictionary<Type, HashSet<string>> dictionary = new Dictionary<Type, HashSet<string>>();
			this._prototypeManager.LoadString(message.PrototypeData, true, dictionary);
			this._prototypeManager.ResolveResults();
			this._prototypeManager.ReloadPrototypes(dictionary);
			this._localizationManager.ReloadLocalizations();
			Logger.InfoS("adminbus", "Loaded adminbus prototype data.");
		}

		// Token: 0x06001FEC RID: 8172 RVA: 0x000B9F08 File Offset: 0x000B8108
		public void SendGamePrototype(string prototype)
		{
			GamePrototypeLoadMessage gamePrototypeLoadMessage = new GamePrototypeLoadMessage
			{
				PrototypeData = prototype
			};
			this._netManager.ClientSendMessage(gamePrototypeLoadMessage);
		}

		// Token: 0x04000F3F RID: 3903
		[Dependency]
		private readonly IClientNetManager _netManager;

		// Token: 0x04000F40 RID: 3904
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04000F41 RID: 3905
		[Dependency]
		private readonly ILocalizationManager _localizationManager;
	}
}
