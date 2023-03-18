using System;
using System.Runtime.CompilerServices;
using Content.Server.Storage.Components;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Administration.Commands
{
	// Token: 0x02000835 RID: 2101
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Admin)]
	public sealed class ClearBluespaceLockerLinks : IConsoleCommand
	{
		// Token: 0x17000729 RID: 1833
		// (get) Token: 0x06002DFF RID: 11775 RVA: 0x000F0963 File Offset: 0x000EEB63
		public string Command
		{
			get
			{
				return "clearbluespacelockerlinks";
			}
		}

		// Token: 0x1700072A RID: 1834
		// (get) Token: 0x06002E00 RID: 11776 RVA: 0x000F096A File Offset: 0x000EEB6A
		public string Description
		{
			get
			{
				return "Removes the bluespace links of the given uid. Does not remove links this uid is the target of.";
			}
		}

		// Token: 0x1700072B RID: 1835
		// (get) Token: 0x06002E01 RID: 11777 RVA: 0x000F0971 File Offset: 0x000EEB71
		public string Help
		{
			get
			{
				return "Usage: clearbluespacelockerlinks <storage uid>";
			}
		}

		// Token: 0x06002E02 RID: 11778 RVA: 0x000F0978 File Offset: 0x000EEB78
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length != 1)
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
			BluespaceLockerComponent originComponent;
			if (entityManager.TryGetComponent<BluespaceLockerComponent>(entityUid, ref originComponent))
			{
				entityManager.RemoveComponent(entityUid, originComponent);
			}
		}
	}
}
