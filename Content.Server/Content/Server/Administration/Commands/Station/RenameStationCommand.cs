using System;
using System.Runtime.CompilerServices;
using Content.Server.Station.Systems;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;

namespace Content.Server.Administration.Commands.Station
{
	// Token: 0x02000870 RID: 2160
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Admin)]
	public sealed class RenameStationCommand : IConsoleCommand
	{
		// Token: 0x170007D8 RID: 2008
		// (get) Token: 0x06002F37 RID: 12087 RVA: 0x000F46E8 File Offset: 0x000F28E8
		public string Command
		{
			get
			{
				return "renamestation";
			}
		}

		// Token: 0x170007D9 RID: 2009
		// (get) Token: 0x06002F38 RID: 12088 RVA: 0x000F46EF File Offset: 0x000F28EF
		public string Description
		{
			get
			{
				return "Renames the given station";
			}
		}

		// Token: 0x170007DA RID: 2010
		// (get) Token: 0x06002F39 RID: 12089 RVA: 0x000F46F6 File Offset: 0x000F28F6
		public string Help
		{
			get
			{
				return "renamestation <station id> <name>";
			}
		}

		// Token: 0x06002F3A RID: 12090 RVA: 0x000F4700 File Offset: 0x000F2900
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length != 2)
			{
				shell.WriteError(Loc.GetString("shell-wrong-arguments-number"));
				return;
			}
			StationSystem stationSystem = EntitySystem.Get<StationSystem>();
			int station;
			if (!int.TryParse(args[0], out station) || !stationSystem.Stations.Contains(new EntityUid(station)))
			{
				shell.WriteError(Loc.GetString("shell-argument-station-id-invalid", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("index", 1)
				}));
				return;
			}
			stationSystem.RenameStation(new EntityUid(station), args[1], true, null, null);
		}
	}
}
