using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.Alert;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Content.Shared.Mobs.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;

namespace Content.Shared.Mobs.Systems
{
	// Token: 0x02000301 RID: 769
	[NullableContext(2)]
	[Nullable(0)]
	public sealed class MobThresholdSystem : EntitySystem
	{
		// Token: 0x060008C2 RID: 2242 RVA: 0x0001D920 File Offset: 0x0001BB20
		public override void Initialize()
		{
			base.SubscribeLocalEvent<MobThresholdsComponent, ComponentShutdown>(new ComponentEventHandler<MobThresholdsComponent, ComponentShutdown>(this.MobThresholdShutdown), null, null);
			base.SubscribeLocalEvent<MobThresholdsComponent, ComponentStartup>(new ComponentEventHandler<MobThresholdsComponent, ComponentStartup>(this.MobThresholdStartup), null, null);
			base.SubscribeLocalEvent<MobThresholdsComponent, DamageChangedEvent>(new ComponentEventHandler<MobThresholdsComponent, DamageChangedEvent>(this.OnDamaged), null, null);
			base.SubscribeLocalEvent<MobThresholdsComponent, ComponentGetState>(new ComponentEventRefHandler<MobThresholdsComponent, ComponentGetState>(this.OnGetComponentState), null, null);
			base.SubscribeLocalEvent<MobThresholdsComponent, ComponentHandleState>(new ComponentEventRefHandler<MobThresholdsComponent, ComponentHandleState>(this.OnHandleComponentState), null, null);
			base.SubscribeLocalEvent<MobThresholdsComponent, UpdateMobStateEvent>(new ComponentEventRefHandler<MobThresholdsComponent, UpdateMobStateEvent>(this.OnUpdateMobState), null, null);
		}

		// Token: 0x060008C3 RID: 2243 RVA: 0x0001D9A8 File Offset: 0x0001BBA8
		public FixedPoint2 GetThresholdForState(EntityUid target, MobState mobState, MobThresholdsComponent thresholdComponent = null)
		{
			if (!base.Resolve<MobThresholdsComponent>(target, ref thresholdComponent, true))
			{
				return FixedPoint2.Zero;
			}
			foreach (KeyValuePair<FixedPoint2, MobState> pair in thresholdComponent.Thresholds)
			{
				if (pair.Value == mobState)
				{
					return pair.Key;
				}
			}
			return FixedPoint2.Zero;
		}

		// Token: 0x060008C4 RID: 2244 RVA: 0x0001DA24 File Offset: 0x0001BC24
		public bool TryGetThresholdForState(EntityUid target, MobState mobState, [NotNullWhen(true)] out FixedPoint2? threshold, MobThresholdsComponent thresholdComponent = null)
		{
			threshold = null;
			if (!base.Resolve<MobThresholdsComponent>(target, ref thresholdComponent, true))
			{
				return false;
			}
			foreach (KeyValuePair<FixedPoint2, MobState> pair in thresholdComponent.Thresholds)
			{
				if (pair.Value == mobState)
				{
					threshold = new FixedPoint2?(pair.Key);
					return true;
				}
			}
			return false;
		}

		// Token: 0x060008C5 RID: 2245 RVA: 0x0001DAAC File Offset: 0x0001BCAC
		public bool TryGetPercentageForState(EntityUid target, MobState mobState, FixedPoint2 damage, [NotNullWhen(true)] out FixedPoint2? percentage, MobThresholdsComponent thresholdComponent = null)
		{
			percentage = null;
			FixedPoint2? threshold;
			if (!this.TryGetThresholdForState(target, mobState, out threshold, thresholdComponent))
			{
				return false;
			}
			percentage = damage / threshold;
			return true;
		}

