using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Content.Server.Fluids.Components;
using Content.Shared;
using Content.Shared.Directions;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Timing;

namespace Content.Server.Fluids.EntitySystems
{
	// Token: 0x020004ED RID: 1261
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class FluidSpreaderSystem : EntitySystem
	{
		// Token: 0x060019EF RID: 6639 RVA: 0x000880F4 File Offset: 0x000862F4
		[NullableContext(2)]
		public void AddOverflowingPuddle(EntityUid puddleUid, PuddleComponent puddle = null, TransformComponent xform = null)
		{
			if (!base.Resolve<PuddleComponent, TransformComponent>(puddleUid, ref puddle, ref xform, false) || xform.MapUid == null)
			{
				return;
			}
			EntityUid mapId = xform.MapUid.Value;
			FluidMapDataComponent component;
			this.EntityManager.EnsureComponent<FluidMapDataComponent>(mapId, ref component);
			component.Puddles.Add(puddleUid);
		}

		// Token: 0x060019F0 RID: 6640 RVA: 0x0008814C File Offset: 0x0008634C
		public unsafe override void Update(float frameTime)
		{
			base.Update(frameTime);
			IntPtr intPtr = stackalloc byte[(UIntPtr)4];
			*intPtr = 4;
			*(intPtr + (IntPtr)sizeof(Direction)) = 2;
			*(intPtr + (IntPtr)2 * (IntPtr)sizeof(Direction)) = 0;
			*(intPtr + (IntPtr)3 * (IntPtr)sizeof(Direction)) = 6;
			Span<Direction> exploreDirections = new Span<Direction>(intPtr, 4);
			List<PuddleComponent> puddles = new List<PuddleComponent>(4);
			EntityQuery<PuddleComponent> puddleQuery = base.GetEntityQuery<PuddleComponent>();
			EntityQuery<TransformComponent> xFormQuery = base.GetEntityQuery<TransformComponent>();
			foreach (FluidMapDataComponent fluidMapData in base.EntityQuery<FluidMapDataComponent>(false))
			{
				if (fluidMapData.Puddles.Count != 0 && !(this._gameTiming.CurTime <= fluidMapData.GoalTime))
				{
					HashSet<EntityUid> newIteration = new HashSet<EntityUid>();
					foreach (EntityUid puddleUid in fluidMapData.Puddles)
					{
						PuddleComponent puddle;
						TransformComponent transform;
						MapGridComponent mapGrid;
						if (puddleQuery.TryGetComponent(puddleUid, ref puddle) && xFormQuery.TryGetComponent(puddleUid, ref transform) && this._mapManager.TryGetGrid(transform.GridUid, ref mapGrid))
						{
							puddles.Clear();
							EntityCoordinates pos = transform.Coordinates;
							FixedPoint2 totalVolume = this._puddleSystem.CurrentVolume(puddle.Owner, puddle);
							exploreDirections.Shuffle(null);
							Span<Direction> span = exploreDirections;
							for (int i = 0; i < span.Length; i++)
							{
								Direction direction = *span[i];
								EntityCoordinates newPos = pos.Offset(direction);
								PuddleComponent puddleComponent;
								if (this.CheckTile(puddle.Owner, puddle, newPos, mapGrid, out puddleComponent))
								{
									puddles.Add(puddleComponent);
									totalVolume += this._puddleSystem.CurrentVolume(puddleComponent.Owner, puddleComponent);
								}
							}
							this._puddleSystem.EqualizePuddles(puddle.Owner, puddles, totalVolume, newIteration, puddle);
						}
					}
					fluidMapData.Puddles.Clear();
					fluidMapData.Puddles.UnionWith(newIteration);
					fluidMapData.UpdateGoal(new TimeSpan?(this._gameTiming.CurTime));
				}
			}
		}

		// Token: 0x060019F1 RID: 6641 RVA: 0x00088398 File Offset: 0x00086598
		private bool CheckTile(EntityUid srcUid, PuddleComponent srcPuddle, EntityCoordinates pos, MapGridComponent mapGrid, [Nullable(2)] [NotNullWhen(true)] out PuddleComponent puddle)
		{
			TileRef tileRef;
			if (!mapGrid.TryGetTileRef(pos, ref tileRef) || tileRef.Tile.IsEmpty)
			{
				puddle = null;
				return false;
			}
			FixedPoint2 puddleCurrentVolume = this._puddleSystem.CurrentVolume(srcUid, srcPuddle);
			foreach (EntityUid entity in mapGrid.GetAnchoredEntities(pos))
			{
				PuddleComponent existingPuddle;
				PhysicsComponent physComponent;
				if (base.TryComp<PuddleComponent>(entity, ref existingPuddle))
				{
					if (this._puddleSystem.CurrentVolume(existingPuddle.Owner, existingPuddle) >= puddleCurrentVolume)
					{
						puddle = null;
						return false;
					}
					puddle = existingPuddle;
					return true;
				}
				else if (base.TryComp<PhysicsComponent>(entity, ref physComponent) && physComponent.CanCollide && (physComponent.CollisionLayer & 30) != 0)
				{
					puddle = null;
					return false;
				}
			}
			puddle = this._puddleSystem.SpawnPuddle(srcUid, pos, srcPuddle);
			return true;
		}

		// Token: 0x0400104E RID: 4174
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x0400104F RID: 4175
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x04001050 RID: 4176
		[Dependency]
		private readonly PuddleSystem _puddleSystem;
	}
}
