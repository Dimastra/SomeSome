using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Server.Chat.Managers;
using Content.Shared.Administration;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.IoC;

namespace Content.Server.Chat.Commands
{
	// Token: 0x020006CD RID: 1741
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Admin)]
	internal sealed class AdminChatCommand : IConsoleCommand
	{
		// Token: 0x1700055D RID: 1373
		// (get) Token: 0x06002459 RID: 9305 RVA: 0x000BDDF4 File Offset: 0x000BBFF4
		public string Command
		{
			get
			{
				return "asay";
			}
		}

		// Token: 0x1700055E RID: 1374
		// (get) Token: 0x0600245A RID: 9306 RVA: 0x000BDDFB File Offset: 0x000BBFFB
		public string Description
		{
			get
			{
				return "Send chat messages to the private admin chat channel.";
			}
		}

		// Token: 0x1700055F RID: 1375
		// (get) Token: 0x0600245B RID: 9307 RVA: 0x000BDE02 File Offset: 0x000BC002
		public string Help
		{
			get
			{
				return "asay <text>";
			}
		}

		// Token: 0x0600245C RID: 9308 RVA: 0x000BDE0C File Offset: 0x000BC00C
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			IPlayerSession player = (IPlayerSession)shell.Player;
			if (player == null)
			{
				shell.WriteError("You can't run this command locally.");
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
			IoCManager.Resolve<IChatManager>().TrySendOOCMessage(player, message, OOCChatType.Admin);
		}
	}
}
