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
	// Token: 0x02000842 RID: 2114
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Admin)]
	public sealed class LinkBluespaceLocker : IConsoleCommand
	{
		// Token: 0x17000750 RID: 1872
		// (get) Token: 0x06002E42 RID: 11842 RVA: 0x000F17D8 File Offset: 0x000EF9D8
		public string Command
		{
			get
			{
				return "linkbluespacelocker";
			}
		}

		// Token: 0x17000751 RID: 1873
		// (get) Token: 0x06002E43 RID: 11843 RVA: 0x000F17DF File Offset: 0x000EF9DF
		public string Description
		{
			get
			{
				return "Links an entity, the target, to another as a bluespace locker target.";
			}
		}

		// Token: 0x17000752 RID: 1874
		// (get) Token: 0x06002E44 RID: 11844 RVA: 0x000F17E6 File Offset: 0x000EF9E6
		public string Help
		{
			get
			{
				return "Usage: linkbluespacelocker <two-way link> <origin storage uid> <target storage uid>";
			}
		}

		// Token: 0x06002E45 RID: 11845 RVA: 0x000F17F0 File Offset: 0x000EF9F0
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length != 3)
			{
				shell.WriteError(Loc.GetString("shell-wrong-arguments-number"));
				return;
			}
			bool bidirectional;
			if (!bool.TryParse(args[0], out bidirectional))
			{
				shell.WriteError(Loc.GetString("shell-invalid-bool"));
				return;
			}
			EntityUid originUid;
			if (!EntityUid.TryParse(args[1], ref originUid))
			{
				shell.WriteError(Loc.GetString("shell-entity-uid-must-be-number"));
				return;
			}
			EntityUid targetUid;
			if (!EntityUid.TryParse(args[2], ref targetUid))
			{
				shell.WriteError(Loc.GetString("shell-entity-uid-must-be-number"));
				return;
			}
			IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
			EntityStorageComponent originComponent;
			if (!entityManager.TryGetComponent<EntityStorageComponent>(originUid, ref originComponent))
			{
				shell.WriteError(Loc.GetString("shell-entity-with-uid-lacks-component", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("uid", originUid),
					new ValueTuple<string, object>("componentName", "EntityStorageComponent")
				}));
				return;
			}
			EntityStorageComponent targetComponent;
			if (!entityManager.TryGetComponent<EntityStorageComponent>(targetUid, ref targetComponent))
			{
				shell.WriteError(Loc.GetString("shell-entity-with-uid-lacks-component", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("uid", targetUid),
					new ValueTuple<string, object>("componentName", "EntityStorageComponent")
				}));
				return;
			}
			BluespaceLockerComponent originBluespaceComponent;
			entityManager.EnsureComponent<BluespaceLockerComponent>(originUid, ref originBluespaceComponent);
			originBluespaceComponent.BluespaceLinks.Add(targetUid);
			BluespaceLockerComponent targetBluespaceComponent;
			entityManager.EnsureComponent<BluespaceLockerComponent>(targetUid, ref targetBluespaceComponent);
			if (bidirectional)
			{
				targetBluespaceComponent.BluespaceLinks.Add(originUid);
				return;
			}
			if (targetBluespaceComponent.BluespaceLinks.Count == 0)
			{
				targetBluespaceComponent.BehaviorProperties.TransportSentient = false;
				targetBluespaceComponent.BehaviorProperties.TransportEntities = false;
				targetBluespaceComponent.BehaviorProperties.TransportGas = false;
			}
		}
	}
}
