using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Nutrition.Components;
using Content.Shared.Alert;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.Rejuvenate;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Random;

namespace Content.Server.Nutrition.EntitySystems
{
	// Token: 0x02000314 RID: 788
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ThirstSystem : EntitySystem
	{
		// Token: 0x0600104A RID: 4170 RVA: 0x000544DC File Offset: 0x000526DC
		public override void Initialize()
		{
			base.Initialize();
			this._sawmill = Logger.GetSawmill("thirst");
			base.SubscribeLocalEvent<ThirstComponent, RefreshMovementSpeedModifiersEvent>(new ComponentEventHandler<ThirstComponent, RefreshMovementSpeedModifiersEvent>(this.OnRefreshMovespeed), null, null);
			base.SubscribeLocalEvent<ThirstComponent, ComponentStartup>(new ComponentEventHandler<ThirstComponent, ComponentStartup>(this.OnComponentStartup), null, null);
			base.SubscribeLocalEvent<ThirstComponent, RejuvenateEvent>(new ComponentEventHandler<ThirstComponent, RejuvenateEvent>(this.OnRejuvenate), null, null);
		}

		// Token: 0x0600104B RID: 4171 RVA: 0x0005453C File Offset: 0x0005273C
		private void OnComponentStartup(EntityUid uid, ThirstComponent component, ComponentStartup args)
		{
			if (component.CurrentThirst < 0f)
			{
				component.CurrentThirst = (float)this._random.Next((int)component.ThirstThresholds[ThirstThreshold.Thirsty] + 10, (int)component.ThirstThresholds[ThirstThreshold.Okay] - 1);
			}
			component.CurrentThirstThreshold = this.GetThirstThreshold(component, component.CurrentThirst);
			component.LastThirstThreshold = ThirstThreshold.Okay;
			this.UpdateEffects(component);
		}

		// Token: 0x0600104C RID: 4172 RVA: 0x000545A8 File Offset: 0x000527A8
		private void OnRefreshMovespeed(EntityUid uid, ThirstComponent component, RefreshMovementSpeedModifiersEvent args)
		{
			if (this._jetpack.IsUserFlying(component.Owner))
			{
				return;
			}
			float mod = (component.CurrentThirstThreshold <= ThirstThreshold.Parched) ? 0.75f : 1f;
			args.ModifySpeed(mod, mod);
		}

		// Token: 0x0600104D RID: 4173 RVA: 0x000545E7 File Offset: 0x000527E7
		private void OnRejuvenate(EntityUid uid, ThirstComponent component, RejuvenateEvent args)
		{
			this.ResetThirst(component);
		}

		// Token: 0x0600104E RID: 4174 RVA: 0x000545F0 File Offset: 0x000527F0
		private ThirstThreshold GetThirstThreshold(ThirstComponent component, float amount)
		{
			ThirstThreshold result = ThirstThreshold.Dead;
			float value = component.ThirstThresholds[ThirstThreshold.OverHydrated];
			foreach (KeyValuePair<ThirstThreshold, float> threshold in component.ThirstThresholds)
			{
				if (threshold.Value <= value && threshold.Value >= amount)
				{
					result = threshold.Key;
					value = threshold.Value;
				}
			}
			return result;
		}

		// Token: 0x0600104F RID: 4175 RVA: 0x00054670 File Offset: 0x00052870
		public void UpdateThirst(ThirstComponent component, float amount)
		{
			component.CurrentThirst = Math.Clamp(component.CurrentThirst + amount, component.ThirstThresholds[ThirstThreshold.Dead], component.ThirstThresholds[ThirstThreshold.OverHydrated]);
		}

		// Token: 0x06001050 RID: 4176 RVA: 0x0005469D File Offset: 0x0005289D
		public void ResetThirst(ThirstComponent component)
		{
			component.CurrentThirst = component.ThirstThresholds[ThirstThreshold.Okay];
		}

		// Token: 0x06001051 RID: 4177 RVA: 0x000546B4 File Offset: 0x000528B4
		private bool IsMovementThreshold(ThirstThreshold threshold)
		{
			switch (threshold)
			{
			case ThirstThreshold.Dead:
			case ThirstThreshold.Parched:
				return true;
			case ThirstThreshold.Thirsty:
			case ThirstThreshold.Okay:
			case ThirstThreshold.OverHydrated:
				return false;
			}
			throw new ArgumentOutOfRangeException("threshold", threshold, null);
		}

		// Token: 0x06001052 RID: 4178 RVA: 0x00054704 File Offset: 0x00052904
		private void UpdateEffects(ThirstComponent component)
		{
			MovementSpeedModifierComponent movementSlowdownComponent;
			if (this.IsMovementThreshold(component.LastThirstThreshold) != this.IsMovementThreshold(component.CurrentThirstThreshold) && base.TryComp<MovementSpeedModifierComponent>(component.Owner, ref movementSlowdownComponent))
			{
				this._movement.RefreshMovementSpeedModifiers(component.Owner, movementSlowdownComponent);
			}
			AlertType alertId;
			if (ThirstComponent.ThirstThresholdAlertTypes.TryGetValue(component.CurrentThirstThreshold, out alertId))
			{
				this._alerts.ShowAlert(component.Owner, alertId, null, null);
			}
			else
			{
				this._alerts.ClearAlertCategory(component.Owner, AlertCategory.Thirst);
			}
			switch (component.CurrentThirstThreshold)
			{
			case ThirstThreshold.Dead:
				return;
			case ThirstThreshold.Parched:
				this._movement.RefreshMovementSpeedModifiers(component.Owner, null);
				component.LastThirstThreshold = component.CurrentThirstThreshold;
				component.ActualDecayRate = component.BaseDecayRate * 0.6f;
				return;
			case ThirstThreshold.Thirsty:
				component.LastThirstThreshold = component.CurrentThirstThreshold;
				component.ActualDecayRate = component.BaseDecayRate * 0.8f;
				return;
			case ThirstThreshold.Okay:
				component.LastThirstThreshold = component.CurrentThirstThreshold;
				component.ActualDecayRate = component.BaseDecayRate;
				return;
			case ThirstThreshold.OverHydrated:
				component.LastThirstThreshold = component.CurrentThirstThreshold;
				component.ActualDecayRate = component.BaseDecayRate * 1.2f;
				return;
			}
			ISawmill sawmill = this._sawmill;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(30, 1);
			defaultInterpolatedStringHandler.AppendLiteral("No thirst threshold found for ");
			defaultInterpolatedStringHandler.AppendFormatted<ThirstThreshold>(component.CurrentThirstThreshold);
			sawmill.Error(defaultInterpolatedStringHandler.ToStringAndClear());
			defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(30, 1);
			defaultInterpolatedStringHandler.AppendLiteral("No thirst threshold found for ");
			defaultInterpolatedStringHandler.AppendFormatted<ThirstThreshold>(component.CurrentThirstThreshold);
			throw new ArgumentOutOfRangeException(defaultInterpolatedStringHandler.ToStringAndClear());
		}

		// Token: 0x06001053 RID: 4179 RVA: 0x000548C0 File Offset: 0x00052AC0
		public override void Update(float frameTime)
		{
			this._accumulatedFrameTime += frameTime;
			if (this._accumulatedFrameTime > 1f)
			{
				foreach (ThirstComponent component in this.EntityManager.EntityQuery<ThirstComponent>(false))
				{
					this.UpdateThirst(component, -component.ActualDecayRate);
					ThirstThreshold calculatedThirstThreshold = this.GetThirstThreshold(component, component.CurrentThirst);
					if (calculatedThirstThreshold != component.CurrentThirstThreshold)
					{
						component.CurrentThirstThreshold = calculatedThirstThreshold;
						this.UpdateEffects(component);
					}
				}
				this._accumulatedFrameTime -= 1f;
			}
		}

		// Token: 0x04000970 RID: 2416
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x04000971 RID: 2417
		[Dependency]
		private readonly AlertsSystem _alerts;

		// Token: 0x04000972 RID: 2418
		[Dependency]
		private readonly MovementSpeedModifierSystem _movement;

		// Token: 0x04000973 RID: 2419
		[Dependency]
		private readonly SharedJetpackSystem _jetpack;

		// Token: 0x04000974 RID: 2420
		private ISawmill _sawmill;

		// Token: 0x04000975 RID: 2421
		private float _accumulatedFrameTime;
	}
}
