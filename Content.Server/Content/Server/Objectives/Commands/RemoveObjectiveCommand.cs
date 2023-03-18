using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Server.Mind;
using Content.Server.Players;
using Content.Shared.Administration;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.IoC;

namespace Content.Server.Objectives.Commands
{
	// Token: 0x02000305 RID: 773
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Admin)]
	public sealed class RemoveObjectiveCommand : IConsoleCommand
	{
		// Token: 0x1700024D RID: 589
		// (get) Token: 0x06000FEF RID: 4079 RVA: 0x000512F7 File Offset: 0x0004F4F7
		public string Command
		{
			get
			{
				return "rmobjective";
			}
		}

		// Token: 0x1700024E RID: 590
		// (get) Token: 0x06000FF0 RID: 4080 RVA: 0x000512FE File Offset: 0x0004F4FE
		public string Description
		{
			get
			{
				return "Removes an objective from the player's mind.";
			}
		}

		// Token: 0x1700024F RID: 591
		// (get) Token: 0x06000FF1 RID: 4081 RVA: 0x00051305 File Offset: 0x0004F505
		public string Help
		{
			get
			{
				return "rmobjective <username> <index>";
			}
		}

		// Token: 0x06000FF2 RID: 4082 RVA: 0x0005130C File Offset: 0x0004F50C
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
			int i;
			if (int.TryParse(args[1], out i))
			{
				shell.WriteLine(mind.TryRemoveObjective(i) ? "Objective successfully removed!" : "Objective removing failed. Maybe the index is out of bounds? Check lsobjectives!");
				return;
			}
			shell.WriteLine("Invalid index " + args[1] + "!");
		}
	}
}
