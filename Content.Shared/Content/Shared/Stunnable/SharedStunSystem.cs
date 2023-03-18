using System;
using System.Runtime.CompilerServices;
using Content.Shared.ActionBlocker;
using Content.Shared.Administration.Logs;
using Content.Shared.Bed.Sleep;
using Content.Shared.Database;
using Content.Shared.Hands;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory.Events;
using Content.Shared.Item;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Events;
using Content.Shared.Movement.Systems;
using Content.Shared.Standing;
using Content.Shared.StatusEffect;
using Content.Shared.Throwing;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;

namespace Content.Shared.Stunnable
{
	// Token: 0x0200010E RID: 270
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedStunSystem : EntitySystem
	{
		// Token: 0x06000307 RID: 775 RVA: 0x0000D884 File Offset: 0x0000BA84
		public override void Initialize()
		{
			base.SubscribeLocalEvent<KnockedDownComponent, ComponentInit>(new ComponentEventHandler<KnockedDownComponent, ComponentInit>(this.OnKnockInit), null, null);
			base.SubscribeLocalEvent<KnockedDownComponent, ComponentShutdown>(new ComponentEventHandler<KnockedDownComponent, ComponentShutdown>(this.OnKnockShutdown), null, null);
			base.SubscribeLocalEvent<KnockedDownComponent, StandAttemptEvent>(new ComponentEventHandler<KnockedDownComponent, StandAttemptEvent>(this.OnStandAttempt), null, null);
			base.SubscribeLocalEvent<SlowedDownComponent, ComponentInit>(new ComponentEventHandler<SlowedDownComponent, ComponentInit>(this.OnSlowInit), null, null);
			base.SubscribeLocalEvent<SlowedDownComponent, ComponentShutdown>(new ComponentEventHandler<SlowedDownComponent, ComponentShutdown>(this.OnSlowRemove), null, null);
			base.SubscribeLocalEvent<StunnedComponent, ComponentStartup>(new ComponentEventHandler<StunnedComponent, ComponentStartup>(this.UpdateCanMove), null, null);
			base.SubscribeLocalEvent<StunnedComponent, ComponentShutdown>(new ComponentEventHandler<StunnedComponent, ComponentShutdown>(this.UpdateCanMove), null, null);
			base.SubscribeLocalEvent<SlowedDownComponent, ComponentGetState>(new ComponentEventRefHandler<SlowedDownComponent, ComponentGetState>(this.OnSlowGetState), null, null);
			base.SubscribeLocalEvent<SlowedDownComponent, ComponentHandleState>(new ComponentEventRefHandler<SlowedDownComponent, ComponentHandleState>(this.OnSlowHandleState), null, null);
			base.SubscribeLocalEvent<KnockedDownComponent, ComponentGetState>(new ComponentEventRefHandler<KnockedDownComponent, ComponentGetState>(this.OnKnockGetState), null, null);
			base.SubscribeLocalEvent<KnockedDownComponent, ComponentHandleState>(new ComponentEventRefHandler<KnockedDownComponent, ComponentHandleState>(this.OnKnockHandleState), null, null);
			base.SubscribeLocalEvent<KnockedDownComponent, InteractHandEvent>(new ComponentEventHandler<KnockedDownComponent, InteractHandEvent>(this.OnInteractHand), null, null);
			base.SubscribeLocalEvent<SlowedDownComponent, RefreshMovementSpeedModifiersEvent>(new ComponentEventHandler<SlowedDownComponent, RefreshMovementSpeedModifiersEvent>(this.OnRefreshMovespeed), null, null);
			base.SubscribeLocalEvent<KnockedDownComponent, TileFrictionEvent>(new ComponentEventRefHandler<KnockedDownComponent, TileFrictionEvent>(this.OnKnockedTileFriction), null, null);
			base.SubscribeLocalEvent<StunnedComponent, ChangeDirectionAttemptEvent>(new ComponentEventHandler<StunnedComponent, ChangeDirectionAttemptEvent>(this.OnAttempt), null, null);
			base.SubscribeLocalEvent<StunnedComponent, UpdateCanMoveEvent>(new ComponentEventHandler<StunnedComponent, UpdateCanMoveEvent>(this.OnMoveAttempt), null, null);
			base.SubscribeLocalEvent<StunnedComponent, InteractionAttemptEvent>(new ComponentEventHandler<StunnedComponent, InteractionAttemptEvent>(this.OnAttempt), null, null);
			base.SubscribeLocalEvent<StunnedComponent, UseAttemptEvent>(new ComponentEventHandler<StunnedComponent, UseAttemptEvent>(this.OnAttempt), null, null);
			base.SubscribeLocalEvent<StunnedComponent, ThrowAttemptEvent>(new ComponentEventHandler<StunnedComponent, ThrowAttemptEvent>(this.OnAttempt), null, null);
			base.SubscribeLocalEvent<StunnedComponent, DropAttemptEvent>(new ComponentEventHandler<StunnedComponent, DropAttemptEvent>(this.OnAttempt), null, null);
			base.SubscribeLocalEvent<StunnedComponent, AttackAttemptEvent>(new ComponentEventHandler<StunnedComponent, AttackAttemptEvent>(this.OnAttempt), null, null);
			base.SubscribeLocalEvent<StunnedComponent, PickupAttemptEvent>(new ComponentEventHandler<StunnedComponent, PickupAttemptEvent>(this.OnAttempt), null, null);
			base.SubscribeLocalEvent<StunnedComponent, IsEquippingAttemptEvent>(new ComponentEventHandler<StunnedComponent, IsEquippingAttemptEvent>(this.OnEquipAttempt), null, null);
			base.SubscribeLocalEvent<StunnedComponent, IsUnequippingAttemptEvent>(new ComponentEventHandler<StunnedComponent, IsUnequippingAttemptEvent>(this.OnUnequipAttempt), null, null);
			base.SubscribeLocalEvent<MobStateComponent, MobStateChangedEvent>(new ComponentEventHandler<MobStateComponent, MobStateChangedEvent>(this.OnMobStateChanged), null, null);
		}

		// Token: 0x06000308 RID: 776 RVA: 0x0000DA88 File Offset: 0x0000BC88
		private void OnMobStateChanged(EntityUid uid, MobStateComponent component, MobStateChangedEvent args)
		{
			StatusEffectsComponent status;
			if (!base.TryComp<StatusEffectsComponent>(uid, ref status))
			{
				return;
			}
			switch (args.NewMobState)
			{
			case MobState.Invalid:
				return;
			case MobState.Alive:
				return;
			case MobState.Critical:
				this._statusEffectSystem.TryRemoveStatusEffect(uid, "Stun", null, true);
				return;
			case MobState.Dead:
				this._statusEffectSystem.TryRemoveStatusEffect(uid, "Stun", null, true);
				return;
			default:
				return;
			}
		}

		// Token: 0x06000309 RID: 777 RVA: 0x0000DAEB File Offset: 0x0000BCEB
		private void UpdateCanMove(EntityUid uid, StunnedComponent component, EntityEventArgs args)
		{
			this._blocker.UpdateCanMove(uid, null);
		}

		// Token: 0x0600030A RID: 778 RVA: 0x0000DAFB File Offset: 0x0000BCFB
		private void OnSlowGetState(EntityUid uid, SlowedDownComponent component, ref ComponentGetState args)
		{
			args.State = new SlowedDownComponentState(component.SprintSpeedModifier, component.WalkSpeedModifier);
		}

		// Token: 0x0600030B RID: 779 RVA: 0x0000DB14 File Offset: 0x0000BD14
		private void OnSlowHandleState(EntityUid uid, SlowedDownComponent component, ref ComponentHandleState args)
		{
			SlowedDownComponentState state = args.Current as SlowedDownComponentState;
			if (state != null)
			{
				component.SprintSpeedModifier = state.SprintSpeedModifier;
				component.WalkSpeedModifier = state.WalkSpeedModifier;
			}
		}

		// Token: 0x0600030C RID: 780 RVA: 0x0000DB48 File Offset: 0x0000BD48
		private void OnKnockGetState(EntityUid uid, KnockedDownComponent component, ref ComponentGetState args)
		{
			args.State = new KnockedDownComponentState(component.HelpInterval, component.HelpTimer);
		}

		// Token: 0x0600030D RID: 781 RVA: 0x0000DB64 File Offset: 0x0000BD64
		private void OnKnockHandleState(EntityUid uid, KnockedDownComponent component, ref ComponentHandleState args)
		{
			KnockedDownComponentState state = args.Current as KnockedDownComponentState;
			if (state != null)
			{
				component.HelpInterval = state.HelpInterval;
				component.HelpTimer = state.HelpTimer;
			}
		}

		// Token: 0x0600030E RID: 782 RVA: 0x0000DB98 File Offset: 0x0000BD98
		private void OnKnockInit(EntityUid uid, KnockedDownComponent component, ComponentInit args)
		{
			this._standingStateSystem.Down(uid, true, true, null, null, null);
		}

		// Token: 0x0600030F RID: 783 RVA: 0x0000DBAC File Offset: 0x0000BDAC
		private void OnKnockShutdown(EntityUid uid, KnockedDownComponent component, ComponentShutdown args)
		{
			this._standingStateSystem.Stand(uid, null, null, false);
		}

		// Token: 0x06000310 RID: 784 RVA: 0x0000DBBE File Offset: 0x0000BDBE
		private void OnStandAttempt(EntityUid uid, KnockedDownComponent component, StandAttemptEvent args)
		{
			if (component.LifeStage <= 6)
			{
				args.Cancel();
			}
		}

		// Token: 0x06000311 RID: 785 RVA: 0x0000DBCF File Offset: 0x0000BDCF
		private void OnSlowInit(EntityUid uid, SlowedDownComponent component, ComponentInit args)
		{
			this._movementSpeedModifierSystem.RefreshMovementSpeedModifiers(uid, null);
		}

		// Token: 0x06000312 RID: 786 RVA: 0x0000DBDE File Offset: 0x0000BDDE
		private void OnSlowRemove(EntityUid uid, SlowedDownComponent component, ComponentShutdown args)
		{
			component.SprintSpeedModifier = 1f;
			component.WalkSpeedModifier = 1f;
			this._movementSpeedModifierSystem.RefreshMovementSpeedModifiers(uid, null);
		}

		// Token: 0x06000313 RID: 787 RVA: 0x0000DC03 File Offset: 0x0000BE03
		private void OnRefreshMovespeed(EntityUid uid, SlowedDownComponent component, RefreshMovementSpeedModifiersEvent args)
		{
			args.ModifySpeed(component.WalkSpeedModifier, component.SprintSpeedModifier);
		}

		// Token: 0x06000314 RID: 788 RVA: 0x0000DC18 File Offset: 0x0000BE18
		[NullableContext(2)]
		public bool TryStun(EntityUid uid, TimeSpan time, bool refresh, StatusEffectsComponent status = null)
		{
			if (time <= TimeSpan.Zero)
			{
				return false;
			}
			if (!base.Resolve<StatusEffectsComponent>(uid, ref status, false))
			{
				return false;
			}
			if (!this._statusEffectSystem.TryAddStatusEffect<StunnedComponent>(uid, "Stun", time, refresh, null))
			{
				return false;
			}
			ISharedAdminLogManager adminLogger = this._adminLogger;
			LogType type = LogType.Stamina;
			LogImpact impact = LogImpact.Medium;
			LogStringHandler logStringHandler = new LogStringHandler(21, 2);
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "user", "ToPrettyString(uid)");
			logStringHandler.AppendLiteral(" stunned for ");
			logStringHandler.AppendFormatted<int>(time.Seconds, "time.Seconds");
			logStringHandler.AppendLiteral(" seconds");
			adminLogger.Add(type, impact, ref logStringHandler);
			return true;
		}

