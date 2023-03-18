using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Server.Station.Systems;
using Content.Shared.Administration;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.AlertLevel.Commands
{
	// Token: 0x020007E2 RID: 2018
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Admin)]
	public sealed class SetAlertLevelCommand : IConsoleCommand
	{
		// Token: 0x170006CB RID: 1739
		// (get) Token: 0x06002BD2 RID: 11218 RVA: 0x000E5E7A File Offset: 0x000E407A
		public string Command
		{
			get
			{
				return "setalertlevel";
			}
		}

		// Token: 0x170006CC RID: 1740
		// (get) Token: 0x06002BD3 RID: 11219 RVA: 0x000E5E81 File Offset: 0x000E4081
		public string Description
		{
			get
			{
				return Loc.GetString("cmd-setalertlevel-desc");
			}
		}

		// Token: 0x170006CD RID: 1741
		// (get) Token: 0x06002BD4 RID: 11220 RVA: 0x000E5E8D File Offset: 0x000E408D
		public string Help
		{
			get
			{
				return Loc.GetString("cmd-setalertlevel-help");
			}
		}

		// Token: 0x06002BD5 RID: 11221 RVA: 0x000E5E9C File Offset: 0x000E409C
		public CompletionResult GetCompletion(IConsoleShell shell, string[] args)
		{
			string[] levelNames = new string[0];
			IPlayerSession player = shell.Player as IPlayerSession;
			if (player != null && player.AttachedEntity != null)
			{
				EntityUid? stationUid = EntitySystem.Get<StationSystem>().GetOwningStation(player.AttachedEntity.Value, null);
				if (stationUid != null)
				{
					levelNames = this.GetStationLevelNames(stationUid.Value);
				}
			}
			int num = args.Length;
			CompletionResult result;
			if (num != 1)
			{
				if (num != 2)
				{
					result = CompletionResult.Empty;
				}
				else
				{
					result = CompletionResult.FromHintOptions(CompletionHelper.Booleans, Loc.GetString("cmd-setalertlevel-hint-2"));
				}
			}
			else
			{
				result = CompletionResult.FromHintOptions(levelNames, Loc.GetString("cmd-setalertlevel-hint-1"));
			}
			return result;
		}

		// Token: 0x06002BD6 RID: 11222 RVA: 0x000E5F48 File Offset: 0x000E4148
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length < 1)
			{
				shell.WriteError(Loc.GetString("shell-wrong-arguments-number"));
				return;
			}
			bool locked = false;
			if (args.Length > 1 && !bool.TryParse(args[1], out locked))
			{
				shell.WriteLine(Loc.GetString("shell-argument-must-be-boolean"));
				return;
			}
			IPlayerSession player = shell.Player as IPlayerSession;
			if (player == null || player.AttachedEntity == null)
			{
				shell.WriteLine(Loc.GetString("shell-only-players-can-run-this-command"));
				return;
			}
			EntityUid? stationUid = EntitySystem.Get<StationSystem>().GetOwningStation(player.AttachedEntity.Value, null);
			if (stationUid == null)
			{
				shell.WriteLine(Loc.GetString("cmd-setalertlevel-invalid-grid"));
				return;
			}
			string level = args[0];
			if (!this.GetStationLevelNames(stationUid.Value).Contains(level))
			{
				shell.WriteLine(Loc.GetString("cmd-setalertlevel-invalid-level"));
				return;
			}
			EntitySystem.Get<AlertLevelSystem>().SetLevel(stationUid.Value, level, true, true, true, locked, null, null);
		}

		// Token: 0x06002BD7 RID: 11223 RVA: 0x000E6040 File Offset: 0x000E4240
		private string[] GetStationLevelNames(EntityUid station)
		{
			AlertLevelComponent alertLevelComp;
			if (!IoCManager.Resolve<IEntityManager>().TryGetComponent<AlertLevelComponent>(station, ref alertLevelComp))
			{
				return new string[0];
			}
			if (alertLevelComp.AlertLevels == null)
			{
				return new string[0];
			}
			return alertLevelComp.AlertLevels.Levels.Keys.ToArray<string>();
		}
	}
}
