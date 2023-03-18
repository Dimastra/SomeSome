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
	// Token: 0x02000851 RID: 2129
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Debug)]
	public sealed class PlayTimeFlushCommand : IConsoleCommand
	{
		// Token: 0x1700077B RID: 1915
		// (get) Token: 0x06002E92 RID: 11922 RVA: 0x000F24FC File Offset: 0x000F06FC
		public string Command
		{
			get
			{
				return "playtime_flush";
			}
		}

		// Token: 0x1700077C RID: 1916
		// (get) Token: 0x06002E93 RID: 11923 RVA: 0x000F2503 File Offset: 0x000F0703
		public string Description
		{
			get
			{
				return Loc.GetString("cmd-playtime_flush-desc");
			}
		}

		// Token: 0x1700077D RID: 1917
		// (get) Token: 0x06002E94 RID: 11924 RVA: 0x000F250F File Offset: 0x000F070F
		public string Help
		{
			get
			{
				return Loc.GetString("cmd-playtime_flush-help", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("command", this.Command)
				});
			}
		}

		// Token: 0x06002E95 RID: 11925 RVA: 0x000F2538 File Offset: 0x000F0738
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			int num = args.Length;
			if (num != 0 && num != 1)
			{
				shell.WriteError(Loc.GetString("cmd-playtime_flush-error-args"));
				return;
			}
			if (args.Length == 0)
			{
				this._playTimeTracking.FlushAllTrackers();
				return;
			}
			string name = args[0];
			IPlayerSession pSession;
			if (!this._playerManager.TryGetSessionByUsername(name, ref pSession))
			{
				shell.WriteError(Loc.GetString("parse-session-fail", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("username", name)
				}));
				return;
			}
			this._playTimeTracking.FlushTracker(pSession);
		}

		// Token: 0x06002E96 RID: 11926 RVA: 0x000F25BA File Offset: 0x000F07BA
		public CompletionResult GetCompletion(IConsoleShell shell, string[] args)
		{
			if (args.Length == 1)
			{
				return CompletionResult.FromHintOptions(CompletionHelper.SessionNames(true, this._playerManager), Loc.GetString("cmd-playtime_flush-arg-user"));
			}
			return CompletionResult.Empty;
		}

		// Token: 0x04001C5E RID: 7262
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04001C5F RID: 7263
		[Dependency]
		private readonly PlayTimeTrackingManager _playTimeTracking;
	}
}
