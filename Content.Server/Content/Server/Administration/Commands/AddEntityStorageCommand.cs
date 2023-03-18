using System;
using System.Runtime.CompilerServices;
using Content.Server.Storage.Components;
using Content.Server.Storage.EntitySystems;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Administration.Commands
{
	// Token: 0x0200082C RID: 2092
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Admin)]
	public sealed class AddEntityStorageCommand : IConsoleCommand
	{
		// Token: 0x17000710 RID: 1808
		// (get) Token: 0x06002DD1 RID: 11729 RVA: 0x000EFEE1 File Offset: 0x000EE0E1
		public string Command
		{
			get
			{
				return "addstorage";
			}
		}

		// Token: 0x17000711 RID: 1809
		// (get) Token: 0x06002DD2 RID: 11730 RVA: 0x000EFEE8 File Offset: 0x000EE0E8
		public string Description
		{
			get
			{
				return "Adds a given entity to a containing storage.";
			}
		}

		// Token: 0x17000712 RID: 1810
		// (get) Token: 0x06002DD3 RID: 11731 RVA: 0x000EFEEF File Offset: 0x000EE0EF
		public string Help
		{
			get
			{
				return "Usage: addstorage <entity uid> <storage uid>";
			}
		}

		// Token: 0x06002DD4 RID: 11732 RVA: 0x000EFEF8 File Offset: 0x000EE0F8
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
			EntityUid storageUid;
			if (!EntityUid.TryParse(args[1], ref storageUid))
			{
				shell.WriteError(Loc.GetString("shell-entity-uid-must-be-number"));
				return;
			}
			IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
			EntityStorageSystem storageSys;
			if (entityManager.HasComponent<EntityStorageComponent>(storageUid) && entityManager.EntitySysManager.TryGetEntitySystem<EntityStorageSystem>(ref storageSys))
			{
				storageSys.Insert(entityUid, storageUid, null);
				return;
			}
			shell.WriteError("Could not insert into non-storage.");
		}
	}
}
