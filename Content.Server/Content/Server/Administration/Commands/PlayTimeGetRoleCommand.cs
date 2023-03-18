using System;
using System.Runtime.CompilerServices;
using Content.Server.Players.PlayTimeTracking;
using Content.Shared.Administration;
using Content.Shared.Players.PlayTimeTracking;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Administration.Commands
{
	// Token: 0x0200084F RID: 2127
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Admin)]
	public sealed class PlayTimeGetRoleCommand : IConsoleCommand
	{
		// Token: 0x17000775 RID: 1909
		// (get) Token: 0x06002E86 RID: 11910 RVA: 0x000F2368 File Offset: 0x000F0568
		public string Command
		{
			get
			{
				return "playtime_getrole";
			}
		}

		// Token: 0x17000776 RID: 1910
		// (get) Token: 0x06002E87 RID: 11911 RVA: 0x000F236F File Offset: 0x000F056F
		public string Description
		{
			get
			{
				return Loc.GetString("cmd-playtime_getrole-desc");
			}
		}

		// Token: 0x17000777 RID: 1911
		// (get) Token: 0x06002E88 RID: 11912 RVA: 0x000F237B File Offset: 0x000F057B
		public string Help
		{
			get
			{
				return Loc.GetString("cmd-playtime_getrole-help", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("command", this.Command)
				});
			}
		}

		// Token: 0x06002E89 RID: 11913 RVA: 0x000F23A4 File Offset: 0x000F05A4
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			PlayTimeGetRoleCommand.<Execute>d__8 <Execute>d__;
			<Execute>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Execute>d__.<>4__this = this;
			<Execute>d__.shell = shell;
			<Execute>d__.args = args;
			<Execute>d__.<>1__state = -1;
			<Execute>d__.<>t__builder.Start<PlayTimeGetRoleCommand.<Execute>d__8>(ref <Execute>d__);
		}

		// Token: 0x06002E8A RID: 11914 RVA: 0x000F23EC File Offset: 0x000F05EC
		public CompletionResult GetCompletion(IConsoleShell shell, string[] args)
		{
			if (args.Length == 1)
			{
				return CompletionResult.FromHintOptions(CompletionHelper.SessionNames(true, this._playerManager), Loc.GetString("cmd-playtime_getrole-arg-user"));
			}
			if (args.Length == 2)
			{
				return CompletionResult.FromHintOptions(CompletionHelper.PrototypeIDs<PlayTimeTrackerPrototype>(true, null), Loc.GetString("cmd-playtime_getrole-arg-role"));
			}
			return CompletionResult.Empty;
		}

		// Token: 0x04001C5A RID: 7258
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04001C5B RID: 7259
		[Dependency]
		private readonly PlayTimeTrackingManager _playTimeTracking;
	}
}
