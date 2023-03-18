using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Server.Mind;
using Content.Server.Players;
using Content.Shared.Administration;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Server.Objectives.Commands
{
	// Token: 0x02000303 RID: 771
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Admin)]
	public sealed class AddObjectiveCommand : IConsoleCommand
	{
		// Token: 0x17000247 RID: 583
		// (get) Token: 0x06000FE5 RID: 4069 RVA: 0x000510D9 File Offset: 0x0004F2D9
		public string Command
		{
			get
			{
				return "addobjective";
			}
		}

		// Token: 0x17000248 RID: 584
		// (get) Token: 0x06000FE6 RID: 4070 RVA: 0x000510E0 File Offset: 0x0004F2E0
		public string Description
		{
			get
			{
				return "Adds an objective to the player's mind.";
			}
		}

		// Token: 0x17000249 RID: 585
		// (get) Token: 0x06000FE7 RID: 4071 RVA: 0x000510E7 File Offset: 0x0004F2E7
		public string Help
		{
			get
			{
				return "addobjective <username> <objectiveID>";
			}
		}

		// Token: 0x06000FE8 RID: 4072 RVA: 0x000510F0 File Offset: 0x0004F2F0
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length != 2)
			{
				shell.WriteLine("Expected exactly 2 arguments.");
				return;
			}
			IPlayerData data;
			if (!IoCManager.Resolve<IPlayerManager>().TryGetPlayerDataByUsername(args[0], ref data))
			{
				shell.WriteLine("Can't find the playerdata.");
				return;
			}
			PlayerData playerData = data.ContentData();
			Mind mind = (playerData != null) ? playerData.Mind : null;
			if (mind == null)
			{
				shell.WriteLine("Can't find the mind.");
				return;
			}
			ObjectivePrototype objectivePrototype;
			if (!IoCManager.Resolve<IPrototypeManager>().TryIndex<ObjectivePrototype>(args[1], ref objectivePrototype))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(39, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Can't find matching ObjectivePrototype ");
				defaultInterpolatedStringHandler.AppendFormatted<ObjectivePrototype>(objectivePrototype);
				shell.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			if (!mind.TryAddObjective(objectivePrototype))
			{
				shell.WriteLine("Objective requirements dont allow that objective to be added.");
			}
		}
	}
}
