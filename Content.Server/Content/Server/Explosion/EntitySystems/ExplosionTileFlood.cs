using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Atmos;
using Robust.Shared.Maths;

namespace Content.Server.Explosion.EntitySystems
{
	// Token: 0x0200050F RID: 1295
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class ExplosionTileFlood
	{
		// Token: 0x06001AFA RID: 6906
		public abstract void InitTile(Vector2i initialTile);

		// Token: 0x06001AFB RID: 6907
		protected abstract void ProcessNewTile(int iteration, Vector2i tile, AtmosDirection entryDirections);

		// Token: 0x06001AFC RID: 6908
		protected abstract AtmosDirection GetUnblockedDirectionOrAll(Vector2i tile);

		// Token: 0x06001AFD RID: 6909 RVA: 0x0009092C File Offset: 0x0008EB2C
		protected void AddNewDiagonalTiles(int iteration, IEnumerable<Vector2i> tiles, bool ignoreLocalBlocker = false)
		{
			AtmosDirection entryDirection = AtmosDirection.Invalid;
			foreach (Vector2i tile in tiles)
			{
				object obj = ignoreLocalBlocker ? AtmosDirection.All : this.GetUnblockedDirectionOrAll(tile);
				AtmosDirection freeDirectionsN = this.GetUnblockedDirectionOrAll(tile.Offset(AtmosDirection.North));
				AtmosDirection freeDirectionsE = this.GetUnblockedDirectionOrAll(tile.Offset(AtmosDirection.East));
				AtmosDirection freeDirectionsS = this.GetUnblockedDirectionOrAll(tile.Offset(AtmosDirection.South));
				AtmosDirection freeDirectionsW = this.GetUnblockedDirectionOrAll(tile.Offset(AtmosDirection.West));
				object direction = obj;
				if (direction.IsFlagSet(AtmosDirection.North) && freeDirectionsN.IsFlagSet(AtmosDirection.SouthEast))
				{
					entryDirection |= AtmosDirection.West;
				}
				if (direction.IsFlagSet(AtmosDirection.East) && freeDirectionsE.IsFlagSet(AtmosDirection.NorthWest))
				{
					entryDirection |= AtmosDirection.South;
				}
				if (entryDirection != AtmosDirection.Invalid)
				{
					this.ProcessNewTile(iteration, tile + new ValueTuple<int, int>(1, 1), entryDirection);
					entryDirection = AtmosDirection.Invalid;
				}
				if (direction.IsFlagSet(AtmosDirection.North) && freeDirectionsN.IsFlagSet(AtmosDirection.SouthWest))
				{
					entryDirection |= AtmosDirection.East;
				}
				if (direction.IsFlagSet(AtmosDirection.West) && freeDirectionsW.IsFlagSet(AtmosDirection.NorthEast))
				{
					entryDirection |= AtmosDirection.West;
				}
				if (entryDirection != AtmosDirection.Invalid)
				{
					this.ProcessNewTile(iteration, tile + new ValueTuple<int, int>(-1, 1), entryDirection);
					entryDirection = AtmosDirection.Invalid;
				}
				if (direction.IsFlagSet(AtmosDirection.South) && freeDirectionsS.IsFlagSet(AtmosDirection.NorthEast))
				{
					entryDirection |= AtmosDirection.West;
				}
				if (direction.IsFlagSet(AtmosDirection.East) && freeDirectionsE.IsFlagSet(AtmosDirection.SouthWest))
				{
					entryDirection |= AtmosDirection.North;
				}
				if (entryDirection != AtmosDirection.Invalid)
				{
					this.ProcessNewTile(iteration, tile + new ValueTuple<int, int>(1, -1), entryDirection);
					entryDirection = AtmosDirection.Invalid;
				}
				if (direction.IsFlagSet(AtmosDirection.South) && freeDirectionsS.IsFlagSet(AtmosDirection.NorthWest))
				{
					entryDirection |= AtmosDirection.West;
				}
				if (direction.IsFlagSet(AtmosDirection.West) && freeDirectionsW.IsFlagSet(AtmosDirection.SouthEast))
				{
					entryDirection |= AtmosDirection.North;
				}
				if (entryDirection != AtmosDirection.Invalid)
				{
					this.ProcessNewTile(iteration, tile + new ValueTuple<int, int>(-1, -1), entryDirection);
					entryDirection = AtmosDirection.Invalid;
				}
			}
		}

		// Token: 0x06001AFE RID: 6910 RVA: 0x00090B00 File Offset: 0x0008ED00
		public void CleanUp()
		{
			foreach (KeyValuePair<int, List<Vector2i>> keyValuePair in this.BlockedTileLists)
			{
				int num;
				List<Vector2i> list;
				keyValuePair.Deconstruct(out num, out list);
				int iteration = num;
				List<Vector2i> blocked = list;
				List<Vector2i> tiles;
				if (this.TileLists.TryGetValue(iteration, out tiles))
				{
					tiles.AddRange(blocked);
				}
				else
				{
					this.TileLists[iteration] = blocked;
				}
			}
		}

		// Token: 0x04001144 RID: 4420
		public Dictionary<int, List<Vector2i>> TileLists = new Dictionary<int, List<Vector2i>>();

		// Token: 0x04001145 RID: 4421
		protected Dictionary<int, List<Vector2i>> BlockedTileLists = new Dictionary<int, List<Vector2i>>();

		// Token: 0x04001146 RID: 4422
		protected Dictionary<int, HashSet<Vector2i>> FreedTileLists = new Dictionary<int, HashSet<Vector2i>>();

		// Token: 0x04001147 RID: 4423
		protected List<Vector2i> NewTiles;

		// Token: 0x04001148 RID: 4424
		protected List<Vector2i> NewBlockedTiles;

		// Token: 0x04001149 RID: 4425
		protected HashSet<Vector2i> NewFreedTiles;

		// Token: 0x0400114A RID: 4426
		protected UniqueVector2iSet ProcessedTiles = new UniqueVector2iSet();

		// Token: 0x0400114B RID: 4427
		protected UniqueVector2iSet UnenteredBlockedTiles = new UniqueVector2iSet();

		// Token: 0x0400114C RID: 4428
		protected UniqueVector2iSet EnteredBlockedTiles = new UniqueVector2iSet();
	}
}
