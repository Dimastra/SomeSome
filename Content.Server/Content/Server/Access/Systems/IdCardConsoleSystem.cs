using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Station.Systems;
using Content.Server.StationRecords;
using Content.Shared.Access.Components;
using Content.Shared.Access.Systems;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Roles;
using Content.Shared.StationRecords;
using Robust.Server.GameObjects;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Prototypes;

namespace Content.Server.Access.Systems
{
	// Token: 0x0200087B RID: 2171
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class IdCardConsoleSystem : SharedIdCardConsoleSystem
	{
		// Token: 0x06002F69 RID: 12137 RVA: 0x000F5204 File Offset: 0x000F3404
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<SharedIdCardConsoleComponent, SharedIdCardConsoleComponent.WriteToTargetIdMessage>(new ComponentEventHandler<SharedIdCardConsoleComponent, SharedIdCardConsoleComponent.WriteToTargetIdMessage>(this.OnWriteToTargetIdMessage), null, null);
			base.SubscribeLocalEvent<SharedIdCardConsoleComponent, ComponentStartup>(new ComponentEventHandler<SharedIdCardConsoleComponent, ComponentStartup>(this.UpdateUserInterface), null, null);
			base.SubscribeLocalEvent<SharedIdCardConsoleComponent, EntInsertedIntoContainerMessage>(new ComponentEventHandler<SharedIdCardConsoleComponent, EntInsertedIntoContainerMessage>(this.UpdateUserInterface), null, null);
			base.SubscribeLocalEvent<SharedIdCardConsoleComponent, EntRemovedFromContainerMessage>(new ComponentEventHandler<SharedIdCardConsoleComponent, EntRemovedFromContainerMessage>(this.UpdateUserInterface), null, null);
		}

		// Token: 0x06002F6A RID: 12138 RVA: 0x000F5268 File Offset: 0x000F3468
		private void OnWriteToTargetIdMessage(EntityUid uid, SharedIdCardConsoleComponent component, SharedIdCardConsoleComponent.WriteToTargetIdMessage args)
		{
			EntityUid? attachedEntity = args.Session.AttachedEntity;
			if (attachedEntity != null)
			{
				EntityUid player = attachedEntity.GetValueOrDefault();
				if (player.Valid)
				{
					this.TryWriteToTargetId(uid, args.FullName, args.JobTitle, args.AccessList, args.JobPrototype, player, component);
					this.UpdateUserInterface(uid, component, args);
					return;
				}
			}
		}

		// Token: 0x06002F6B RID: 12139 RVA: 0x000F52C8 File Offset: 0x000F34C8
		private void UpdateUserInterface(EntityUid uid, SharedIdCardConsoleComponent component, EntityEventArgs args)
		{
			if (!component.Initialized)
			{
				return;
			}
			EntityUid? entityUid = component.TargetIdSlot.Item;
			SharedIdCardConsoleComponent.IdCardConsoleBoundUserInterfaceState newState;
			if (entityUid != null)
			{
				EntityUid targetId = entityUid.GetValueOrDefault();
				if (targetId.Valid)
				{
					IdCardComponent targetIdComponent = this.EntityManager.GetComponent<IdCardComponent>(targetId);
					AccessComponent targetAccessComponent = this.EntityManager.GetComponent<AccessComponent>(targetId);
					string name = string.Empty;
					entityUid = component.PrivilegedIdSlot.Item;
					if (entityUid != null)
					{
						EntityUid item = entityUid.GetValueOrDefault();
						if (item.Valid)
						{
							name = this.EntityManager.GetComponent<MetaDataComponent>(item).EntityName;
						}
					}
					string jobProto = string.Empty;
					entityUid = this._station.GetOwningStation(uid, null);
					if (entityUid != null)
					{
						EntityUid station = entityUid.GetValueOrDefault();
						StationRecordKeyStorageComponent keyStorage;
						GeneralStationRecord record;
						if (this.EntityManager.TryGetComponent<StationRecordKeyStorageComponent>(targetId, ref keyStorage) && keyStorage.Key != null && this._record.TryGetRecord<GeneralStationRecord>(station, keyStorage.Key.Value, out record, null))
						{
							jobProto = record.JobPrototype;
						}
					}
					newState = new SharedIdCardConsoleComponent.IdCardConsoleBoundUserInterfaceState(component.PrivilegedIdSlot.HasItem, this.PrivilegedIdIsAuthorized(uid, component), true, targetIdComponent.FullName, targetIdComponent.JobTitle, targetAccessComponent.Tags.ToArray<string>(), jobProto, name, this.EntityManager.GetComponent<MetaDataComponent>(targetId).EntityName);
					goto IL_1AF;
				}
			}
			string privilegedIdName = string.Empty;
			entityUid = component.PrivilegedIdSlot.Item;
			if (entityUid != null)
			{
				EntityUid item2 = entityUid.GetValueOrDefault();
				if (item2.Valid)
				{
					privilegedIdName = this.EntityManager.GetComponent<MetaDataComponent>(item2).EntityName;
				}
			}
			newState = new SharedIdCardConsoleComponent.IdCardConsoleBoundUserInterfaceState(component.PrivilegedIdSlot.HasItem, this.PrivilegedIdIsAuthorized(uid, component), false, null, null, null, string.Empty, privilegedIdName, string.Empty);
			IL_1AF:
			this._userInterface.TrySetUiState(uid, SharedIdCardConsoleComponent.IdCardConsoleUiKey.Key, newState, null, null, true);
		}

