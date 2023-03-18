using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Coordinates.Helpers;
using Content.Server.Decals;
using Content.Shared.Decals;
using Content.Shared.Maps;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Random;

namespace Content.Server.Maps
{
	// Token: 0x020003DA RID: 986
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class TileSystem : EntitySystem
	{
		// Token: 0x06001449 RID: 5193 RVA: 0x00068EFC File Offset: 0x000670FC
		public bool PryTile(Vector2i indices, EntityUid gridId)
		{
			TileRef tileRef = this._mapManager.GetGrid(gridId).GetTileRef(indices);
			return this.PryTile(tileRef);
		}

		// Token: 0x0600144A RID: 5194 RVA: 0x00068F24 File Offset: 0x00067124
		public bool PryTile(TileRef tileRef)
		{
			Tile tile = tileRef.Tile;
			return !tile.IsEmpty && ((ContentTileDefinition)this._tileDefinitionManager[(int)tile.TypeId]).CanCrowbar && this.DeconstructTile(tileRef);
		}

		// Token: 0x0600144B RID: 5195 RVA: 0x00068F6C File Offset: 0x0006716C
		public bool CutTile(TileRef tileRef)
		{
			Tile tile = tileRef.Tile;
			return !tile.IsEmpty && ((ContentTileDefinition)this._tileDefinitionManager[(int)tile.TypeId]).CanWirecutter && this.DeconstructTile(tileRef);
		}

		// Token: 0x0600144C RID: 5196 RVA: 0x00068FB4 File Offset: 0x000671B4
		public bool ReplaceTile(TileRef tileref, ContentTileDefinition replacementTile)
		{
			MapGridComponent grid;
			return base.TryComp<MapGridComponent>(tileref.GridUid, ref grid) && this.ReplaceTile(tileref, replacementTile, tileref.GridUid, grid);
		}

		// Token: 0x0600144D RID: 5197 RVA: 0x00068FE4 File Offset: 0x000671E4
		public bool ReplaceTile(TileRef tileref, ContentTileDefinition replacementTile, EntityUid grid, [Nullable(2)] MapGridComponent component = null)
		{
			if (!base.Resolve<MapGridComponent>(grid, ref component, true))
			{
				return false;
			}
			byte variant = RandomExtensions.Pick<byte>(this._robustRandom, replacementTile.PlacementVariants);
			foreach (ValueTuple<uint, Decal> valueTuple in this._decal.GetDecalsInRange(tileref.GridUid, tileref.GridPosition(null).SnapToGrid(this.EntityManager, this._mapManager).Position, 0.5f, null))
			{
				uint id = valueTuple.Item1;
				this._decal.RemoveDecal(tileref.GridUid, id, null);
			}
			component.SetTile(tileref.GridIndices, new Tile(replacementTile.TileId, 0, variant));
			return true;
		}

		// Token: 0x0600144E RID: 5198 RVA: 0x000690B4 File Offset: 0x000672B4
		private bool DeconstructTile(TileRef tileRef)
		{
			Vector2i indices = tileRef.GridIndices;
			ContentTileDefinition tileDef = (ContentTileDefinition)this._tileDefinitionManager[(int)tileRef.Tile.TypeId];
			MapGridComponent mapGrid = this._mapManager.GetGrid(tileRef.GridUid);
			float bounds = (float)mapGrid.TileSize - 0.2f;
			EntityCoordinates coordinates = mapGrid.GridTileToLocal(indices).Offset(new Vector2((this._robustRandom.NextFloat() - 0.5f) * bounds, (this._robustRandom.NextFloat() - 0.5f) * bounds));
			EntityUid tileItem = base.Spawn(tileDef.ItemDropPrototypeName, coordinates);
			base.Transform(tileItem).LocalRotation = this._robustRandom.NextDouble() * 6.283185307179586;
			foreach (ValueTuple<uint, Decal> valueTuple in this._decal.GetDecalsInRange(tileRef.GridUid, coordinates.SnapToGrid(this.EntityManager, this._mapManager).Position, 0.5f, null))
			{
				uint id = valueTuple.Item1;
				this._decal.RemoveDecal(tileRef.GridUid, id, null);
			}
			ITileDefinitionManager tileDefinitionManager = this._tileDefinitionManager;
			List<string> baseTurfs = tileDef.BaseTurfs;
			ITileDefinition plating = tileDefinitionManager[baseTurfs[baseTurfs.Count - 1]];
			mapGrid.SetTile(tileRef.GridIndices, new Tile(plating.TileId, 0, 0));
			return true;
		}

		// Token: 0x04000C88 RID: 3208
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x04000C89 RID: 3209
		[Dependency]
		private readonly ITileDefinitionManager _tileDefinitionManager;

		// Token: 0x04000C8A RID: 3210
		[Dependency]
		private readonly IRobustRandom _robustRandom;

		// Token: 0x04000C8B RID: 3211
		[Dependency]
		private readonly DecalSystem _decal;
	}
}
