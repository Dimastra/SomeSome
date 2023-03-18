using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Maps;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;

namespace Content.Server.GameTicking
{
	// Token: 0x020004AE RID: 1198
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PostGameMapLoad : EntityEventArgs
	{
		// Token: 0x06001869 RID: 6249 RVA: 0x0007F798 File Offset: 0x0007D998
		public PostGameMapLoad(GameMapPrototype gameMap, MapId map, IReadOnlyList<EntityUid> grids, [Nullable(2)] string stationName)
		{
			this.GameMap = gameMap;
			this.Map = map;
			this.Grids = grids;
			this.StationName = stationName;
		}

		// Token: 0x04000F2E RID: 3886
		public readonly GameMapPrototype GameMap;

		// Token: 0x04000F2F RID: 3887
		public readonly MapId Map;

		// Token: 0x04000F30 RID: 3888
		public readonly IReadOnlyList<EntityUid> Grids;

		// Token: 0x04000F31 RID: 3889
		[Nullable(2)]
		public readonly string StationName;
	}
}
