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

namespace Content.Server.CrewManifest
{
	// Token: 0x020005D9 RID: 1497
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Admin)]
	public sealed class CrewManifestCommand : IConsoleCommand
	{
		// Token: 0x170004C2 RID: 1218
		// (get) Token: 0x06001FFC RID: 8188 RVA: 0x000A747E File Offset: 0x000A567E
		public string Command
		{
			get
			{
				return "crewmanifest";
			}
		}

		// Token: 0x170004C3 RID: 1219
		// (get) Token: 0x06001FFD RID: 8189 RVA: 0x000A7485 File Offset: 0x000A5685
		public string Description
		{
			get
			{
				return "Opens the crew manifest for the given station.";
			}
		}

		// Token: 0x170004C4 RID: 1220
		// (get) Token: 0x06001FFE RID: 8190 RVA: 0x000A748C File Offset: 0x000A568C
		public string Help
		{
			get
			{
				return "Usage: " + this.Command + " <entity uid>";
			}
		}

		// Token: 0x06001FFF RID: 8191 RVA: 0x000A74A3 File Offset: 0x000A56A3
		public CrewManifestCommand()
		{
			IoCManager.InjectDependencies<CrewManifestCommand>(this);
		}

		// Token: 0x06002000 RID: 8192 RVA: 0x000A74B4 File Offset: 0x000A56B4
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length != 1)
			{
				shell.WriteLine("Invalid argument count.\n" + this.Help);
				return;
			}
			EntityUid uid;
			if (!EntityUid.TryParse(args[0], ref uid))
			{
				shell.WriteLine(args[0] + " is not a valid entity UID.");
				return;
			}
			if (shell.Player != null)
			{
				IPlayerSession session = shell.Player as IPlayerSession;
				if (session != null)
				{
					this._entityManager.System<CrewManifestSystem>().OpenEui(uid, session, null);
					return;
				}
			}
			shell.WriteLine("You must run this from a client.");
		}

		// Token: 0x06002001 RID: 8193 RVA: 0x000A7541 File Offset: 0x000A5741
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

		// Token: 0x040013DA RID: 5082
		[Dependency]
		private readonly IEntityManager _entityManager;
	}
}
