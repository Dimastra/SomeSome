using System;
using System.Runtime.CompilerServices;
using Content.Server.Players.PlayTimeTracking;
using Content.Shared.Administration;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Administration.Commands
{
	// Token: 0x0200084E RID: 2126
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Admin)]
	public sealed class PlayTimeGetOverallCommand : IConsoleCommand
	{
		// Token: 0x17000772 RID: 1906
		// (get) Token: 0x06002E80 RID: 11904 RVA: 0x000F22B3 File Offset: 0x000F04B3
		public string Command
		{
			get
			{
				return "playtime_getoverall";
			}
		}

		// Token: 0x17000773 RID: 1907
		// (get) Token: 0x06002E81 RID: 11905 RVA: 0x000F22BA File Offset: 0x000F04BA
		public string Description
		{
			get
			{
				return Loc.GetString("cmd-playtime_getoverall-desc");
			}
		}

		// Token: 0x17000774 RID: 1908
		// (get) Token: 0x06002E82 RID: 11906 RVA: 0x000F22C6 File Offset: 0x000F04C6
		public string Help
		{
			get
			{
				return Loc.GetString("cmd-playtime_getoverall-help", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("command", this.Command)
				});
			}
		}

		// Token: 0x06002E83 RID: 11907 RVA: 0x000F22F0 File Offset: 0x000F04F0
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			PlayTimeGetOverallCommand.<Execute>d__8 <Execute>d__;
			<Execute>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Execute>d__.<>4__this = this;
			<Execute>d__.shell = shell;
			<Execute>d__.args = args;
			<Execute>d__.<>1__state = -1;
			<Execute>d__.<>t__builder.Start<PlayTimeGetOverallCommand.<Execute>d__8>(ref <Execute>d__);
		}

		// Token: 0x06002E84 RID: 11908 RVA: 0x000F2337 File Offset: 0x000F0537
		public CompletionResult GetCompletion(IConsoleShell shell, string[] args)
		{
			if (args.Length == 1)
			{
				return CompletionResult.FromHintOptions(CompletionHelper.SessionNames(true, this._playerManager), Loc.GetString("cmd-playtime_getoverall-arg-user"));
			}
			return CompletionResult.Empty;
		}

		// Token: 0x04001C58 RID: 7256
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04001C59 RID: 7257
		[Dependency]
		private readonly PlayTimeTrackingManager _playTimeTracking;
	}
}
