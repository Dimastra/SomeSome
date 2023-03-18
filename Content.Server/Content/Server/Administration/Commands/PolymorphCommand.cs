using System;
using System.Runtime.CompilerServices;
using Content.Server.Polymorph.Components;
using Content.Server.Polymorph.Systems;
using Content.Shared.Administration;
using Content.Shared.Polymorph;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;

namespace Content.Server.Administration.Commands
{
	// Token: 0x02000852 RID: 2130
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Fun)]
	public sealed class PolymorphCommand : IConsoleCommand
	{
		// Token: 0x1700077E RID: 1918
		// (get) Token: 0x06002E98 RID: 11928 RVA: 0x000F25EB File Offset: 0x000F07EB
		public string Command
		{
			get
			{
				return "polymorph";
			}
		}

		// Token: 0x1700077F RID: 1919
		// (get) Token: 0x06002E99 RID: 11929 RVA: 0x000F25F2 File Offset: 0x000F07F2
		public string Description
		{
			get
			{
				return Loc.GetString("polymorph-command-description");
			}
		}

		// Token: 0x17000780 RID: 1920
		// (get) Token: 0x06002E9A RID: 11930 RVA: 0x000F25FE File Offset: 0x000F07FE
		public string Help
		{
			get
			{
				return Loc.GetString("polymorph-command-help");
			}
		}

		// Token: 0x06002E9B RID: 11931 RVA: 0x000F260C File Offset: 0x000F080C
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
			PolymorphPrototype polyproto;
			if (!IoCManager.Resolve<IPrototypeManager>().TryIndex<PolymorphPrototype>(args[1], ref polyproto))
			{
				shell.WriteError(Loc.GetString("polymorph-not-valid-prototype-error"));
				return;
			}
			IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
			PolymorphableSystem polySystem = entityManager.EntitySysManager.GetEntitySystem<PolymorphableSystem>();
			entityManager.EnsureComponent<PolymorphableComponent>(entityUid);
			polySystem.PolymorphEntity(entityUid, polyproto);
		}
	}
}
