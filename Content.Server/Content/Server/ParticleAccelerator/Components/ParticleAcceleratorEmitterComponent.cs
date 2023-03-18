using System;
using System.Runtime.CompilerServices;
using Content.Shared.Singularity.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.ParticleAccelerator.Components
{
	// Token: 0x020002E5 RID: 741
	[RegisterComponent]
	[ComponentReference(typeof(ParticleAcceleratorPartComponent))]
	public sealed class ParticleAcceleratorEmitterComponent : ParticleAcceleratorPartComponent
	{
		// Token: 0x06000F46 RID: 3910 RVA: 0x0004E710 File Offset: 0x0004C910
		public void Fire(ParticleAcceleratorPowerState strength)
		{
			IEntityManager entities = IoCManager.Resolve<IEntityManager>();
			EntityUid projectile = entities.SpawnEntity("ParticlesProjectile", entities.GetComponent<TransformComponent>(base.Owner).Coordinates);
			ParticleProjectileComponent particleProjectileComponent;
			if (!entities.TryGetComponent<ParticleProjectileComponent>(projectile, ref particleProjectileComponent))
			{
				Logger.Error("ParticleAcceleratorEmitter tried firing particles, but they was spawned without a ParticleProjectileComponent");
				return;
			}
			particleProjectileComponent.Fire(strength, entities.GetComponent<TransformComponent>(base.Owner).WorldRotation, base.Owner);
		}

		// Token: 0x06000F47 RID: 3911 RVA: 0x0004E774 File Offset: 0x0004C974
		[NullableContext(1)]
		public override string ToString()
		{
			string str = base.ToString();
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(13, 1);
			defaultInterpolatedStringHandler.AppendLiteral(" EmitterType:");
			defaultInterpolatedStringHandler.AppendFormatted<ParticleAcceleratorEmitterType>(this.Type);
			return str + defaultInterpolatedStringHandler.ToStringAndClear();
		}

		// Token: 0x040008F7 RID: 2295
		[DataField("emitterType", false, 1, false, false, null)]
		public ParticleAcceleratorEmitterType Type = ParticleAcceleratorEmitterType.Center;
	}
}
