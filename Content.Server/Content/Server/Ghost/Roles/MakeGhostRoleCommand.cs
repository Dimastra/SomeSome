using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Server.Ghost.Roles.Components;
using Content.Server.Mind.Components;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Ghost.Roles
{
	// Token: 0x02000497 RID: 1175
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Admin)]
	public sealed class MakeGhostRoleCommand : IConsoleCommand
	{
		// Token: 0x17000336 RID: 822
		// (get) Token: 0x060017A4 RID: 6052 RVA: 0x0007BD7E File Offset: 0x00079F7E
		public string Command
		{
			get
			{
				return "makeghostrole";
			}
		}

		// Token: 0x17000337 RID: 823
		// (get) Token: 0x060017A5 RID: 6053 RVA: 0x0007BD85 File Offset: 0x00079F85
		public string Description
		{
			get
			{
				return "Turns an entity into a ghost role.";
			}
		}

		// Token: 0x17000338 RID: 824
		// (get) Token: 0x060017A6 RID: 6054 RVA: 0x0007BD8C File Offset: 0x00079F8C
		public string Help
		{
			get
			{
				return "Usage: " + this.Command + " <entity uid> <name> <description> [<rules>]";
			}
		}

		// Token: 0x060017A7 RID: 6055 RVA: 0x0007BDA4 File Offset: 0x00079FA4
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length < 3 || args.Length > 4)
			{
				shell.WriteLine("Invalid amount of arguments.\n" + this.Help);
				return;
			}
			IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
			EntityUid uid;
			if (!EntityUid.TryParse(args[0], ref uid))
			{
				shell.WriteLine(args[0] + " is not a valid entity uid.");
				return;
			}
			MetaDataComponent metaData;
			if (!entityManager.TryGetComponent<MetaDataComponent>(uid, ref metaData))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(25, 1);
				defaultInterpolatedStringHandler.AppendLiteral("No entity found with uid ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(uid);
				shell.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			MindComponent mind;
			if (entityManager.TryGetComponent<MindComponent>(uid, ref mind) && mind.HasMind)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(36, 2);
				defaultInterpolatedStringHandler.AppendLiteral("Entity ");
				defaultInterpolatedStringHandler.AppendFormatted(metaData.EntityName);
				defaultInterpolatedStringHandler.AppendLiteral(" with id ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(uid);
				defaultInterpolatedStringHandler.AppendLiteral(" already has a mind.");
				shell.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			string name = args[1];
			string description = args[2];
			string rules = (args.Length >= 4) ? args[3] : Loc.GetString("ghost-role-component-default-rules");
			GhostTakeoverAvailableComponent takeOver;
			if (entityManager.TryGetComponent<GhostTakeoverAvailableComponent>(uid, ref takeOver))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(31, 3);
				defaultInterpolatedStringHandler.AppendLiteral("Entity ");
				defaultInterpolatedStringHandler.AppendFormatted(metaData.EntityName);
				defaultInterpolatedStringHandler.AppendLiteral(" with id ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(uid);
				defaultInterpolatedStringHandler.AppendLiteral(" already has a ");
				defaultInterpolatedStringHandler.AppendFormatted("GhostTakeoverAvailableComponent");
				shell.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			takeOver = entityManager.AddComponent<GhostTakeoverAvailableComponent>(uid);
			takeOver.RoleName = name;
			takeOver.RoleDescription = description;
			takeOver.RoleRules = rules;
			shell.WriteLine("Made entity " + metaData.EntityName + " a ghost role.");
		}
	}
}
