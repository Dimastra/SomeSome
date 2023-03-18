using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Server.Chat.Systems;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Server.Announcements
{
	// Token: 0x020007CD RID: 1997
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Admin)]
	public sealed class AnnounceCommand : IConsoleCommand
	{
		// Token: 0x170006B2 RID: 1714
		// (get) Token: 0x06002B6E RID: 11118 RVA: 0x000E3DA7 File Offset: 0x000E1FA7
		public string Command
		{
			get
			{
				return "announce";
			}
		}

		// Token: 0x170006B3 RID: 1715
		// (get) Token: 0x06002B6F RID: 11119 RVA: 0x000E3DAE File Offset: 0x000E1FAE
		public string Description
		{
			get
			{
				return "Send an in-game announcement.";
			}
		}

		// Token: 0x170006B4 RID: 1716
		// (get) Token: 0x06002B70 RID: 11120 RVA: 0x000E3DB5 File Offset: 0x000E1FB5
		public string Help
		{
			get
			{
				return this.Command + " <sender> <message> or " + this.Command + " <message> to send announcement as CentCom.";
			}
		}

		// Token: 0x06002B71 RID: 11121 RVA: 0x000E3DD4 File Offset: 0x000E1FD4
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			ChatSystem chat = IoCManager.Resolve<IEntitySystemManager>().GetEntitySystem<ChatSystem>();
			if (args.Length == 0)
			{
				shell.WriteError("Not enough arguments! Need at least 1.");
				return;
			}
			if (args.Length == 1)
			{
				chat.DispatchGlobalAnnouncement(args[0], "Central Command", true, null, new Color?(Color.Gold));
			}
			else
			{
				string message = string.Join<string>(' ', new ArraySegment<string>(args, 1, args.Length - 1));
				chat.DispatchGlobalAnnouncement(message, args[0], true, null, new Color?(Color.Gold));
			}
			shell.WriteLine("Sent!");
		}
	}
}
