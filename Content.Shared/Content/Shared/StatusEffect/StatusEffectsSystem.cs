using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Content.Shared.Alert;
using Content.Shared.Mobs.Systems;
using Content.Shared.Rejuvenate;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Shared.StatusEffect
{
	// Token: 0x0200015A RID: 346
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class StatusEffectsSystem : EntitySystem
	{
		// Token: 0x06000425 RID: 1061 RVA: 0x00010764 File Offset: 0x0000E964
		public override void Initialize()
		{
			base.Initialize();
			base.UpdatesOutsidePrediction = true;
			base.SubscribeLocalEvent<StatusEffectsComponent, ComponentGetState>(new ComponentEventRefHandler<StatusEffectsComponent, ComponentGetState>(this.OnGetState), null, null);
			base.SubscribeLocalEvent<StatusEffectsComponent, ComponentHandleState>(new ComponentEventRefHandler<StatusEffectsComponent, ComponentHandleState>(this.OnHandleState), null, null);
			base.SubscribeLocalEvent<StatusEffectsComponent, RejuvenateEvent>(new ComponentEventHandler<StatusEffectsComponent, RejuvenateEvent>(this.OnRejuvenate), null, null);
		}

		// Token: 0x06000426 RID: 1062 RVA: 0x000107BC File Offset: 0x0000E9BC
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			TimeSpan curTime = this._gameTiming.CurTime;
			ActiveStatusEffectsComponent activeStatusEffectsComponent;
			StatusEffectsComponent status;
			while (base.EntityQueryEnumerator<ActiveStatusEffectsComponent, StatusEffectsComponent>().MoveNext(ref activeStatusEffectsComponent, ref status))
			{
				foreach (KeyValuePair<string, StatusEffectState> state in Extensions.ToArray<string, StatusEffectState>(status.ActiveEffects))
				{
					if (curTime > state.Value.Cooldown.Item2)
					{
						this.TryRemoveStatusEffect(status.Owner, state.Key, status, true);
					}
				}
			}
		}

		// Token: 0x06000427 RID: 1063 RVA: 0x0001084C File Offset: 0x0000EA4C
		private void OnGetState(EntityUid uid, StatusEffectsComponent component, ref ComponentGetState args)
		{
			args.State = new StatusEffectsComponentState(new Dictionary<string, StatusEffectState>(component.ActiveEffects), new List<string>(component.AllowedEffects));
		}

		// Token: 0x06000428 RID: 1064 RVA: 0x00010870 File Offset: 0x0000EA70
		private void OnHandleState(EntityUid uid, StatusEffectsComponent component, ref ComponentHandleState args)
		{
			StatusEffectsComponentState state = args.Current as StatusEffectsComponentState;
			if (state == null)
			{
				return;
			}
			component.AllowedEffects = new List<string>(state.AllowedEffects);
			foreach (string effect in component.ActiveEffects.Keys)
			{
				if (!state.ActiveEffects.ContainsKey(effect))
				{
					this.TryRemoveStatusEffect(uid, effect, component, false);
				}
			}
			foreach (KeyValuePair<string, StatusEffectState> keyValuePair in state.ActiveEffects)
			{
				string text;
				StatusEffectState statusEffectState;
				keyValuePair.Deconstruct(out text, out statusEffectState);
				string key = text;
				StatusEffectState effect2 = statusEffectState;
				if (component.ActiveEffects.ContainsKey(key))
				{
					component.ActiveEffects[key] = new StatusEffectState(effect2);
				}
				else
				{
					TimeSpan time = effect2.Cooldown.Item2 - effect2.Cooldown.Item1;
					this.TryAddStatusEffect(uid, key, time, true, component, new TimeSpan?(effect2.Cooldown.Item1));
					component.ActiveEffects[key].RelevantComponent = effect2.RelevantComponent;
				}
			}
		}

		// Token: 0x06000429 RID: 1065 RVA: 0x000109CC File Offset: 0x0000EBCC
		private void OnRejuvenate(EntityUid uid, StatusEffectsComponent component, RejuvenateEvent args)
		{
			this.TryRemoveAllStatusEffects(uid, component);
		}

		// Token: 0x0600042A RID: 1066 RVA: 0x000109D8 File Offset: 0x0000EBD8
		public bool TryAddStatusEffect<[Nullable(0)] T>(EntityUid uid, string key, TimeSpan time, bool refresh, [Nullable(2)] StatusEffectsComponent status = null) where T : Component, new()
		{
			if (!base.Resolve<StatusEffectsComponent>(uid, ref status, false))
			{
				return false;
			}
			if (this.TryAddStatusEffect(uid, key, time, refresh, status, null))
			{
				if (!this.EntityManager.HasComponent<T>(uid))
				{
					T comp = this.EntityManager.AddComponent<T>(uid);
					status.ActiveEffects[key].RelevantComponent = this._componentFactory.GetComponentName(comp.GetType());
				}
				return true;
			}
			return false;
		}

		// Token: 0x0600042B RID: 1067 RVA: 0x00010A54 File Offset: 0x0000EC54
		public bool TryAddStatusEffect(EntityUid uid, string key, TimeSpan time, bool refresh, string component, [Nullable(2)] StatusEffectsComponent status = null)
		{
			if (!base.Resolve<StatusEffectsComponent>(uid, ref status, false))
			{
				return false;
			}
			if (this.TryAddStatusEffect(uid, key, time, refresh, status, null))
			{
				if (!this.EntityManager.HasComponent(uid, this._componentFactory.GetRegistration(component, false).Type))
				{
					Component newComponent = (Component)this._componentFactory.GetComponent(component, false);
					newComponent.Owner = uid;
					this.EntityManager.AddComponent<Component>(uid, newComponent, false);
					status.ActiveEffects[key].RelevantComponent = component;
				}
				return true;
			}
			return false;
		}

		// Token: 0x0600042C RID: 1068 RVA: 0x00010AE8 File Offset: 0x0000ECE8
		public bool TryAddStatusEffect(EntityUid uid, string key, TimeSpan time, bool refresh, [Nullable(2)] StatusEffectsComponent status = null, TimeSpan? startTime = null)
		{
			if (!base.Resolve<StatusEffectsComponent>(uid, ref status, false))
			{
				return false;
			}
			if (!this.CanApplyEffect(uid, key, status))
			{
				return false;
			}
			StatusEffectPrototype proto = this._prototypeManager.Index<StatusEffectPrototype>(key);
			TimeSpan timeSpan = startTime ?? this._gameTiming.CurTime;
			ValueTuple<TimeSpan, TimeSpan> cooldown = new ValueTuple<TimeSpan, TimeSpan>(timeSpan, timeSpan + time);
			if (this.HasStatusEffect(uid, key, status))
			{
				status.ActiveEffects[key].CooldownRefresh = refresh;
				if (refresh)
				{
					if (status.ActiveEffects[key].Cooldown.Item2 - this._gameTiming.CurTime < time)
					{
						status.ActiveEffects[key].Cooldown = cooldown;
					}
				}
				else
				{
					StatusEffectState statusEffectState = status.ActiveEffects[key];
					statusEffectState.Cooldown.Item2 = statusEffectState.Cooldown.Item2 + time;
				}
			}
			else
			{
				status.ActiveEffects.Add(key, new StatusEffectState(cooldown, refresh, null));
				base.EnsureComp<ActiveStatusEffectsComponent>(uid);
			}
			if (proto.Alert != null)
			{
				ValueTuple<TimeSpan, TimeSpan>? cooldown2 = this.GetAlertCooldown(uid, proto.Alert.Value, status);
				this._alertsSystem.ShowAlert(uid, proto.Alert.Value, null, cooldown2);
			}
			base.Dirty(status, null);
			base.RaiseLocalEvent<StatusEffectAddedEvent>(uid, new StatusEffectAddedEvent(uid, key), false);
			return true;
		}

		// Token: 0x0600042D RID: 1069 RVA: 0x00010C68 File Offset: 0x0000EE68
		[NullableContext(0)]
		private ValueTuple<TimeSpan, TimeSpan>? GetAlertCooldown(EntityUid uid, AlertType alert, [Nullable(1)] StatusEffectsComponent status)
		{
			ValueTuple<TimeSpan, TimeSpan>? maxCooldown = null;
			foreach (KeyValuePair<string, StatusEffectState> kvp in status.ActiveEffects)
			{
				AlertType? alert2 = this._prototypeManager.Index<StatusEffectPrototype>(kvp.Key).Alert;
				if ((alert2.GetValueOrDefault() == alert & alert2 != null) && (maxCooldown == null || kvp.Value.Cooldown.Item2 > maxCooldown.Value.Item2))
				{
					maxCooldown = new ValueTuple<TimeSpan, TimeSpan>?(kvp.Value.Cooldown);
				}
			}
			return maxCooldown;
		}

		// Token: 0x0600042E RID: 1070 RVA: 0x00010D2C File Offset: 0x0000EF2C
		public bool TryRemoveStatusEffect(EntityUid uid, string key, [Nullable(2)] StatusEffectsComponent status = null, bool remComp = true)
		{
			if (!base.Resolve<StatusEffectsComponent>(uid, ref status, false))
			{
				return false;
			}
			if (!status.ActiveEffects.ContainsKey(key))
			{
				return false;
			}
			StatusEffectPrototype proto;
			if (!this._prototypeManager.TryIndex<StatusEffectPrototype>(key, ref proto))
			{
				return false;
			}
			StatusEffectState state = status.ActiveEffects[key];
			ComponentRegistration registration;
			if (remComp && state.RelevantComponent != null && this._componentFactory.TryGetRegistration(state.RelevantComponent, ref registration, false))
			{
				Type type = registration.Type;
				this.EntityManager.RemoveComponent(uid, type);
			}
			if (proto.Alert != null)
			{
				this._alertsSystem.ClearAlert(uid, proto.Alert.Value);
			}
			status.ActiveEffects.Remove(key);
			if (status.ActiveEffects.Count == 0)
			{
				base.RemComp<ActiveStatusEffectsComponent>(uid);
			}
			base.Dirty(status, null);
			base.RaiseLocalEvent<StatusEffectEndedEvent>(uid, new StatusEffectEndedEvent(uid, key), false);
			return true;
		}

		// Token: 0x0600042F RID: 1071 RVA: 0x00010E14 File Offset: 0x0000F014
		[NullableContext(2)]
		public bool TryRemoveAllStatusEffects(EntityUid uid, StatusEffectsComponent status = null)
		{
			if (!base.Resolve<StatusEffectsComponent>(uid, ref status, false))
			{
				return false;
			}
			bool failed = false;
			foreach (KeyValuePair<string, StatusEffectState> effect in status.ActiveEffects)
			{
				if (!this.TryRemoveStatusEffect(uid, effect.Key, status, true))
				{
					failed = true;
				}
			}
			base.Dirty(status, null);
			return failed;
		}

		// Token: 0x06000430 RID: 1072 RVA: 0x00010E90 File Offset: 0x0000F090
		public bool HasStatusEffect(EntityUid uid, string key, [Nullable(2)] StatusEffectsComponent status = null)
		{
			return base.Resolve<StatusEffectsComponent>(uid, ref status, false) && status.ActiveEffects.ContainsKey(key);
		}

		// Token: 0x06000431 RID: 1073 RVA: 0x00010EB4 File Offset: 0x0000F0B4
		public bool CanApplyEffect(EntityUid uid, string key, [Nullable(2)] StatusEffectsComponent status = null)
		{
			StatusEffectPrototype proto;
			return base.Resolve<StatusEffectsComponent>(uid, ref status, false) && this._prototypeManager.TryIndex<StatusEffectPrototype>(key, ref proto) && (status.AllowedEffects.Contains(key) || proto.AlwaysAllowed);
		}

		// Token: 0x06000432 RID: 1074 RVA: 0x00010EFC File Offset: 0x0000F0FC
		public bool TryAddTime(EntityUid uid, string key, TimeSpan time, [Nullable(2)] StatusEffectsComponent status = null)
		{
			if (!base.Resolve<StatusEffectsComponent>(uid, ref status, false))
			{
				return false;
			}
			if (!this.HasStatusEffect(uid, key, status))
			{
				return false;
			}
			ValueTuple<TimeSpan, TimeSpan> timer = status.ActiveEffects[key].Cooldown;
			timer.Item2 += time;
			status.ActiveEffects[key].Cooldown = timer;
			StatusEffectPrototype proto;
			if (this._prototypeManager.TryIndex<StatusEffectPrototype>(key, ref proto) && proto.Alert != null)
			{
				ValueTuple<TimeSpan, TimeSpan>? cooldown = this.GetAlertCooldown(uid, proto.Alert.Value, status);
				this._alertsSystem.ShowAlert(uid, proto.Alert.Value, null, cooldown);
			}
			base.Dirty(status, null);
			return true;
		}

		// Token: 0x06000433 RID: 1075 RVA: 0x00010FCC File Offset: 0x0000F1CC
		public bool TryRemoveTime(EntityUid uid, string key, TimeSpan time, [Nullable(2)] StatusEffectsComponent status = null)
		{
			if (!base.Resolve<StatusEffectsComponent>(uid, ref status, false))
			{
				return false;
			}
			if (!this.HasStatusEffect(uid, key, status))
			{
				return false;
			}
			ValueTuple<TimeSpan, TimeSpan> timer = status.ActiveEffects[key].Cooldown;
			if (time > timer.Item2)
			{
				return false;
			}
			timer.Item2 -= time;
			status.ActiveEffects[key].Cooldown = timer;
			StatusEffectPrototype proto;
			if (this._prototypeManager.TryIndex<StatusEffectPrototype>(key, ref proto) && proto.Alert != null)
			{
				ValueTuple<TimeSpan, TimeSpan>? cooldown = this.GetAlertCooldown(uid, proto.Alert.Value, status);
				this._alertsSystem.ShowAlert(uid, proto.Alert.Value, null, cooldown);
			}
			base.Dirty(status, null);
			return true;
		}

		// Token: 0x06000434 RID: 1076 RVA: 0x000110AC File Offset: 0x0000F2AC
		public bool TrySetTime(EntityUid uid, string key, TimeSpan time, [Nullable(2)] StatusEffectsComponent status = null)
		{
			if (!base.Resolve<StatusEffectsComponent>(uid, ref status, false))
			{
				return false;
			}
			if (!this.HasStatusEffect(uid, key, status))
			{
				return false;
			}
			status.ActiveEffects[key].Cooldown = new ValueTuple<TimeSpan, TimeSpan>(this._gameTiming.CurTime, this._gameTiming.CurTime + time);
			base.Dirty(status, null);
			return true;
		}

		// Token: 0x06000435 RID: 1077 RVA: 0x00011114 File Offset: 0x0000F314
		[NullableContext(0)]
		public bool TryGetTime(EntityUid uid, [Nullable(1)] string key, [NotNullWhen(true)] out ValueTuple<TimeSpan, TimeSpan>? time, [Nullable(2)] StatusEffectsComponent status = null)
		{
			if (!base.Resolve<StatusEffectsComponent>(uid, ref status, false) || !this.HasStatusEffect(uid, key, status))
			{
				time = null;
				return false;
			}
			time = new ValueTuple<TimeSpan, TimeSpan>?(status.ActiveEffects[key].Cooldown);
			return true;
		}

		// Token: 0x040003FB RID: 1019
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x040003FC RID: 1020
		[Dependency]
		private readonly IComponentFactory _componentFactory;

		// Token: 0x040003FD RID: 1021
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x040003FE RID: 1022
		[Dependency]
		private readonly MobStateSystem _mobStateSystem;

		// Token: 0x040003FF RID: 1023
		[Dependency]
		private readonly AlertsSystem _alertsSystem;
	}
}
