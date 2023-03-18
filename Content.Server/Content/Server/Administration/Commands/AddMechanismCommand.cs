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
	// Token: 0x0200082D RID: 2093
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Admin)]
	public sealed class AddMechanismCommand : IConsoleCommand
	{
		// Token: 0x17000713 RID: 1811
		// (get) Token: 0x06002DD6 RID: 11734 RVA: 0x000EFF9C File Offset: 0x000EE19C
		public string Command
		{
			get
			{
				return "addmechanism";
			}
		}

		// Token: 0x17000714 RID: 1812
		// (get) Token: 0x06002DD7 RID: 11735 RVA: 0x000EFFA3 File Offset: 0x000EE1A3
		public string Description
		{
			get
			{
				return "Adds a given entity to a containing body.";
			}
		}

		// Token: 0x17000715 RID: 1813
		// (get) Token: 0x06002DD8 RID: 11736 RVA: 0x000EFFAA File Offset: 0x000EE1AA
		public string Help
		{
			get
			{
				return "Usage: addmechanism <entity uid> <bodypart uid>";
			}
		}

		// Token: 0x06002DD9 RID: 11737 RVA: 0x000EFFB4 File Offset: 0x000EE1B4
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length != 2)
			{
				shell.WriteError(Loc.GetString("shell-wrong-arguments-number"));
				return;
			}
			EntityUid organId;
			if (!EntityUid.TryParse(args[0], ref organId))
			{
				shell.WriteError(Loc.GetString("shell-entity-uid-must-be-number"));
				return;
			}
			EntityUid partId;
			if (!EntityUid.TryParse(args[1], ref partId))
			{
				shell.WriteError(Loc.GetString("shell-entity-uid-must-be-number"));
				return;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler;
			if (IoCManager.Resolve<IEntityManager>().System<BodySystem>().AddOrganToFirstValidSlot(new EntityUid?(organId), new EntityUid?(partId), null, null))
			{
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(11, 2);
				defaultInterpolatedStringHandler.AppendLiteral("Added ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(organId);
				defaultInterpolatedStringHandler.AppendLiteral(" to ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(partId);
				defaultInterpolatedStringHandler.AppendLiteral(".");
				shell.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(19, 2);
			defaultInterpolatedStringHandler.AppendLiteral("Could not add ");
			defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(organId);
			defaultInterpolatedStringHandler.AppendLiteral(" to ");
			defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(partId);
			defaultInterpolatedStringHandler.AppendLiteral(".");
			shell.WriteError(defaultInterpolatedStringHandler.ToStringAndClear());
		}
	}
}
