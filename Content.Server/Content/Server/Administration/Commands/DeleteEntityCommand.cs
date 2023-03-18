using System;
using System.Runtime.CompilerServices;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Administration.Commands
{
	// Token: 0x0200083B RID: 2107
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Spawn)]
	public sealed class DeleteEntityCommand : IConsoleCommand
	{
		// Token: 0x1700073B RID: 1851
		// (get) Token: 0x06002E1D RID: 11805 RVA: 0x000F0ED8 File Offset: 0x000EF0D8
		public string Command
		{
			get
			{
				return "deleteentity";
			}
		}

		// Token: 0x1700073C RID: 1852
		// (get) Token: 0x06002E1E RID: 11806 RVA: 0x000F0EDF File Offset: 0x000EF0DF
		public string Description
		{
			get
			{
				return "Deletes an entity with the given id.";
			}
		}

		// Token: 0x1700073D RID: 1853
		// (get) Token: 0x06002E1F RID: 11807 RVA: 0x000F0EE6 File Offset: 0x000EF0E6
		public string Help
		{
			get
			{
				return "Usage: " + this.Command + " <id>";
			}
		}

		// Token: 0x06002E20 RID: 11808 RVA: 0x000F0F00 File Offset: 0x000EF100
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length != 1)
			{
				shell.WriteLine("Invalid amount of arguments.\n" + this.Help);
				return;
			}
			EntityUid id;
			if (!EntityUid.TryParse(args[0], ref id))
			{
				shell.WriteLine(args[0] + " is not a valid entity id.");
				return;
			}
			IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler;
			if (!entityManager.EntityExists(id))
			{
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(25, 1);
				defaultInterpolatedStringHandler.AppendLiteral("No entity found with id ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(id);
				defaultInterpolatedStringHandler.AppendLiteral(".");
				shell.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			entityManager.DeleteEntity(id);
			defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(24, 1);
			defaultInterpolatedStringHandler.AppendLiteral("Deleted entity with id ");
			defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(id);
			defaultInterpolatedStringHandler.AppendLiteral(".");
			shell.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
		}
	}
}
