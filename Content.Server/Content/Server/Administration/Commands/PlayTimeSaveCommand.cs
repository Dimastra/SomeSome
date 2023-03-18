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
	// Token: 0x02000850 RID: 2128
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Admin)]
	public sealed class PlayTimeSaveCommand : IConsoleCommand
	{
		// Token: 0x17000778 RID: 1912
		// (get) Token: 0x06002E8C RID: 11916 RVA: 0x000F2445 File Offset: 0x000F0645
		public string Command
		{
			get
			{
				return "playtime_save";
			}
		}

		// Token: 0x17000779 RID: 1913
		// (get) Token: 0x06002E8D RID: 11917 RVA: 0x000F244C File Offset: 0x000F064C
		public string Description
		{
			get
			{
				return Loc.GetString("cmd-playtime_save-desc");
			}
		}

		// Token: 0x1700077A RID: 1914
		// (get) Token: 0x06002E8E RID: 11918 RVA: 0x000F2458 File Offset: 0x000F0658
		public string Help
		{
			get
			{
				return Loc.GetString("cmd-playtime_save-help", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("command", this.Command)
				});
			}
		}

		// Token: 0x06002E8F RID: 11919 RVA: 0x000F2484 File Offset: 0x000F0684
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			PlayTimeSaveCommand.<Execute>d__8 <Execute>d__;
			<Execute>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Execute>d__.<>4__this = this;
			<Execute>d__.shell = shell;
			<Execute>d__.args = args;
			<Execute>d__.<>1__state = -1;
			<Execute>d__.<>t__builder.Start<PlayTimeSaveCommand.<Execute>d__8>(ref <Execute>d__);
		}

		// Token: 0x06002E90 RID: 11920 RVA: 0x000F24CB File Offset: 0x000F06CB
		public CompletionResult GetCompletion(IConsoleShell shell, string[] args)
		{
			if (args.Length == 1)
			{
				return CompletionResult.FromHintOptions(CompletionHelper.SessionNames(true, this._playerManager), Loc.GetString("cmd-playtime_save-arg-user"));
			}
			return CompletionResult.Empty;
		}

		// Token: 0x04001C5C RID: 7260
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04001C5D RID: 7261
		[Dependency]
		private readonly PlayTimeTrackingManager _playTimeTracking;
	}
}
