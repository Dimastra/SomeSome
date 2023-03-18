using System;
using System.Runtime.CompilerServices;
using Content.Server.Chat.Managers;
using Content.Shared.Administration;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.IoC;

namespace Content.Server.Chat.Commands
{
	// Token: 0x020006D1 RID: 1745
	[NullableContext(1)]
	[Nullable(0)]
	[AnyCommand]
	internal sealed class OOCCommand : IConsoleCommand
	{
		// Token: 0x17000569 RID: 1385
		// (get) Token: 0x0600246D RID: 9325 RVA: 0x000BE09E File Offset: 0x000BC29E
		public string Command
		{
			get
			{
				return "ooc";
			}
		}

		// Token: 0x1700056A RID: 1386
		// (get) Token: 0x0600246E RID: 9326 RVA: 0x000BE0A5 File Offset: 0x000BC2A5
		public string Description
		{
			get
			{
				return "Send Out Of Character chat messages.";
			}
		}

		// Token: 0x1700056B RID: 1387
		// (get) Token: 0x0600246F RID: 9327 RVA: 0x000BE0AC File Offset: 0x000BC2AC
		public string Help
		{
			get
			{
				return "ooc <text>";
			}
		}

		// Token: 0x06002470 RID: 9328 RVA: 0x000BE0B4 File Offset: 0x000BC2B4
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			IPlayerSession player = shell.Player as IPlayerSession;
			if (player == null)
			{
				shell.WriteError("This command cannot be run from the server.");
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
			IoCManager.Resolve<IChatManager>().TrySendOOCMessage(player, message, OOCChatType.OOC);
		}
	}
}
