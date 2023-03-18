using System;
using System.Runtime.CompilerServices;
using Content.Server.GameTicking;
using Content.Server.Mind;
using Content.Server.Players;
using Content.Shared.Administration;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;

namespace Content.Server.Ghost
{
	// Token: 0x0200048F RID: 1167
	[NullableContext(1)]
	[Nullable(0)]
	[AnyCommand]
	public sealed class Ghost : IConsoleCommand
	{
		// Token: 0x17000328 RID: 808
		// (get) Token: 0x0600175F RID: 5983 RVA: 0x0007A858 File Offset: 0x00078A58
		public string Command
		{
			get
			{
				return "ghost";
			}
		}

		// Token: 0x17000329 RID: 809
		// (get) Token: 0x06001760 RID: 5984 RVA: 0x0007A85F File Offset: 0x00078A5F
		public string Description
		{
			get
			{
				return "Give up on life and become a ghost.";
			}
		}

		// Token: 0x1700032A RID: 810
		// (get) Token: 0x06001761 RID: 5985 RVA: 0x0007A866 File Offset: 0x00078A66
		public string Help
		{
			get
			{
				return "ghost";
			}
		}

		// Token: 0x06001762 RID: 5986 RVA: 0x0007A870 File Offset: 0x00078A70
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			IPlayerSession player = shell.Player as IPlayerSession;
			if (player == null)
			{
				shell.WriteLine("You have no session, you can't ghost.");
				return;
			}
			PlayerData playerData = player.ContentData();
			Mind mind = (playerData != null) ? playerData.Mind : null;
			if (mind == null)
			{
				shell.WriteLine("You have no Mind, you can't ghost.");
				return;
			}
			if (!EntitySystem.Get<GameTicker>().OnGhostAttempt(mind, true, true))
			{
				shell.WriteLine("You can't ghost right now.");
			}
		}
	}
}
