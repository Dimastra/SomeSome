using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Shared.Administration;
using Content.Shared.Roles;
using Robust.Shared.Console;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Server.Roles
{
	// Token: 0x0200022A RID: 554
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Admin)]
	public sealed class ListRolesCommand : IConsoleCommand
	{
		// Token: 0x170001B8 RID: 440
		// (get) Token: 0x06000B12 RID: 2834 RVA: 0x0003A194 File Offset: 0x00038394
		public string Command
		{
			get
			{
				return "listroles";
			}
		}

		// Token: 0x170001B9 RID: 441
		// (get) Token: 0x06000B13 RID: 2835 RVA: 0x0003A19B File Offset: 0x0003839B
		public string Description
		{
			get
			{
				return "Lists roles";
			}
		}

		// Token: 0x170001BA RID: 442
		// (get) Token: 0x06000B14 RID: 2836 RVA: 0x0003A1A2 File Offset: 0x000383A2
		public string Help
		{
			get
			{
				return "listroles";
			}
		}

		// Token: 0x06000B15 RID: 2837 RVA: 0x0003A1AC File Offset: 0x000383AC
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length != 0)
			{
				shell.WriteLine("Expected no arguments.");
				return;
			}
			foreach (JobPrototype job in IoCManager.Resolve<IPrototypeManager>().EnumeratePrototypes<JobPrototype>())
			{
				shell.WriteLine(job.ID);
			}
		}
	}
}
