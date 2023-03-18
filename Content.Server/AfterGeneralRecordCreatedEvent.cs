using System;
using System.Runtime.CompilerServices;
using Content.Shared.Preferences;
using Content.Shared.StationRecords;
using Robust.Shared.GameObjects;

// Token: 0x0200000D RID: 13
[NullableContext(1)]
[Nullable(0)]
public sealed class AfterGeneralRecordCreatedEvent : EntityEventArgs
{
	// Token: 0x17000005 RID: 5
	// (get) Token: 0x06000021 RID: 33 RVA: 0x00002684 File Offset: 0x00000884
	public StationRecordKey Key { get; }

	// Token: 0x17000006 RID: 6
	// (get) Token: 0x06000022 RID: 34 RVA: 0x0000268C File Offset: 0x0000088C
	public GeneralStationRecord Record { get; }

	// Token: 0x17000007 RID: 7
	// (get) Token: 0x06000023 RID: 35 RVA: 0x00002694 File Offset: 0x00000894
	[Nullable(2)]
	public HumanoidCharacterProfile Profile { [NullableContext(2)] get; }

	// Token: 0x06000024 RID: 36 RVA: 0x0000269C File Offset: 0x0000089C
	public AfterGeneralRecordCreatedEvent(StationRecordKey key, GeneralStationRecord record, [Nullable(2)] HumanoidCharacterProfile profile)
	{
		this.Key = key;
		this.Record = record;
		this.Profile = profile;
	}
}