		// Token: 0x06000315 RID: 789 RVA: 0x0000DCB8 File Offset: 0x0000BEB8
		[NullableContext(2)]
		public bool TryKnockdown(EntityUid uid, TimeSpan time, bool refresh, StatusEffectsComponent status = null)
		{
			return !(time <= TimeSpan.Zero) && base.Resolve<StatusEffectsComponent>(uid, ref status, false) && this._statusEffectSystem.TryAddStatusEffect<KnockedDownComponent>(uid, "KnockedDown", time, refresh, null);
		}

		// Token: 0x06000316 RID: 790 RVA: 0x0000DCEB File Offset: 0x0000BEEB
		[NullableContext(2)]
		public bool TryParalyze(EntityUid uid, TimeSpan time, bool refresh, StatusEffectsComponent status = null)
		{
			return base.Resolve<StatusEffectsComponent>(uid, ref status, false) && this.TryKnockdown(uid, time, refresh, status) && this.TryStun(uid, time, refresh, status);
		}

		// Token: 0x06000317 RID: 791 RVA: 0x0000DD18 File Offset: 0x0000BF18
		[NullableContext(2)]
		public bool TrySlowdown(EntityUid uid, TimeSpan time, bool refresh, float walkSpeedMultiplier = 1f, float runSpeedMultiplier = 1f, StatusEffectsComponent status = null)
		{
			if (!base.Resolve<StatusEffectsComponent>(uid, ref status, false))
			{
				return false;
			}
			if (time <= TimeSpan.Zero)
			{
				return false;
			}
			if (this._statusEffectSystem.TryAddStatusEffect<SlowedDownComponent>(uid, "SlowedDown", time, refresh, status))
			{
				SlowedDownComponent component = this.EntityManager.GetComponent<SlowedDownComponent>(uid);
				walkSpeedMultiplier = Math.Clamp(walkSpeedMultiplier, 0f, 1f);
				runSpeedMultiplier = Math.Clamp(runSpeedMultiplier, 0f, 1f);
				component.WalkSpeedModifier *= walkSpeedMultiplier;
				component.SprintSpeedModifier *= runSpeedMultiplier;
				this._movementSpeedModifierSystem.RefreshMovementSpeedModifiers(uid, null);
				return true;
			}
			return false;
		}

