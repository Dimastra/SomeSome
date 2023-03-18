using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Maps;
using Content.Shared.Anomaly.Components;
using Content.Shared.Anomaly.Effects.Components;
using Content.Shared.Maps;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Random;

namespace Content.Server.Anomaly.Effects
{
	// Token: 0x020007C5 RID: 1989
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class FleshAnomalySystem : EntitySystem
	{
		// Token: 0x06002B59 RID: 11097 RVA: 0x000E352C File Offset: 0x000E172C
		public override void Initialize()
		{
			base.SubscribeLocalEvent<FleshAnomalyComponent, AnomalyPulseEvent>(new ComponentEventRefHandler<FleshAnomalyComponent, AnomalyPulseEvent>(this.OnPulse), null, null);
			base.SubscribeLocalEvent<FleshAnomalyComponent, AnomalySupercriticalEvent>(new ComponentEventRefHandler<FleshAnomalyComponent, AnomalySupercriticalEvent>(this.OnSupercritical), null, null);
			base.SubscribeLocalEvent<FleshAnomalyComponent, AnomalyStabilityChangedEvent>(new ComponentEventRefHandler<FleshAnomalyComponent, AnomalyStabilityChangedEvent>(this.OnSeverityChanged), null, null);
		}

		// Token: 0x06002B5A RID: 11098 RVA: 0x000E356C File Offset: 0x000E176C
		private void OnPulse(EntityUid uid, FleshAnomalyComponent component, ref AnomalyPulseEvent args)
		{
			float range = component.SpawnRange * args.Stability;
			int amount = (int)((float)component.MaxSpawnAmount * args.Severity + 0.5f);
			TransformComponent xform = base.Transform(uid);
			this.SpawnMonstersOnOpenTiles(component, xform, amount, range);
		}

		// Token: 0x06002B5B RID: 11099 RVA: 0x000E35B0 File Offset: 0x000E17B0
		private void OnSupercritical(EntityUid uid, FleshAnomalyComponent component, ref AnomalySupercriticalEvent args)
		{
			TransformComponent xform = base.Transform(uid);
			this.SpawnMonstersOnOpenTiles(component, xform, component.MaxSpawnAmount, component.SpawnRange);
			base.Spawn(component.SupercriticalSpawn, xform.Coordinates);
		}

		// Token: 0x06002B5C RID: 11100 RVA: 0x000E35EC File Offset: 0x000E17EC
		private void OnSeverityChanged(EntityUid uid, FleshAnomalyComponent component, ref AnomalyStabilityChangedEvent args)
		{
			TransformComponent xform = base.Transform(uid);
			MapGridComponent grid;
			if (!this._map.TryGetGrid(xform.GridUid, ref grid))
			{
				return;
			}
			float radius = component.SpawnRange * args.Stability;
			ContentTileDefinition fleshTile = (ContentTileDefinition)this._tiledef[component.FleshTileId];
			Vector2 localpos = xform.Coordinates.Position;
			foreach (TileRef tileref in grid.GetLocalTilesIntersecting(new Box2(localpos + new ValueTuple<float, float>(-radius, -radius), localpos + new ValueTuple<float, float>(radius, radius)), true, null))
			{
				if (RandomExtensions.Prob(this._random, 0.33f))
				{
					this._tile.ReplaceTile(tileref, fleshTile);
				}
			}
		}

		// Token: 0x06002B5D RID: 11101 RVA: 0x000E36D8 File Offset: 0x000E18D8
		private void SpawnMonstersOnOpenTiles(FleshAnomalyComponent component, TransformComponent xform, int amount, float radius)
		{
			MapGridComponent grid;
			if (!this._map.TryGetGrid(xform.GridUid, ref grid))
			{
				return;
			}
			Vector2 localpos = xform.Coordinates.Position;
			TileRef[] tilerefs = grid.GetLocalTilesIntersecting(new Box2(localpos + new ValueTuple<float, float>(-radius, -radius), localpos + new ValueTuple<float, float>(radius, radius)), true, null).ToArray<TileRef>();
			this._random.Shuffle<TileRef>(tilerefs);
			EntityQuery<PhysicsComponent> physQuery = base.GetEntityQuery<PhysicsComponent>();
			int amountCounter = 0;
			foreach (TileRef tileref in tilerefs)
			{
				bool valid = true;
				foreach (EntityUid ent in grid.GetAnchoredEntities(tileref.GridIndices))
				{
					PhysicsComponent body;
					if (physQuery.TryGetComponent(ent, ref body) && body.BodyType == 4 && body.Hard && (body.CollisionLayer & 2) != 0)
					{
						valid = false;
						break;
					}
				}
				if (valid)
				{
					amountCounter++;
					base.Spawn(RandomExtensions.Pick<string>(this._random, component.Spawns), CoordinatesExtensions.ToEntityCoordinates(tileref.GridIndices, xform.GridUid.Value, this._map));
					if (amountCounter >= amount)
					{
						return;
					}
				}
			}
		}

		// Token: 0x04001AC1 RID: 6849
		[Dependency]
		private readonly IMapManager _map;

		// Token: 0x04001AC2 RID: 6850
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x04001AC3 RID: 6851
		[Dependency]
		private readonly ITileDefinitionManager _tiledef;

		// Token: 0x04001AC4 RID: 6852
		[Dependency]
		private readonly TileSystem _tile;
	}
}
