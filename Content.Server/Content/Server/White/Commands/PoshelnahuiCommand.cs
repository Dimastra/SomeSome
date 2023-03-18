using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Shared.Administration;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.IoC;

namespace Content.Server.White.Commands
{
	// Token: 0x0200009B RID: 155
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Admin)]
	public sealed class PoshelnahuiCommand : IConsoleCommand
	{
		// Token: 0x1700004F RID: 79
		// (get) Token: 0x06000272 RID: 626 RVA: 0x0000D433 File Offset: 0x0000B633
		public string Command
		{
			get
			{
				return "poshelnahui";
			}
		}

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x06000273 RID: 627 RVA: 0x0000D43A File Offset: 0x0000B63A
		public string Description
		{
			get
			{
				return "Close client game lol";
			}
		}

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x06000274 RID: 628 RVA: 0x0000D441 File Offset: 0x0000B641
		public string Help
		{
			get
			{
				return "poshelnahui <ckey>";
			}
		}

		// Token: 0x06000275 RID: 629 RVA: 0x0000D448 File Offset: 0x0000B648
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length != 1 || string.IsNullOrEmpty(args[0]))
			{
				shell.WriteLine("Wrong number of arguments");
				return;
			}
			IPlayerSession player = IoCManager.Resolve<IPlayerManager>().ServerSessions.ToList<IPlayerSession>().Find((IPlayerSession x) => x.Name == args[0]);
			if (player == null)
			{
				shell.WriteLine("Player not found");
				return;
			}
			IoCManager.Resolve<IConsoleHost>().RemoteExecuteCommand(player, "hardquit");
			shell.WriteLine("Message sent");
		}

		// Token: 0x06000276 RID: 630 RVA: 0x0000D4D4 File Offset: 0x0000B6D4
		public CompletionResult GetCompletion(IConsoleShell shell, string[] args)
		{
			if (args.Length == 1)
			{
				return CompletionResult.FromHintOptions((from c in IoCManager.Resolve<IPlayerManager>().ServerSessions
				select c.Name into c
				orderby c
				select c).ToArray<string>(), "ckey");
			}
			return CompletionResult.Empty;
		}
	}
}
