using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Server.Administration.Commands
{
	// Token: 0x0200083A RID: 2106
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Spawn)]
	public sealed class DeleteEntitiesWithId : IConsoleCommand
	{
		// Token: 0x17000738 RID: 1848
		// (get) Token: 0x06002E18 RID: 11800 RVA: 0x000F0DC4 File Offset: 0x000EEFC4
		public string Command
		{
			get
			{
				return "deleteewi";
			}
		}

		// Token: 0x17000739 RID: 1849
		// (get) Token: 0x06002E19 RID: 11801 RVA: 0x000F0DCB File Offset: 0x000EEFCB
		public string Description
		{
			get
			{
				return "Deletes entities with the specified prototype ID.";
			}
		}

		// Token: 0x1700073A RID: 1850
		// (get) Token: 0x06002E1A RID: 11802 RVA: 0x000F0DD2 File Offset: 0x000EEFD2
		public string Help
		{
			get
			{
				return "Usage: " + this.Command + " <prototypeID>";
			}
		}

		// Token: 0x06002E1B RID: 11803 RVA: 0x000F0DEC File Offset: 0x000EEFEC
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length != 1)
			{
				shell.WriteLine(this.Help);
				return;
			}
			string id = args[0].ToLower();
			IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
			IEnumerable<EntityUid> enumerable = entityManager.GetEntities().Where(delegate(EntityUid e)
			{
				EntityPrototype entityPrototype = entityManager.GetComponent<MetaDataComponent>(e).EntityPrototype;
				return ((entityPrototype != null) ? entityPrototype.ID.ToLower() : null) == id;
			});
			int i = 0;
			foreach (EntityUid entity in enumerable)
			{
				entityManager.DeleteEntity(entity);
				i++;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(44, 2);
			defaultInterpolatedStringHandler.AppendLiteral("Deleted all entities with id ");
			defaultInterpolatedStringHandler.AppendFormatted(id);
			defaultInterpolatedStringHandler.AppendLiteral(". Occurrences: ");
			defaultInterpolatedStringHandler.AppendFormatted<int>(i);
			shell.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
		}
	}
}
