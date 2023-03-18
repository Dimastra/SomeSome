using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.GameTicking;
using Content.Server.Maps;
using Content.Shared.Administration;
using Robust.Server.Maps;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Server.Administration.Commands
{
	// Token: 0x02000843 RID: 2115
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Spawn | AdminFlags.Round)]
	public sealed class LoadGameMapCommand : IConsoleCommand
	{
		// Token: 0x17000753 RID: 1875
		// (get) Token: 0x06002E47 RID: 11847 RVA: 0x000F198A File Offset: 0x000EFB8A
		public string Command
		{
			get
			{
				return "loadgamemap";
			}
		}

		// Token: 0x17000754 RID: 1876
		// (get) Token: 0x06002E48 RID: 11848 RVA: 0x000F1991 File Offset: 0x000EFB91
		public string Description
		{
			get
			{
				return "Loads the given game map at the given coordinates.";
			}
		}

		// Token: 0x17000755 RID: 1877
		// (get) Token: 0x06002E49 RID: 11849 RVA: 0x000F1998 File Offset: 0x000EFB98
		public string Help
		{
			get
			{
				return "loadgamemap <mapid> <gamemap> [<x> <y> [<name>]] ";
			}
		}

		// Token: 0x06002E4A RID: 11850 RVA: 0x000F19A0 File Offset: 0x000EFBA0
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			IPrototypeManager prototypeManager = IoCManager.Resolve<IPrototypeManager>();
			GameTicker gameTicker = IoCManager.Resolve<IEntityManager>().EntitySysManager.GetEntitySystem<GameTicker>();
			int num = args.Length;
			if (num != 2 && num != 4 && num != 5)
			{
				shell.WriteError(Loc.GetString("shell-wrong-arguments-number"));
				return;
			}
			GameMapPrototype gameMap;
			if (!prototypeManager.TryIndex<GameMapPrototype>(args[1], ref gameMap))
			{
				shell.WriteError("The given map prototype " + args[0] + " is invalid.");
				return;
			}
			int mapId;
			if (!int.TryParse(args[0], out mapId))
			{
				return;
			}
			MapLoadOptions loadOptions = new MapLoadOptions
			{
				LoadMap = false
			};
			string stationName = (args.Length == 5) ? args[4] : null;
			int x;
			int y;
			if (args.Length >= 4 && int.TryParse(args[2], out x) && int.TryParse(args[3], out y))
			{
				loadOptions.Offset = new Vector2((float)x, (float)y);
			}
			IReadOnlyList<EntityUid> grids = gameTicker.LoadGameMap(gameMap, new MapId(mapId), loadOptions, stationName);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(14, 1);
			defaultInterpolatedStringHandler.AppendLiteral("Loaded ");
			defaultInterpolatedStringHandler.AppendFormatted<int>(grids.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" grids.");
			shell.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
		}

		// Token: 0x06002E4B RID: 11851 RVA: 0x000F1AB8 File Offset: 0x000EFCB8
		public CompletionResult GetCompletion(IConsoleShell shell, string[] args)
		{
			switch (args.Length)
			{
			case 1:
				return CompletionResult.FromHint(Loc.GetString("cmd-hint-savemap-id"));
			case 2:
				return CompletionResult.FromHintOptions(CompletionHelper.PrototypeIDs<GameMapPrototype>(true, null), Loc.GetString("cmd-hint-savemap-path"));
			case 3:
				return CompletionResult.FromHint(Loc.GetString("cmd-hint-loadmap-x-position"));
			case 4:
				return CompletionResult.FromHint(Loc.GetString("cmd-hint-loadmap-y-position"));
			case 5:
				return CompletionResult.FromHint(Loc.GetString("cmd-hint-loadmap-rotation"));
			case 6:
				return CompletionResult.FromHint(Loc.GetString("cmd-hint-loadmap-uids"));
			default:
				return CompletionResult.Empty;
			}
		}
	}
}