		// Token: 0x06000318 RID: 792 RVA: 0x0000DDB8 File Offset: 0x0000BFB8
		private void OnInteractHand(EntityUid uid, KnockedDownComponent knocked, InteractHandEvent args)
		{
			if (args.Handled || knocked.HelpTimer > 0f)
			{
				return;
			}
			if (base.HasComp<SleepingComponent>(uid))
			{
				return;
			}
			knocked.HelpTimer = knocked.HelpInterval / 2f;
			this._statusEffectSystem.TryRemoveTime(uid, "KnockedDown", TimeSpan.FromSeconds((double)knocked.HelpInterval), null);
			this._audio.PlayPredicted(knocked.StunAttemptSound, uid, new EntityUid?(args.User), null);
			base.Dirty(knocked, null);
			args.Handled = true;
		}

		// Token: 0x06000319 RID: 793 RVA: 0x0000DE4C File Offset: 0x0000C04C
		private void OnKnockedTileFriction(EntityUid uid, KnockedDownComponent component, ref TileFrictionEvent args)
		{
			args.Modifier *= 0.4f;
		}

		// Token: 0x0600031A RID: 794 RVA: 0x0000DE5D File Offset: 0x0000C05D
		private void OnMoveAttempt(EntityUid uid, StunnedComponent stunned, UpdateCanMoveEvent args)
		{
			if (stunned.LifeStage > 6)
			{
				return;
			}
			args.Cancel();
		}

