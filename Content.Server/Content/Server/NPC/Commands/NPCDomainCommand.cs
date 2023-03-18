using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Server.NPC.HTN;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Server.NPC.Commands
{
	// Token: 0x02000376 RID: 886
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Debug)]
	public sealed class NPCDomainCommand : IConsoleCommand
	{
		// Token: 0x17000288 RID: 648
		// (get) Token: 0x0600121C RID: 4636 RVA: 0x0005EC99 File Offset: 0x0005CE99
		public string Command
		{
			get
			{
				return "npcdomain";
			}
		}

		// Token: 0x17000289 RID: 649
		// (get) Token: 0x0600121D RID: 4637 RVA: 0x0005ECA0 File Offset: 0x0005CEA0
		public string Description
		{
			get
			{
				return "Lists the domain of a particular HTN compound task";
			}
		}

		// Token: 0x1700028A RID: 650
		// (get) Token: 0x0600121E RID: 4638 RVA: 0x0005ECA7 File Offset: 0x0005CEA7
		public string Help
		{
			get
			{
				return this.Command + " <htncompoundtask>";
			}
		}

		// Token: 0x0600121F RID: 4639 RVA: 0x0005ECBC File Offset: 0x0005CEBC
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length != 1)
			{
				shell.WriteError("shell-need-exactly-one-argument");
				return;
			}
			HTNCompoundTask compound;
			if (!IoCManager.Resolve<IPrototypeManager>().TryIndex<HTNCompoundTask>(args[0], ref compound))
			{
				shell.WriteError("Unable to find HTN compound task for '" + args[0] + "'");
				return;
			}
			foreach (string line in IoCManager.Resolve<IEntitySystemManager>().GetEntitySystem<HTNSystem>().GetDomain(compound).Split("\n", StringSplitOptions.None))
			{
				shell.WriteLine(line);
			}
		}

		// Token: 0x06001220 RID: 4640 RVA: 0x0005ED3C File Offset: 0x0005CF3C
		public CompletionResult GetCompletion(IConsoleShell shell, string[] args)
		{
			IPrototypeManager protoManager = IoCManager.Resolve<IPrototypeManager>();
			if (args.Length > 1)
			{
				return CompletionResult.Empty;
			}
			return CompletionResult.FromHintOptions(from o in protoManager.EnumeratePrototypes<HTNCompoundTask>()
			select o.ID, "compound task");
		}
	}
}
