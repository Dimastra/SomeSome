using System;
using System.Runtime.CompilerServices;
using Content.Shared.Administration.Logs;
using Content.Shared.Anomaly.Components;
using Content.Shared.Damage;
using Content.Shared.Database;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Network;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Shared.Anomaly
{
	// Token: 0x02000703 RID: 1795
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedAnomalySystem : EntitySystem
	{
		// Token: 0x06001583 RID: 5507 RVA: 0x00046398 File Offset: 0x00044598
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<AnomalyComponent, ComponentGetState>(new ComponentEventRefHandler<AnomalyComponent, ComponentGetState>(this.OnAnomalyGetState), null, null);
			base.SubscribeLocalEvent<AnomalyComponent, ComponentHandleState>(new ComponentEventRefHandler<AnomalyComponent, ComponentHandleState>(this.OnAnomalyHandleState), null, null);
			base.SubscribeLocalEvent<AnomalySupercriticalComponent, ComponentGetState>(new ComponentEventRefHandler<AnomalySupercriticalComponent, ComponentGetState>(this.OnSupercriticalGetState), null, null);
			base.SubscribeLocalEvent<AnomalySupercriticalComponent, ComponentHandleState>(new ComponentEventRefHandler<AnomalySupercriticalComponent, ComponentHandleState>(this.OnSupercriticalHandleState), null, null);
			base.SubscribeLocalEvent<AnomalyComponent, InteractHandEvent>(new ComponentEventHandler<AnomalyComponent, InteractHandEvent>(this.OnInteractHand), null, null);
			base.SubscribeLocalEvent<AnomalyComponent, AttackedEvent>(new ComponentEventHandler<AnomalyComponent, AttackedEvent>(this.OnAttacked), null, null);
			base.SubscribeLocalEvent<AnomalyComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<AnomalyComponent, EntityUnpausedEvent>(this.OnAnomalyUnpause), null, null);
			base.SubscribeLocalEvent<AnomalyPulsingComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<AnomalyPulsingComponent, EntityUnpausedEvent>(this.OnPulsingUnpause), null, null);
			base.SubscribeLocalEvent<AnomalySupercriticalComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<AnomalySupercriticalComponent, EntityUnpausedEvent>(this.OnSupercriticalUnpause), null, null);
		}

		// Token: 0x06001584 RID: 5508 RVA: 0x0004645F File Offset: 0x0004465F
		private void OnAnomalyGetState(EntityUid uid, AnomalyComponent component, ref ComponentGetState args)
		{
			args.State = new AnomalyComponentState(component.Severity, component.Stability, component.Health, component.NextPulseTime);
		}

		// Token: 0x06001585 RID: 5509 RVA: 0x00046484 File Offset: 0x00044684
		private void OnAnomalyHandleState(EntityUid uid, AnomalyComponent component, ref ComponentHandleState args)
		{
			AnomalyComponentState state = args.Current as AnomalyComponentState;
			if (state == null)
			{
				return;
			}
			component.Severity = state.Severity;
			component.Stability = state.Stability;
			component.Health = state.Health;
			component.NextPulseTime = state.NextPulseTime;
		}

		// Token: 0x06001586 RID: 5510 RVA: 0x000464D1 File Offset: 0x000446D1
		private void OnSupercriticalGetState(EntityUid uid, AnomalySupercriticalComponent component, ref ComponentGetState args)
		{
			args.State = new AnomalySupercriticalComponentState
			{
				EndTime = component.EndTime,
				Duration = component.SupercriticalDuration
			};
		}

		// Token: 0x06001587 RID: 5511 RVA: 0x000464F8 File Offset: 0x000446F8
		private void OnSupercriticalHandleState(EntityUid uid, AnomalySupercriticalComponent component, ref ComponentHandleState args)
		{
			AnomalySupercriticalComponentState state = args.Current as AnomalySupercriticalComponentState;
			if (state == null)
			{
				return;
			}
			component.EndTime = state.EndTime;
			component.SupercriticalDuration = state.Duration;
		}

		// Token: 0x06001588 RID: 5512 RVA: 0x0004652D File Offset: 0x0004472D
		private void OnInteractHand(EntityUid uid, AnomalyComponent component, InteractHandEvent args)
		{
			this.DoAnomalyBurnDamage(uid, args.User, component);
			args.Handled = true;
		}

		// Token: 0x06001589 RID: 5513 RVA: 0x00046544 File Offset: 0x00044744
		private void OnAttacked(EntityUid uid, AnomalyComponent component, AttackedEvent args)
		{
			this.DoAnomalyBurnDamage(uid, args.User, component);
		}

		// Token: 0x0600158A RID: 5514 RVA: 0x00046554 File Offset: 0x00044754
		public void DoAnomalyBurnDamage(EntityUid source, EntityUid target, AnomalyComponent component)
		{
			this._damageable.TryChangeDamage(new EntityUid?(target), component.AnomalyContactDamage, true, true, null, null);
			if (!this.Timing.IsFirstTimePredicted || this._net.IsServer)
			{
				return;
			}
			this.Audio.PlayPvs(component.AnomalyContactDamageSound, source, null);
			this.Popup.PopupEntity(Loc.GetString("anomaly-component-contact-damage"), target, target, PopupType.Small);
		}

		// Token: 0x0600158B RID: 5515 RVA: 0x000465D4 File Offset: 0x000447D4
		private void OnAnomalyUnpause(EntityUid uid, AnomalyComponent component, ref EntityUnpausedEvent args)
		{
			component.NextPulseTime += args.PausedTime;
			base.Dirty(component, null);
		}

		// Token: 0x0600158C RID: 5516 RVA: 0x000465F5 File Offset: 0x000447F5
		private void OnPulsingUnpause(EntityUid uid, AnomalyPulsingComponent component, ref EntityUnpausedEvent args)
		{
			component.EndTime += args.PausedTime;
		}

		// Token: 0x0600158D RID: 5517 RVA: 0x0004660E File Offset: 0x0004480E
		private void OnSupercriticalUnpause(EntityUid uid, AnomalySupercriticalComponent component, ref EntityUnpausedEvent args)
		{
			component.EndTime += args.PausedTime;
			base.Dirty(component, null);
		}

		// Token: 0x0600158E RID: 5518 RVA: 0x00046630 File Offset: 0x00044830
		[NullableContext(2)]
		public void DoAnomalyPulse(EntityUid uid, AnomalyComponent component = null)
		{
			if (!base.Resolve<AnomalyComponent>(uid, ref component, true))
			{
				return;
			}
			if (!this.Timing.IsFirstTimePredicted)
			{
				return;
			}
			float variation = this.Random.NextFloat(-component.PulseVariation, component.PulseVariation) + 1f;
			component.NextPulseTime = this.Timing.CurTime + this.GetPulseLength(component) * (double)variation;
			if (component.Stability > component.GrowthThreshold)
			{
				this.ChangeAnomalySeverity(uid, this.GetSeverityIncreaseFromGrowth(component), component);
			}
			float stability = this.Random.NextFloat(-component.PulseStabilityVariation, component.PulseStabilityVariation);
			this.ChangeAnomalyStability(uid, stability, component);
			ISharedAdminLogManager log = this.Log;
			LogType type = LogType.Anomaly;
			LogImpact impact = LogImpact.Medium;
			LogStringHandler logStringHandler = new LogStringHandler(31, 2);
			logStringHandler.AppendLiteral("Anomaly ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "ToPrettyString(uid)");
			logStringHandler.AppendLiteral(" pulsed with severity ");
			logStringHandler.AppendFormatted<float>(component.Severity, "component.Severity");
			logStringHandler.AppendLiteral(".");
			log.Add(type, impact, ref logStringHandler);
			if (this._net.IsServer)
			{
				this.Audio.PlayPvs(component.PulseSound, uid, null);
			}
			AnomalyPulsingComponent pulse = base.EnsureComp<AnomalyPulsingComponent>(uid);
			pulse.EndTime = this.Timing.CurTime + pulse.PulseDuration;
			this.Appearance.SetData(uid, AnomalyVisuals.IsPulsing, true, null);
			AnomalyPulseEvent ev = new AnomalyPulseEvent(component.Stability, component.Severity);
			base.RaiseLocalEvent<AnomalyPulseEvent>(uid, ref ev, false);
		}

		// Token: 0x0600158F RID: 5519 RVA: 0x000467C4 File Offset: 0x000449C4
		public void StartSupercriticalEvent(EntityUid uid)
		{
			if (base.HasComp<AnomalySupercriticalComponent>(uid))
			{
				return;
			}
			ISharedAdminLogManager log = this.Log;
			LogType type = LogType.Anomaly;
			LogImpact impact = LogImpact.High;
			LogStringHandler logStringHandler = new LogStringHandler(35, 1);
			logStringHandler.AppendLiteral("Anomaly ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "ToPrettyString(uid)");
			logStringHandler.AppendLiteral(" began to go supercritical.");
			log.Add(type, impact, ref logStringHandler);
			AnomalySupercriticalComponent super = base.EnsureComp<AnomalySupercriticalComponent>(uid);
			super.EndTime = this.Timing.CurTime + super.SupercriticalDuration;
			this.Appearance.SetData(uid, AnomalyVisuals.Supercritical, true, null);
			base.Dirty(super, null);
		}

		// Token: 0x06001590 RID: 5520 RVA: 0x00046868 File Offset: 0x00044A68
		[NullableContext(2)]
		public void DoAnomalySupercriticalEvent(EntityUid uid, AnomalyComponent component = null)
		{
			if (!base.Resolve<AnomalyComponent>(uid, ref component, true))
			{
				return;
			}
			if (!this.Timing.IsFirstTimePredicted)
			{
				return;
			}
			this.Audio.PlayPvs(component.SupercriticalSound, uid, null);
			AnomalySupercriticalEvent ev = default(AnomalySupercriticalEvent);
			base.RaiseLocalEvent<AnomalySupercriticalEvent>(uid, ref ev, false);
			this.EndAnomaly(uid, component, true);
		}

		// Token: 0x06001591 RID: 5521 RVA: 0x000468C8 File Offset: 0x00044AC8
		[NullableContext(2)]
		public void EndAnomaly(EntityUid uid, AnomalyComponent component = null, bool supercritical = false)
		{
			if (!base.Resolve<AnomalyComponent>(uid, ref component, true))
			{
				return;
			}
			AnomalyShutdownEvent ev = new AnomalyShutdownEvent(uid, supercritical);
			base.RaiseLocalEvent<AnomalyShutdownEvent>(uid, ref ev, true);
			ISharedAdminLogManager log = this.Log;
			LogType type = LogType.Anomaly;
			LogImpact impact = LogImpact.Extreme;
			LogStringHandler logStringHandler = new LogStringHandler(28, 1);
			logStringHandler.AppendLiteral("Anomaly ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "ToPrettyString(uid)");
			logStringHandler.AppendLiteral(" went supercritical.");
			log.Add(type, impact, ref logStringHandler);
			if (base.Terminating(uid, null) || this._net.IsClient)
			{
				return;
			}
			base.Del(uid);
		}

		// Token: 0x06001592 RID: 5522 RVA: 0x0004695C File Offset: 0x00044B5C
		[NullableContext(2)]
		public void ChangeAnomalyStability(EntityUid uid, float change, AnomalyComponent component = null)
		{
			if (!base.Resolve<AnomalyComponent>(uid, ref component, true))
			{
				return;
			}
			float newVal = component.Stability + change;
			component.Stability = Math.Clamp(newVal, 0f, 1f);
			base.Dirty(component, null);
			AnomalyStabilityChangedEvent ev = new AnomalyStabilityChangedEvent(uid, component.Stability);
			base.RaiseLocalEvent<AnomalyStabilityChangedEvent>(uid, ref ev, true);
		}

		// Token: 0x06001593 RID: 5523 RVA: 0x000469B8 File Offset: 0x00044BB8
		[NullableContext(2)]
		public void ChangeAnomalySeverity(EntityUid uid, float change, AnomalyComponent component = null)
		{
			if (!base.Resolve<AnomalyComponent>(uid, ref component, true))
			{
				return;
			}
			float newVal = component.Severity + change;
			if (newVal >= 1f)
			{
				this.StartSupercriticalEvent(uid);
			}
			component.Severity = Math.Clamp(newVal, 0f, 1f);
			base.Dirty(component, null);
			AnomalySeverityChangedEvent ev = new AnomalySeverityChangedEvent(uid, component.Severity);
			base.RaiseLocalEvent<AnomalySeverityChangedEvent>(uid, ref ev, true);
		}

		// Token: 0x06001594 RID: 5524 RVA: 0x00046A20 File Offset: 0x00044C20
		[NullableContext(2)]
		public void ChangeAnomalyHealth(EntityUid uid, float change, AnomalyComponent component = null)
		{
			if (!base.Resolve<AnomalyComponent>(uid, ref component, true))
			{
				return;
			}
			float newVal = component.Health + change;
			if (newVal < 0f)
			{
				this.EndAnomaly(uid, component, false);
				return;
			}
			component.Health = Math.Clamp(newVal, 0f, 1f);
			base.Dirty(component, null);
			AnomalyHealthChangedEvent ev = new AnomalyHealthChangedEvent(uid, component.Health);
			base.RaiseLocalEvent<AnomalyHealthChangedEvent>(uid, ref ev, true);
		}

		// Token: 0x06001595 RID: 5525 RVA: 0x00046A8C File Offset: 0x00044C8C
		public TimeSpan GetPulseLength(AnomalyComponent component)
		{
			float modifier = Math.Clamp((component.Stability - component.GrowthThreshold) / component.GrowthThreshold, 0f, 1f);
			return (component.MaxPulseLength - component.MinPulseLength) * (double)modifier + component.MinPulseLength;
		}

		// Token: 0x06001596 RID: 5526 RVA: 0x00046AE0 File Offset: 0x00044CE0
		private float GetSeverityIncreaseFromGrowth(AnomalyComponent component)
		{
			return (1f + Math.Max(component.Stability - component.GrowthThreshold, 0f) * 10f) * component.SeverityGrowthCoefficient;
		}

		// Token: 0x06001597 RID: 5527 RVA: 0x00046B0C File Offset: 0x00044D0C
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			foreach (AnomalyComponent anomaly in base.EntityQuery<AnomalyComponent>(false))
			{
				EntityUid ent = anomaly.Owner;
				if (anomaly.Stability < anomaly.DecayThreshold)
				{
					this.ChangeAnomalyHealth(ent, anomaly.HealthChangePerSecond * frameTime, anomaly);
				}
				if (this.Timing.CurTime > anomaly.NextPulseTime)
				{
					this.DoAnomalyPulse(ent, anomaly);
				}
			}
			foreach (AnomalyPulsingComponent pulse in base.EntityQuery<AnomalyPulsingComponent>(false))
			{
				EntityUid ent2 = pulse.Owner;
				if (this.Timing.CurTime > pulse.EndTime)
				{
					this.Appearance.SetData(ent2, AnomalyVisuals.IsPulsing, false, null);
					base.RemComp(ent2, pulse);
				}
			}
			foreach (ValueTuple<AnomalySupercriticalComponent, AnomalyComponent> valueTuple in base.EntityQuery<AnomalySupercriticalComponent, AnomalyComponent>(false))
			{
				AnomalySupercriticalComponent super = valueTuple.Item1;
				AnomalyComponent anom = valueTuple.Item2;
				EntityUid ent3 = anom.Owner;
				if (!(this.Timing.CurTime <= super.EndTime))
				{
					this.DoAnomalySupercriticalEvent(ent3, anom);
					base.RemComp(ent3, super);
				}
			}
		}

		// Token: 0x040015D0 RID: 5584
		[Dependency]
		protected readonly IGameTiming Timing;

		// Token: 0x040015D1 RID: 5585
		[Dependency]
		private readonly INetManager _net;

		// Token: 0x040015D2 RID: 5586
		[Dependency]
		protected readonly IRobustRandom Random;

		// Token: 0x040015D3 RID: 5587
		[Dependency]
		protected readonly ISharedAdminLogManager Log;

		// Token: 0x040015D4 RID: 5588
		[Dependency]
		private readonly DamageableSystem _damageable;

		// Token: 0x040015D5 RID: 5589
		[Dependency]
		protected readonly SharedAudioSystem Audio;

		// Token: 0x040015D6 RID: 5590
		[Dependency]
		protected readonly SharedAppearanceSystem Appearance;

		// Token: 0x040015D7 RID: 5591
		[Dependency]
		protected readonly SharedPopupSystem Popup;
	}
}
