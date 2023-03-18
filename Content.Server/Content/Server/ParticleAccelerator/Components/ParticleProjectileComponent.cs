using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Server.Projectiles;
using Content.Server.Singularity.Components;
using Content.Shared.Projectiles;
using Content.Shared.Singularity.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Timing;

namespace Content.Server.ParticleAccelerator.Components
{
	// Token: 0x020002EB RID: 747
	[RegisterComponent]
	public sealed class ParticleProjectileComponent : Component
	{
		// Token: 0x06000F53 RID: 3923 RVA: 0x0004E850 File Offset: 0x0004CA50
		public void Fire(ParticleAcceleratorPowerState state, Angle angle, EntityUid firer)
		{
			this.State = state;
			PhysicsComponent physicsComponent;
			if (!this._entMan.TryGetComponent<PhysicsComponent>(base.Owner, ref physicsComponent))
			{
				Logger.Error("ParticleProjectile tried firing, but it was spawned without a CollidableComponent");
				return;
			}
			SharedPhysicsSystem physics = this._entMan.System<SharedPhysicsSystem>();
			physics.SetBodyStatus(physicsComponent, 1, true);
			ProjectileComponent projectileComponent;
			if (!this._entMan.TryGetComponent<ProjectileComponent>(base.Owner, ref projectileComponent))
			{
				Logger.Error("ParticleProjectile tried firing, but it was spawned without a ProjectileComponent");
				return;
			}
			this._entMan.EntitySysManager.GetEntitySystem<ProjectileSystem>().SetShooter(projectileComponent, firer);
			SinguloFoodComponent singuloFoodComponent;
			if (!this._entMan.TryGetComponent<SinguloFoodComponent>(base.Owner, ref singuloFoodComponent))
			{
				Logger.Error("ParticleProjectile tried firing, but it was spawned without a SinguloFoodComponent");
				return;
			}
			int num;
			switch (this.State)
			{
			case ParticleAcceleratorPowerState.Standby:
				num = 0;
				break;
			case ParticleAcceleratorPowerState.Level0:
				num = 1;
				break;
			case ParticleAcceleratorPowerState.Level1:
				num = 3;
				break;
			case ParticleAcceleratorPowerState.Level2:
				num = 6;
				break;
			case ParticleAcceleratorPowerState.Level3:
				num = 10;
				break;
			default:
				num = 0;
				break;
			}
			int multiplier = num;
			singuloFoodComponent.Energy = (float)(10 * multiplier);
			AppearanceComponent appearance;
			if (this._entMan.TryGetComponent<AppearanceComponent>(base.Owner, ref appearance))
			{
				appearance.SetData(ParticleAcceleratorVisuals.VisualState, state);
			}
			physics.SetLinearVelocity(base.Owner, angle.ToWorldVec() * 20f, true, true, null, physicsComponent);
			this._entMan.GetComponent<TransformComponent>(base.Owner).LocalRotation = angle;
			Timer.Spawn(3000, delegate()
			{
				this._entMan.DeleteEntity(base.Owner);
			}, default(CancellationToken));
		}

		// Token: 0x040008FE RID: 2302
		[Nullable(1)]
		[Dependency]
		private readonly IEntityManager _entMan;

		// Token: 0x040008FF RID: 2303
		public ParticleAcceleratorPowerState State;
	}
}
