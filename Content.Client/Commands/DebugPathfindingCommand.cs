using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Client.NPC;
using Content.Shared.NPC;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Commands
{
	// Token: 0x020003A8 RID: 936
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DebugPathfindingCommand : IConsoleCommand
	{
		// Token: 0x170004B9 RID: 1209
		// (get) Token: 0x0600174A RID: 5962 RVA: 0x000866C6 File Offset: 0x000848C6
		public string Command
		{
			get
			{
				return "pathfinder";
			}
		}

		// Token: 0x170004BA RID: 1210
		// (get) Token: 0x0600174B RID: 5963 RVA: 0x000866CD File Offset: 0x000848CD
		public string Description
		{
			get
			{
				return "Toggles visibility of pathfinding debuggers.";
			}
		}

		// Token: 0x170004BB RID: 1211
		// (get) Token: 0x0600174C RID: 5964 RVA: 0x000866D4 File Offset: 0x000848D4
		public string Help
		{
			get
			{
				return "pathfinder [options]";
			}
		}

		// Token: 0x0600174D RID: 5965 RVA: 0x000866DC File Offset: 0x000848DC
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			PathfindingSystem entitySystem = IoCManager.Resolve<IEntitySystemManager>().GetEntitySystem<PathfindingSystem>();
			if (args.Length == 0)
			{
				entitySystem.Modes = PathfindingDebugMode.None;
				return;
			}
			foreach (string text in args)
			{
				PathfindingDebugMode pathfindingDebugMode;
				if (!Enum.TryParse<PathfindingDebugMode>(text, out pathfindingDebugMode))
				{
					shell.WriteError("Unrecognised pathfinder args " + text);
				}
				else
				{
					entitySystem.Modes ^= pathfindingDebugMode;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(12, 2);
					defaultInterpolatedStringHandler.AppendLiteral("Toggled ");
					defaultInterpolatedStringHandler.AppendFormatted(text);
					defaultInterpolatedStringHandler.AppendLiteral(" to ");
					defaultInterpolatedStringHandler.AppendFormatted<bool>((entitySystem.Modes & pathfindingDebugMode) > PathfindingDebugMode.None);
					shell.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
				}
			}
		}

		// Token: 0x0600174E RID: 5966 RVA: 0x00086790 File Offset: 0x00084990
		public CompletionResult GetCompletion(IConsoleShell shell, string[] args)
		{
			if (args.Length > 1)
			{
				return CompletionResult.Empty;
			}
			List<PathfindingDebugMode> list = Enum.GetValues<PathfindingDebugMode>().ToList<PathfindingDebugMode>();
			List<CompletionOption> list2 = new List<CompletionOption>();
			foreach (PathfindingDebugMode pathfindingDebugMode in list)
			{
				if (pathfindingDebugMode != PathfindingDebugMode.None)
				{
					list2.Add(new CompletionOption(pathfindingDebugMode.ToString(), null, 0));
				}
			}
			return CompletionResult.FromOptions(list2);
		}
	}
}
