using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Shared.Administration;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.IoC;

namespace Content.Server.Afk
{
	// Token: 0x020007F5 RID: 2037
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Admin)]
	public sealed class IsAfkCommand : IConsoleCommand
	{
		// Token: 0x170006D4 RID: 1748
		// (get) Token: 0x06002C12 RID: 11282 RVA: 0x000E6C93 File Offset: 0x000E4E93
		public string Command
		{
			get
			{
				return "isafk";
			}
		}

		// Token: 0x170006D5 RID: 1749
		// (get) Token: 0x06002C13 RID: 11283 RVA: 0x000E6C9A File Offset: 0x000E4E9A
		public string Description
		{
			get
			{
				return "Checks if a specified player is AFK";
			}
		}

		// Token: 0x170006D6 RID: 1750
		// (get) Token: 0x06002C14 RID: 11284 RVA: 0x000E6CA1 File Offset: 0x000E4EA1
		public string Help
		{
			get
			{
				return "Usage: isafk <playerName>";
			}
		}

		// Token: 0x06002C15 RID: 11285 RVA: 0x000E6CA8 File Offset: 0x000E4EA8
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			IAfkManager afkManager = IoCManager.Resolve<IAfkManager>();
			if (args.Length == 0)
			{
				shell.WriteError("Need one argument");
				return;
			}
			IPlayerSession player;
			if (!this._players.TryGetSessionByUsername(args[0], ref player))
			{
				shell.WriteError("Unable to find that player");
				return;
			}
			shell.WriteLine(afkManager.IsAfk(player) ? "They are indeed AFK" : "They are not AFK");
		}

		// Token: 0x06002C16 RID: 11286 RVA: 0x000E6D04 File Offset: 0x000E4F04
		public CompletionResult GetCompletion(IConsoleShell shell, string[] args)
		{
			if (args.Length == 1)
			{
				return CompletionResult.FromHintOptions(CompletionHelper.SessionNames(true, this._players), "<playerName>");
			}
			return CompletionResult.Empty;
		}

		// Token: 0x04001B50 RID: 6992
		[Dependency]
		private readonly IPlayerManager _players;
	}
}
