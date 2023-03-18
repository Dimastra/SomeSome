using System;
using System.Runtime.CompilerServices;
using Content.Server.Damage.Components;
using Content.Server.Stunnable;
using Content.Shared.Audio;
using Content.Shared.Damage;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Events;
using Robust.Shared.Player;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Server.Damage.Systems
{
	// Token: 0x020005C0 RID: 1472
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class DamageOnHighSpeedImpactSystem : EntitySystem
	{
		// Token: 0x06001F74 RID: 8052 RVA: 0x000A51D7 File Offset: 0x000A33D7
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<DamageOnHighSpeedImpactComponent, StartCollideEvent>(new ComponentEventRefHandler<DamageOnHighSpeedImpactComponent, StartCollideEvent>(this.HandleCollide), null, null);
		}

		// Token: 0x06001F75 RID: 8053 RVA: 0x000A51F4 File Offset: 0x000A33F4
		private void HandleCollide(EntityUid uid, DamageOnHighSpeedImpactComponent component, ref StartCollideEvent args)
		{
			if (!this.EntityManager.HasComponent<DamageableComponent>(uid))
			{
				return;
			}
			EntityUid otherBody = args.OtherFixture.Body.Owner;
			float speed = args.OurFixture.Body.LinearVelocity.Length;
			if (speed < component.MinimumSpeed)
			{
				return;
			}
			SoundSystem.Play(component.SoundHit.GetSound(null, null), Filter.Pvs(otherBody, 2f, null, null, null), otherBody, new AudioParams?(AudioHelpers.WithVariation(0.125f).WithVolume(-0.125f)));
			if ((this._gameTiming.CurTime - component.LastHit).TotalSeconds < (double)component.DamageCooldown)
			{
				return;
			}
			component.LastHit = this._gameTiming.CurTime;
			if (RandomExtensions.Prob(this._robustRandom, component.StunChance))
			{
				this._stunSystem.TryStun(uid, TimeSpan.FromSeconds((double)component.StunSeconds), true, null);
			}
			float damageScale = speed / component.MinimumSpeed * component.Factor;
			this._damageableSystem.TryChangeDamage(new EntityUid?(uid), component.Damage * damageScale, false, true, null, null);
		}

		// Token: 0x04001389 RID: 5001
		[Dependency]
		private readonly IRobustRandom _robustRandom;

		// Token: 0x0400138A RID: 5002
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x0400138B RID: 5003
		[Dependency]
		private readonly DamageableSystem _damageableSystem;

		// Token: 0x0400138C RID: 5004
		[Dependency]
		private readonly StunSystem _stunSystem;
	}
}
