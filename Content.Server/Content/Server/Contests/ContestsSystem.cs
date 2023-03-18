using System;
using System.Runtime.CompilerServices;
using Content.Shared.Damage;
using Content.Shared.Damage.Components;
using Content.Shared.FixedPoint;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Components;

namespace Content.Server.Contests
{
	// Token: 0x020005E7 RID: 1511
	[NullableContext(2)]
	[Nullable(0)]
	public sealed class ContestsSystem : EntitySystem
	{
		// Token: 0x0600203E RID: 8254 RVA: 0x000A8088 File Offset: 0x000A6288
		public float MassContest(EntityUid roller, EntityUid target, PhysicsComponent rollerPhysics = null, PhysicsComponent targetPhysics = null)
		{
			if (!base.Resolve<PhysicsComponent>(roller, ref rollerPhysics, false) || !base.Resolve<PhysicsComponent>(target, ref targetPhysics, false))
			{
				return 1f;
			}
			if (targetPhysics.FixturesMass == 0f)
			{
				return 1f;
			}
			return rollerPhysics.FixturesMass / targetPhysics.FixturesMass;
		}

		// Token: 0x0600203F RID: 8255 RVA: 0x000A80D8 File Offset: 0x000A62D8
		public float DamageContest(EntityUid roller, EntityUid target, DamageableComponent rollerDamage = null, DamageableComponent targetDamage = null)
		{
			if (!base.Resolve<DamageableComponent>(roller, ref rollerDamage, false) || !base.Resolve<DamageableComponent>(target, ref targetDamage, false))
			{
				return 1f;
			}
			float rollerThreshold = 100f;
			FixedPoint2? rollerCritThreshold;
			if (!this._mobThresholdSystem.TryGetThresholdForState(roller, MobState.Critical, out rollerCritThreshold, null))
			{
				FixedPoint2? rollerdeadThreshold;
				if (this._mobThresholdSystem.TryGetThresholdForState(roller, MobState.Critical, out rollerdeadThreshold, null))
				{
					rollerThreshold = rollerdeadThreshold.Value.Float();
				}
			}
			else
			{
				rollerThreshold = rollerCritThreshold.Value.Float();
			}
			float targetThreshold = 100f;
			FixedPoint2? targetCritThreshold;
			if (!this._mobThresholdSystem.TryGetThresholdForState(roller, MobState.Critical, out targetCritThreshold, null))
			{
				FixedPoint2? targetdeadThreshold;
				if (this._mobThresholdSystem.TryGetThresholdForState(roller, MobState.Critical, out targetdeadThreshold, null))
				{
					targetThreshold = targetdeadThreshold.Value.Float();
				}
			}
			else
			{
				targetThreshold = targetCritThreshold.Value.Float();
			}
			float rollerDamageScore = (float)rollerDamage.TotalDamage / rollerThreshold;
			float targetDamageScore = (float)targetDamage.TotalDamage / targetThreshold;
			return this.DamageThresholdConverter(rollerDamageScore) / this.DamageThresholdConverter(targetDamageScore);
		}

		// Token: 0x06002040 RID: 8256 RVA: 0x000A81D4 File Offset: 0x000A63D4
		public float StaminaContest(EntityUid roller, EntityUid target, StaminaComponent rollerStamina = null, StaminaComponent targetStamina = null)
		{
			if (!base.Resolve<StaminaComponent>(roller, ref rollerStamina, false) || !base.Resolve<StaminaComponent>(target, ref targetStamina, false))
			{
				return 1f;
			}
			float rollerDamageScore = rollerStamina.StaminaDamage / rollerStamina.CritThreshold;
			float targetDamageScore = targetStamina.StaminaDamage / targetStamina.CritThreshold;
			return this.DamageThresholdConverter(rollerDamageScore) / this.DamageThresholdConverter(targetDamageScore);
		}

		// Token: 0x06002041 RID: 8257 RVA: 0x000A822C File Offset: 0x000A642C
		public float OverallStrengthContest(EntityUid roller, EntityUid target, float damageWeight = 1f, float massWeight = 1f, float stamWeight = 1f)
		{
			float weightTotal = damageWeight + massWeight + stamWeight;
			float damageMultiplier = damageWeight / weightTotal;
			float massMultiplier = massWeight / weightTotal;
			float stamMultiplier = stamWeight / weightTotal;
			return this.DamageContest(roller, target, null, null) * damageMultiplier + this.MassContest(roller, target, null, null) * massMultiplier + this.StaminaContest(roller, target, null, null) * stamMultiplier;
		}

		// Token: 0x06002042 RID: 8258 RVA: 0x000A8278 File Offset: 0x000A6478
		public float DamageThresholdConverter(float score)
		{
			float result;
			if (score > 0f)
			{
				if (score > 0.25f)
				{
					if (score > 0.5f)
					{
						if (score > 0.75f)
						{
							if (score > 0.95f)
							{
								result = 0.05f;
							}
							else
							{
								result = 0.45f;
							}
						}
						else
						{
							result = 0.6f;
						}
					}
					else
					{
						result = 0.75f;
					}
				}
				else
				{
					result = 0.9f;
				}
			}
			else
			{
				result = 1f;
			}
			return result;
		}

		// Token: 0x040013FD RID: 5117
		[Nullable(1)]
		[Dependency]
		private readonly MobStateSystem _mobStateSystem;

		// Token: 0x040013FE RID: 5118
		[Nullable(1)]
		[Dependency]
		private readonly MobThresholdSystem _mobThresholdSystem;
	}
}
