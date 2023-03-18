using System;
using System.Runtime.CompilerServices;
using Content.Server.Atmos;
using Content.Server.Atmos.Components;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Interaction;
using Content.Shared.Anomaly.Components;
using Content.Shared.Anomaly.Effects.Components;
using Content.Shared.Physics;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;

namespace Content.Server.Anomaly.Effects
{
	// Token: 0x020007C7 RID: 1991
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PyroclasticAnomalySystem : EntitySystem
	{
		// Token: 0x06002B63 RID: 11107 RVA: 0x000E3917 File Offset: 0x000E1B17
		public override void Initialize()
		{
			base.SubscribeLocalEvent<PyroclasticAnomalyComponent, AnomalyPulseEvent>(new ComponentEventRefHandler<PyroclasticAnomalyComponent, AnomalyPulseEvent>(this.OnPulse), null, null);
			base.SubscribeLocalEvent<PyroclasticAnomalyComponent, AnomalySupercriticalEvent>(new ComponentEventRefHandler<PyroclasticAnomalyComponent, AnomalySupercriticalEvent>(this.OnSupercritical), null, null);
		}

		// Token: 0x06002B64 RID: 11108 RVA: 0x000E3944 File Offset: 0x000E1B44
		private void OnPulse(EntityUid uid, PyroclasticAnomalyComponent component, ref AnomalyPulseEvent args)
		{
			TransformComponent xform = base.Transform(uid);
			float ignitionRadius = component.MaximumIgnitionRadius * args.Stability;
			this.IgniteNearby(xform.Coordinates, args.Severity, ignitionRadius);
		}

		// Token: 0x06002B65 RID: 11109 RVA: 0x000E397C File Offset: 0x000E1B7C
		private void OnSupercritical(EntityUid uid, PyroclasticAnomalyComponent component, ref AnomalySupercriticalEvent args)
		{
			TransformComponent xform = base.Transform(uid);
			EntityUid? grid = xform.GridUid;
			EntityUid? map = xform.MapUid;
			Vector2i indices = this._xform.GetGridOrMapTilePosition(uid, xform);
			GasMixture mixture = this._atmosphere.GetTileMixture(grid, map, indices, true);
			if (mixture == null)
			{
				return;
			}
			mixture.AdjustMoles(component.SupercriticalGas, component.SupercriticalMoleAmount);
			if (grid != null)
			{
				foreach (Vector2i vector2i in this._atmosphere.GetAdjacentTiles(grid.Value, indices))
				{
					GasMixture mix = this._atmosphere.GetTileMixture(grid, map, indices, true);
					if (mix != null)
					{
						mix.AdjustMoles(component.SupercriticalGas, component.SupercriticalMoleAmount);
						mix.Temperature += component.HotspotExposeTemperature;
						this._atmosphere.HotspotExpose(grid.Value, indices, component.HotspotExposeTemperature, mix.Volume, true);
					}
				}
			}
			this.IgniteNearby(xform.Coordinates, 1f, component.MaximumIgnitionRadius * 2f);
		}

		// Token: 0x06002B66 RID: 11110 RVA: 0x000E3AA8 File Offset: 0x000E1CA8
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			foreach (ValueTuple<PyroclasticAnomalyComponent, AnomalyComponent, TransformComponent> valueTuple in base.EntityQuery<PyroclasticAnomalyComponent, AnomalyComponent, TransformComponent>(false))
			{
				PyroclasticAnomalyComponent pyro = valueTuple.Item1;
				AnomalyComponent anom = valueTuple.Item2;
				TransformComponent xform = valueTuple.Item3;
				EntityUid ent = pyro.Owner;
				EntityUid? grid = xform.GridUid;
				EntityUid? map = xform.MapUid;
				Vector2i indices = this._xform.GetGridOrMapTilePosition(ent, xform);
				GasMixture mixture = this._atmosphere.GetTileMixture(grid, map, indices, true);
				if (mixture != null)
				{
					mixture.Temperature += pyro.HeatPerSecond * anom.Severity * frameTime;
				}
				if (grid != null && anom.Severity > pyro.AnomalyHotspotThreshold)
				{
					this._atmosphere.HotspotExpose(grid.Value, indices, pyro.HotspotExposeTemperature, pyro.HotspotExposeVolume, true);
				}
			}
		}

		// Token: 0x06002B67 RID: 11111 RVA: 0x000E3BA4 File Offset: 0x000E1DA4
		public void IgniteNearby(EntityCoordinates coordinates, float severity, float radius)
		{
			foreach (FlammableComponent flammable in this._lookup.GetComponentsInRange<FlammableComponent>(coordinates, radius))
			{
				EntityUid ent = flammable.Owner;
				if (this._interaction.InRangeUnobstructed(coordinates.ToMap(this.EntityManager), ent, -1f, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null))
				{
					int stackAmount = 1 + (int)(severity / 0.25f);
					this._flammable.AdjustFireStacks(ent, (float)stackAmount, flammable);
					this._flammable.Ignite(ent, flammable);
				}
			}
		}

		// Token: 0x04001AC5 RID: 6853
		[Dependency]
		private readonly AtmosphereSystem _atmosphere;

		// Token: 0x04001AC6 RID: 6854
		[Dependency]
		private readonly EntityLookupSystem _lookup;

		// Token: 0x04001AC7 RID: 6855
		[Dependency]
		private readonly FlammableSystem _flammable;

		// Token: 0x04001AC8 RID: 6856
		[Dependency]
		private readonly InteractionSystem _interaction;

		// Token: 0x04001AC9 RID: 6857
		[Dependency]
		private readonly TransformSystem _xform;
	}
}
