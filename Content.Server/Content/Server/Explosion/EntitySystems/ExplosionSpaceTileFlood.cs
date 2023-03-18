using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Atmos;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;

namespace Content.Server.Explosion.EntitySystems
{
	// Token: 0x0200050B RID: 1291
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ExplosionSpaceTileFlood : ExplosionTileFlood
	{
		// Token: 0x06001AA4 RID: 6820 RVA: 0x0008CC90 File Offset: 0x0008AE90
		public ExplosionSpaceTileFlood(ExplosionSystem system, MapCoordinates epicentre, EntityUid? referenceGrid, List<EntityUid> localGrids, float maxDistance)
		{
			ValueTuple<Dictionary<Vector2i, BlockedSpaceTile>, ushort> valueTuple = system.TransformGridEdges(epicentre, referenceGrid, localGrids, maxDistance);
			this._gridBlockMap = valueTuple.Item1;
			this.TileSize = valueTuple.Item2;
			system.GetUnblockedDirections(this._gridBlockMap, (float)this.TileSize);
		}

		// Token: 0x06001AA5 RID: 6821 RVA: 0x0008CCF0 File Offset: 0x0008AEF0
		public int AddNewTiles(int iteration, HashSet<Vector2i> inputSpaceTiles)
		{
			this.NewTiles = new List<Vector2i>();
			this.NewBlockedTiles = new List<Vector2i>();
			this.NewFreedTiles = new HashSet<Vector2i>();
			this.GridJump = new Dictionary<EntityUid, HashSet<Vector2i>>();
			List<Vector2i> adjacent;
			if (this.TileLists.TryGetValue(iteration - 2, out adjacent))
			{
				this.AddNewAdjacentTiles(iteration, adjacent);
			}
			HashSet<Vector2i> delayedAdjacent;
			if (this.FreedTileLists.TryGetValue((iteration - 2) % 3, out delayedAdjacent))
			{
				this.AddNewAdjacentTiles(iteration, delayedAdjacent);
			}
			List<Vector2i> diagonal;
			if (this.TileLists.TryGetValue(iteration - 3, out diagonal))
			{
				base.AddNewDiagonalTiles(iteration, diagonal, false);
			}
			HashSet<Vector2i> delayedDiagonal;
			if (this.FreedTileLists.TryGetValue((iteration - 3) % 3, out delayedDiagonal))
			{
				base.AddNewDiagonalTiles(iteration, delayedDiagonal, false);
			}
			foreach (Vector2i tile in inputSpaceTiles)
			{
				this.ProcessNewTile(iteration, tile, AtmosDirection.All);
			}
			if (this.NewTiles.Count != 0)
			{
				this.TileLists[iteration] = this.NewTiles;
			}
			if (this.NewBlockedTiles.Count != 0)
			{
				this.BlockedTileLists[iteration] = this.NewBlockedTiles;
			}
			this.FreedTileLists[iteration % 3] = this.NewFreedTiles;
			return this.NewTiles.Count + this.NewBlockedTiles.Count;
		}

		// Token: 0x06001AA6 RID: 6822 RVA: 0x0008CE48 File Offset: 0x0008B048
		private void JumpToGrid(BlockedSpaceTile blocker)
		{
			foreach (BlockedSpaceTile.GridEdgeData edge in blocker.BlockingGridEdges)
			{
				if (edge.Grid != null)
				{
					HashSet<Vector2i> set;
					if (!this.GridJump.TryGetValue(edge.Grid.Value, out set))
					{
						set = new HashSet<Vector2i>();
						this.GridJump[edge.Grid.Value] = set;
					}
					set.Add(edge.Tile);
				}
			}
		}

		// Token: 0x06001AA7 RID: 6823 RVA: 0x0008CEE8 File Offset: 0x0008B0E8
		private void AddNewAdjacentTiles(int iteration, IEnumerable<Vector2i> tiles)
		{
			foreach (Vector2i tile in tiles)
			{
				AtmosDirection unblockedDirections = this.GetUnblockedDirectionOrAll(tile);
				if (unblockedDirections != AtmosDirection.Invalid)
				{
					for (int i = 0; i < 4; i++)
					{
						AtmosDirection direction = (AtmosDirection)(1 << i);
						if (unblockedDirections.IsFlagSet(direction))
						{
							this.ProcessNewTile(iteration, tile.Offset(direction), direction.GetOpposite());
						}
					}
				}
			}
		}

		// Token: 0x06001AA8 RID: 6824 RVA: 0x0008CF68 File Offset: 0x0008B168
		public override void InitTile(Vector2i initialTile)
		{
			this.ProcessedTiles.Add(initialTile);
			this.TileLists[0] = new List<Vector2i>
			{
				initialTile
			};
			BlockedSpaceTile blocker;
			if (this._gridBlockMap.TryGetValue(initialTile, out blocker))
			{
				this.JumpToGrid(blocker);
			}
		}

		// Token: 0x06001AA9 RID: 6825 RVA: 0x0008CFB4 File Offset: 0x0008B1B4
		protected override void ProcessNewTile(int iteration, Vector2i tile, AtmosDirection entryDirection)
		{
			BlockedSpaceTile blocker;
			if (!this._gridBlockMap.TryGetValue(tile, out blocker))
			{
				if (this.ProcessedTiles.Add(tile))
				{
					this.NewTiles.Add(tile);
				}
				return;
			}
			if ((blocker.UnblockedDirections & entryDirection) == AtmosDirection.Invalid)
			{
				if (this.EnteredBlockedTiles.Contains(tile))
				{
					return;
				}
				if (!this.UnenteredBlockedTiles.Add(tile))
				{
					return;
				}
				this.NewBlockedTiles.Add(tile);
				this.JumpToGrid(blocker);
			}
			if (!this.EnteredBlockedTiles.Add(tile))
			{
				return;
			}
			if (this.UnenteredBlockedTiles.Contains(tile))
			{
				this.NewFreedTiles.Add(tile);
				return;
			}
			this.NewTiles.Add(tile);
			this.JumpToGrid(blocker);
		}

		// Token: 0x06001AAA RID: 6826 RVA: 0x0008D068 File Offset: 0x0008B268
		protected override AtmosDirection GetUnblockedDirectionOrAll(Vector2i tile)
		{
			BlockedSpaceTile blocker;
			if (!this._gridBlockMap.TryGetValue(tile, out blocker))
			{
				return AtmosDirection.All;
			}
			return blocker.UnblockedDirections;
		}

		// Token: 0x040010FA RID: 4346
		private Dictionary<Vector2i, BlockedSpaceTile> _gridBlockMap;

		// Token: 0x040010FB RID: 4347
		public Dictionary<EntityUid, HashSet<Vector2i>> GridJump = new Dictionary<EntityUid, HashSet<Vector2i>>();

		// Token: 0x040010FC RID: 4348
		public ushort TileSize = 1;
	}
}
