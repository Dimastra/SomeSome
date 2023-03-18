using System;
using System.Runtime.CompilerServices;
using Content.Server.Physics.Components;
using Content.Shared.Throwing;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Controllers;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Server.Physics.Controllers
{
	// Token: 0x020002DC RID: 732
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class RandomWalkController : VirtualController
	{
		// Token: 0x06000EEE RID: 3822 RVA: 0x0004C834 File Offset: 0x0004AA34
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<RandomWalkComponent, ComponentStartup>(new ComponentEventHandler<RandomWalkComponent, ComponentStartup>(this.OnRandomWalkStartup), null, null);
		}

		// Token: 0x06000EEF RID: 3823 RVA: 0x0004C850 File Offset: 0x0004AA50
		public override void UpdateBeforeSolve(bool prediction, float frameTime)
		{
			base.UpdateBeforeSolve(prediction, frameTime);
			foreach (ValueTuple<RandomWalkComponent, PhysicsComponent> valueTuple in this.EntityManager.EntityQuery<RandomWalkComponent, PhysicsComponent>(false))
			{
				RandomWalkComponent randomWalk = valueTuple.Item1;
				PhysicsComponent physics = valueTuple.Item2;
				if (!this.EntityManager.HasComponent<ActorComponent>(randomWalk.Owner) && !this.EntityManager.HasComponent<ThrownItemComponent>(randomWalk.Owner))
				{
					TimeSpan curTime = this._timing.CurTime;
					if (randomWalk.NextStepTime <= curTime)
					{
						this.Update(randomWalk.Owner, randomWalk, physics);
					}
				}
			}
		}

		// Token: 0x06000EF0 RID: 3824 RVA: 0x0004C900 File Offset: 0x0004AB00
		[NullableContext(2)]
		public void Update(EntityUid uid, RandomWalkComponent randomWalk = null, PhysicsComponent physics = null)
		{
			if (!base.Resolve<RandomWalkComponent>(uid, ref randomWalk, true))
			{
				return;
			}
			TimeSpan curTime = this._timing.CurTime;
			randomWalk.NextStepTime = curTime + TimeSpan.FromSeconds(this._random.NextDouble(randomWalk.MinStepCooldown.TotalSeconds, randomWalk.MaxStepCooldown.TotalSeconds));
			if (!base.Resolve<PhysicsComponent>(randomWalk.Owner, ref physics, true))
			{
				return;
			}
			Angle pushAngle = this._random.NextAngle();
			float pushStrength = this._random.NextFloat(randomWalk.MinSpeed, randomWalk.MaxSpeed);
			this._physics.SetLinearVelocity(uid, physics.LinearVelocity * randomWalk.AccumulatorRatio, true, true, null, physics);
			this._physics.ApplyLinearImpulse(uid, pushAngle.ToVec() * (pushStrength * physics.Mass), null, physics);
		}

		// Token: 0x06000EF1 RID: 3825 RVA: 0x0004C9D8 File Offset: 0x0004ABD8
		private void OnRandomWalkStartup(EntityUid uid, RandomWalkComponent comp, ComponentStartup args)
		{
			if (comp.StepOnStartup)
			{
				this.Update(uid, comp, null);
				return;
			}
			comp.NextStepTime = this._timing.CurTime + TimeSpan.FromSeconds(this._random.NextDouble(comp.MinStepCooldown.TotalSeconds, comp.MaxStepCooldown.TotalSeconds));
		}

		// Token: 0x040008C4 RID: 2244
		[Dependency]
		private readonly IGameTiming _timing;

		// Token: 0x040008C5 RID: 2245
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x040008C6 RID: 2246
		[Dependency]
		private readonly PhysicsSystem _physics;
	}
}
