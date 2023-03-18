using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Content.Server.Forensics;
using Content.Server.GameTicking;
using Content.Server.Station.Systems;
using Content.Server.StationRecords;
using Content.Server.StationRecords.Systems;
using Content.Shared.Inventory;
using Content.Shared.PDA;
using Content.Shared.Preferences;
using Content.Shared.Roles;
using Content.Shared.StationRecords;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;

// Token: 0x0200000C RID: 12
[NullableContext(2)]
[Nullable(0)]
public sealed class StationRecordsSystem : EntitySystem
{
	// Token: 0x06000015 RID: 21 RVA: 0x000022E7 File Offset: 0x000004E7
	public override void Initialize()
	{
		base.Initialize();
		base.SubscribeLocalEvent<StationInitializedEvent>(new EntityEventHandler<StationInitializedEvent>(this.OnStationInitialize), null, null);
		base.SubscribeLocalEvent<PlayerSpawnCompleteEvent>(new EntityEventHandler<PlayerSpawnCompleteEvent>(this.OnPlayerSpawn), null, null);
	}

	// Token: 0x06000016 RID: 22 RVA: 0x00002317 File Offset: 0x00000517
	[NullableContext(1)]
	private void OnStationInitialize(StationInitializedEvent args)
	{
		base.AddComp<StationRecordsComponent>(args.Station);
	}

	// Token: 0x06000017 RID: 23 RVA: 0x00002326 File Offset: 0x00000526
	[NullableContext(1)]
	private void OnPlayerSpawn(PlayerSpawnCompleteEvent args)
	{
		this.CreateGeneralRecord(args.Station, args.Mob, args.Profile, args.JobId, null);
	}

	// Token: 0x06000018 RID: 24 RVA: 0x00002348 File Offset: 0x00000548
	private void CreateGeneralRecord(EntityUid station, EntityUid player, [Nullable(1)] HumanoidCharacterProfile profile, string jobId, StationRecordsComponent records = null)
	{
		if (!base.Resolve<StationRecordsComponent>(station, ref records, true) || string.IsNullOrEmpty(jobId) || !this._prototypeManager.HasIndex<JobPrototype>(jobId))
		{
			return;
		}
		EntityUid? idUid;
		if (!this._inventorySystem.TryGetSlotEntity(player, "id", out idUid, null, null))
		{
			return;
		}
		string fingers = Loc.GetString("general-station-record-console-record-fingerprint-empty");
		FingerprintComponent fingerprint;
		if (this.EntityManager.TryGetComponent<FingerprintComponent>(player, ref fingerprint) && fingerprint.Fingerprint != null)
		{
			fingers = fingerprint.Fingerprint;
		}
		this.CreateGeneralRecord(station, new EntityUid?(idUid.Value), profile.Name, profile.Age, profile.Species, profile.Gender, jobId, fingers, profile, records);
	}

	// Token: 0x06000019 RID: 25 RVA: 0x000023F0 File Offset: 0x000005F0
	[NullableContext(1)]
	public void CreateGeneralRecord(EntityUid station, EntityUid? idUid, string name, int age, string species, Gender gender, string jobId, string fingerprint, [Nullable(2)] HumanoidCharacterProfile profile = null, [Nullable(2)] StationRecordsComponent records = null)
	{
		if (!base.Resolve<StationRecordsComponent>(station, ref records, true))
		{
			return;
		}
		JobPrototype jobPrototype;
		if (!this._prototypeManager.TryIndex<JobPrototype>(jobId, ref jobPrototype))
		{
			throw new ArgumentException("Invalid job prototype ID: " + jobId);
		}
		GeneralStationRecord record = new GeneralStationRecord
		{
			Name = name,
			Age = age,
			JobTitle = jobPrototype.LocalizedName,
			JobIcon = jobPrototype.Icon,
			JobPrototype = jobId,
			Species = species,
			Gender = gender,
			DisplayPriority = jobPrototype.Weight,
			Fingerprint = fingerprint
		};
		StationRecordKey key = this.AddRecord(station, records);
		this.AddRecordEntry<GeneralStationRecord>(key, record, records);
		if (idUid != null)
		{
			EntityUid? keyStorageEntity = idUid;
			PDAComponent pdaComponent;
			if (base.TryComp<PDAComponent>(idUid, ref pdaComponent) && pdaComponent.ContainedID != null)
			{
				keyStorageEntity = pdaComponent.IdSlot.Item;
			}
			if (keyStorageEntity != null)
			{
				this._keyStorageSystem.AssignKey(keyStorageEntity.Value, key, null);
			}
		}
		base.RaiseLocalEvent<AfterGeneralRecordCreatedEvent>(new AfterGeneralRecordCreatedEvent(key, record, profile));
	}

