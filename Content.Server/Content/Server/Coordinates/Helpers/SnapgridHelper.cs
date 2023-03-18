using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;

namespace Content.Server.Coordinates.Helpers
{
	// Token: 0x020005E4 RID: 1508
	public static class SnapgridHelper
	{
		// Token: 0x06002036 RID: 8246 RVA: 0x000A7F28 File Offset: 0x000A6128
		[NullableContext(2)]
		public static EntityCoordinates SnapToGrid(this EntityCoordinates coordinates, IEntityManager entMan = null, IMapManager mapManager = null)
		{
			IoCManager.Resolve<IEntityManager, IMapManager>(ref entMan, ref mapManager);
			EntityUid? gridIdOpt = coordinates.GetGridUid(entMan);
			float tileSize = 1f;
			if (gridIdOpt != null)
			{
				EntityUid gridId = gridIdOpt.GetValueOrDefault();
				if (gridId.IsValid())
				{
					tileSize = (float)mapManager.GetGrid(gridId).TileSize;
				}
			}
			Vector2 position = coordinates.Position;
			float x = (float)((int)Math.Floor((double)(position.X / tileSize))) + tileSize / 2f;
			float y = (float)((int)Math.Floor((double)(position.Y / tileSize))) + tileSize / 2f;
			return new EntityCoordinates(coordinates.EntityId, x, y);
		}

		// Token: 0x06002037 RID: 8247 RVA: 0x000A7FBC File Offset: 0x000A61BC
		[NullableContext(1)]
		public static EntityCoordinates SnapToGrid(this EntityCoordinates coordinates, MapGridComponent grid)
		{
			ushort tileSize = grid.TileSize;
			Vector2 position = coordinates.Position;
			float x = (float)((int)Math.Floor((double)(position.X / (float)tileSize))) + (float)tileSize / 2f;
			float y = (float)((int)Math.Floor((double)(position.Y / (float)tileSize))) + (float)tileSize / 2f;
			return new EntityCoordinates(coordinates.EntityId, x, y);
		}
	}
}
