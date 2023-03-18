using System;
using System.Runtime.CompilerServices;
using Content.Server.RoundEnd;
using Content.Shared.Administration;
using Content.Shared.Localizations;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Players;

namespace Content.Server.Administration.Commands
{
	// Token: 0x02000866 RID: 2150
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Round)]
	public sealed class CallShuttleCommand : IConsoleCommand
	{
		// Token: 0x170007BA RID: 1978
		// (get) Token: 0x06002F01 RID: 12033 RVA: 0x000F3ADF File Offset: 0x000F1CDF
		public string Command
		{
			get
			{
				return "callshuttle";
			}
		}

		// Token: 0x170007BB RID: 1979
		// (get) Token: 0x06002F02 RID: 12034 RVA: 0x000F3AE6 File Offset: 0x000F1CE6
		public string Description
		{
			get
			{
				return Loc.GetString("call-shuttle-command-description");
			}
		}

		// Token: 0x170007BC RID: 1980
		// (get) Token: 0x06002F03 RID: 12035 RVA: 0x000F3AF2 File Offset: 0x000F1CF2
		public string Help
		{
			get
			{
				return Loc.GetString("call-shuttle-command-help-text", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("command", this.Command)
				});
			}
		}

		// Token: 0x06002F04 RID: 12036 RVA: 0x000F3B1C File Offset: 0x000F1D1C
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			ILocalizationManager loc = IoCManager.Resolve<ILocalizationManager>();
			TimeSpan timeSpan;
			if (args.Length == 1 && TimeSpan.TryParseExact(args[0], ContentLocalizationManager.TimeSpanMinutesFormats, loc.DefaultCulture, out timeSpan))
			{
				RoundEndSystem roundEndSystem = EntitySystem.Get<RoundEndSystem>();
				TimeSpan countdownTime = timeSpan;
				ICommonSession player = shell.Player;
				roundEndSystem.RequestRoundEnd(countdownTime, (player != null) ? player.AttachedEntity : null, false, false, null);
				return;
			}
			if (args.Length == 1)
			{
				shell.WriteLine(Loc.GetString("shell-timespan-minutes-must-be-correct"));
				return;
			}
			RoundEndSystem roundEndSystem2 = EntitySystem.Get<RoundEndSystem>();
			ICommonSession player2 = shell.Player;
			roundEndSystem2.RequestRoundEnd((player2 != null) ? player2.AttachedEntity : null, false, false, null);
		}
	}
}