		// Token: 0x0600031B RID: 795 RVA: 0x0000DE6F File Offset: 0x0000C06F
		private void OnAttempt(EntityUid uid, StunnedComponent stunned, CancellableEntityEventArgs args)
		{
			args.Cancel();
		}

		// Token: 0x0600031C RID: 796 RVA: 0x0000DE77 File Offset: 0x0000C077
		private void OnEquipAttempt(EntityUid uid, StunnedComponent stunned, IsEquippingAttemptEvent args)
		{
			if (args.Equipee == uid)
			{
				args.Cancel();
			}
		}

		// Token: 0x0600031D RID: 797 RVA: 0x0000DE8D File Offset: 0x0000C08D
		private void OnUnequipAttempt(EntityUid uid, StunnedComponent stunned, IsUnequippingAttemptEvent args)
		{
			if (args.Unequipee == uid)
			{
				args.Cancel();
			}
		}

		// Token: 0x0400034E RID: 846
		[Dependency]
		private readonly ActionBlockerSystem _blocker;

		// Token: 0x0400034F RID: 847
		[Dependency]
		private readonly StandingStateSystem _standingStateSystem;

		// Token: 0x04000350 RID: 848
		[Dependency]
		private readonly StatusEffectsSystem _statusEffectSystem;

		// Token: 0x04000351 RID: 849
		[Dependency]
		private readonly MovementSpeedModifierSystem _movementSpeedModifierSystem;

		// Token: 0x04000352 RID: 850
		[Dependency]
		private readonly SharedAudioSystem _audio;

		// Token: 0x04000353 RID: 851
		[Dependency]
		private readonly MobStateSystem _mobStateSystem;

		// Token: 0x04000354 RID: 852
		[Dependency]
		private readonly ISharedAdminLogManager _adminLogger;

		// Token: 0x04000355 RID: 853
		public const float KnockDownModifier = 0.4f;
	}
}
