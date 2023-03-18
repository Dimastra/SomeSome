using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;

namespace Content.Shared.Maps
{
	// Token: 0x0200033E RID: 830
	[NullableContext(2)]
	[Nullable(0)]
	public static class TurfHelpers
	{
		// Token: 0x060009BB RID: 2491 RVA: 0x00020230 File Offset: 0x0001E430
		public static TileRef GetTileRef(this Vector2i vector2i, EntityUid gridId, IMapManager mapManager = null)
		{
			if (mapManager == null)
			{
				mapManager = IoCManager.Resolve<IMapManager>();
			}
			MapGridComponent grid;
			if (!mapManager.TryGetGrid(new EntityUid?(gridId), ref grid))
			{
				return default(TileRef);
			}
			TileRef tile;
			if (!grid.TryGetTileRef(vector2i, ref tile))
			{
				return default(TileRef);
			}
			return tile;
		}

		// Token: 0x060009BC RID: 2492 RVA: 0x00020278 File Offset: 0x0001E478
		public static TileRef? GetTileRef(this EntityCoordinates coordinates, IEntityManager entityManager = null, IMapManager mapManager = null)
		{
			if (entityManager == null)
			{
				entityManager = IoCManager.Resolve<IEntityManager>();
			}
			if (!coordinates.IsValid(entityManager))
			{
				return null;
			}
			if (mapManager == null)
			{
				mapManager = IoCManager.Resolve<IMapManager>();
			}
			MapGridComponent grid;
			if (!mapManager.TryGetGrid(coordinates.GetGridUid(entityManager), ref grid))
			{
				return null;
			}
			TileRef tile;
			if (!grid.TryGetTileRef(coordinates, ref tile))
			{
				return null;
			}
			return new TileRef?(tile);
		}

		// Token: 0x060009BD RID: 2493 RVA: 0x000202E4 File Offset: 0x0001E4E4
		public static bool TryGetTileRef(this EntityCoordinates coordinates, [NotNullWhen(true)] out TileRef? turf, IEntityManager entityManager = null, IMapManager mapManager = null)
		{
			TileRef? tileRef = turf = coordinates.GetTileRef(entityManager, mapManager);
			return tileRef != null;
		}

		// Token: 0x060009BE RID: 2494 RVA: 0x0002030A File Offset: 0x0001E50A
		[NullableContext(1)]
		public static ContentTileDefinition GetContentTileDefinition(this Tile tile, [Nullable(2)] ITileDefinitionManager tileDefinitionManager = null)
		{
			if (tileDefinitionManager == null)
			{
				tileDefinitionManager = IoCManager.Resolve<ITileDefinitionManager>();
			}
			return (ContentTileDefinition)tileDefinitionManager[(int)tile.TypeId];
		}

		// Token: 0x060009BF RID: 2495 RVA: 0x00020327 File Offset: 0x0001E527
		public static bool IsSpace(this Tile tile, ITileDefinitionManager tileDefinitionManager = null)
		{
			return tile.GetContentTileDefinition(tileDefinitionManager).IsSpace;
		}

		// Token: 0x060009C0 RID: 2496 RVA: 0x00020335 File Offset: 0x0001E535
		[NullableContext(1)]
		public static ContentTileDefinition GetContentTileDefinition(this TileRef tile, [Nullable(2)] ITileDefinitionManager tileDefinitionManager = null)
		{
			return tile.Tile.GetContentTileDefinition(tileDefinitionManager);
		}

		// Token: 0x060009C1 RID: 2497 RVA: 0x00020343 File Offset: 0x0001E543
		public static bool IsSpace(this TileRef tile, ITileDefinitionManager tileDefinitionManager = null)
		{
			return tile.Tile.IsSpace(tileDefinitionManager);
		}

