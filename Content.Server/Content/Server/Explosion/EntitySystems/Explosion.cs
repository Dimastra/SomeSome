using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Damage;
using Content.Shared.Explosion;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;

namespace Content.Server.Explosion.EntitySystems
{
	// Token: 0x0200050E RID: 1294
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class Explosion
	{
		// Token: 0x170003FE RID: 1022
		// (get) Token: 0x06001AF3 RID: 6899 RVA: 0x00090339 File Offset: 0x0008E539
		// (set) Token: 0x06001AF4 RID: 6900 RVA: 0x00090341 File Offset: 0x0008E541
		public int CurrentIteration { get; private set; }

		// Token: 0x06001AF5 RID: 6901 RVA: 0x0009034C File Offset: 0x0008E54C
		public Explosion(ExplosionSystem system, ExplosionPrototype explosionType, [Nullable(2)] ExplosionSpaceTileFlood spaceData, List<ExplosionGridTileFlood> gridData, List<float> tileSetIntensity, MapCoordinates epicenter, Matrix3 spaceMatrix, int area, float tileBreakScale, int maxTileBreak, bool canCreateVacuum, IEntityManager entMan, IMapManager mapMan, EntityUid visualEnt)
		{
			this.VisualEnt = visualEnt;
			this._system = system;
			this.ExplosionType = explosionType;
			this._tileSetIntensity = tileSetIntensity;
			this.Epicenter = epicenter;
			this.Area = area;
			this._tileBreakScale = tileBreakScale;
			this._maxTileBreak = maxTileBreak;
			this._canCreateVacuum = canCreateVacuum;
			this._entMan = entMan;
			if (this.Area > 100)
			{
				this._flags |= 1;
			}
			this._xformQuery = entMan.GetEntityQuery<TransformComponent>();
			this._physicsQuery = entMan.GetEntityQuery<PhysicsComponent>();
			this._damageQuery = entMan.GetEntityQuery<DamageableComponent>();
			if (spaceData != null)
			{
				EntityUid mapUid = mapMan.GetMapEntityId(epicenter.MapId);
				this._explosionData.Add(new Explosion.ExplosionData
				{
					TileLists = spaceData.TileLists,
					Lookup = entMan.GetComponent<BroadphaseComponent>(mapUid),
					MapGrid = null
				});
				this._spaceMatrix = spaceMatrix;
				this._invSpaceMatrix = Matrix3.Invert(ref spaceMatrix);
			}
			foreach (ExplosionGridTileFlood grid in gridData)
			{
				this._explosionData.Add(new Explosion.ExplosionData
				{
					TileLists = grid.TileLists,
					Lookup = entMan.GetComponent<BroadphaseComponent>(grid.Grid.Owner),
					MapGrid = grid.Grid
				});
			}
			if (this.TryGetNextTileEnumerator())
			{
				this.MoveNext();
			}
		}

		// Token: 0x06001AF6 RID: 6902 RVA: 0x00090500 File Offset: 0x0008E700
		private bool TryGetNextTileEnumerator()
		{
			while (this.CurrentIteration < this._tileSetIntensity.Count)
			{
				this._currentIntensity = this._tileSetIntensity[this.CurrentIteration];
				this._currentDamage = this.ExplosionType.DamagePerIntensity * this._currentIntensity;
				this._currentThrowForce = ((this.Area < this._system.ThrowLimit || this.CurrentIteration > this._tileSetIntensity.Count - 6) ? (10f * MathF.Sqrt(this._currentIntensity)) : 0f);
				while (this._currentDataIndex < this._explosionData.Count)
				{
					List<Vector2i> tileList;
					if (!this._explosionData[this._currentDataIndex].TileLists.TryGetValue(this.CurrentIteration, out tileList))
					{
						this._currentDataIndex++;
					}
					else
					{
						this._currentEnumerator = tileList.GetEnumerator();
						this._currentLookup = this._explosionData[this._currentDataIndex].Lookup;
						this._currentGrid = this._explosionData[this._currentDataIndex].MapGrid;
						this._currentDataIndex++;
						if (!this._currentLookup.Deleted && (this._currentGrid == null || this._entMan.EntityExists(this._currentGrid.Owner)))
						{
							return true;
						}
					}
				}
				int currentIteration = this.CurrentIteration;
				this.CurrentIteration = currentIteration + 1;
				this._currentDataIndex = 0;
			}
			this.FinishedProcessing = true;
			return false;
		}

		// Token: 0x06001AF7 RID: 6903 RVA: 0x0009069B File Offset: 0x0008E89B
		private bool MoveNext()
		{
			if (this.FinishedProcessing)
			{
				return false;
			}
			while (!this.FinishedProcessing)
			{
				if (this._currentEnumerator.MoveNext())
				{
					return true;
				}
				this.TryGetNextTileEnumerator();
			}
			return false;
		}

		// Token: 0x06001AF8 RID: 6904 RVA: 0x000906C8 File Offset: 0x0008E8C8
		public int Process(int processingTarget)
		{
			this.SetTiles();
			int processed = 0;
			while (processed < processingTarget && (processed % ExplosionSystem.TileCheckIteration != 0 || this._system.Stopwatch.Elapsed.TotalMilliseconds <= (double)this._system.MaxProcessingTime))
			{
				TileRef tileRef;
				if (this._currentGrid != null && this._currentGrid.TryGetTileRef(this._currentEnumerator.Current, ref tileRef) && !tileRef.Tile.IsEmpty)
				{
					List<ValueTuple<Vector2i, Tile>> tileUpdateList;
					if (!this._tileUpdateDict.TryGetValue(this._currentGrid, out tileUpdateList))
					{
						tileUpdateList = new List<ValueTuple<Vector2i, Tile>>();
						this._tileUpdateDict[this._currentGrid] = tileUpdateList;
					}
					if (this._system.ExplodeTile(this._currentLookup, this._currentGrid, this._currentEnumerator.Current, this._currentThrowForce, this._currentDamage, this.Epicenter, this.ProcessedEntities, this.ExplosionType.ID, this._xformQuery, this._damageQuery, this._physicsQuery, this._flags))
					{
						this._system.DamageFloorTile(tileRef, this._currentIntensity * this._tileBreakScale, this._maxTileBreak, this._canCreateVacuum, tileUpdateList, this.ExplosionType);
					}
				}
				else
				{
					this._system.ExplodeSpace(this._currentLookup, this._spaceMatrix, this._invSpaceMatrix, this._currentEnumerator.Current, this._currentThrowForce, this._currentDamage, this.Epicenter, this.ProcessedEntities, this.ExplosionType.ID, this._xformQuery, this._damageQuery, this._physicsQuery, this._flags);
				}
				if (!this.MoveNext())
				{
					break;
				}
				processed++;
			}
			this.SetTiles();
			return processed;
		}

		// Token: 0x06001AF9 RID: 6905 RVA: 0x00090888 File Offset: 0x0008EA88
		private void SetTiles()
		{
			if (!this._system.IncrementalTileBreaking && !this.FinishedProcessing)
			{
				return;
			}
			foreach (KeyValuePair<MapGridComponent, List<ValueTuple<Vector2i, Tile>>> keyValuePair in this._tileUpdateDict)
			{
				MapGridComponent mapGridComponent;
				List<ValueTuple<Vector2i, Tile>> list2;
				keyValuePair.Deconstruct(out mapGridComponent, out list2);
				MapGridComponent grid = mapGridComponent;
				List<ValueTuple<Vector2i, Tile>> list = list2;
				if (list.Count > 0 && this._entMan.EntityExists(grid.Owner))
				{
					grid.SetTiles(list);
				}
			}
			this._tileUpdateDict.Clear();
		}

		// Token: 0x04001128 RID: 4392
		private readonly List<Explosion.ExplosionData> _explosionData = new List<Explosion.ExplosionData>();

		// Token: 0x04001129 RID: 4393
		private readonly List<float> _tileSetIntensity;

		// Token: 0x0400112A RID: 4394
		public readonly HashSet<EntityUid> ProcessedEntities = new HashSet<EntityUid>();

		// Token: 0x0400112C RID: 4396
		public readonly ExplosionPrototype ExplosionType;

		// Token: 0x0400112D RID: 4397
		public readonly MapCoordinates Epicenter;

		// Token: 0x0400112E RID: 4398
		private readonly Matrix3 _spaceMatrix;

		// Token: 0x0400112F RID: 4399
		private readonly Matrix3 _invSpaceMatrix;

		// Token: 0x04001130 RID: 4400
		public bool FinishedProcessing;

		// Token: 0x04001131 RID: 4401
		private DamageSpecifier _currentDamage;

		// Token: 0x04001132 RID: 4402
		private BroadphaseComponent _currentLookup;

		// Token: 0x04001133 RID: 4403
		[Nullable(2)]
		private MapGridComponent _currentGrid;

		// Token: 0x04001134 RID: 4404
		private float _currentIntensity;

		// Token: 0x04001135 RID: 4405
		private float _currentThrowForce;

		// Token: 0x04001136 RID: 4406
		[Nullable(0)]
		private List<Vector2i>.Enumerator _currentEnumerator;

		// Token: 0x04001137 RID: 4407
		private int _currentDataIndex;

		// Token: 0x04001138 RID: 4408
		[Nullable(new byte[]
		{
			1,
			1,
			1,
			0
		})]
		private readonly Dictionary<MapGridComponent, List<ValueTuple<Vector2i, Tile>>> _tileUpdateDict = new Dictionary<MapGridComponent, List<ValueTuple<Vector2i, Tile>>>();

		// Token: 0x04001139 RID: 4409
		[Nullable(new byte[]
		{
			0,
			1
		})]
		private readonly EntityQuery<TransformComponent> _xformQuery;

		// Token: 0x0400113A RID: 4410
		[Nullable(new byte[]
		{
			0,
			1
		})]
		private readonly EntityQuery<PhysicsComponent> _physicsQuery;

		// Token: 0x0400113B RID: 4411
		[Nullable(new byte[]
		{
			0,
			1
		})]
		private readonly EntityQuery<DamageableComponent> _damageQuery;

		// Token: 0x0400113C RID: 4412
		public readonly int Area;

		// Token: 0x0400113D RID: 4413
		private readonly LookupFlags _flags;

		// Token: 0x0400113E RID: 4414
		private readonly float _tileBreakScale;

		// Token: 0x0400113F RID: 4415
		private readonly int _maxTileBreak;

		// Token: 0x04001140 RID: 4416
		private readonly bool _canCreateVacuum;

		// Token: 0x04001141 RID: 4417
		private readonly IEntityManager _entMan;

		// Token: 0x04001142 RID: 4418
		private readonly ExplosionSystem _system;

		// Token: 0x04001143 RID: 4419
		public readonly EntityUid VisualEnt;

		// Token: 0x02000A08 RID: 2568
		[Nullable(0)]
		private struct ExplosionData
		{
			// Token: 0x0400231D RID: 8989
			public Dictionary<int, List<Vector2i>> TileLists;

			// Token: 0x0400231E RID: 8990
			public BroadphaseComponent Lookup;

			// Token: 0x0400231F RID: 8991
			[Nullable(2)]
			public MapGridComponent MapGrid;
		}
	}
}
