using System;
using System.Runtime.CompilerServices;
using Content.Server.Chat.Systems;
using Content.Shared.Administration;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Chat.Commands
{
	// Token: 0x020006D7 RID: 1751
	[NullableContext(1)]
	[Nullable(0)]
	[AnyCommand]
	internal sealed class WhisperCommand : IConsoleCommand
	{
		// Token: 0x1700057C RID: 1404
		// (get) Token: 0x0600248C RID: 9356 RVA: 0x000BE49B File Offset: 0x000BC69B
		public string Command
		{
			get
			{
				return "whisper";
			}
		}

		// Token: 0x1700057D RID: 1405
		// (get) Token: 0x0600248D RID: 9357 RVA: 0x000BE4A2 File Offset: 0x000BC6A2
		public string Description
		{
			get
			{
				return "Send chat messages to the local channel as a whisper";
			}
		}

		// Token: 0x1700057E RID: 1406
		// (get) Token: 0x0600248E RID: 9358 RVA: 0x000BE4A9 File Offset: 0x000BC6A9
		public string Help
		{
			get
			{
				return "whisper <text>";
			}
		}

		// Token: 0x0600248F RID: 9359 RVA: 0x000BE4B0 File Offset: 0x000BC6B0
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			IPlayerSession player = shell.Player as IPlayerSession;
			if (player == null)
			{
				shell.WriteError("This command cannot be run from the server.");
				return;
			}
			if (player.Status != 3)
			{
				return;
			}
			EntityUid? attachedEntity = player.AttachedEntity;
			if (attachedEntity == null)
			{
				shell.WriteError("You don't have an entity!");
				return;
			}
			EntityUid playerEntity = attachedEntity.GetValueOrDefault();
			if (args.Length < 1)
			{
				return;
			}
			string message = string.Join(" ", args).Trim();
			if (string.IsNullOrEmpty(message))
			{
				return;
			}
			IoCManager.Resolve<IEntitySystemManager>().GetEntitySystem<ChatSystem>().TrySendInGameICMessage(playerEntity, message, InGameICChatType.Whisper, false, false, shell, player, null, true, false);
		}
	}
}
