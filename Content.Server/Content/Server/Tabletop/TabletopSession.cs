using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Server.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;

namespace Content.Server.Tabletop
{
	// Token: 0x0200012F RID: 303
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class TabletopSession
	{
		// Token: 0x0600057E RID: 1406 RVA: 0x0001B454 File Offset: 0x00019654
		public TabletopSession(MapId tabletopMap, Vector2 position)
		{
			this.Position = new MapCoordinates(position, tabletopMap);
		}

		// Token: 0x04000352 RID: 850
		public readonly MapCoordinates Position;

		// Token: 0x04000353 RID: 851
		public readonly Dictionary<IPlayerSession, TabletopSessionPlayerData> Players = new Dictionary<IPlayerSession, TabletopSessionPlayerData>();

		// Token: 0x04000354 RID: 852
		public readonly HashSet<EntityUid> Entities = new HashSet<EntityUid>();
	}
}
