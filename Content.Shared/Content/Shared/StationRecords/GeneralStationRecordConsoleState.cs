using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.StationRecords
{
	// Token: 0x0200015F RID: 351
	[NullableContext(2)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class GeneralStationRecordConsoleState : BoundUserInterfaceState
	{
		// Token: 0x170000C2 RID: 194
		// (get) Token: 0x0600043A RID: 1082 RVA: 0x000111E4 File Offset: 0x0000F3E4
		public StationRecordKey? SelectedKey { get; }

		// Token: 0x170000C3 RID: 195
		// (get) Token: 0x0600043B RID: 1083 RVA: 0x000111EC File Offset: 0x0000F3EC
		public GeneralStationRecord Record { get; }

		// Token: 0x170000C4 RID: 196
		// (get) Token: 0x0600043C RID: 1084 RVA: 0x000111F4 File Offset: 0x0000F3F4
		[Nullable(new byte[]
		{
			2,
			1
		})]
		public Dictionary<StationRecordKey, string> RecordListing { [return: Nullable(new byte[]
		{
			2,
			1
		})] get; }

		// Token: 0x0600043D RID: 1085 RVA: 0x000111FC File Offset: 0x0000F3FC
		public GeneralStationRecordConsoleState(StationRecordKey? key, GeneralStationRecord record, [Nullable(new byte[]
		{
			2,
			1
		})] Dictionary<StationRecordKey, string> recordListing)
		{
			this.SelectedKey = key;
			this.Record = record;
			this.RecordListing = recordListing;
		}

		// Token: 0x0600043E RID: 1086 RVA: 0x0001121C File Offset: 0x0000F41C
		public bool IsEmpty()
		{
			return this.SelectedKey == null && this.Record == null && this.RecordListing == null;
		}
	}
}
