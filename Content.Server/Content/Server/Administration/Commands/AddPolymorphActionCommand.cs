using System;
using System.Runtime.CompilerServices;
using Content.Server.Polymorph.Components;
using Content.Server.Polymorph.Systems;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Administration.Commands
{
	// Token: 0x0200082E RID: 2094
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Fun)]
	public sealed class AddPolymorphActionCommand : IConsoleCommand
	{
		// Token: 0x17000716 RID: 1814
		// (get) Token: 0x06002DDB RID: 11739 RVA: 0x000F00DA File Offset: 0x000EE2DA
		public string Command
		{
			get
			{
				return "addpolymorphaction";
			}
		}

		// Token: 0x17000717 RID: 1815
		// (get) Token: 0x06002DDC RID: 11740 RVA: 0x000F00E1 File Offset: 0x000EE2E1
		public string Description
		{
			get
			{
				return Loc.GetString("add-polymorph-action-command-description");
			}
		}

		// Token: 0x17000718 RID: 1816
		// (get) Token: 0x06002DDD RID: 11741 RVA: 0x000F00ED File Offset: 0x000EE2ED
		public string Help
		{
			get
			{
				return Loc.GetString("add-polymorph-action-command-help");
			}
		}

		// Token: 0x06002DDE RID: 11742 RVA: 0x000F00FC File Offset: 0x000EE2FC
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length != 2)
			{
				shell.WriteError(Loc.GetString("shell-wrong-arguments-number"));
				return;
			}
			EntityUid entityUid;
			if (!EntityUid.TryParse(args[0], ref entityUid))
			{
				shell.WriteError(Loc.GetString("shell-entity-uid-must-be-number"));
				return;
			}
			IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
			PolymorphableSystem polySystem = entityManager.EntitySysManager.GetEntitySystem<PolymorphableSystem>();
			entityManager.EnsureComponent<PolymorphableComponent>(entityUid);
			polySystem.CreatePolymorphAction(args[1], entityUid);
		}
	}
}
