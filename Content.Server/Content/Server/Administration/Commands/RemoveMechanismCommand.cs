using System;
using System.Runtime.CompilerServices;
using Content.Server.Body.Systems;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Administration.Commands
{
	// Token: 0x0200085A RID: 2138
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Admin)]
	public sealed class RemoveMechanismCommand : IConsoleCommand
	{
		// Token: 0x17000796 RID: 1942
		// (get) Token: 0x06002EC1 RID: 11969 RVA: 0x000F2C58 File Offset: 0x000F0E58
		public string Command
		{
			get
			{
				return "rmmechanism";
			}
		}

		// Token: 0x17000797 RID: 1943
		// (get) Token: 0x06002EC2 RID: 11970 RVA: 0x000F2C5F File Offset: 0x000F0E5F
		public string Description
		{
			get
			{
				return "Removes a given entity from it's containing bodypart, if any.";
			}
		}

		// Token: 0x17000798 RID: 1944
		// (get) Token: 0x06002EC3 RID: 11971 RVA: 0x000F2C66 File Offset: 0x000F0E66
		public string Help
		{
			get
			{
				return "Usage: rmmechanism <uid>";
			}
		}

		// Token: 0x06002EC4 RID: 11972 RVA: 0x000F2C70 File Offset: 0x000F0E70
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
			if (entityManager.System<BodySystem>().DropOrgan(new EntityUid?(entityUid), null))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(14, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Removed organ ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(entityManager.ToPrettyString(entityUid));
				shell.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			shell.WriteError("Was not a mechanism, or did not have a parent.");
		}
	}
}
