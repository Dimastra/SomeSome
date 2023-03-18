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
	// Token: 0x020006D3 RID: 1747
	[NullableContext(1)]
	[Nullable(0)]
	[AnyCommand]
	internal sealed class SayCommand : IConsoleCommand
	{
		// Token: 0x17000570 RID: 1392
		// (get) Token: 0x06002478 RID: 9336 RVA: 0x000BE179 File Offset: 0x000BC379
		public string Command
		{
			get
			{
				return "say";
			}
		}

		// Token: 0x17000571 RID: 1393
		// (get) Token: 0x06002479 RID: 9337 RVA: 0x000BE180 File Offset: 0x000BC380
		public string Description
		{
			get
			{
				return "Send chat messages to the local channel or a specified radio channel.";
			}
		}

		// Token: 0x17000572 RID: 1394
		// (get) Token: 0x0600247A RID: 9338 RVA: 0x000BE187 File Offset: 0x000BC387
		public string Help
		{
			get
			{
				return "say <text>";
			}
		}

		// Token: 0x0600247B RID: 9339 RVA: 0x000BE190 File Offset: 0x000BC390
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
			IoCManager.Resolve<IEntitySystemManager>().GetEntitySystem<ChatSystem>().TrySendInGameICMessage(playerEntity, message, InGameICChatType.Speak, false, false, shell, player, null, true, false);
		}
	}
}