		// Token: 0x060008C6 RID: 2246 RVA: 0x0001DB03 File Offset: 0x0001BD03
		public bool TryGetIncapThreshold(EntityUid target, [NotNullWhen(true)] out FixedPoint2? threshold, MobThresholdsComponent thresholdComponent = null)
		{
			threshold = null;
			return base.Resolve<MobThresholdsComponent>(target, ref thresholdComponent, true) && (this.TryGetThresholdForState(target, MobState.Critical, out threshold, thresholdComponent) || this.TryGetThresholdForState(target, MobState.Dead, out threshold, thresholdComponent));
		}

		// Token: 0x060008C7 RID: 2247 RVA: 0x0001DB34 File Offset: 0x0001BD34
		public bool TryGetIncapPercentage(EntityUid target, FixedPoint2 damage, [NotNullWhen(true)] out FixedPoint2? percentage, MobThresholdsComponent thresholdComponent = null)
		{
			percentage = null;
			FixedPoint2? threshold;
			if (!this.TryGetIncapThreshold(target, out threshold, thresholdComponent))
			{
				return false;
			}
			if (damage == 0)
			{
				percentage = new FixedPoint2?(0);
				return true;
			}
			percentage = new FixedPoint2?(FixedPoint2.Min(1f, damage / threshold.Value));
			return true;
		}

		// Token: 0x060008C8 RID: 2248 RVA: 0x0001DB9B File Offset: 0x0001BD9B
		public bool TryGetDeadThreshold(EntityUid target, [NotNullWhen(true)] out FixedPoint2? threshold, MobThresholdsComponent thresholdComponent = null)
		{
			threshold = null;
			return base.Resolve<MobThresholdsComponent>(target, ref thresholdComponent, true) && this.TryGetThresholdForState(target, MobState.Dead, out threshold, thresholdComponent);
		}

		// Token: 0x060008C9 RID: 2249 RVA: 0x0001DBBC File Offset: 0x0001BDBC
		public bool TryGetDeadPercentage(EntityUid target, FixedPoint2 damage, [NotNullWhen(true)] out FixedPoint2? percentage, MobThresholdsComponent thresholdComponent = null)
		{
			percentage = null;
			FixedPoint2? threshold;
			if (!this.TryGetDeadThreshold(target, out threshold, thresholdComponent))
			{
				return false;
			}
			if (damage == 0)
			{
				percentage = new FixedPoint2?(0);
				return true;
			}
			percentage = new FixedPoint2?(FixedPoint2.Min(1f, damage / threshold.Value));
			return true;
		}

		// Token: 0x060008CA RID: 2250 RVA: 0x0001DC24 File Offset: 0x0001BE24
		public bool GetScaledDamage(EntityUid target1, EntityUid target2, out DamageSpecifier damage)
		{
			damage = null;
			DamageableComponent oldDamage;
			if (!base.TryComp<DamageableComponent>(target1, ref oldDamage))
			{
				return false;
			}
			MobThresholdsComponent threshold;
			MobThresholdsComponent threshold2;
			if (!base.TryComp<MobThresholdsComponent>(target1, ref threshold) || !base.TryComp<MobThresholdsComponent>(target2, ref threshold2))
			{
				return false;
			}
			FixedPoint2? ent1DeadThreshold;
			if (!this.TryGetThresholdForState(target1, MobState.Dead, out ent1DeadThreshold, threshold))
			{
				ent1DeadThreshold = new FixedPoint2?(0);
			}
			FixedPoint2? ent2DeadThreshold;
			if (!this.TryGetThresholdForState(target2, MobState.Dead, out ent2DeadThreshold, threshold2))
			{
				ent2DeadThreshold = new FixedPoint2?(0);
			}
			damage = oldDamage.Damage / ent1DeadThreshold.Value * ent2DeadThreshold.Value;
			return true;
		}

		// Token: 0x060008CB RID: 2251 RVA: 0x0001DCAE File Offset: 0x0001BEAE
		public void SetMobStateThreshold(EntityUid target, FixedPoint2 damage, MobState mobState, MobThresholdsComponent threshold = null)
		{
			if (!base.Resolve<MobThresholdsComponent>(target, ref threshold, true))
			{
				return;
			}
			threshold.Thresholds[damage] = mobState;
			this.VerifyThresholds(target, threshold, null, null);
		}

