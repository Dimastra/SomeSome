using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Damage.Components;
using Content.Shared.FixedPoint;
using Content.Shared.Movement.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Damage
{
	// Token: 0x02000537 RID: 1335
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SlowOnDamageSystem : EntitySystem
	{
		// Token: 0x0600103F RID: 4159 RVA: 0x00034E20 File Offset: 0x00033020
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<SlowOnDamageComponent, DamageChangedEvent>(new ComponentEventHandler<SlowOnDamageComponent, DamageChangedEvent>(this.OnDamageChanged), null, null);
			base.SubscribeLocalEvent<SlowOnDamageComponent, RefreshMovementSpeedModifiersEvent>(new ComponentEventHandler<SlowOnDamageComponent, RefreshMovementSpeedModifiersEvent>(this.OnRefreshMovespeed), null, null);
		}

		// Token: 0x06001040 RID: 4160 RVA: 0x00034E50 File Offset: 0x00033050
		private void OnRefreshMovespeed(EntityUid uid, SlowOnDamageComponent component, RefreshMovementSpeedModifiersEvent args)
		{
			DamageableComponent damage;
			if (!this.EntityManager.TryGetComponent<DamageableComponent>(uid, ref damage))
			{
				return;
			}
			if (damage.TotalDamage == FixedPoint2.Zero)
			{
				return;
			}
			FixedPoint2 closest = FixedPoint2.Zero;
			FixedPoint2 total = damage.TotalDamage;
			foreach (KeyValuePair<FixedPoint2, float> thres in component.SpeedModifierThresholds)
			{
				if (total >= thres.Key && thres.Key > closest)
				{
					closest = thres.Key;
				}
			}
			if (closest != FixedPoint2.Zero)
			{
				float speed = component.SpeedModifierThresholds[closest];
				args.ModifySpeed(speed, speed);
			}
		}

		// Token: 0x06001041 RID: 4161 RVA: 0x00034F1C File Offset: 0x0003311C
		private void OnDamageChanged(EntityUid uid, SlowOnDamageComponent component, DamageChangedEvent args)
		{
			this._movementSpeedModifierSystem.RefreshMovementSpeedModifiers(uid, null);
		}

		// Token: 0x04000F4F RID: 3919
		[Dependency]
		private readonly MovementSpeedModifierSystem _movementSpeedModifierSystem;
	}
}
