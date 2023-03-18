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
	// Token: 0x02000857 RID: 2135
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Admin)]
	public sealed class RemoveBodyPartCommand : IConsoleCommand
	{
		// Token: 0x1700078D RID: 1933
		// (get) Token: 0x06002EB2 RID: 11954 RVA: 0x000F28EE File Offset: 0x000F0AEE
		public string Command
		{
			get
			{
				return "rmbodypart";
			}
		}

		// Token: 0x1700078E RID: 1934
		// (get) Token: 0x06002EB3 RID: 11955 RVA: 0x000F28F5 File Offset: 0x000F0AF5
		public string Description
		{
			get
			{
				return "Removes a given entity from it's containing body, if any.";
			}
		}

		// Token: 0x1700078F RID: 1935
		// (get) Token: 0x06002EB4 RID: 11956 RVA: 0x000F28FC File Offset: 0x000F0AFC
		public string Help
		{
			get
			{
				return "Usage: rmbodypart <uid>";
			}
		}

		// Token: 0x06002EB5 RID: 11957 RVA: 0x000F2904 File Offset: 0x000F0B04
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
			if (entityManager.System<BodySystem>().DropPart(new EntityUid?(entityUid), null))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(19, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Removed body part ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(entityManager.ToPrettyString(entityUid));
				defaultInterpolatedStringHandler.AppendLiteral(".");
				shell.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			shell.WriteError("Was not a body part, or did not have a parent.");
		}
	}
}
