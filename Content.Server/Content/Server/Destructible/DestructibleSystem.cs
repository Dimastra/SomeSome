using System;
using System.Runtime.CompilerServices;
using Content.Server.Body.Systems;
using Content.Server.Chemistry.EntitySystems;
using Content.Server.Construction;
using Content.Server.Destructible.Thresholds;
using Content.Server.Destructible.Thresholds.Behaviors;
using Content.Server.Destructible.Thresholds.Triggers;
using Content.Server.Explosion.EntitySystems;
using Content.Server.Fluids.EntitySystems;
using Content.Server.Stack;
using Content.Shared.Damage;
using Content.Shared.Destructible;
using Content.Shared.FixedPoint;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server.Destructible
{
	// Token: 0x02000596 RID: 1430
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DestructibleSystem : SharedDestructibleSystem
	{
		// Token: 0x17000462 RID: 1122
		// (get) Token: 0x06001DD8 RID: 7640 RVA: 0x0009EB14 File Offset: 0x0009CD14
		public IEntityManager EntityManager
		{
			get
			{
				return this.EntityManager;
			}
		}

		// Token: 0x06001DD9 RID: 7641 RVA: 0x0009EB1C File Offset: 0x0009CD1C
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<DestructibleComponent, DamageChangedEvent>(new ComponentEventHandler<DestructibleComponent, DamageChangedEvent>(this.Execute), null, null);
		}

		// Token: 0x06001DDA RID: 7642 RVA: 0x0009EB38 File Offset: 0x0009CD38
		public void Execute(EntityUid uid, DestructibleComponent component, DamageChangedEvent args)
		{
			foreach (DamageThreshold threshold in component.Thresholds)
			{
				if (threshold.Reached(args.Damageable, this))
				{
					base.RaiseLocalEvent<DamageThresholdReached>(uid, new DamageThresholdReached(component, threshold), true);
					threshold.Execute(uid, this, this.EntityManager, args.Origin);
				}
				if (this.EntityManager.IsQueuedForDeletion(uid) || base.Deleted(uid, null))
				{
					break;
				}
			}
		}

		// Token: 0x06001DDB RID: 7643 RVA: 0x0009EBD0 File Offset: 0x0009CDD0
		[NullableContext(2)]
		public FixedPoint2 DestroyedAt(EntityUid uid, DestructibleComponent destructible = null)
		{
			if (!base.Resolve<DestructibleComponent>(uid, ref destructible, false))
			{
				return FixedPoint2.MaxValue;
			}
			FixedPoint2 damageNeeded = FixedPoint2.MaxValue;
			foreach (DamageThreshold threshold in destructible.Thresholds)
			{
				DamageTrigger trigger = threshold.Trigger as DamageTrigger;
				if (trigger != null)
				{
					foreach (IThresholdBehavior thresholdBehavior in threshold.Behaviors)
					{
						DoActsBehavior actBehavior = thresholdBehavior as DoActsBehavior;
						if (actBehavior != null && actBehavior.HasAct(ThresholdActs.Breakage | ThresholdActs.Destruction))
						{
							damageNeeded = Math.Min(damageNeeded.Float(), (float)trigger.Damage);
						}
					}
				}
			}
			return damageNeeded;
		}

		// Token: 0x0400131F RID: 4895
		[Dependency]
		public readonly IRobustRandom Random;

		// Token: 0x04001320 RID: 4896
		[Dependency]
		public readonly AudioSystem AudioSystem;

		// Token: 0x04001321 RID: 4897
		[Dependency]
		public readonly BodySystem BodySystem;

		// Token: 0x04001322 RID: 4898
		[Dependency]
		public readonly ConstructionSystem ConstructionSystem;

		// Token: 0x04001323 RID: 4899
		[Dependency]
		public readonly ExplosionSystem ExplosionSystem;

		// Token: 0x04001324 RID: 4900
		[Dependency]
		public readonly StackSystem StackSystem;

		// Token: 0x04001325 RID: 4901
		[Dependency]
		public readonly TriggerSystem TriggerSystem;

		// Token: 0x04001326 RID: 4902
		[Dependency]
		public readonly SolutionContainerSystem SolutionContainerSystem;

		// Token: 0x04001327 RID: 4903
		[Dependency]
		public readonly SpillableSystem SpillableSystem;

		// Token: 0x04001328 RID: 4904
		[Dependency]
		public readonly IPrototypeManager PrototypeManager;

		// Token: 0x04001329 RID: 4905
		[Dependency]
		public readonly IComponentFactory ComponentFactory;
	}
}
