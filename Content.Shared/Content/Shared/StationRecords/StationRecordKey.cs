using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.ViewVariables;

namespace Content.Shared.StationRecords
{
	// Token: 0x02000161 RID: 353
	[NetSerializable]
	[Serializable]
	public readonly struct StationRecordKey
	{
		// Token: 0x170000C6 RID: 198
		// (get) Token: 0x06000441 RID: 1089 RVA: 0x00011263 File Offset: 0x0000F463
		[ViewVariables]
		public uint ID { get; }

		// Token: 0x170000C7 RID: 199
		// (get) Token: 0x06000442 RID: 1090 RVA: 0x0001126B File Offset: 0x0000F46B
		[ViewVariables]
		public EntityUid OriginStation { get; }

		// Token: 0x06000443 RID: 1091 RVA: 0x00011273 File Offset: 0x0000F473
		public StationRecordKey(uint id, EntityUid originStation)
		{
			this.ID = id;
			this.OriginStation = originStation;
		}
	}
}
