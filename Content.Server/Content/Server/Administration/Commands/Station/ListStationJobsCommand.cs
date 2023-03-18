using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Station.Systems;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;

namespace Content.Server.Administration.Commands.Station
{
	// Token: 0x0200086E RID: 2158
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Admin)]
	public sealed class ListStationJobsCommand : IConsoleCommand
	{
		// Token: 0x170007D2 RID: 2002
		// (get) Token: 0x06002F2D RID: 12077 RVA: 0x000F4515 File Offset: 0x000F2715
		public string Command
		{
			get
			{
				return "lsstationjobs";
			}
		}

		// Token: 0x170007D3 RID: 2003
		// (get) Token: 0x06002F2E RID: 12078 RVA: 0x000F451C File Offset: 0x000F271C
		public string Description
		{
			get
			{
				return "Lists all jobs on the given station.";
			}
		}

		// Token: 0x170007D4 RID: 2004
		// (get) Token: 0x06002F2F RID: 12079 RVA: 0x000F4523 File Offset: 0x000F2723
		public string Help
		{
			get
			{
				return "lsstationjobs <station id>";
			}
		}

		// Token: 0x06002F30 RID: 12080 RVA: 0x000F452C File Offset: 0x000F272C
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length != 1)
			{
				shell.WriteError(Loc.GetString("shell-wrong-arguments-number"));
				return;
			}
			StationSystem stationSystem = EntitySystem.Get<StationSystem>();
			StationJobsSystem stationJobs = EntitySystem.Get<StationJobsSystem>();
			int station;
			if (!int.TryParse(args[0], out station) || !stationSystem.Stations.Contains(new EntityUid(station)))
			{
				shell.WriteError(Loc.GetString("shell-argument-station-id-invalid", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("index", 1)
				}));
				return;
			}
			foreach (KeyValuePair<string, uint?> keyValuePair in stationJobs.GetJobs(new EntityUid(station), null))
			{
				string text;
				uint? num;
				keyValuePair.Deconstruct(out text, out num);
				string job = text;
				uint? amount = num;
				string amountText = (amount == null) ? "Infinite" : amount.ToString();
				shell.WriteLine(job + ": " + amountText);
			}
		}
	}
}
