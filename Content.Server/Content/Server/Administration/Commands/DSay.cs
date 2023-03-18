using System;
using System.Runtime.CompilerServices;
using Content.Server.Chat.Systems;
using Content.Shared.Administration;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;

namespace Content.Server.Administration.Commands
{
	// Token: 0x0200083E RID: 2110
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Admin)]
	internal sealed class DSay : IConsoleCommand
	{
		// Token: 0x17000744 RID: 1860
		// (get) Token: 0x06002E2E RID: 11822 RVA: 0x000F1248 File Offset: 0x000EF448
		public string Command
		{
			get
			{
				return "dsay";
			}
		}

		// Token: 0x17000745 RID: 1861
		// (get) Token: 0x06002E2F RID: 11823 RVA: 0x000F124F File Offset: 0x000EF44F
		public string Description
		{
			get
			{
				return Loc.GetString("dsay-command-description");
			}
		}

		// Token: 0x17000746 RID: 1862
		// (get) Token: 0x06002E30 RID: 11824 RVA: 0x000F125B File Offset: 0x000EF45B
		public string Help
		{
			get
			{
				return Loc.GetString("dsay-command-help-text", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("command", this.Command)
				});
			}
		}

		// Token: 0x06002E31 RID: 11825 RVA: 0x000F1284 File Offset: 0x000EF484
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			IPlayerSession player = shell.Player as IPlayerSession;
			if (player == null)
			{
				shell.WriteLine("shell-only-players-can-run-this-command");
				return;
			}
			EntityUid? attachedEntity = player.AttachedEntity;
			if (attachedEntity != null)
			{
				EntityUid entity = attachedEntity.GetValueOrDefault();
				if (entity.Valid)
				{
					if (args.Length < 1)
					{
						return;
					}
					string message = string.Join(" ", args).Trim();
					if (string.IsNullOrEmpty(message))
					{
						return;
					}
					EntitySystem.Get<ChatSystem>().TrySendInGameOOCMessage(entity, message, InGameOOCChatType.Dead, false, shell, player);
					return;
				}
			}
		}
	}
}
