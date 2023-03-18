using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.Anomaly.Components;
using Content.Shared.Anomaly.Effects.Components;
using Content.Shared.Throwing;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;

namespace Content.Shared.Anomaly.Effects
{
	// Token: 0x02000705 RID: 1797
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedGravityAnomalySystem : EntitySystem
	{
		// Token: 0x0600159E RID: 5534 RVA: 0x00046F4E File Offset: 0x0004514E
		public override void Initialize()
		{
			base.SubscribeLocalEvent<GravityAnomalyComponent, AnomalyPulseEvent>(new ComponentEventRefHandler<GravityAnomalyComponent, AnomalyPulseEvent>(this.OnAnomalyPulse), null, null);
			base.SubscribeLocalEvent<GravityAnomalyComponent, AnomalySupercriticalEvent>(new ComponentEventRefHandler<GravityAnomalyComponent, AnomalySupercriticalEvent>(this.OnSupercritical), null, null);
		}

		// Token: 0x0600159F RID: 5535 RVA: 0x00046F78 File Offset: 0x00045178
		private void OnAnomalyPulse(EntityUid uid, GravityAnomalyComponent component, ref AnomalyPulseEvent args)
		{
			TransformComponent xform = base.Transform(uid);
			float range = component.MaxThrowRange * args.Severity;
			float strength = component.MaxThrowStrength * args.Severity;
			foreach (EntityUid ent in this._lookup.GetEntitiesInRange(uid, range, 10))
			{
				Vector2 foo = base.Transform(ent).MapPosition.Position - xform.MapPosition.Position;
				this._throwing.TryThrow(ent, foo * 10f, strength, new EntityUid?(uid), 0f, null, null, null, null);
			}
		}

		// Token: 0x060015A0 RID: 5536 RVA: 0x00047054 File Offset: 0x00045254
		private void OnSupercritical(EntityUid uid, GravityAnomalyComponent component, ref AnomalySupercriticalEvent args)
		{
			TransformComponent xform = base.Transform(uid);
			MapGridComponent grid;
			if (!this._map.TryGetGrid(xform.GridUid, ref grid))
			{
				return;
			}
			Vector2 worldPos = this._xform.GetWorldPosition(xform);
			List<ValueTuple<Vector2i, Tile>> tiles = (from t in grid.GetTilesIntersecting(new Circle(worldPos, component.SpaceRange), true, null).ToArray<TileRef>()
			select new ValueTuple<Vector2i, Tile>(t.GridIndices, Tile.Empty)).ToList<ValueTuple<Vector2i, Tile>>();
			grid.SetTiles(tiles);
			float range = component.MaxThrowRange * 2f;
			float strength = component.MaxThrowStrength * 2f;
			foreach (EntityUid ent in this._lookup.GetEntitiesInRange(uid, range, 10))
			{
				Vector2 foo = base.Transform(ent).MapPosition.Position - xform.MapPosition.Position;
				this._throwing.TryThrow(ent, foo * 5f, strength, new EntityUid?(uid), 0f, null, null, null, null);
			}
		}

		// Token: 0x040015DC RID: 5596
		[Dependency]
		private readonly IMapManager _map;

		// Token: 0x040015DD RID: 5597
		[Dependency]
		private readonly EntityLookupSystem _lookup;

		// Token: 0x040015DE RID: 5598
		[Dependency]
		private readonly ThrowingSystem _throwing;

		// Token: 0x040015DF RID: 5599
		[Dependency]
		private readonly SharedTransformSystem _xform;
	}
}
