using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.EUI;
using Content.Server.Station.Systems;
using Content.Server.StationRecords;
using Content.Shared.CCVar;
using Content.Shared.CrewManifest;
using Content.Shared.GameTicking;
using Content.Shared.StationRecords;
using Robust.Server.Player;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.CrewManifest
{
	// Token: 0x020005D8 RID: 1496
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class CrewManifestSystem : EntitySystem
	{
		// Token: 0x06001FEF RID: 8175 RVA: 0x000A6F1C File Offset: 0x000A511C
		public override void Initialize()
		{
			base.SubscribeLocalEvent<AfterGeneralRecordCreatedEvent>(new EntityEventHandler<AfterGeneralRecordCreatedEvent>(this.AfterGeneralRecordCreated), null, null);
			base.SubscribeLocalEvent<RecordModifiedEvent>(new EntityEventHandler<RecordModifiedEvent>(this.OnRecordModified), null, null);
			base.SubscribeLocalEvent<CrewManifestViewerComponent, BoundUIClosedEvent>(new ComponentEventHandler<CrewManifestViewerComponent, BoundUIClosedEvent>(this.OnBoundUiClose), null, null);
			base.SubscribeLocalEvent<CrewManifestViewerComponent, CrewManifestOpenUiMessage>(new ComponentEventHandler<CrewManifestViewerComponent, CrewManifestOpenUiMessage>(this.OpenEuiFromBui), null, null);
			base.SubscribeLocalEvent<RoundRestartCleanupEvent>(new EntityEventHandler<RoundRestartCleanupEvent>(this.OnRoundRestart), null, null);
			base.SubscribeNetworkEvent<RequestCrewManifestMessage>(new EntitySessionEventHandler<RequestCrewManifestMessage>(this.OnRequestCrewManifest), null, null);
		}

		// Token: 0x06001FF0 RID: 8176 RVA: 0x000A6FA4 File Offset: 0x000A51A4
		private void OnRoundRestart(RoundRestartCleanupEvent ev)
		{
			foreach (KeyValuePair<EntityUid, Dictionary<IPlayerSession, CrewManifestEui>> keyValuePair in this._openEuis)
			{
				EntityUid entityUid;
				Dictionary<IPlayerSession, CrewManifestEui> dictionary;
				keyValuePair.Deconstruct(out entityUid, out dictionary);
				foreach (KeyValuePair<IPlayerSession, CrewManifestEui> keyValuePair2 in dictionary)
				{
					IPlayerSession playerSession;
					CrewManifestEui crewManifestEui;
					keyValuePair2.Deconstruct(out playerSession, out crewManifestEui);
					crewManifestEui.Close();
				}
			}
			this._openEuis.Clear();
			this._cachedEntries.Clear();
		}

		// Token: 0x06001FF1 RID: 8177 RVA: 0x000A705C File Offset: 0x000A525C
		private void OnRequestCrewManifest(RequestCrewManifestMessage message, EntitySessionEventArgs args)
		{
			IPlayerSession sessionCast = args.SenderSession as IPlayerSession;
			if (sessionCast == null || !this._configManager.GetCVar<bool>(CCVars.CrewManifestWithoutEntity))
			{
				return;
			}
			this.OpenEui(message.Id, sessionCast, null);
		}

		// Token: 0x06001FF2 RID: 8178 RVA: 0x000A70A4 File Offset: 0x000A52A4
		private void AfterGeneralRecordCreated(AfterGeneralRecordCreatedEvent ev)
		{
			this.BuildCrewManifest(ev.Key.OriginStation);
			this.UpdateEuis(ev.Key.OriginStation);
		}

		// Token: 0x06001FF3 RID: 8179 RVA: 0x000A70DC File Offset: 0x000A52DC
		private void OnRecordModified(RecordModifiedEvent ev)
		{
			this.BuildCrewManifest(ev.Key.OriginStation);
			this.UpdateEuis(ev.Key.OriginStation);
		}

		// Token: 0x06001FF4 RID: 8180 RVA: 0x000A7114 File Offset: 0x000A5314
		private void OnBoundUiClose(EntityUid uid, CrewManifestViewerComponent component, BoundUIClosedEvent ev)
		{
			EntityUid? owningStation = this._stationSystem.GetOwningStation(uid, null);
			if (owningStation != null)
			{
				IPlayerSession sessionCast = ev.Session as IPlayerSession;
				if (sessionCast != null)
				{
					this.CloseEui(owningStation.Value, sessionCast, new EntityUid?(uid));
					return;
				}
			}
		}

		// Token: 0x06001FF5 RID: 8181 RVA: 0x000A715C File Offset: 0x000A535C
		[return: TupleElementNames(new string[]
		{
			"name",
			"entries"
		})]
		[return: Nullable(new byte[]
		{
			0,
			1,
			2
		})]
		public ValueTuple<string, CrewManifestEntries> GetCrewManifest(EntityUid station)
		{
			CrewManifestEntries manifest;
			bool valid = this._cachedEntries.TryGetValue(station, out manifest);
			return new ValueTuple<string, CrewManifestEntries>(valid ? base.MetaData(station).EntityName : string.Empty, valid ? manifest : null);
		}

		// Token: 0x06001FF6 RID: 8182 RVA: 0x000A719C File Offset: 0x000A539C
		private void UpdateEuis(EntityUid station)
		{
			Dictionary<IPlayerSession, CrewManifestEui> euis;
			if (this._openEuis.TryGetValue(station, out euis))
			{
				foreach (CrewManifestEui crewManifestEui in euis.Values)
				{
					crewManifestEui.StateDirty();
				}
			}
		}

		// Token: 0x06001FF7 RID: 8183 RVA: 0x000A71FC File Offset: 0x000A53FC
		private void OpenEuiFromBui(EntityUid uid, CrewManifestViewerComponent component, CrewManifestOpenUiMessage msg)
		{
			EntityUid? owningStation = this._stationSystem.GetOwningStation(uid, null);
			if (owningStation != null)
			{
				IPlayerSession sessionCast = msg.Session as IPlayerSession;
				if (sessionCast != null)
				{
					if (!this._configManager.GetCVar<bool>(CCVars.CrewManifestUnsecure) && component.Unsecure)
					{
						return;
					}
					this.OpenEui(owningStation.Value, sessionCast, new EntityUid?(uid));
					return;
				}
			}
		}

		// Token: 0x06001FF8 RID: 8184 RVA: 0x000A7260 File Offset: 0x000A5460
		public void OpenEui(EntityUid station, IPlayerSession session, EntityUid? owner = null)
		{
			if (!base.HasComp<StationRecordsComponent>(station))
			{
				return;
			}
			Dictionary<IPlayerSession, CrewManifestEui> euis;
			if (!this._openEuis.TryGetValue(station, out euis))
			{
				euis = new Dictionary<IPlayerSession, CrewManifestEui>();
				this._openEuis.Add(station, euis);
			}
			if (euis.ContainsKey(session))
			{
				return;
			}
			CrewManifestEui eui = new CrewManifestEui(station, owner, this);
			euis.Add(session, eui);
			this._euiManager.OpenEui(eui, session);
			eui.StateDirty();
		}

		// Token: 0x06001FF9 RID: 8185 RVA: 0x000A72C8 File Offset: 0x000A54C8
		public void CloseEui(EntityUid station, IPlayerSession session, EntityUid? owner = null)
		{
			if (!base.HasComp<StationRecordsComponent>(station))
			{
				return;
			}
			Dictionary<IPlayerSession, CrewManifestEui> euis;
			CrewManifestEui eui;
			if (!this._openEuis.TryGetValue(station, out euis) || !euis.TryGetValue(session, out eui))
			{
				return;
			}
			if (eui.Owner == owner)
			{
				euis.Remove(session);
				eui.Close();
			}
			if (euis.Count == 0)
			{
				this._openEuis.Remove(station);
			}
		}

		// Token: 0x06001FFA RID: 8186 RVA: 0x000A735C File Offset: 0x000A555C
		private void BuildCrewManifest(EntityUid station)
		{
			IEnumerable<ValueTuple<StationRecordKey, GeneralStationRecord>> recordsOfType = this._recordsSystem.GetRecordsOfType<GeneralStationRecord>(station, null);
			CrewManifestEntries entries = new CrewManifestEntries();
			foreach (ValueTuple<StationRecordKey, GeneralStationRecord> valueTuple in recordsOfType)
			{
				GeneralStationRecord record = valueTuple.Item2;
				CrewManifestEntry entry = new CrewManifestEntry(record.Name, record.JobTitle, record.JobIcon, record.JobPrototype);
				entries.Entries.Add(entry);
			}
			entries.Entries = (from e in entries.Entries
			orderby e.JobTitle, e.Name
			select e).ToList<CrewManifestEntry>();
			if (this._cachedEntries.ContainsKey(station))
			{
				this._cachedEntries[station] = entries;
				return;
			}
			this._cachedEntries.Add(station, entries);
		}

		// Token: 0x040013D4 RID: 5076
		[Dependency]
		private readonly StationSystem _stationSystem;

		// Token: 0x040013D5 RID: 5077
		[Dependency]
		private readonly StationRecordsSystem _recordsSystem;

		// Token: 0x040013D6 RID: 5078
		[Dependency]
		private readonly EuiManager _euiManager;

		// Token: 0x040013D7 RID: 5079
		[Dependency]
		private readonly IConfigurationManager _configManager;

		// Token: 0x040013D8 RID: 5080
		private readonly Dictionary<EntityUid, CrewManifestEntries> _cachedEntries = new Dictionary<EntityUid, CrewManifestEntries>();

		// Token: 0x040013D9 RID: 5081
		private readonly Dictionary<EntityUid, Dictionary<IPlayerSession, CrewManifestEui>> _openEuis = new Dictionary<EntityUid, Dictionary<IPlayerSession, CrewManifestEui>>();
	}
}
