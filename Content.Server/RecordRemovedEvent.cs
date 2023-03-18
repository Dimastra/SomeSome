using System;
using Content.Shared.StationRecords;
using Robust.Shared.GameObjects;

// Token: 0x0200000E RID: 14
public sealed class RecordRemovedEvent : EntityEventArgs
{
	// Token: 0x17000008 RID: 8
	// (get) Token: 0x06000025 RID: 37 RVA: 0x000026B9 File Offset: 0x000008B9
	public StationRecordKey Key { get; }

	// Token: 0x06000026 RID: 38 RVA: 0x000026C1 File Offset: 0x000008C1
	public RecordRemovedEvent(StationRecordKey key)
	{
		this.Key = key;
	}
}
