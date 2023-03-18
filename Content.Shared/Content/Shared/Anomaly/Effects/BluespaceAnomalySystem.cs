using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.Anomaly.Components;
using Content.Shared.Anomaly.Effects.Components;
using Content.Shared.Mobs.Components;
using Content.Shared.Teleportation.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Random;

namespace Content.Shared.Anomaly.Effects
{
	// Token: 0x02000704 RID: 1796
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class BluespaceAnomalySystem : EntitySystem
	{
		// Token: 0x06001599 RID: 5529 RVA: 0x00046CA4 File Offset: 0x00044EA4
		public override void Initialize()
		{
			base.SubscribeLocalEvent<BluespaceAnomalyComponent, AnomalyPulseEvent>(new ComponentEventRefHandler<BluespaceAnomalyComponent, AnomalyPulseEvent>(this.OnPulse), null, null);
			base.SubscribeLocalEvent<BluespaceAnomalyComponent, AnomalySupercriticalEvent>(new ComponentEventRefHandler<BluespaceAnomalyComponent, AnomalySupercriticalEvent>(this.OnSupercritical), null, null);
			base.SubscribeLocalEvent<BluespaceAnomalyComponent, AnomalySeverityChangedEvent>(new ComponentEventRefHandler<BluespaceAnomalyComponent, AnomalySeverityChangedEvent>(this.OnSeverityChanged), null, null);
		}

		// Token: 0x0600159A RID: 5530 RVA: 0x00046CE4 File Offset: 0x00044EE4
		private void OnPulse(EntityUid uid, BluespaceAnomalyComponent component, ref AnomalyPulseEvent args)
		{
			TransformComponent xform = base.Transform(uid);
			float range = component.MaxShuffleRadius * args.Severity;
			List<EntityUid> allEnts = (from x in this._lookup.GetComponentsInRange<MobStateComponent>(xform.Coordinates, range)
			select x.Owner).ToList<EntityUid>();
			allEnts.Add(uid);
			EntityQuery<TransformComponent> xformQuery = base.GetEntityQuery<TransformComponent>();
			List<Vector2> coords = new List<Vector2>();
			foreach (EntityUid ent in allEnts)
			{
				TransformComponent xf;
				if (xformQuery.TryGetComponent(ent, ref xf))
				{
					coords.Add(xf.MapPosition.Position);
				}
			}
			this._random.Shuffle<Vector2>(coords);
			for (int i = 0; i < allEnts.Count; i++)
			{
				this._xform.SetWorldPosition(allEnts[i], coords[i], xformQuery);
			}
		}

		// Token: 0x0600159B RID: 5531 RVA: 0x00046DF4 File Offset: 0x00044FF4
		private void OnSupercritical(EntityUid uid, BluespaceAnomalyComponent component, ref AnomalySupercriticalEvent args)
		{
			TransformComponent xform = base.Transform(uid);
			Vector2 mapPos = this._xform.GetWorldPosition(xform);
			float radius = component.SupercriticalTeleportRadius;
			Box2 gridBounds;
			gridBounds..ctor(mapPos - new ValueTuple<float, float>(radius, radius), mapPos + new ValueTuple<float, float>(radius, radius));
			foreach (MobStateComponent mobStateComponent in this._lookup.GetComponentsInRange<MobStateComponent>(xform.Coordinates, component.MaxShuffleRadius))
			{
				EntityUid ent = mobStateComponent.Owner;
				float randomX = this._random.NextFloat(gridBounds.Left, gridBounds.Right);
				float randomY = this._random.NextFloat(gridBounds.Bottom, gridBounds.Top);
				Vector2 pos;
				pos..ctor(randomX, randomY);
				this._xform.SetWorldPosition(ent, pos);
				this._audio.PlayPvs(component.TeleportSound, ent, null);
			}
		}

		// Token: 0x0600159C RID: 5532 RVA: 0x00046F0C File Offset: 0x0004510C
		private void OnSeverityChanged(EntityUid uid, BluespaceAnomalyComponent component, ref AnomalySeverityChangedEvent args)
		{
			PortalComponent portal;
			if (!base.TryComp<PortalComponent>(uid, ref portal))
			{
				return;
			}
			portal.MaxRandomRadius = (component.MaxPortalRadius - component.MinPortalRadius) * args.Severity + component.MinPortalRadius;
		}

		// Token: 0x040015D8 RID: 5592
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x040015D9 RID: 5593
		[Dependency]
		private readonly SharedAudioSystem _audio;

		// Token: 0x040015DA RID: 5594
		[Dependency]
		private readonly EntityLookupSystem _lookup;

		// Token: 0x040015DB RID: 5595
		[Dependency]
		private readonly SharedTransformSystem _xform;
	}
}
