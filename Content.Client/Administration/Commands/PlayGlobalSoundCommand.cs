using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Robust.Client.Player;
using Robust.Shared.Console;
using Robust.Shared.ContentPack;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Players;

namespace Content.Client.Administration.Commands
{
	// Token: 0x020004EC RID: 1260
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PlayGlobalSoundCommand : IConsoleCommand
	{
		// Token: 0x170006F4 RID: 1780
		// (get) Token: 0x06002004 RID: 8196 RVA: 0x000BA122 File Offset: 0x000B8322
		public string Command
		{
			get
			{
				return "playglobalsound";
			}
		}

		// Token: 0x170006F5 RID: 1781
		// (get) Token: 0x06002005 RID: 8197 RVA: 0x000BA129 File Offset: 0x000B8329
		public string Description
		{
			get
			{
				return Loc.GetString("play-global-sound-command-description");
			}
		}

		// Token: 0x170006F6 RID: 1782
		// (get) Token: 0x06002006 RID: 8198 RVA: 0x000BA135 File Offset: 0x000B8335
		public string Help
		{
			get
			{
				return Loc.GetString("play-global-sound-command-help");
			}
		}

		// Token: 0x06002007 RID: 8199 RVA: 0x000BA141 File Offset: 0x000B8341
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			shell.RemoteExecuteCommand(argStr);
		}

		// Token: 0x06002008 RID: 8200 RVA: 0x000BA14C File Offset: 0x000B834C
		public CompletionResult GetCompletion(IConsoleShell shell, string[] args)
		{
			if (args.Length == 1)
			{
				string @string = Loc.GetString("play-global-sound-command-arg-path");
				IResourceManager resourceManager = IoCManager.Resolve<IResourceManager>();
				return CompletionResult.FromHintOptions(CompletionHelper.ContentFilePath(args[0], resourceManager), @string);
			}
			if (args.Length == 2)
			{
				return CompletionResult.FromHint(Loc.GetString("play-global-sound-command-arg-volume"));
			}
			if (args.Length > 2)
			{
				return CompletionResult.FromHintOptions(from c in IoCManager.Resolve<IPlayerManager>().Sessions
				select c.Name, Loc.GetString("play-global-sound-command-arg-usern", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("user", args.Length - 2)
				}));
			}
			return CompletionResult.Empty;
		}
	}
}
