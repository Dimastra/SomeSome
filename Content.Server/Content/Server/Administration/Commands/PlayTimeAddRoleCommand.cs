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
	// Token: 0x0200084D RID: 2125
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Admin)]
	public sealed class PlayTimeAddRoleCommand : IConsoleCommand
	{
		// Token: 0x1700076F RID: 1903
		// (get) Token: 0x06002E7A RID: 11898 RVA: 0x000F21BD File Offset: 0x000F03BD
		public string Command
		{
			get
			{
				return "playtime_addrole";
			}
		}

		// Token: 0x17000770 RID: 1904
		// (get) Token: 0x06002E7B RID: 11899 RVA: 0x000F21C4 File Offset: 0x000F03C4
		public string Description
		{
			get
			{
				return Loc.GetString("cmd-playtime_addrole-desc");
			}
		}

		// Token: 0x17000771 RID: 1905
		// (get) Token: 0x06002E7C RID: 11900 RVA: 0x000F21D0 File Offset: 0x000F03D0
		public string Help
		{
			get
			{
				return Loc.GetString("cmd-playtime_addrole-help", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("command", this.Command)
				});
			}
		}

		// Token: 0x06002E7D RID: 11901 RVA: 0x000F21FC File Offset: 0x000F03FC
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			PlayTimeAddRoleCommand.<Execute>d__8 <Execute>d__;
			<Execute>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Execute>d__.<>4__this = this;
			<Execute>d__.shell = shell;
			<Execute>d__.args = args;
			<Execute>d__.<>1__state = -1;
			<Execute>d__.<>t__builder.Start<PlayTimeAddRoleCommand.<Execute>d__8>(ref <Execute>d__);
		}

		// Token: 0x06002E7E RID: 11902 RVA: 0x000F2244 File Offset: 0x000F0444
		public CompletionResult GetCompletion(IConsoleShell shell, string[] args)
		{
			if (args.Length == 1)
			{
				return CompletionResult.FromHintOptions(CompletionHelper.SessionNames(true, this._playerManager), Loc.GetString("cmd-playtime_addrole-arg-user"));
			}
			if (args.Length == 2)
			{
				return CompletionResult.FromHintOptions(CompletionHelper.PrototypeIDs<PlayTimeTrackerPrototype>(true, null), Loc.GetString("cmd-playtime_addrole-arg-role"));
			}
			if (args.Length == 3)
			{
				return CompletionResult.FromHint(Loc.GetString("cmd-playtime_addrole-arg-minutes"));
			}
			return CompletionResult.Empty;
		}

		// Token: 0x04001C56 RID: 7254
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04001C57 RID: 7255
		[Dependency]
		private readonly PlayTimeTrackingManager _playTimeTracking;
	}
}
