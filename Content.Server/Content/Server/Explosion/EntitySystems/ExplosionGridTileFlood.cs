using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Atmos;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;

namespace Content.Server.Explosion.EntitySystems
{
	// Token: 0x0200050A RID: 1290
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ExplosionGridTileFlood : ExplosionTileFlood
	{
		// Token: 0x06001A9C RID: 6812 RVA: 0x0008C408 File Offset: 0x0008A608
		public ExplosionGridTileFlood(MapGridComponent grid, Dictionary<Vector2i, ExplosionSystem.TileData> airtightMap, float maxIntensity, float intensityStepSize, int typeIndex, Dictionary<Vector2i, ExplosionSystem.NeighborFlag> edgeTiles, EntityUid? referenceGrid, Matrix3 spaceMatrix, Angle spaceAngle)
		{
			this.Grid = grid;
			this._airtightMap = airtightMap;
			this._maxIntensity = maxIntensity;
			this._intensityStepSize = intensityStepSize;
			this._typeIndex = typeIndex;
			this._edgeTiles = edgeTiles;
			foreach (KeyValuePair<Vector2i, ExplosionSystem.NeighborFlag> keyValuePair in this._edgeTiles)
			{
				Vector2i vector2i;
				ExplosionSystem.NeighborFlag neighborFlag;
				keyValuePair.Deconstruct(out vector2i, out neighborFlag);
				Vector2i tile = vector2i;
				ExplosionSystem.NeighborFlag spaceNeighbors = neighborFlag;
				for (int i = 0; i < ExplosionSystem.NeighbourVectors.Length; i++)
				{
					ExplosionSystem.NeighborFlag dir = (ExplosionSystem.NeighborFlag)(1 << i);
					if ((spaceNeighbors & dir) != ExplosionSystem.NeighborFlag.Invalid)
					{
						this._spaceTiles.Add(tile + ExplosionSystem.NeighbourVectors[i]);
					}
				}
			}
			EntityUid? entityUid = referenceGrid;
			EntityUid owner = this.Grid.Owner;
			if (entityUid != null && (entityUid == null || entityUid.GetValueOrDefault() == owner))
			{
				return;
			}
			this._needToTransform = true;
			TransformComponent transform = IoCManager.Resolve<IEntityManager>().GetComponent<TransformComponent>(this.Grid.Owner);
			float size = (float)this.Grid.TileSize;
			this._matrix.R0C2 = size / 2f;
			this._matrix.R1C2 = size / 2f;
			Matrix3 worldMatrix = transform.WorldMatrix;
			Matrix3 matrix = Matrix3.Invert(ref spaceMatrix);
			Matrix3 matrix2 = ref worldMatrix * ref matrix;
			this._matrix = ref this._matrix * ref matrix2;
			Angle relativeAngle = transform.WorldRotation - spaceAngle;
			Vector2 vector = new ValueTuple<float, float>(size / 4f, size / 4f);
			this._offset = relativeAngle.RotateVec(ref vector);
		}

		// Token: 0x06001A9D RID: 6813 RVA: 0x0008C608 File Offset: 0x0008A808
		public override void InitTile(Vector2i initialTile)
		{
			this.TileLists[0] = new List<Vector2i>
			{
				initialTile
			};
			if (this._airtightMap.ContainsKey(initialTile))
			{
				this.EnteredBlockedTiles.Add(initialTile);
				return;
			}
			this.ProcessedTiles.Add(initialTile);
		}

		// Token: 0x06001A9E RID: 6814 RVA: 0x0008C658 File Offset: 0x0008A858
		[NullableContext(2)]
		public int AddNewTiles(int iteration, HashSet<Vector2i> gridJump)
		{
			this.SpaceJump = new HashSet<Vector2i>();
			this.NewTiles = new List<Vector2i>();
			this.NewBlockedTiles = new List<Vector2i>();
			HashSet<Vector2i> freed;
			if (this.FreedTileLists.TryGetValue(iteration, out freed))
			{
				HashSet<Vector2i> toRemove = new HashSet<Vector2i>();
				foreach (Vector2i tile in freed)
				{
					if (!this.EnteredBlockedTiles.Add(tile))
					{
						toRemove.Add(tile);
					}
				}
				freed.ExceptWith(toRemove);
				this.NewFreedTiles = freed;
			}
			else
			{
				this.NewFreedTiles = new HashSet<Vector2i>();
				this.FreedTileLists[iteration] = this.NewFreedTiles;
			}
			List<Vector2i> adjacent;
			if (this.TileLists.TryGetValue(iteration - 2, out adjacent))
			{
				this.AddNewAdjacentTiles(iteration, adjacent, false);
			}
			HashSet<Vector2i> delayedAdjacent;
			if (this.FreedTileLists.TryGetValue(iteration - 2, out delayedAdjacent))
			{
				this.AddNewAdjacentTiles(iteration, delayedAdjacent, true);
			}
			List<Vector2i> diagonal;
			if (this.TileLists.TryGetValue(iteration - 3, out diagonal))
			{
				base.AddNewDiagonalTiles(iteration, diagonal, false);
			}
			HashSet<Vector2i> delayedDiagonal;
			if (this.FreedTileLists.TryGetValue(iteration - 3, out delayedDiagonal))
			{
				base.AddNewDiagonalTiles(iteration, delayedDiagonal, true);
			}
			this.AddDelayedNeighbors(iteration);
			if (gridJump != null)
			{
				foreach (Vector2i tile2 in gridJump)
				{
					this.ProcessNewTile(iteration, tile2, AtmosDirection.Invalid);
				}
			}
			if (this.NewTiles.Count != 0)
			{
				this.TileLists[iteration] = this.NewTiles;
			}
			if (this.NewBlockedTiles.Count != 0)
			{
				this.BlockedTileLists[iteration] = this.NewBlockedTiles;
			}
			return this.NewTiles.Count + this.NewBlockedTiles.Count;
		}

		// Token: 0x06001A9F RID: 6815 RVA: 0x0008C830 File Offset: 0x0008AA30
		protected override void ProcessNewTile(int iteration, Vector2i tile, AtmosDirection entryDirections)
		{
			ExplosionSystem.TileData tileData;
			if (!this._airtightMap.TryGetValue(tile, out tileData))
			{
				if (this._spaceTiles.Contains(tile))
				{
					this.JumpToSpace(tile);
					return;
				}
				if (this.ProcessedTiles.Add(tile))
				{
					this.NewTiles.Add(tile);
				}
				return;
			}
			else
			{
				AtmosDirection blockedDirections = tileData.BlockedDirections;
				bool blocked;
				if (entryDirections == AtmosDirection.Invalid)
				{
					blocked = ExplosionSystem.AnyNeighborBlocked(this._edgeTiles[tile], blockedDirections);
				}
				else
				{
					blocked = ((blockedDirections & entryDirections) == entryDirections);
				}
				if (blocked)
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
					float required = tileData.ExplosionTolerance[this._typeIndex];
					if (required > this._maxIntensity)
					{
						return;
					}
					int clearIteration = iteration + (int)MathF.Ceiling(required / this._intensityStepSize);
					HashSet<Vector2i> list;
					if (this.FreedTileLists.TryGetValue(clearIteration, out list))
					{
						list.Add(tile);
						return;
					}
					this.FreedTileLists[clearIteration] = new HashSet<Vector2i>
					{
						tile
					};
					return;
				}
				else
				{
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
					return;
				}
			}
		}