		// Token: 0x06002F6C RID: 12140 RVA: 0x000F549C File Offset: 0x000F369C
		private void TryWriteToTargetId(EntityUid uid, string newFullName, string newJobTitle, List<string> newAccessList, string newJobProto, EntityUid player, [Nullable(2)] SharedIdCardConsoleComponent component = null)
		{
			if (!base.Resolve<SharedIdCardConsoleComponent>(uid, ref component, true))
			{
				return;
			}
			EntityUid? item = component.TargetIdSlot.Item;
			if (item != null)
			{
				EntityUid targetId = item.GetValueOrDefault();
				if (targetId.Valid && this.PrivilegedIdIsAuthorized(uid, component))
				{
					this._idCard.TryChangeFullName(targetId, newFullName, null, new EntityUid?(player));
					this._idCard.TryChangeJobTitle(targetId, newJobTitle, null, new EntityUid?(player));
					if (!newAccessList.TrueForAll((string x) => component.AccessLevels.Contains(x)))
					{
						Logger.Warning("Tried to write unknown access tag.");
						return;
					}
					IEnumerable<string> oldTags = this._access.TryGetTags(targetId, null) ?? new List<string>();
					oldTags = oldTags.ToList<string>();
					if (oldTags.SequenceEqual(newAccessList))
					{
						return;
					}
					List<string> addedTags = (from tag in newAccessList.Except(oldTags)
					select "+" + tag).ToList<string>();
					List<string> removedTags = (from tag in oldTags.Except(newAccessList)
					select "-" + tag).ToList<string>();
					this._access.TrySetTags(targetId, newAccessList, null);
					ISharedAdminLogManager adminLogger = this._adminLogger;
					LogType type = LogType.Action;
					LogImpact impact = LogImpact.Medium;
					LogStringHandler logStringHandler = new LogStringHandler(49, 4);
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(player), "player", "ToPrettyString(player)");
					logStringHandler.AppendLiteral(" has modified ");
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(targetId), "entity", "ToPrettyString(targetId)");
					logStringHandler.AppendLiteral(" with the following accesses: [");
					logStringHandler.AppendFormatted(string.Join(", ", addedTags.Union(removedTags)));
					logStringHandler.AppendLiteral("] [");
					logStringHandler.AppendFormatted(string.Join(", ", newAccessList));
					logStringHandler.AppendLiteral("]");
					adminLogger.Add(type, impact, ref logStringHandler);
					this.UpdateStationRecord(uid, targetId, newFullName, newJobTitle, newJobProto);
					return;
				}
			}
		}

		// Token: 0x06002F6D RID: 12141 RVA: 0x000F56A4 File Offset: 0x000F38A4
		[NullableContext(2)]
		private bool PrivilegedIdIsAuthorized(EntityUid uid, SharedIdCardConsoleComponent component = null)
		{
			if (!base.Resolve<SharedIdCardConsoleComponent>(uid, ref component, true))
			{
				return true;
			}
			AccessReaderComponent reader;
			if (!this.EntityManager.TryGetComponent<AccessReaderComponent>(uid, ref reader))
			{
				return true;
			}
			EntityUid? privilegedId = component.PrivilegedIdSlot.Item;
			return privilegedId != null && this._accessReader.IsAllowed(privilegedId.Value, reader);
		}

		// Token: 0x06002F6E RID: 12142 RVA: 0x000F56FC File Offset: 0x000F38FC
		private void UpdateStationRecord(EntityUid uid, EntityUid targetId, string newFullName, string newJobTitle, string newJobProto)
		{
			EntityUid? owningStation = this._station.GetOwningStation(uid, null);
			if (owningStation != null)
			{
				EntityUid station = owningStation.GetValueOrDefault();
				StationRecordKeyStorageComponent keyStorage;
				if (this.EntityManager.TryGetComponent<StationRecordKeyStorageComponent>(targetId, ref keyStorage))
				{
					StationRecordKey? key2 = keyStorage.Key;
					if (key2 != null)
					{
						StationRecordKey key = key2.GetValueOrDefault();
						GeneralStationRecord record;
						if (this._record.TryGetRecord<GeneralStationRecord>(station, key, out record, null))
						{
							record.Name = newFullName;
							record.JobTitle = newJobTitle;
							JobPrototype job;
							if (this._prototype.TryIndex<JobPrototype>(newJobProto, ref job))
							{
								record.JobPrototype = newJobProto;
								record.JobIcon = job.Icon;
							}
							this._record.Synchronize(station, null);
							return;
						}
					}
				}
			}
		}

		// Token: 0x04001C82 RID: 7298
		[Dependency]
		private readonly IPrototypeManager _prototype;

		// Token: 0x04001C83 RID: 7299
		[Dependency]
		private readonly StationRecordsSystem _record;

		// Token: 0x04001C84 RID: 7300
		[Dependency]
		private readonly StationSystem _station;

		// Token: 0x04001C85 RID: 7301
		[Dependency]
		private readonly UserInterfaceSystem _userInterface;

		// Token: 0x04001C86 RID: 7302
		[Dependency]
		private readonly AccessReaderSystem _accessReader;

		// Token: 0x04001C87 RID: 7303
		[Dependency]
		private readonly AccessSystem _access;

		// Token: 0x04001C88 RID: 7304
		[Dependency]
		private readonly IdCardSystem _idCard;

		// Token: 0x04001C89 RID: 7305
		[Dependency]
		private readonly ISharedAdminLogManager _adminLogger;
	}
}
