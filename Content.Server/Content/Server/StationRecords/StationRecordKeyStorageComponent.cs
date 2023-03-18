using System;
using Content.Shared.StationRecords;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Server.StationRecords
{
	// Token: 0x02000177 RID: 375
	[RegisterComponent]
	public sealed class StationRecordKeyStorageComponent : Component
	{
		// Token: 0x04000475 RID: 1141
		[ViewVariables]
		public StationRecordKey? Key;
	}
}