		// Token: 0x06001AA0 RID: 6816 RVA: 0x0008C96C File Offset: 0x0008AB6C
		private void JumpToSpace(Vector2i tile)
		{
			if (!this._processedSpaceTiles.Add(tile))
			{
				return;
			}
			if (!this._needToTransform)
			{
				this.SpaceJump.Add(tile);
				return;
			}
			Vector2 center = this._matrix.Transform(tile);
			this.SpaceJump.Add(new Vector2i((int)MathF.Floor(center.X + this._offset.X), (int)MathF.Floor(center.Y + this._offset.Y)));
			this.SpaceJump.Add(new Vector2i((int)MathF.Floor(center.X - this._offset.Y), (int)MathF.Floor(center.Y + this._offset.X)));
			this.SpaceJump.Add(new Vector2i((int)MathF.Floor(center.X - this._offset.X), (int)MathF.Floor(center.Y - this._offset.Y)));
			this.SpaceJump.Add(new Vector2i((int)MathF.Floor(center.X + this._offset.Y), (int)MathF.Floor(center.Y - this._offset.X)));
		}

		// Token: 0x06001AA1 RID: 6817 RVA: 0x0008CAB4 File Offset: 0x0008ACB4
		private void AddDelayedNeighbors(int iteration)
		{
			List<ValueTuple<Vector2i, AtmosDirection>> delayed;
			if (!this._delayedNeighbors.TryGetValue(iteration, out delayed))
			{
				return;
			}
			foreach (ValueTuple<Vector2i, AtmosDirection> valueTuple in delayed)
			{
				Vector2i tile = valueTuple.Item1;
				AtmosDirection direction = valueTuple.Item2;
				this.ProcessNewTile(iteration, tile, direction);
			}
			this._delayedNeighbors.Remove(iteration);
		}

