using System;
using System.Runtime.CompilerServices;
using Content.Server.Access.Systems;
using Content.Server.Administration;
using Content.Server.Administration.Systems;
using Content.Server.Mind.Components;
using Content.Server.PDA;
using Content.Server.StationRecords;
using Content.Shared.Access.Components;
using Content.Shared.Administration;
using Content.Shared.PDA;
using Content.Shared.StationRecords;
using Robust.Server.GameObjects;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Mind.Commands
{
	// Token: 0x020003AC RID: 940
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.VarEdit)]
	public sealed class RenameCommand : IConsoleCommand
	{
		// Token: 0x170002C6 RID: 710
		// (get) Token: 0x0600133D RID: 4925 RVA: 0x0006316C File Offset: 0x0006136C
		public string Command
		{
			get
			{
				return "rename";
			}
		}

		// Token: 0x170002C7 RID: 711
		// (get) Token: 0x0600133E RID: 4926 RVA: 0x00063173 File Offset: 0x00061373
		public string Description
		{
			get
			{
				return "Renames an entity and its cloner entries, ID cards, and PDAs.";
			}
		}

		// Token: 0x170002C8 RID: 712
		// (get) Token: 0x0600133F RID: 4927 RVA: 0x0006317A File Offset: 0x0006137A
		public string Help
		{
			get
			{
				return "rename <Username|EntityUid> <New character name>";
			}
		}

		// Token: 0x06001340 RID: 4928 RVA: 0x00063184 File Offset: 0x00061384
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length != 2)
			{
				shell.WriteLine(this.Help);
				return;
			}
			string name = args[1];
			if (name.Length > 30)
			{
				shell.WriteLine("Name is too long.");
				return;
			}
			IEntityManager entMan = IoCManager.Resolve<IEntityManager>();
			EntityUid entityUid;
			if (!RenameCommand.TryParseUid(args[0], shell, entMan, out entityUid))
			{
				return;
			}
			MetaDataComponent component = entMan.GetComponent<MetaDataComponent>(entityUid);
			string oldName = component.EntityName;
			component.EntityName = name;
			IEntitySystemManager entSysMan = IoCManager.Resolve<IEntitySystemManager>();
			MindComponent mind;
			if (entMan.TryGetComponent<MindComponent>(entityUid, ref mind) && mind.Mind != null)
			{
				mind.Mind.CharacterName = name;
			}
			IdCardSystem idCardSystem;
			IdCardComponent idCard;
			if (entSysMan.TryGetEntitySystem<IdCardSystem>(ref idCardSystem) && idCardSystem.TryFindIdCard(entityUid, out idCard))
			{
				idCardSystem.TryChangeFullName(idCard.Owner, name, idCard, null);
				StationRecordsSystem recordsSystem;
				StationRecordKeyStorageComponent keyStorage;
				if (entSysMan.TryGetEntitySystem<StationRecordsSystem>(ref recordsSystem) && entMan.TryGetComponent<StationRecordKeyStorageComponent>(idCard.Owner, ref keyStorage) && keyStorage.Key != null)
				{
					GeneralStationRecord generalRecord;
					if (recordsSystem.TryGetRecord<GeneralStationRecord>(keyStorage.Key.Value.OriginStation, keyStorage.Key.Value, out generalRecord, null))
					{
						generalRecord.Name = name;
					}
					recordsSystem.Synchronize(keyStorage.Key.Value.OriginStation, null);
				}
			}
			PDASystem pdaSystem;
			if (entSysMan.TryGetEntitySystem<PDASystem>(ref pdaSystem))
			{
				foreach (PDAComponent pdaComponent in entMan.EntityQuery<PDAComponent>(false))
				{
					if (!(pdaComponent.OwnerName != oldName))
					{
						pdaSystem.SetOwner(pdaComponent, name);
					}
				}
			}
			AdminSystem adminSystem;
			ActorComponent actorComp;
			if (entSysMan.TryGetEntitySystem<AdminSystem>(ref adminSystem) && entMan.TryGetComponent<ActorComponent>(entityUid, ref actorComp))
			{
				adminSystem.UpdatePlayerList(actorComp.PlayerSession);
			}
		}

		// Token: 0x06001341 RID: 4929 RVA: 0x0006334C File Offset: 0x0006154C
		private static bool TryParseUid(string str, IConsoleShell shell, IEntityManager entMan, out EntityUid entityUid)
		{
			if (EntityUid.TryParse(str, ref entityUid) && entMan.EntityExists(entityUid))
			{
				return true;
			}
			IPlayerSession session;
			if (IoCManager.Resolve<IPlayerManager>().TryGetSessionByUsername(str, ref session) && session.AttachedEntity != null)
			{
				entityUid = session.AttachedEntity.Value;
				return true;
			}
			if (session == null)
			{
				shell.WriteError("Can't find username/uid: " + str);
			}
			else
			{
				shell.WriteError(str + " does not have an entity.");
			}
			return false;
		}
	}
}
