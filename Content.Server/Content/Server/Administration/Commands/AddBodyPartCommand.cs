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
	// Token: 0x0200082B RID: 2091
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Admin)]
	public sealed class AddBodyPartCommand : IConsoleCommand
	{
		// Token: 0x1700070D RID: 1805
		// (get) Token: 0x06002DCC RID: 11724 RVA: 0x000EFDA2 File Offset: 0x000EDFA2
		public string Command
		{
			get
			{
				return "addbodypart";
			}
		}

		// Token: 0x1700070E RID: 1806
		// (get) Token: 0x06002DCD RID: 11725 RVA: 0x000EFDA9 File Offset: 0x000EDFA9
		public string Description
		{
			get
			{
				return "Adds a given entity to a containing body.";
			}
		}

		// Token: 0x1700070F RID: 1807
		// (get) Token: 0x06002DCE RID: 11726 RVA: 0x000EFDB0 File Offset: 0x000EDFB0
		public string Help
		{
			get
			{
				return "Usage: addbodypart <entity uid> <body uid> <part slot>";
			}
		}

		// Token: 0x06002DCF RID: 11727 RVA: 0x000EFDB8 File Offset: 0x000EDFB8
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length != 3)
			{
				shell.WriteError(Loc.GetString("shell-wrong-arguments-number"));
				return;
			}
			EntityUid childId;
			if (!EntityUid.TryParse(args[0], ref childId))
			{
				shell.WriteError(Loc.GetString("shell-entity-uid-must-be-number"));
				return;
			}
			EntityUid parentId;
			if (!EntityUid.TryParse(args[1], ref parentId))
			{
				shell.WriteError(Loc.GetString("shell-entity-uid-must-be-number"));
				return;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler;
			if (IoCManager.Resolve<IEntityManager>().System<BodySystem>().TryCreatePartSlotAndAttach(new EntityUid?(parentId), args[3], new EntityUid?(childId), null, null))
			{
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(11, 2);
				defaultInterpolatedStringHandler.AppendLiteral("Added ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(childId);
				defaultInterpolatedStringHandler.AppendLiteral(" to ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(parentId);
				defaultInterpolatedStringHandler.AppendLiteral(".");
				shell.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(19, 2);
			defaultInterpolatedStringHandler.AppendLiteral("Could not add ");
			defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(childId);
			defaultInterpolatedStringHandler.AppendLiteral(" to ");
			defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(parentId);
			defaultInterpolatedStringHandler.AppendLiteral(".");
			shell.WriteError(defaultInterpolatedStringHandler.ToStringAndClear());
		}
	}
}