	// Token: 0x0600001A RID: 26 RVA: 0x000024F2 File Offset: 0x000006F2
	public bool RemoveRecord(EntityUid station, StationRecordKey key, StationRecordsComponent records = null)
	{
		if (station != key.OriginStation || !base.Resolve<StationRecordsComponent>(station, ref records, true))
		{
			return false;
		}
		base.RaiseLocalEvent<RecordRemovedEvent>(new RecordRemovedEvent(key));
		return records.Records.RemoveAllRecords(key);
	}

	// Token: 0x0600001B RID: 27 RVA: 0x00002529 File Offset: 0x00000729
	public bool TryGetRecord<T>(EntityUid station, StationRecordKey key, [NotNullWhen(true)] out T entry, StationRecordsComponent records = null)
	{
		entry = default(T);
		return !(key.OriginStation != station) && base.Resolve<StationRecordsComponent>(station, ref records, true) && records.Records.TryGetRecordEntry<T>(key, out entry);
	}

	// Token: 0x0600001C RID: 28 RVA: 0x0000255D File Offset: 0x0000075D
	[return: Nullable(new byte[]
	{
		1,
		0,
		1
	})]
	public IEnumerable<ValueTuple<StationRecordKey, T>> GetRecordsOfType<T>(EntityUid station, StationRecordsComponent records = null)
	{
		if (!base.Resolve<StationRecordsComponent>(station, ref records, true))
		{
			return new ValueTuple<StationRecordKey, T>[0];
		}
		return records.Records.GetRecordsOfType<T>();
	}

	// Token: 0x0600001D RID: 29 RVA: 0x00002580 File Offset: 0x00000780
	public StationRecordKey AddRecord(EntityUid station, StationRecordsComponent records)
	{
		if (!base.Resolve<StationRecordsComponent>(station, ref records, true))
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(34, 2);
			defaultInterpolatedStringHandler.AppendLiteral("Could not retrieve a ");
			defaultInterpolatedStringHandler.AppendFormatted("StationRecordsComponent");
			defaultInterpolatedStringHandler.AppendLiteral(" from entity ");
			defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(station);
			throw new ArgumentException(defaultInterpolatedStringHandler.ToStringAndClear());
		}
		return records.Records.AddRecord(station);
	}

	// Token: 0x0600001E RID: 30 RVA: 0x000025E8 File Offset: 0x000007E8
	public void AddRecordEntry<T>(StationRecordKey key, [Nullable(1)] T record, StationRecordsComponent records = null)
	{
		if (!base.Resolve<StationRecordsComponent>(key.OriginStation, ref records, true))
		{
			return;
		}
		records.Records.AddRecordEntry<T>(key, record);
	}

	// Token: 0x0600001F RID: 31 RVA: 0x0000260C File Offset: 0x0000080C
	public void Synchronize(EntityUid station, StationRecordsComponent records = null)
	{
		if (!base.Resolve<StationRecordsComponent>(station, ref records, true))
		{
			return;
		}
		foreach (StationRecordKey key in records.Records.GetRecentlyAccessed())
		{
			base.RaiseLocalEvent<RecordModifiedEvent>(new RecordModifiedEvent(key));
		}
		records.Records.ClearRecentlyAccessed();
	}

	// Token: 0x0400000F RID: 15
	[Nullable(1)]
	[Dependency]
	private readonly InventorySystem _inventorySystem;

	// Token: 0x04000010 RID: 16
	[Nullable(1)]
	[Dependency]
	private readonly StationRecordKeyStorageSystem _keyStorageSystem;

	// Token: 0x04000011 RID: 17
	[Nullable(1)]
	[Dependency]
	private readonly IPrototypeManager _prototypeManager;
}
