using System;
using Content.Shared.StationRecords;
using Robust.Shared.GameObjects;

// Token: 0x0200000F RID: 15
public sealed class RecordModifiedEvent : EntityEventArgs
{
	// Token: 0x17000009 RID: 9
	// (get) Token: 0x06000027 RID: 39 RVA: 0x000026D0 File Offset: 0x000008D0
	public StationRecordKey Key { get; }

	// Token: 0x06000028 RID: 40 RVA: 0x000026D8 File Offset: 0x000008D8
	public RecordModifiedEvent(StationRecordKey key)
	{
		this.Key = key;
	}
}
