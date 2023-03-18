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
	// Token: 0x0200084C RID: 2124
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Admin)]
	public sealed class PlayTimeAddOverallCommand : IConsoleCommand
	{
		// Token: 0x1700076C RID: 1900
		// (get) Token: 0x06002E74 RID: 11892 RVA: 0x000F20F7 File Offset: 0x000F02F7
		public string Command
		{
			get
			{
				return "playtime_addoverall";
			}
		}

		// Token: 0x1700076D RID: 1901
		// (get) Token: 0x06002E75 RID: 11893 RVA: 0x000F20FE File Offset: 0x000F02FE
		public string Description
		{
			get
			{
				return Loc.GetString("cmd-playtime_addoverall-desc");
			}
		}

		// Token: 0x1700076E RID: 1902
		// (get) Token: 0x06002E76 RID: 11894 RVA: 0x000F210A File Offset: 0x000F030A
		public string Help
		{
			get
			{
				return Loc.GetString("cmd-playtime_addoverall-help", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("command", this.Command)
				});
			}
		}

		// Token: 0x06002E77 RID: 11895 RVA: 0x000F2134 File Offset: 0x000F0334
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			PlayTimeAddOverallCommand.<Execute>d__8 <Execute>d__;
			<Execute>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Execute>d__.<>4__this = this;
			<Execute>d__.shell = shell;
			<Execute>d__.args = args;
			<Execute>d__.<>1__state = -1;
			<Execute>d__.<>t__builder.Start<PlayTimeAddOverallCommand.<Execute>d__8>(ref <Execute>d__);
		}

		// Token: 0x06002E78 RID: 11896 RVA: 0x000F217B File Offset: 0x000F037B
		public CompletionResult GetCompletion(IConsoleShell shell, string[] args)
		{
			if (args.Length == 1)
			{
				return CompletionResult.FromHintOptions(CompletionHelper.SessionNames(true, null), Loc.GetString("cmd-playtime_addoverall-arg-user"));
			}
			if (args.Length == 2)
			{
				return CompletionResult.FromHint(Loc.GetString("cmd-playtime_addoverall-arg-minutes"));
			}
			return CompletionResult.Empty;
		}

		// Token: 0x04001C54 RID: 7252
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04001C55 RID: 7253
		[Dependency]
		private readonly PlayTimeTrackingManager _playTimeTracking;
	}
}
