using System;
using System.Runtime.CompilerServices;
using Content.Server.Stunnable.Components;
using Content.Shared.Standing;
using Content.Shared.StatusEffect;
using Content.Shared.Throwing;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Events;

namespace Content.Server.Stunnable
{
	// Token: 0x0200014B RID: 331
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class StunOnCollideSystem : EntitySystem
	{
		// Token: 0x06000646 RID: 1606 RVA: 0x0001E41D File Offset: 0x0001C61D
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<StunOnCollideComponent, StartCollideEvent>(new ComponentEventRefHandler<StunOnCollideComponent, StartCollideEvent>(this.HandleCollide), null, null);
			base.SubscribeLocalEvent<StunOnCollideComponent, ThrowDoHitEvent>(new ComponentEventHandler<StunOnCollideComponent, ThrowDoHitEvent>(this.HandleThrow), null, null);
		}

		// Token: 0x06000647 RID: 1607 RVA: 0x0001E450 File Offset: 0x0001C650
		private void TryDoCollideStun(EntityUid uid, StunOnCollideComponent component, EntityUid target)
		{
			StatusEffectsComponent status;
			if (this.EntityManager.TryGetComponent<StatusEffectsComponent>(target, ref status))
			{
				StandingStateComponent standingState = null;
				AppearanceComponent appearance = null;
				base.Resolve<StandingStateComponent, AppearanceComponent>(target, ref standingState, ref appearance, false);
				this._stunSystem.TryStun(target, TimeSpan.FromSeconds((double)component.StunAmount), true, status);
				this._stunSystem.TryKnockdown(target, TimeSpan.FromSeconds((double)component.KnockdownAmount), true, status);
				this._stunSystem.TrySlowdown(target, TimeSpan.FromSeconds((double)component.SlowdownAmount), true, component.WalkSpeedMultiplier, component.RunSpeedMultiplier, status);
			}
		}

		// Token: 0x06000648 RID: 1608 RVA: 0x0001E4DB File Offset: 0x0001C6DB
		private void HandleCollide(EntityUid uid, StunOnCollideComponent component, ref StartCollideEvent args)
		{
			if (args.OurFixture.ID != component.FixtureID)
			{
				return;
			}
			this.TryDoCollideStun(uid, component, args.OtherFixture.Body.Owner);
		}

		// Token: 0x06000649 RID: 1609 RVA: 0x0001E50E File Offset: 0x0001C70E
		private void HandleThrow(EntityUid uid, StunOnCollideComponent component, ThrowDoHitEvent args)
		{
			this.TryDoCollideStun(uid, component, args.Target);
		}

		// Token: 0x040003A9 RID: 937
		[Dependency]
		private readonly StunSystem _stunSystem;
	}
}
