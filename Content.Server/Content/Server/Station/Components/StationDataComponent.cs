using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Station.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Station.Components
{
	// Token: 0x020001A2 RID: 418
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(StationSystem)
	})]
	public sealed class StationDataComponent : Component
	{
		// Token: 0x04000517 RID: 1303
		[Nullable(2)]
		[DataField("stationConfig", false, 1, false, false, null)]
		public StationConfig StationConfig;

		// Token: 0x04000518 RID: 1304
		[Nullable(1)]
		[DataField("grids", false, 1, false, false, null)]
		public readonly HashSet<EntityUid> Grids = new HashSet<EntityUid>();

		// Token: 0x04000519 RID: 1305
		[ViewVariables]
		[Access]
		public EntityUid? EmergencyShuttle;
	}
}
