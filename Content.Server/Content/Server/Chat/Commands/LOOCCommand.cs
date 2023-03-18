using System;
using System.Runtime.CompilerServices;
using Content.Server.Chat.Systems;
using Content.Shared.Administration;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;

namespace Content.Server.Chat.Commands
{
	// Token: 0x020006CF RID: 1743
	[NullableContext(1)]
	[Nullable(0)]
	[AnyCommand]
	internal sealed class LOOCCommand : IConsoleCommand
	{
		// Token: 0x17000563 RID: 1379
		// (get) Token: 0x06002463 RID: 9315 RVA: 0x000BDF48 File Offset: 0x000BC148
		public string Command
		{
			get
			{
				return "looc";
			}
		}

		// Token: 0x17000564 RID: 1380
		// (get) Token: 0x06002464 RID: 9316 RVA: 0x000BDF4F File Offset: 0x000BC14F
		public string Description
		{
			get
			{
				return "Send Local Out Of Character chat messages.";
			}
		}

		// Token: 0x17000565 RID: 1381
		// (get) Token: 0x06002465 RID: 9317 RVA: 0x000BDF56 File Offset: 0x000BC156
		public string Help
		{
			get
			{
				return "looc <text>";
			}
		}

		// Token: 0x06002466 RID: 9318 RVA: 0x000BDF60 File Offset: 0x000BC160
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			IPlayerSession player = shell.Player as IPlayerSession;
			if (player == null)
			{
				shell.WriteError("This command cannot be run from the server.");
				return;
			}
			EntityUid? attachedEntity = player.AttachedEntity;
			if (attachedEntity != null)
			{
				EntityUid entity = attachedEntity.GetValueOrDefault();
				if (entity.Valid)
				{
					if (player.Status != 3)
					{
						return;
					}
					if (args.Length < 1)
					{
						return;
					}
					string message = string.Join(" ", args).Trim();
					if (string.IsNullOrEmpty(message))
					{
						return;
					}
					EntitySystem.Get<ChatSystem>().TrySendInGameOOCMessage(entity, message, InGameOOCChatType.Looc, false, shell, player);
					return;
				}
			}
		}
	}
}
