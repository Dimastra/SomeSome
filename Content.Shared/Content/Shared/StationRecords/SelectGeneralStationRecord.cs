using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.StationRecords
{
	// Token: 0x02000160 RID: 352
	[NetSerializable]
	[Serializable]
	public sealed class SelectGeneralStationRecord : BoundUserInterfaceMessage
	{
		// Token: 0x170000C5 RID: 197
		// (get) Token: 0x0600043F RID: 1087 RVA: 0x0001124C File Offset: 0x0000F44C
		public StationRecordKey? SelectedKey { get; }

		// Token: 0x06000440 RID: 1088 RVA: 0x00011254 File Offset: 0x0000F454
		public SelectGeneralStationRecord(StationRecordKey? selectedKey)
		{
			this.SelectedKey = selectedKey;
		}
	}
}
