using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Managers;
using Content.Server.GameTicking;
using Content.Server.RoundEnd;
using Content.Shared.CCVar;
using Robust.Server.Player;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Players;

namespace Content.Server.UtkaIntegration
{
	// Token: 0x020000DD RID: 221
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class UtkaStatusCommand : IUtkaCommand
	{
		// Token: 0x1700009C RID: 156
		// (get) Token: 0x060003FD RID: 1021 RVA: 0x00014F37 File Offset: 0x00013137
		public string Name
		{
			get
			{
				return "status";
			}
		}

		// Token: 0x1700009D RID: 157
		// (get) Token: 0x060003FE RID: 1022 RVA: 0x00014F3E File Offset: 0x0001313E
		public Type RequestMessageType
		{
			get
			{
				return typeof(UtkaStatusRequsets);
			}
		}

		// Token: 0x060003FF RID: 1023 RVA: 0x00014F4C File Offset: 0x0001314C
		public void Execute(UtkaTCPSession session, UtkaBaseMessage baseMessage)
		{
			if (!(baseMessage is UtkaStatusRequsets))
			{
				return;
			}
			IoCManager.InjectDependencies<UtkaStatusCommand>(this);
			IEnumerable<string> playerNames = from player in Filter.GetAllPlayers(null).ToList<ICommonSession>()
			where player.Status != 4
			select player into x
			select x.Name;
			List<string> admins = (from x in this._adminManager.ActiveAdmins
			select x.Name).ToList<string>();
			string shuttleData = string.Empty;
			if (this._roundEndSystem.ExpectedCountdownEnd == null)
			{
				shuttleData = "shuttle is not on the way";
			}
			else
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(28, 1);
				defaultInterpolatedStringHandler.AppendLiteral("shuttle is on the way, ETA: ");
				defaultInterpolatedStringHandler.AppendFormatted<TimeSpan?>(this._roundEndSystem.ShuttleTimeLeft);
				shuttleData = defaultInterpolatedStringHandler.ToStringAndClear();
			}
			double roundDuration = this._gameTicker.RoundDuration().TotalSeconds;
			string gameMap = this._cfg.GetCVar<string>(CCVars.GameMap);
			UtkaStatusResponse toUtkaMessage = new UtkaStatusResponse
			{
				Players = playerNames.ToList<string>(),
				Admins = admins,
				Map = gameMap,
				ShuttleStatus = shuttleData,
				RoundDuration = roundDuration
			};
			this._utkaSocketWrapper.SendMessageToAll(toUtkaMessage);
		}

		// Token: 0x04000270 RID: 624
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04000271 RID: 625
		[Dependency]
		private readonly IAdminManager _adminManager;

		// Token: 0x04000272 RID: 626
		[Dependency]
		private readonly RoundEndSystem _roundEndSystem;

		// Token: 0x04000273 RID: 627
		[Dependency]
		private readonly GameTicker _gameTicker;

		// Token: 0x04000274 RID: 628
		[Dependency]
		private readonly IConfigurationManager _cfg;

		// Token: 0x04000275 RID: 629
		[Dependency]
		private readonly UtkaTCPWrapper _utkaSocketWrapper;
	}
}
