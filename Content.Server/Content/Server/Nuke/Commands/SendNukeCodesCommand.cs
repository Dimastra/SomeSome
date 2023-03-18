using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Server.Station.Systems;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Nuke.Commands
{
	// Token: 0x0200032C RID: 812
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Fun)]
	public sealed class SendNukeCodesCommand : IConsoleCommand
	{
		// Token: 0x17000270 RID: 624
		// (get) Token: 0x060010C4 RID: 4292 RVA: 0x000566A1 File Offset: 0x000548A1
		public string Command
		{
			get
			{
				return "nukecodes";
			}
		}

		// Token: 0x17000271 RID: 625
		// (get) Token: 0x060010C5 RID: 4293 RVA: 0x000566A8 File Offset: 0x000548A8
		public string Description
		{
			get
			{
				return "Send nuke codes to a station's communication consoles";
			}
		}

		// Token: 0x17000272 RID: 626
		// (get) Token: 0x060010C6 RID: 4294 RVA: 0x000566AF File Offset: 0x000548AF
		public string Help
		{
			get
			{
				return "nukecodes [station EntityUid]";
			}
		}

		// Token: 0x060010C7 RID: 4295 RVA: 0x000566B6 File Offset: 0x000548B6
		public SendNukeCodesCommand()
		{
			IoCManager.InjectDependencies<SendNukeCodesCommand>(this);
		}

		// Token: 0x060010C8 RID: 4296 RVA: 0x000566C8 File Offset: 0x000548C8
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length != 1)
			{
				shell.WriteError(Loc.GetString("shell-need-exactly-one-argument"));
				return;
			}
			EntityUid uid;
			if (!EntityUid.TryParse(args[0], ref uid))
			{
				shell.WriteError(Loc.GetString("shell-entity-uid-must-be-number"));
				return;
			}
			this._entityManager.System<NukeCodePaperSystem>().SendNukeCodes(uid);
		}

		// Token: 0x060010C9 RID: 4297 RVA: 0x00056720 File Offset: 0x00054920
		public CompletionResult GetCompletion(IConsoleShell shell, string[] args)
		{
			if (args.Length != 1)
			{
				return CompletionResult.Empty;
			}
			return CompletionResult.FromHintOptions(this._entityManager.System<StationSystem>().Stations.Select(delegate(EntityUid station)
			{
				MetaDataComponent meta = this._entityManager.GetComponent<MetaDataComponent>(station);
				return new CompletionOption(station.ToString(), meta.EntityName, 0);
			}), null);
		}

		// Token: 0x040009F5 RID: 2549
		[Dependency]
		private readonly IEntityManager _entityManager;
	}
}