		// Token: 0x060008CC RID: 2252 RVA: 0x0001DCD8 File Offset: 0x0001BED8
		public void VerifyThresholds(EntityUid target, MobThresholdsComponent threshold = null, MobStateComponent mobState = null, DamageableComponent damageable = null)
		{
			if (!base.Resolve<MobStateComponent, MobThresholdsComponent, DamageableComponent>(target, ref mobState, ref threshold, ref damageable, true))
			{
				return;
			}
			this.CheckThresholds(target, mobState, threshold, damageable);
			MobThresholdChecked ev = new MobThresholdChecked(target, mobState, threshold, damageable);
			base.RaiseLocalEvent<MobThresholdChecked>(target, ref ev, true);
			this.UpdateAlerts(target, mobState.CurrentState, threshold, damageable);
		}

		// Token: 0x060008CD RID: 2253 RVA: 0x0001DD28 File Offset: 0x0001BF28
		[NullableContext(1)]
		private void CheckThresholds(EntityUid target, MobStateComponent mobStateComponent, MobThresholdsComponent thresholdsComponent, DamageableComponent damageableComponent)
		{
			foreach (KeyValuePair<FixedPoint2, MobState> keyValuePair in thresholdsComponent.Thresholds.Reverse<KeyValuePair<FixedPoint2, MobState>>())
			{
				FixedPoint2 fixedPoint;
				MobState mobState2;
				keyValuePair.Deconstruct(out fixedPoint, out mobState2);
				FixedPoint2 threshold = fixedPoint;
				MobState mobState = mobState2;
				if (!(damageableComponent.TotalDamage < threshold))
				{
					this.TriggerThreshold(target, mobState, mobStateComponent, thresholdsComponent);
					break;
				}
			}
		}

		// Token: 0x060008CE RID: 2254 RVA: 0x0001DDA0 File Offset: 0x0001BFA0
		private void TriggerThreshold(EntityUid target, MobState newState, MobStateComponent mobState = null, MobThresholdsComponent thresholds = null)
		{
			if (!base.Resolve<MobStateComponent, MobThresholdsComponent>(target, ref mobState, ref thresholds, true) || mobState.CurrentState == newState)
			{
				return;
			}
			thresholds.CurrentThresholdState = newState;
			this._mobStateSystem.UpdateMobState(target, mobState, null);
			base.Dirty(target, null);
		}

		// Token: 0x060008CF RID: 2255 RVA: 0x0001DDEC File Offset: 0x0001BFEC
		private void UpdateAlerts(EntityUid target, MobState currentMobState, MobThresholdsComponent threshold = null, DamageableComponent damageable = null)
		{
			if (!base.Resolve<MobThresholdsComponent, DamageableComponent>(target, ref threshold, ref damageable, true))
			{
				return;
			}
			if (!threshold.TriggersAlerts)
			{
				return;
			}
			switch (currentMobState)
			{
			case MobState.Alive:
			{
				short severity = this._alerts.GetMinSeverity(AlertType.HumanHealth);
				FixedPoint2? percentage;
				if (this.TryGetIncapPercentage(target, damageable.TotalDamage, out percentage, null))
				{
					severity = (short)MathF.Floor(percentage.Value.Float() * (float)this._alerts.GetSeverityRange(AlertType.HumanHealth));
					severity += this._alerts.GetMinSeverity(AlertType.HumanHealth);
				}
				this._alerts.ShowAlert(target, AlertType.HumanHealth, new short?(severity), null);
				return;
			}
			case MobState.Critical:
				this._alerts.ShowAlert(target, AlertType.HumanCrit, null, null);
				return;
			case MobState.Dead:
				this._alerts.ShowAlert(target, AlertType.HumanDead, null, null);
				return;
			}
			throw new ArgumentOutOfRangeException("currentMobState", currentMobState, null);
		}

