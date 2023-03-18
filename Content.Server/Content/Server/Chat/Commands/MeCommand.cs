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
	// Token: 0x020006D0 RID: 1744
	[NullableContext(1)]
	[Nullable(0)]
	[AnyCommand]
	internal sealed class MeCommand : IConsoleCommand
	{
		// Token: 0x17000566 RID: 1382
		// (get) Token: 0x06002468 RID: 9320 RVA: 0x000BDFED File Offset: 0x000BC1ED
		public string Command
		{
			get
			{
				return "me";
			}
		}

		// Token: 0x17000567 RID: 1383
		// (get) Token: 0x06002469 RID: 9321 RVA: 0x000BDFF4 File Offset: 0x000BC1F4
		public string Description
		{
			get
			{
				return "Perform an action.";
			}
		}

		// Token: 0x17000568 RID: 1384
		// (get) Token: 0x0600246A RID: 9322 RVA: 0x000BDFFB File Offset: 0x000BC1FB
		public string Help
		{
			get
			{
				return "me <text>";
			}
		}

		// Token: 0x0600246B RID: 9323 RVA: 0x000BE004 File Offset: 0x000BC204
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
			IoCManager.Resolve<IEntitySystemManager>().GetEntitySystem<ChatSystem>().TrySendInGameICMessage(playerEntity, message, InGameICChatType.Emote, false, false, shell, player, null, true, false);
		}
	}
}
