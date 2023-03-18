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
	// Token: 0x02000858 RID: 2136
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Admin)]
	public sealed class RemoveEntityStorageCommand : IConsoleCommand
	{
		// Token: 0x17000790 RID: 1936
		// (get) Token: 0x06002EB7 RID: 11959 RVA: 0x000F29B5 File Offset: 0x000F0BB5
		public string Command
		{
			get
			{
				return "rmstorage";
			}
		}

		// Token: 0x17000791 RID: 1937
		// (get) Token: 0x06002EB8 RID: 11960 RVA: 0x000F29BC File Offset: 0x000F0BBC
		public string Description
		{
			get
			{
				return "Removes a given entity from it's containing storage, if any.";
			}
		}

		// Token: 0x17000792 RID: 1938
		// (get) Token: 0x06002EB9 RID: 11961 RVA: 0x000F29C3 File Offset: 0x000F0BC3
		public string Help
		{
			get
			{
				return "Usage: rmstorage <uid>";
			}
		}

		// Token: 0x06002EBA RID: 11962 RVA: 0x000F29CC File Offset: 0x000F0BCC
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
			EntityStorageSystem entstorage;
			if (!entityManager.EntitySysManager.TryGetEntitySystem<EntityStorageSystem>(ref entstorage))
			{
				return;
			}
			TransformComponent transform;
			if (!entityManager.TryGetComponent<TransformComponent>(entityUid, ref transform))
			{
				return;
			}
			EntityUid parent = transform.ParentUid;
			EntityStorageComponent storage;
			if (entityManager.TryGetComponent<EntityStorageComponent>(parent, ref storage))
			{
				entstorage.Remove(entityUid, storage.Owner, storage, null);
				return;
			}
			shell.WriteError("Could not remove from storage.");
		}
	}
}
