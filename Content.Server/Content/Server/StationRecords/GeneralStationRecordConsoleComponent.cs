using System;
using Content.Shared.StationRecords;
using Robust.Shared.GameObjects;

namespace Content.Server.StationRecords
{
	// Token: 0x02000176 RID: 374
	[RegisterComponent]
	public sealed class GeneralStationRecordConsoleComponent : Component
	{
		// Token: 0x17000146 RID: 326
		// (get) Token: 0x06000760 RID: 1888 RVA: 0x00024A00 File Offset: 0x00022C00
		// (set) Token: 0x06000761 RID: 1889 RVA: 0x00024A08 File Offset: 0x00022C08
		public StationRecordKey? ActiveKey { get; set; }
	}
}
