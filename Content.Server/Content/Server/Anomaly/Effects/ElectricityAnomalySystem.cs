using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Electrocution;
using Content.Server.Lightning;
using Content.Server.Power.Components;
using Content.Shared.Anomaly.Components;
using Content.Shared.Anomaly.Effects.Components;
using Content.Shared.Mobs.Components;
using Content.Shared.StatusEffect;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Server.Anomaly.Effects
{
	// Token: 0x020007C4 RID: 1988
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ElectricityAnomalySystem : EntitySystem
	{
		// Token: 0x06002B54 RID: 11092 RVA: 0x000E31E9 File Offset: 0x000E13E9
		public override void Initialize()
		{
			base.SubscribeLocalEvent<ElectricityAnomalyComponent, AnomalyPulseEvent>(new ComponentEventRefHandler<ElectricityAnomalyComponent, AnomalyPulseEvent>(this.OnPulse), null, null);
			base.SubscribeLocalEvent<ElectricityAnomalyComponent, AnomalySupercriticalEvent>(new ComponentEventRefHandler<ElectricityAnomalyComponent, AnomalySupercriticalEvent>(this.OnSupercritical), null, null);
		}

		// Token: 0x06002B55 RID: 11093 RVA: 0x000E3214 File Offset: 0x000E1414
		private void OnPulse(EntityUid uid, ElectricityAnomalyComponent component, ref AnomalyPulseEvent args)
		{
			float range = component.MaxElectrocuteRange * args.Stability;
			TransformComponent xform = base.Transform(uid);
			foreach (MobStateComponent mobStateComponent in this._lookup.GetComponentsInRange<MobStateComponent>(xform.MapPosition, range))
			{
				EntityUid ent = mobStateComponent.Owner;
				this._lightning.ShootLightning(uid, ent, "Lightning");
			}
		}

		// Token: 0x06002B56 RID: 11094 RVA: 0x000E329C File Offset: 0x000E149C
		private void OnSupercritical(EntityUid uid, ElectricityAnomalyComponent component, ref AnomalySupercriticalEvent args)
		{
			EntityQuery<ApcPowerReceiverComponent> poweredQuery = base.GetEntityQuery<ApcPowerReceiverComponent>();
			EntityQuery<MobThresholdsComponent> mobQuery = base.GetEntityQuery<MobThresholdsComponent>();
			HashSet<EntityUid> validEnts = new HashSet<EntityUid>();
			foreach (EntityUid ent in this._lookup.GetEntitiesInRange(uid, component.MaxElectrocuteRange * 2f, 46))
			{
				if (mobQuery.HasComponent(ent))
				{
					validEnts.Add(ent);
				}
				if (RandomExtensions.Prob(this._random, 0.2f) && poweredQuery.HasComponent(ent))
				{
					validEnts.Add(ent);
				}
			}
			foreach (EntityUid ent2 in validEnts)
			{
				this._lightning.ShootLightning(uid, ent2, "Lightning");
			}
		}

		// Token: 0x06002B57 RID: 11095 RVA: 0x000E3398 File Offset: 0x000E1598
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			foreach (ValueTuple<ElectricityAnomalyComponent, AnomalyComponent, TransformComponent> valueTuple in base.EntityQuery<ElectricityAnomalyComponent, AnomalyComponent, TransformComponent>(false))
			{
				ElectricityAnomalyComponent elec = valueTuple.Item1;
				AnomalyComponent anom = valueTuple.Item2;
				TransformComponent xform = valueTuple.Item3;
				if (!(this._timing.CurTime < elec.NextSecond))
				{
					elec.NextSecond = this._timing.CurTime + TimeSpan.FromSeconds(1.0);
					EntityUid owner = xform.Owner;
					if (RandomExtensions.Prob(this._random, elec.PassiveElectrocutionChance * anom.Stability))
					{
						float range = elec.MaxElectrocuteRange * anom.Stability;
						int damage = (int)(elec.MaxElectrocuteDamage * anom.Severity);
						TimeSpan duration = elec.MaxElectrocuteDuration * (double)anom.Severity;
						foreach (StatusEffectsComponent comp in this._lookup.GetComponentsInRange<StatusEffectsComponent>(xform.MapPosition, range))
						{
							EntityUid ent = comp.Owner;
							this._electrocution.TryDoElectrocution(ent, new EntityUid?(owner), damage, duration, true, 1f, comp, true);
						}
					}
				}
			}
		}

		// Token: 0x04001ABC RID: 6844
		[Dependency]
		private readonly IGameTiming _timing;

		// Token: 0x04001ABD RID: 6845
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x04001ABE RID: 6846
		[Dependency]
		private readonly LightningSystem _lightning;

		// Token: 0x04001ABF RID: 6847
		[Dependency]
		private readonly ElectrocutionSystem _electrocution;

		// Token: 0x04001AC0 RID: 6848
		[Dependency]
		private readonly EntityLookupSystem _lookup;
	}
}
