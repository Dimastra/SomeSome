using System;
using System.Runtime.CompilerServices;
using Content.Server.Actions;
using Content.Server.Popups;
using Content.Server.Sound.Components;
using Content.Shared.Actions.ActionTypes;
using Content.Shared.Audio;
using Content.Shared.Bed.Sleep;
using Content.Shared.Damage;
using Content.Shared.Examine;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Popups;
using Content.Shared.Slippery;
using Content.Shared.Stunnable;
using Content.Shared.Verbs;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Server.Bed.Sleep
{
	// Token: 0x02000726 RID: 1830
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SleepingSystem : SharedSleepingSystem
	{
		// Token: 0x0600266E RID: 9838 RVA: 0x000CB4B4 File Offset: 0x000C96B4
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<MobStateComponent, SleepStateChangedEvent>(new ComponentEventHandler<MobStateComponent, SleepStateChangedEvent>(this.OnSleepStateChanged), null, null);
			base.SubscribeLocalEvent<SleepingComponent, DamageChangedEvent>(new ComponentEventHandler<SleepingComponent, DamageChangedEvent>(this.OnDamageChanged), null, null);
			base.SubscribeLocalEvent<MobStateComponent, SleepActionEvent>(new ComponentEventHandler<MobStateComponent, SleepActionEvent>(this.OnSleepAction), null, null);
			base.SubscribeLocalEvent<MobStateComponent, WakeActionEvent>(new ComponentEventHandler<MobStateComponent, WakeActionEvent>(this.OnWakeAction), null, null);
			base.SubscribeLocalEvent<SleepingComponent, MobStateChangedEvent>(new ComponentEventHandler<SleepingComponent, MobStateChangedEvent>(this.OnMobStateChanged), null, null);
			base.SubscribeLocalEvent<SleepingComponent, GetVerbsEvent<AlternativeVerb>>(new ComponentEventHandler<SleepingComponent, GetVerbsEvent<AlternativeVerb>>(this.AddWakeVerb), null, null);
			base.SubscribeLocalEvent<SleepingComponent, InteractHandEvent>(new ComponentEventHandler<SleepingComponent, InteractHandEvent>(this.OnInteractHand), null, null);
			base.SubscribeLocalEvent<SleepingComponent, ExaminedEvent>(new ComponentEventHandler<SleepingComponent, ExaminedEvent>(this.OnExamined), null, null);
			base.SubscribeLocalEvent<SleepingComponent, SlipAttemptEvent>(new ComponentEventHandler<SleepingComponent, SlipAttemptEvent>(this.OnSlip), null, null);
			base.SubscribeLocalEvent<ForcedSleepingComponent, ComponentInit>(new ComponentEventHandler<ForcedSleepingComponent, ComponentInit>(this.OnInit), null, null);
		}

		// Token: 0x0600266F RID: 9839 RVA: 0x000CB590 File Offset: 0x000C9790
		private void OnSleepStateChanged(EntityUid uid, MobStateComponent component, SleepStateChangedEvent args)
		{
			InstantActionPrototype wakeAction;
			this._prototypeManager.TryIndex<InstantActionPrototype>("Wake", ref wakeAction);
			if (args.FellAsleep)
			{
				base.EnsureComp<StunnedComponent>(uid);
				base.EnsureComp<KnockedDownComponent>(uid);
				SpamEmitSoundComponent spamEmitSoundComponent = base.EnsureComp<SpamEmitSoundComponent>(uid);
				spamEmitSoundComponent.Sound = new SoundCollectionSpecifier("Snores", new AudioParams?(AudioParams.Default.WithVariation(new float?(0.2f))));
				spamEmitSoundComponent.PlayChance = 0.33f;
				spamEmitSoundComponent.RollInterval = 5f;
				spamEmitSoundComponent.PopUp = "sleep-onomatopoeia";
				if (wakeAction != null)
				{
					InstantAction wakeInstance = new InstantAction(wakeAction);
					wakeInstance.Cooldown = new ValueTuple<TimeSpan, TimeSpan>?(new ValueTuple<TimeSpan, TimeSpan>(this._gameTiming.CurTime, this._gameTiming.CurTime + TimeSpan.FromSeconds(15.0)));
					this._actionsSystem.AddAction(uid, wakeInstance, null, null, true);
				}
				return;
			}
			if (wakeAction != null)
			{
				this._actionsSystem.RemoveAction(uid, wakeAction, null);
			}
			base.RemComp<StunnedComponent>(uid);
			base.RemComp<KnockedDownComponent>(uid);
			base.RemComp<SpamEmitSoundComponent>(uid);
		}

		// Token: 0x06002670 RID: 9840 RVA: 0x000CB6A0 File Offset: 0x000C98A0
		private void OnDamageChanged(EntityUid uid, SleepingComponent component, DamageChangedEvent args)
		{
			if (!args.DamageIncreased || args.DamageDelta == null)
			{
				return;
			}
			if (args.DamageDelta.Total >= component.WakeThreshold)
			{
				this.TryWaking(uid, component, false, null);
			}
		}

		// Token: 0x06002671 RID: 9841 RVA: 0x000CB6E9 File Offset: 0x000C98E9
		private void OnSleepAction(EntityUid uid, MobStateComponent component, SleepActionEvent args)
		{
			this.TrySleeping(uid);
		}

		// Token: 0x06002672 RID: 9842 RVA: 0x000CB6F4 File Offset: 0x000C98F4
		private void OnWakeAction(EntityUid uid, MobStateComponent component, WakeActionEvent args)
		{
			if (!this.TryWakeCooldown(uid, null))
			{
				return;
			}
			if (this.TryWaking(uid, null, false, null))
			{
				args.Handled = true;
			}
		}

		// Token: 0x06002673 RID: 9843 RVA: 0x000CB728 File Offset: 0x000C9928
		private void OnMobStateChanged(EntityUid uid, SleepingComponent component, MobStateChangedEvent args)
		{
			if (args.NewMobState == MobState.Dead)
			{
				base.RemComp<SpamEmitSoundComponent>(uid);
				base.RemComp<SleepingComponent>(uid);
				return;
			}
			SpamEmitSoundComponent spam;
			if (base.TryComp<SpamEmitSoundComponent>(uid, ref spam))
			{
				spam.Enabled = (args.NewMobState == MobState.Alive);
			}
		}

		// Token: 0x06002674 RID: 9844 RVA: 0x000CB76C File Offset: 0x000C996C
		private void AddWakeVerb(EntityUid uid, SleepingComponent component, GetVerbsEvent<AlternativeVerb> args)
		{
			if (!args.CanInteract || !args.CanAccess)
			{
				return;
			}
			AlternativeVerb verb = new AlternativeVerb
			{
				Act = delegate()
				{
					if (!this.TryWakeCooldown(uid, null))
					{
						return;
					}
					this.TryWaking(args.Target, null, false, new EntityUid?(args.User));
				},
				Text = Loc.GetString("action-name-wake"),
				Priority = 2
			};
			args.Verbs.Add(verb);
		}

		// Token: 0x06002675 RID: 9845 RVA: 0x000CB7F0 File Offset: 0x000C99F0
		private void OnInteractHand(EntityUid uid, SleepingComponent component, InteractHandEvent args)
		{
			if (!this.TryWakeCooldown(uid, null))
			{
				return;
			}
			args.Handled = true;
			this.TryWaking(args.Target, null, false, new EntityUid?(args.User));
		}

		// Token: 0x06002676 RID: 9846 RVA: 0x000CB820 File Offset: 0x000C9A20
		private void OnExamined(EntityUid uid, SleepingComponent component, ExaminedEvent args)
		{
			if (args.IsInDetailsRange)
			{
				args.PushMarkup(Loc.GetString("sleep-examined", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("target", Identity.Entity(uid, this.EntityManager))
				}));
			}
		}

		// Token: 0x06002677 RID: 9847 RVA: 0x000CB86D File Offset: 0x000C9A6D
		private void OnSlip(EntityUid uid, SleepingComponent component, SlipAttemptEvent args)
		{
			args.Cancel();
		}

		// Token: 0x06002678 RID: 9848 RVA: 0x000CB875 File Offset: 0x000C9A75
		private void OnInit(EntityUid uid, ForcedSleepingComponent component, ComponentInit args)
		{
			this.TrySleeping(uid);
		}

		// Token: 0x06002679 RID: 9849 RVA: 0x000CB880 File Offset: 0x000C9A80
		public bool TrySleeping(EntityUid uid)
		{
			if (!base.HasComp<MobStateComponent>(uid))
			{
				return false;
			}
			TryingToSleepEvent tryingToSleepEvent = new TryingToSleepEvent(uid, false);
			base.RaiseLocalEvent<TryingToSleepEvent>(uid, ref tryingToSleepEvent, false);
			if (tryingToSleepEvent.Cancelled)
			{
				return false;
			}
			InstantActionPrototype sleepAction;
			if (this._prototypeManager.TryIndex<InstantActionPrototype>("Sleep", ref sleepAction))
			{
				this._actionsSystem.RemoveAction(uid, sleepAction, null);
			}
			base.EnsureComp<SleepingComponent>(uid);
			return true;
		}

		// Token: 0x0600267A RID: 9850 RVA: 0x000CB8E4 File Offset: 0x000C9AE4
		[NullableContext(2)]
		private bool TryWakeCooldown(EntityUid uid, SleepingComponent component = null)
		{
			if (!base.Resolve<SleepingComponent>(uid, ref component, false))
			{
				return false;
			}
			TimeSpan curTime = this._gameTiming.CurTime;
			if (curTime < component.CoolDownEnd)
			{
				return false;
			}
			component.CoolDownEnd = curTime + component.Cooldown;
			return true;
		}

		// Token: 0x0600267B RID: 9851 RVA: 0x000CB930 File Offset: 0x000C9B30
		[NullableContext(2)]
		public bool TryWaking(EntityUid uid, SleepingComponent component = null, bool force = false, EntityUid? user = null)
		{
			if (!base.Resolve<SleepingComponent>(uid, ref component, false))
			{
				return false;
			}
			if (!force && base.HasComp<ForcedSleepingComponent>(uid))
			{
				if (user != null)
				{
					this._audio.PlayPvs("/Audio/Effects/thudswoosh.ogg", uid, new AudioParams?(AudioHelpers.WithVariation(0.05f, this._robustRandom)));
					this._popupSystem.PopupEntity(Loc.GetString("wake-other-failure", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("target", Identity.Entity(uid, this.EntityManager))
					}), uid, Filter.Entities(new EntityUid[]
					{
						user.Value
					}), true, PopupType.SmallCaution);
				}
				return false;
			}
			if (user != null)
			{
				this._audio.PlayPvs("/Audio/Effects/thudswoosh.ogg", uid, new AudioParams?(AudioHelpers.WithVariation(0.05f, this._robustRandom)));
				this._popupSystem.PopupEntity(Loc.GetString("wake-other-success", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("target", Identity.Entity(uid, this.EntityManager))
				}), uid, Filter.Entities(new EntityUid[]
				{
					user.Value
				}), true, PopupType.Small);
			}
			base.RemComp<SleepingComponent>(uid);
			return true;
		}

		// Token: 0x040017E9 RID: 6121
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x040017EA RID: 6122
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x040017EB RID: 6123
		[Dependency]
		private readonly IRobustRandom _robustRandom;

		// Token: 0x040017EC RID: 6124
		[Dependency]
		private readonly ActionsSystem _actionsSystem;

		// Token: 0x040017ED RID: 6125
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x040017EE RID: 6126
		[Dependency]
		private readonly SharedAudioSystem _audio;
	}
}
