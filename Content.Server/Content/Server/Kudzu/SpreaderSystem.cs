using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Atmos.Components;
using Content.Server.Atmos.EntitySystems;
using Content.Shared.Atmos;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Random;

namespace Content.Server.Kudzu
{
	// Token: 0x0200042D RID: 1069
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SpreaderSystem : EntitySystem
	{
		// Token: 0x06001598 RID: 5528 RVA: 0x00071357 File Offset: 0x0006F557
		public override void Initialize()
		{
			base.SubscribeLocalEvent<SpreaderComponent, ComponentAdd>(new ComponentEventHandler<SpreaderComponent, ComponentAdd>(this.SpreaderAddHandler), null, null);
			base.SubscribeLocalEvent<AirtightChanged>(new EntityEventRefHandler<AirtightChanged>(this.OnAirtightChanged), null, null);
		}

		// Token: 0x06001599 RID: 5529 RVA: 0x00071381 File Offset: 0x0006F581
		private void OnAirtightChanged(ref AirtightChanged ev)
		{
			this.UpdateNearbySpreaders(ev.Entity, ev.Airtight);
		}

		// Token: 0x0600159A RID: 5530 RVA: 0x00071395 File Offset: 0x0006F595
		private void SpreaderAddHandler(EntityUid uid, SpreaderComponent component, ComponentAdd args)
		{
			if (component.Enabled)
			{
				this._edgeGrowths.Add(uid);
			}
		}

		// Token: 0x0600159B RID: 5531 RVA: 0x000713AC File Offset: 0x0006F5AC
		public void UpdateNearbySpreaders(EntityUid blocker, AirtightComponent comp)
		{
			TransformComponent transform;
			if (!this.EntityManager.TryGetComponent<TransformComponent>(blocker, ref transform))
			{
				return;
			}
			MapGridComponent grid;
			if (!this._mapManager.TryGetGrid(transform.GridUid, ref grid))
			{
				return;
			}
			EntityQuery<SpreaderComponent> spreaderQuery = base.GetEntityQuery<SpreaderComponent>();
			Vector2i tile = grid.TileIndicesFor(transform.Coordinates);
			for (int i = 0; i < 4; i++)
			{
				AtmosDirection direction = (AtmosDirection)(1 << i);
				if (comp.AirBlockedDirection.IsFlagSet(direction))
				{
					EntityUid? ent;
					while (grid.GetAnchoredEntitiesEnumerator(SharedMapSystem.GetDirection(tile, direction.ToDirection(), 1)).MoveNext(ref ent))
					{
						SpreaderComponent s;
						if (spreaderQuery.TryGetComponent(ent, ref s) && s.Enabled)
						{
							this._edgeGrowths.Add(ent.Value);
						}
					}
				}
			}
		}

		// Token: 0x0600159C RID: 5532 RVA: 0x00071468 File Offset: 0x0006F668
		public override void Update(float frameTime)
		{
			this._accumulatedFrameTime += frameTime;
			if (this._accumulatedFrameTime < 1f)
			{
				return;
			}
			this._accumulatedFrameTime -= 1f;
			List<EntityUid> growthList = this._edgeGrowths.ToList<EntityUid>();
			this._robustRandom.Shuffle<EntityUid>(growthList);
			int successes = 0;
			foreach (EntityUid entity in growthList)
			{
				if (this.TryGrow(entity, null, null))
				{
					successes++;
					if (successes >= 1)
					{
						break;
					}
				}
			}
		}

		// Token: 0x0600159D RID: 5533 RVA: 0x0007150C File Offset: 0x0006F70C
		[NullableContext(2)]
		private bool TryGrow(EntityUid ent, TransformComponent transform = null, SpreaderComponent spreader = null)
		{
			if (!base.Resolve<TransformComponent, SpreaderComponent>(ent, ref transform, ref spreader, false))
			{
				return false;
			}
			if (!spreader.Enabled)
			{
				return false;
			}
			MapGridComponent grid;
			if (!this._mapManager.TryGetGrid(transform.GridUid, ref grid))
			{
				return false;
			}
			bool didGrow = false;
			for (int i = 0; i < 4; i++)
			{
				DirectionFlag direction = (sbyte)(1 << i);
				EntityCoordinates coords = transform.Coordinates.Offset(DirectionExtensions.ToVec(DirectionExtensions.AsDir(direction)));
				if (!grid.GetTileRef(coords).Tile.IsEmpty && !RandomExtensions.Prob(this._robustRandom, 1f - spreader.Chance) && !grid.GetLocal(coords).Any((EntityUid x) => this.IsTileBlockedFrom(x, direction)))
				{
					didGrow = true;
					this.EntityManager.SpawnEntity(spreader.GrowthResult, transform.Coordinates.Offset(DirectionExtensions.ToVec(DirectionExtensions.AsDir(direction))));
				}
			}
			return didGrow;
		}

		// Token: 0x0600159E RID: 5534 RVA: 0x00071619 File Offset: 0x0006F819
		[NullableContext(2)]
		public void EnableSpreader(EntityUid ent, SpreaderComponent component = null)
		{
			if (!base.Resolve<SpreaderComponent>(ent, ref component, true))
			{
				return;
			}
			component.Enabled = true;
			this._edgeGrowths.Add(ent);
		}

		// Token: 0x0600159F RID: 5535 RVA: 0x0007163C File Offset: 0x0006F83C
		private bool IsTileBlockedFrom(EntityUid ent, DirectionFlag dir)
		{
			SpreaderComponent spreaderComponent;
			if (this.EntityManager.TryGetComponent<SpreaderComponent>(ent, ref spreaderComponent))
			{
				return true;
			}
			AirtightComponent airtight;
			if (!this.EntityManager.TryGetComponent<AirtightComponent>(ent, ref airtight))
			{
				return false;
			}
			AtmosDirection oppositeDir = DirectionExtensions.GetOpposite(DirectionExtensions.AsDir(dir)).ToAtmosDirection();
			return airtight.AirBlocked && airtight.AirBlockedDirection.IsFlagSet(oppositeDir);
		}

		// Token: 0x04000D70 RID: 3440
		[Dependency]
		private readonly IRobustRandom _robustRandom;

		// Token: 0x04000D71 RID: 3441
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x04000D72 RID: 3442
		private const int GrowthsPerInterval = 1;

		// Token: 0x04000D73 RID: 3443
		private float _accumulatedFrameTime;

		// Token: 0x04000D74 RID: 3444
		private readonly HashSet<EntityUid> _edgeGrowths = new HashSet<EntityUid>();
	}
}
