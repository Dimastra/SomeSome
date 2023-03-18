using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Server.StationRecords
{
	// Token: 0x02000178 RID: 376
	[Access(new Type[]
	{
		typeof(StationRecordsSystem)
	})]
	[RegisterComponent]
	public sealed class StationRecordsComponent : Component
	{
		// Token: 0x04000476 RID: 1142
		[Nullable(1)]
		[ViewVariables]
		public readonly StationRecordSet Records = new StationRecordSet();
	}
}
