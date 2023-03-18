using System;
using System.Runtime.CompilerServices;
using Content.Client.UserInterface.Systems.Bwoink;
using Content.Shared.Administration;
using Robust.Client.UserInterface;
using Robust.Shared.Console;
using Robust.Shared.IoC;
using Robust.Shared.Network;

namespace Content.Client.Commands
{
	// Token: 0x020003AC RID: 940
	[NullableContext(1)]
	[Nullable(0)]
	[AnyCommand]
	public sealed class OpenAHelpCommand : IConsoleCommand
	{
		// Token: 0x170004C5 RID: 1221
		// (get) Token: 0x0600175F RID: 5983 RVA: 0x00086A23 File Offset: 0x00084C23
		public string Command
		{
			get
			{
				return "openahelp";
			}
		}

		// Token: 0x170004C6 RID: 1222
		// (get) Token: 0x06001760 RID: 5984 RVA: 0x00086A2A File Offset: 0x00084C2A
		public string Description
		{
			get
			{
				return "Opens AHelp channel for a given NetUserID, or your personal channel if none given.";
			}
		}

		// Token: 0x170004C7 RID: 1223
		// (get) Token: 0x06001761 RID: 5985 RVA: 0x00086A31 File Offset: 0x00084C31
		public string Help
		{
			get
			{
				return this.Command + " [<netuserid>]";
			}
		}

		// Token: 0x06001762 RID: 5986 RVA: 0x00086A44 File Offset: 0x00084C44
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length >= 2)
			{
				shell.WriteLine(this.Help);
				return;
			}
			if (args.Length == 0)
			{
				IoCManager.Resolve<IUserInterfaceManager>().GetUIController<AHelpUIController>().Open();
				return;
			}
			Guid guid;
			if (Guid.TryParse(args[0], out guid))
			{
				NetUserId userId;
				userId..ctor(guid);
				IoCManager.Resolve<IUserInterfaceManager>().GetUIController<AHelpUIController>().Open(userId);
				return;
			}
			shell.WriteLine("Bad GUID!");
		}
	}
}
