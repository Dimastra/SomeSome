using System;
using System.Runtime.CompilerServices;
using Content.Server.Station.Systems;
using Content.Shared.Administration;
using Content.Shared.Roles;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;

namespace Content.Server.Administration.Commands.Station
{
	// Token: 0x0200086D RID: 2157
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Round)]
	public sealed class AdjustStationJobCommand : IConsoleCommand
	{
		// Token: 0x170007CF RID: 1999
		// (get) Token: 0x06002F28 RID: 12072 RVA: 0x000F438B File Offset: 0x000F258B
		public string Command
		{
			get
			{
				return "adjstationjob";
			}
		}

		// Token: 0x170007D0 RID: 2000
		// (get) Token: 0x06002F29 RID: 12073 RVA: 0x000F4392 File Offset: 0x000F2592
		public string Description
		{
			get
			{
				return "Adjust the job manifest on a station.";
			}
		}

		// Token: 0x170007D1 RID: 2001
		// (get) Token: 0x06002F2A RID: 12074 RVA: 0x000F4399 File Offset: 0x000F2599
		public string Help
		{
			get
			{
				return "adjstationjob <station id> <job id> <amount>";
			}
		}

		// Token: 0x06002F2B RID: 12075 RVA: 0x000F43A0 File Offset: 0x000F25A0
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length != 3)
			{
				shell.WriteError(Loc.GetString("shell-wrong-arguments-number"));
				return;
			}
			IPrototypeManager prototypeManager = IoCManager.Resolve<IPrototypeManager>();
			StationSystem stationSystem = EntitySystem.Get<StationSystem>();
			StationJobsSystem stationJobs = EntitySystem.Get<StationJobsSystem>();
			int stationInt;
			if (!int.TryParse(args[0], out stationInt) || !stationSystem.Stations.Contains(new EntityUid(stationInt)))
			{
				shell.WriteError(Loc.GetString("shell-argument-station-id-invalid", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("index", 1)
				}));
				return;
			}
			EntityUid station;
			station..ctor(stationInt);
			JobPrototype job;
			if (!prototypeManager.TryIndex<JobPrototype>(args[1], ref job))
			{
				shell.WriteError(Loc.GetString("shell-argument-must-be-prototype", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("index", 2),
					new ValueTuple<string, object>("prototypeName", "JobPrototype")
				}));
				return;
			}
			int amount;
			if (!int.TryParse(args[2], out amount) || amount < -1)
			{
				shell.WriteError(Loc.GetString("shell-argument-number-must-be-between", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("index", 3),
					new ValueTuple<string, object>("lower", -1),
					new ValueTuple<string, object>("upper", int.MaxValue)
				}));
				return;
			}
			if (amount == -1)
			{
				stationJobs.MakeJobUnlimited(station, job, null);
				return;
			}
			stationJobs.TrySetJobSlot(station, job, amount, true, null);
		}
	}
}
