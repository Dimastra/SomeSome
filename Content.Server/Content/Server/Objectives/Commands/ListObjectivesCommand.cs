using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Server.Mind;
using Content.Server.Players;
using Content.Shared.Administration;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.IoC;
using Robust.Shared.Network;

namespace Content.Server.Objectives.Commands
{
	// Token: 0x02000304 RID: 772
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Logs)]
	public sealed class ListObjectivesCommand : IConsoleCommand
	{
		// Token: 0x1700024A RID: 586
		// (get) Token: 0x06000FEA RID: 4074 RVA: 0x000511A7 File Offset: 0x0004F3A7
		public string Command
		{
			get
			{
				return "lsobjectives";
			}
		}

		// Token: 0x1700024B RID: 587
		// (get) Token: 0x06000FEB RID: 4075 RVA: 0x000511AE File Offset: 0x0004F3AE
		public string Description
		{
			get
			{
				return "Lists all objectives in a players mind.";
			}
		}

		// Token: 0x1700024C RID: 588
		// (get) Token: 0x06000FEC RID: 4076 RVA: 0x000511B5 File Offset: 0x0004F3B5
		public string Help
		{
			get
			{
				return "lsobjectives [<username>]";
			}
		}

		// Token: 0x06000FED RID: 4077 RVA: 0x000511BC File Offset: 0x0004F3BC
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			IPlayerSession player = shell.Player as IPlayerSession;
			IPlayerData data;
			if (args.Length == 0 && player != null)
			{
				data = player.Data;
			}
			else if (player == null || !IoCManager.Resolve<IPlayerManager>().TryGetPlayerDataByUsername(args[0], ref data))
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
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(23, 1);
			defaultInterpolatedStringHandler.AppendLiteral("Objectives for player ");
			defaultInterpolatedStringHandler.AppendFormatted<NetUserId>(data.UserId);
			defaultInterpolatedStringHandler.AppendLiteral(":");
			shell.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
			List<Objective> objectives = mind.AllObjectives.ToList<Objective>();
			if (objectives.Count == 0)
			{
				shell.WriteLine("None.");
			}
			for (int i = 0; i < objectives.Count; i++)
			{
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(5, 2);
				defaultInterpolatedStringHandler.AppendLiteral("- [");
				defaultInterpolatedStringHandler.AppendFormatted<int>(i + 1);
				defaultInterpolatedStringHandler.AppendLiteral("] ");
				defaultInterpolatedStringHandler.AppendFormatted(objectives[i].Conditions[0].Title);
				shell.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
			}
		}
	}
}