		// Token: 0x060009C2 RID: 2498 RVA: 0x00020354 File Offset: 0x0001E554
		[NullableContext(1)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<EntityUid> GetEntitiesInTile(this TileRef turf, LookupFlags flags = 4, [Nullable(2)] EntityLookupSystem lookupSystem = null)
		{
			if (lookupSystem == null)
			{
				lookupSystem = EntitySystem.Get<EntityLookupSystem>();
			}
			Box2Rotated worldBox;
			if (!TurfHelpers.GetWorldTileBox(turf, out worldBox))
			{
				return Enumerable.Empty<EntityUid>();
			}
			return lookupSystem.GetEntitiesIntersecting(turf.GridUid, worldBox, flags);
		}

		// Token: 0x060009C3 RID: 2499 RVA: 0x0002038C File Offset: 0x0001E58C
		[NullableContext(1)]
		public static IEnumerable<EntityUid> GetEntitiesInTile(this EntityCoordinates coordinates, LookupFlags flags = 4, [Nullable(2)] EntityLookupSystem lookupSystem = null)
		{
			TileRef? turf = coordinates.GetTileRef(null, null);
			if (turf == null)
			{
				return Enumerable.Empty<EntityUid>();
			}
			return turf.Value.GetEntitiesInTile(flags, lookupSystem);
		}

		// Token: 0x060009C4 RID: 2500 RVA: 0x000203BF File Offset: 0x0001E5BF
		[NullableContext(1)]
		public static IEnumerable<EntityUid> GetEntitiesInTile(this Vector2i indices, EntityUid gridId, LookupFlags flags = 4, [Nullable(2)] EntityLookupSystem lookupSystem = null)
		{
			return indices.GetTileRef(gridId, null).GetEntitiesInTile(flags, lookupSystem);
		}

		// Token: 0x060009C5 RID: 2501 RVA: 0x000203D0 File Offset: 0x0001E5D0
		public static bool IsBlockedTurf(this TileRef turf, bool filterMobs, EntityLookupSystem physics = null, IEntitySystemManager entSysMan = null)
		{
			if (physics == null)
			{
				IoCManager.Resolve<IEntitySystemManager>(ref entSysMan);
				physics = entSysMan.GetEntitySystem<EntityLookupSystem>();
			}
			Box2Rotated worldBox;
			if (!TurfHelpers.GetWorldTileBox(turf, out worldBox))
			{
				return false;
			}
			IEntityManager entManager = IoCManager.Resolve<IEntityManager>();
			foreach (EntityUid ent in physics.GetEntitiesIntersecting(turf.GridUid, worldBox, 46))
			{
				PhysicsComponent body;
				if (entManager.TryGetComponent<PhysicsComponent>(ent, ref body))
				{
					if (body.CanCollide && body.Hard && (body.CollisionLayer & 2) != 0)
					{
						return true;
					}
					if (filterMobs && (body.CollisionLayer & 30) != 0)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x060009C6 RID: 2502 RVA: 0x00020490 File Offset: 0x0001E690
		public static EntityCoordinates GridPosition(this TileRef turf, IMapManager mapManager = null)
		{
			if (mapManager == null)
			{
				mapManager = IoCManager.Resolve<IMapManager>();
			}
			return CoordinatesExtensions.ToEntityCoordinates(turf.GridIndices, turf.GridUid, mapManager);
		}

		// Token: 0x060009C7 RID: 2503 RVA: 0x000204B0 File Offset: 0x0001E6B0
		[Obsolete]
		private static bool GetWorldTileBox(TileRef turf, out Box2Rotated res)
		{
			IEntityManager entManager = IoCManager.Resolve<IEntityManager>();
			MapGridComponent tileGrid;
			if (IoCManager.Resolve<IMapManager>().TryGetGrid(new EntityUid?(turf.GridUid), ref tileGrid))
			{
				Angle gridRot = entManager.GetComponent<TransformComponent>(tileGrid.Owner).WorldRotation;
				Box2 tileBox = Box2.UnitCentered.Scale(0.9f).Scale((float)tileGrid.TileSize);
				Vector2 worldPos = tileGrid.GridTileToWorldPos(turf.GridIndices);
				tileBox = tileBox.Translated(worldPos);
				res = new Box2Rotated(tileBox, gridRot, worldPos);
				return true;
			}
			res = Box2Rotated.UnitCentered;
			return false;
		}
	}
}
