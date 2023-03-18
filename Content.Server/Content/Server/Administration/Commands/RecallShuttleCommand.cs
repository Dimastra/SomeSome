using System;
using System.Runtime.CompilerServices;
using Content.Server.RoundEnd;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Players;

namespace Content.Server.Administration.Commands
{
	// Token: 0x02000867 RID: 2151
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Round)]
	public sealed class RecallShuttleCommand : IConsoleCommand
	{
		// Token: 0x170007BD RID: 1981
		// (get) Token: 0x06002F06 RID: 12038 RVA: 0x000F3BCB File Offset: 0x000F1DCB
		public string Command
		{
			get
			{
				return "recallshuttle";
			}
		}

		// Token: 0x170007BE RID: 1982
		// (get) Token: 0x06002F07 RID: 12039 RVA: 0x000F3BD2 File Offset: 0x000F1DD2
		public string Description
		{
			get
			{
				return Loc.GetString("recall-shuttle-command-description");
			}
		}

		// Token: 0x170007BF RID: 1983
		// (get) Token: 0x06002F08 RID: 12040 RVA: 0x000F3BDE File Offset: 0x000F1DDE
		public string Help
		{
			get
			{
				return Loc.GetString("recall-shuttle-command-help-text", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("command", this.Command)
				});
			}
		}

		// Token: 0x06002F09 RID: 12041 RVA: 0x000F3C08 File Offset: 0x000F1E08
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			RoundEndSystem roundEndSystem = EntitySystem.Get<RoundEndSystem>();
			ICommonSession player = shell.Player;
			roundEndSystem.CancelRoundEndCountdown((player != null) ? player.AttachedEntity : null, false, null);
		}
	}
}