		// Token: 0x060008D0 RID: 2256 RVA: 0x0001DEF8 File Offset: 0x0001C0F8
		[NullableContext(1)]
		private void OnDamaged(EntityUid target, MobThresholdsComponent thresholds, DamageChangedEvent args)
		{
			MobStateComponent mobState;
			if (!base.TryComp<MobStateComponent>(target, ref mobState))
			{
				return;
			}
			this.CheckThresholds(target, mobState, thresholds, args.Damageable);
			MobThresholdChecked ev = new MobThresholdChecked(target, mobState, thresholds, args.Damageable);
			base.RaiseLocalEvent<MobThresholdChecked>(target, ref ev, true);
			this.UpdateAlerts(target, mobState.CurrentState, thresholds, args.Damageable);
		}

		// Token: 0x060008D1 RID: 2257 RVA: 0x0001DF50 File Offset: 0x0001C150
		[NullableContext(1)]
		private void OnHandleComponentState(EntityUid target, MobThresholdsComponent component, ref ComponentHandleState args)
		{
			MobThresholdComponentState state = args.Current as MobThresholdComponentState;
			if (state == null)
			{
				return;
			}
			component.Thresholds = new SortedDictionary<FixedPoint2, MobState>(state.Thresholds);
			component.CurrentThresholdState = state.CurrentThresholdState;
		}

		// Token: 0x060008D2 RID: 2258 RVA: 0x0001DF8A File Offset: 0x0001C18A
		[NullableContext(1)]
		private void OnGetComponentState(EntityUid target, MobThresholdsComponent component, ref ComponentGetState args)
		{
			args.State = new MobThresholdComponentState(component.CurrentThresholdState, new Dictionary<FixedPoint2, MobState>(component.Thresholds));
		}

		// Token: 0x060008D3 RID: 2259 RVA: 0x0001DFA8 File Offset: 0x0001C1A8
		[NullableContext(1)]
		private void MobThresholdStartup(EntityUid target, MobThresholdsComponent thresholds, ComponentStartup args)
		{
			MobStateComponent mobState;
			DamageableComponent damageable;
			if (!base.TryComp<MobStateComponent>(target, ref mobState) || !base.TryComp<DamageableComponent>(target, ref damageable))
			{
				return;
			}
			this.CheckThresholds(target, mobState, thresholds, damageable);
			MobThresholdChecked ev = new MobThresholdChecked(target, mobState, thresholds, damageable);
			base.RaiseLocalEvent<MobThresholdChecked>(target, ref ev, true);
			this.UpdateAlerts(target, mobState.CurrentState, thresholds, damageable);
		}

		// Token: 0x060008D4 RID: 2260 RVA: 0x0001DFFA File Offset: 0x0001C1FA
		[NullableContext(1)]
		private void MobThresholdShutdown(EntityUid target, MobThresholdsComponent component, ComponentShutdown args)
		{
			if (component.TriggersAlerts)
			{
				this._alerts.ClearAlertCategory(target, AlertCategory.Health);
			}
		}

		// Token: 0x060008D5 RID: 2261 RVA: 0x0001E011 File Offset: 0x0001C211
		[NullableContext(1)]
		private void OnUpdateMobState(EntityUid target, MobThresholdsComponent component, ref UpdateMobStateEvent args)
		{
			if (component.CurrentThresholdState != MobState.Invalid)
			{
				args.State = component.CurrentThresholdState;
			}
		}

		// Token: 0x040008C7 RID: 2247
		[Nullable(1)]
		[Dependency]
		private readonly MobStateSystem _mobStateSystem;

		// Token: 0x040008C8 RID: 2248
		[Nullable(1)]
		[Dependency]
		private readonly AlertsSystem _alerts;
	}
}
