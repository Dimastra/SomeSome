using System;
using System.Runtime.CompilerServices;
using Content.Server.Maps;
using Robust.Server.Maps;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;

namespace Content.Server.GameTicking
{
	// Token: 0x020004AD RID: 1197
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PreGameMapLoad : EntityEventArgs
	{
		// Token: 0x06001868 RID: 6248 RVA: 0x0007F77B File Offset: 0x0007D97B
		public PreGameMapLoad(MapId map, GameMapPrototype gameMap, MapLoadOptions options)
		{
			this.Map = map;
			this.GameMap = gameMap;
			this.Options = options;
		}

		// Token: 0x04000F2B RID: 3883
		public readonly MapId Map;

		// Token: 0x04000F2C RID: 3884
		public GameMapPrototype GameMap;

		// Token: 0x04000F2D RID: 3885
		public MapLoadOptions Options;
	}
}
