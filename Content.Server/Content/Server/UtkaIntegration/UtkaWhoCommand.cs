using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Players;

namespace Content.Server.UtkaIntegration
{
	// Token: 0x020000DE RID: 222
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class UtkaWhoCommand : IUtkaCommand
	{
		// Token: 0x1700009E RID: 158
		// (get) Token: 0x06000401 RID: 1025 RVA: 0x000150B2 File Offset: 0x000132B2
		public string Name
		{
			get
			{
				return "who";
			}
		}

		// Token: 0x1700009F RID: 159
		// (get) Token: 0x06000402 RID: 1026 RVA: 0x000150B9 File Offset: 0x000132B9
		public Type RequestMessageType
		{
			get
			{
				return typeof(UtkaWhoRequest);
			}
		}

		// Token: 0x06000403 RID: 1027 RVA: 0x000150C8 File Offset: 0x000132C8
		public void Execute(UtkaTCPSession session, UtkaBaseMessage baseMessage)
		{
			if (!(baseMessage is UtkaWhoRequest))
			{
				return;
			}
			IoCManager.InjectDependencies<UtkaWhoCommand>(this);
			IEnumerable<string> playerNames = from player in Filter.GetAllPlayers(null).ToList<ICommonSession>()
			where player.Status != 4
			select player into x
			select x.Name;
			UtkaWhoResponse toUtkaMessage = new UtkaWhoResponse
			{
				Players = playerNames.ToList<string>()
			};
			this._utkaSocketWrapper.SendMessageToAll(toUtkaMessage);
		}

		// Token: 0x04000276 RID: 630
		[Dependency]
		private readonly IConfigurationManager _cfg;

		// Token: 0x04000277 RID: 631
		[Dependency]
		private UtkaTCPWrapper _utkaSocketWrapper;
	}
}