		// Token: 0x06001AA2 RID: 6818 RVA: 0x0008CB30 File Offset: 0x0008AD30
		private void AddNewAdjacentTiles(int iteration, IEnumerable<Vector2i> tiles, bool ignoreTileBlockers = false)
		{
			foreach (Vector2i tile in tiles)
			{
				AtmosDirection blockedDirections = AtmosDirection.Invalid;
				float sealIntegrity = 0f;
				ExplosionSystem.TileData tileData;
				if (this._airtightMap.TryGetValue(tile, out tileData))
				{
					blockedDirections = tileData.BlockedDirections;
					sealIntegrity = tileData.ExplosionTolerance[this._typeIndex];
				}
				for (int i = 0; i < 4; i++)
				{
					AtmosDirection direction = (AtmosDirection)(1 << i);
					if (ignoreTileBlockers || !blockedDirections.IsFlagSet(direction))
					{
						this.ProcessNewTile(iteration, tile.Offset(direction), direction.GetOpposite());
					}
				}
				if (!ignoreTileBlockers && blockedDirections != AtmosDirection.Invalid && sealIntegrity <= this._maxIntensity)
				{
					int clearIteration = iteration + (int)MathF.Ceiling(sealIntegrity / this._intensityStepSize);
					List<ValueTuple<Vector2i, AtmosDirection>> list;
					if (!this._delayedNeighbors.TryGetValue(clearIteration, out list))
					{
						list = new List<ValueTuple<Vector2i, AtmosDirection>>();
						this._delayedNeighbors[clearIteration] = list;
					}
					for (int j = 0; j < 4; j++)
					{
						AtmosDirection direction2 = (AtmosDirection)(1 << j);
						if (blockedDirections.IsFlagSet(direction2))
						{
							list.Add(new ValueTuple<Vector2i, AtmosDirection>(tile.Offset(direction2), direction2.GetOpposite()));
						}
					}
				}
			}
		}

		// Token: 0x06001AA3 RID: 6819 RVA: 0x0008CC7C File Offset: 0x0008AE7C
		protected override AtmosDirection GetUnblockedDirectionOrAll(Vector2i tile)
		{
			return ~this._airtightMap.GetValueOrDefault(tile).BlockedDirections;
		}

		// Token: 0x040010ED RID: 4333
		public MapGridComponent Grid;

		// Token: 0x040010EE RID: 4334
		private bool _needToTransform;

		// Token: 0x040010EF RID: 4335
		private Matrix3 _matrix = Matrix3.Identity;

		// Token: 0x040010F0 RID: 4336
		private Vector2 _offset;

		// Token: 0x040010F1 RID: 4337
		[Nullable(new byte[]
		{
			1,
			1,
			0
		})]
		private Dictionary<int, List<ValueTuple<Vector2i, AtmosDirection>>> _delayedNeighbors = new Dictionary<int, List<ValueTuple<Vector2i, AtmosDirection>>>();

		// Token: 0x040010F2 RID: 4338
		private Dictionary<Vector2i, ExplosionSystem.TileData> _airtightMap;

		// Token: 0x040010F3 RID: 4339
		private float _maxIntensity;

		// Token: 0x040010F4 RID: 4340
		private float _intensityStepSize;

		// Token: 0x040010F5 RID: 4341
		private int _typeIndex;

		// Token: 0x040010F6 RID: 4342
		private UniqueVector2iSet _spaceTiles = new UniqueVector2iSet();

		// Token: 0x040010F7 RID: 4343
		private UniqueVector2iSet _processedSpaceTiles = new UniqueVector2iSet();

		// Token: 0x040010F8 RID: 4344
		public HashSet<Vector2i> SpaceJump = new HashSet<Vector2i>();

		// Token: 0x040010F9 RID: 4345
		private Dictionary<Vector2i, ExplosionSystem.NeighborFlag> _edgeTiles;
	}
}
