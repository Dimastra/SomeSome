using System;
using System.Runtime.CompilerServices;
using Content.Server.GameTicking;
using Content.Server.Mind;
using Content.Server.Mind.Components;
using Content.Server.Players;
using Content.Shared.Administration;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;

namespace Content.Server.Chat.Commands
{
	// Token: 0x020006D6 RID: 1750
	[NullableContext(1)]
	[Nullable(0)]
	[AnyCommand]
	internal sealed class SuicideCommand : IConsoleCommand
	{
		// Token: 0x17000579 RID: 1401
		// (get) Token: 0x06002487 RID: 9351 RVA: 0x000BE38D File Offset: 0x000BC58D
		public string Command
		{
			get
			{
				return "suicide";
			}
		}

		// Token: 0x1700057A RID: 1402
		// (get) Token: 0x06002488 RID: 9352 RVA: 0x000BE394 File Offset: 0x000BC594
		public string Description
		{
			get
			{
				return Loc.GetString("suicide-command-description");
			}
		}

		// Token: 0x1700057B RID: 1403
		// (get) Token: 0x06002489 RID: 9353 RVA: 0x000BE3A0 File Offset: 0x000BC5A0
		public string Help
		{
			get
			{
				return Loc.GetString("suicide-command-help-text");
			}
		}

		// Token: 0x0600248A RID: 9354 RVA: 0x000BE3AC File Offset: 0x000BC5AC
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			IPlayerSession player = shell.Player as IPlayerSession;
			if (player == null)
			{
				shell.WriteLine(Loc.GetString("shell-cannot-run-command-from-server"));
				return;
			}
			if (player.Status != 3 || player.AttachedEntity == null)
			{
				return;
			}
			PlayerData playerData = player.ContentData();
			Mind mind = (playerData != null) ? playerData.Mind : null;
			EntityUid? entityUid;
			if (mind == null)
			{
				entityUid = null;
			}
			else
			{
				MindComponent ownedComponent = mind.OwnedComponent;
				entityUid = ((ownedComponent != null) ? new EntityUid?(ownedComponent.Owner) : null);
			}
			EntityUid? entityUid2 = entityUid;
			if (entityUid2 != null)
			{
				EntityUid victim = entityUid2.GetValueOrDefault();
				if (victim.Valid)
				{
					GameTicker gameTicker = EntitySystem.Get<GameTicker>();
					if (EntitySystem.Get<SuicideSystem>().Suicide(victim))
					{
						gameTicker.OnGhostAttempt(mind, false, false);
						return;
					}
					if (gameTicker.OnGhostAttempt(mind, true, false))
					{
						return;
					}
					shell.WriteLine("You can't ghost right now.");
					return;
				}
			}
			shell.WriteLine("You don't have a mind!");
		}
	}
}
