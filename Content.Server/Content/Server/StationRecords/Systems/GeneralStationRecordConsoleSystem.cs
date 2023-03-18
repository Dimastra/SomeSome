using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Station.Systems;
using Content.Shared.StationRecords;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.StationRecords.Systems
{
	// Token: 0x0200017A RID: 378
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GeneralStationRecordConsoleSystem : EntitySystem
	{
		// Token: 0x0600076E RID: 1902 RVA: 0x00024C30 File Offset: 0x00022E30
		public override void Initialize()
		{
			base.SubscribeLocalEvent<GeneralStationRecordConsoleComponent, BoundUIOpenedEvent>(new ComponentEventHandler<GeneralStationRecordConsoleComponent, BoundUIOpenedEvent>(this.UpdateUserInterface<BoundUIOpenedEvent>), null, null);
			base.SubscribeLocalEvent<GeneralStationRecordConsoleComponent, SelectGeneralStationRecord>(new ComponentEventHandler<GeneralStationRecordConsoleComponent, SelectGeneralStationRecord>(this.OnKeySelected), null, null);
			base.SubscribeLocalEvent<GeneralStationRecordConsoleComponent, RecordModifiedEvent>(new ComponentEventHandler<GeneralStationRecordConsoleComponent, RecordModifiedEvent>(this.UpdateUserInterface<RecordModifiedEvent>), null, null);
			base.SubscribeLocalEvent<GeneralStationRecordConsoleComponent, AfterGeneralRecordCreatedEvent>(new ComponentEventHandler<GeneralStationRecordConsoleComponent, AfterGeneralRecordCreatedEvent>(this.UpdateUserInterface<AfterGeneralRecordCreatedEvent>), null, null);
		}

		// Token: 0x0600076F RID: 1903 RVA: 0x00024C8D File Offset: 0x00022E8D
		private void UpdateUserInterface<[Nullable(2)] T>(EntityUid uid, GeneralStationRecordConsoleComponent component, T ev)
		{
			this.UpdateUserInterface(uid, component);
		}

		// Token: 0x06000770 RID: 1904 RVA: 0x00024C97 File Offset: 0x00022E97
		private void OnKeySelected(EntityUid uid, GeneralStationRecordConsoleComponent component, SelectGeneralStationRecord msg)
		{
			component.ActiveKey = msg.SelectedKey;
			this.UpdateUserInterface(uid, component);
		}

		// Token: 0x06000771 RID: 1905 RVA: 0x00024CB0 File Offset: 0x00022EB0
		[NullableContext(2)]
		private void UpdateUserInterface(EntityUid uid, GeneralStationRecordConsoleComponent console = null)
		{
			if (!base.Resolve<GeneralStationRecordConsoleComponent>(uid, ref console, true))
			{
				return;
			}
			EntityUid? owningStation = this._stationSystem.GetOwningStation(uid, null);
			StationRecordsComponent stationRecordsComponent;
			if (!base.TryComp<StationRecordsComponent>(owningStation, ref stationRecordsComponent))
			{
				BoundUserInterface uiOrNull = this._userInterface.GetUiOrNull(uid, GeneralStationRecordConsoleKey.Key, null);
				if (uiOrNull == null)
				{
					return;
				}
				uiOrNull.SetState(new GeneralStationRecordConsoleState(null, null, null), null, true);
				return;
			}
			else
			{
				IEnumerable<ValueTuple<StationRecordKey, GeneralStationRecord>> recordsOfType = this._stationRecordsSystem.GetRecordsOfType<GeneralStationRecord>(owningStation.Value, stationRecordsComponent);
				Dictionary<StationRecordKey, string> listing = new Dictionary<StationRecordKey, string>();
				foreach (ValueTuple<StationRecordKey, GeneralStationRecord> pair in recordsOfType)
				{
					listing.Add(pair.Item1, pair.Item2.Name);
				}
				if (listing.Count == 0)
				{
					BoundUserInterface uiOrNull2 = this._userInterface.GetUiOrNull(uid, GeneralStationRecordConsoleKey.Key, null);
					if (uiOrNull2 == null)
					{
						return;
					}
					uiOrNull2.SetState(new GeneralStationRecordConsoleState(null, null, null), null, true);
					return;
				}
				else
				{
					GeneralStationRecord record = null;
					if (console.ActiveKey != null)
					{
						this._stationRecordsSystem.TryGetRecord<GeneralStationRecord>(owningStation.Value, console.ActiveKey.Value, out record, stationRecordsComponent);
					}
					BoundUserInterface uiOrNull3 = this._userInterface.GetUiOrNull(uid, GeneralStationRecordConsoleKey.Key, null);
					if (uiOrNull3 == null)
					{
						return;
					}
					uiOrNull3.SetState(new GeneralStationRecordConsoleState(console.ActiveKey, record, listing), null, true);
					return;
				}
			}
		}

		// Token: 0x0400047B RID: 1147
		[Dependency]
		private readonly UserInterfaceSystem _userInterface;

		// Token: 0x0400047C RID: 1148
		[Dependency]
		private readonly StationSystem _stationSystem;

		// Token: 0x0400047D RID: 1149
		[Dependency]
		private readonly StationRecordsSystem _stationRecordsSystem;
	}
}
