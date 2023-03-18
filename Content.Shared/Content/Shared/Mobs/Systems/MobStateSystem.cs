using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.ActionBlocker;
using Content.Shared.Administration.Logs;
using Content.Shared.Bed.Sleep;
using Content.Shared.Database;
using Content.Shared.Disease.Events;
using Content.Shared.Emoting;
using Content.Shared.Hands;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory.Events;
using Content.Shared.Item;
using Content.Shared.Mobs.Components;
using Content.Shared.Movement.Events;
using Content.Shared.Pulling.Events;
using Content.Shared.Speech;
using Content.Shared.Standing;
using Content.Shared.Strip.Components;
using Content.Shared.Throwing;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;

namespace Content.Shared.Mobs.Systems
{
	// Token: 0x020002FF RID: 767
	[NullableContext(1)]
	[Nullable(0)]
	[Virtual]
	public class MobStateSystem : EntitySystem
	{
		// Token: 0x06000898 RID: 2200 RVA: 0x0001D000 File Offset: 0x0001B200
		public override void Initialize()
		{
			this._sawmill = this._logManager.GetSawmill("MobState");
			base.Initialize();
			this.SubscribeEvents();
			base.SubscribeLocalEvent<MobStateComponent, ComponentGetState>(new ComponentEventRefHandler<MobStateComponent, ComponentGetState>(this.OnGetComponentState), null, null);
			base.SubscribeLocalEvent<MobStateComponent, ComponentHandleState>(new ComponentEventRefHandler<MobStateComponent, ComponentHandleState>(this.OnHandleComponentState), null, null);
		}

		// Token: 0x06000899 RID: 2201 RVA: 0x0001D057 File Offset: 0x0001B257
		[NullableContext(2)]
		public bool IsAlive(EntityUid target, MobStateComponent component = null)
		{
			return base.Resolve<MobStateComponent>(target, ref component, false) && component.CurrentState == MobState.Alive;
		}

		// Token: 0x0600089A RID: 2202 RVA: 0x0001D070 File Offset: 0x0001B270
		[NullableContext(2)]
		public bool IsCritical(EntityUid target, MobStateComponent component = null)
		{
			return base.Resolve<MobStateComponent>(target, ref component, false) && component.CurrentState == MobState.Critical;
		}

		// Token: 0x0600089B RID: 2203 RVA: 0x0001D089 File Offset: 0x0001B289
		[NullableContext(2)]
		public bool IsDead(EntityUid target, MobStateComponent component = null)
		{
			return base.Resolve<MobStateComponent>(target, ref component, false) && component.CurrentState == MobState.Dead;
		}

		// Token: 0x0600089C RID: 2204 RVA: 0x0001D0A4 File Offset: 0x0001B2A4
		[NullableContext(2)]
		public bool IsIncapacitated(EntityUid target, MobStateComponent component = null)
		{
			if (!base.Resolve<MobStateComponent>(target, ref component, false))
			{
				return false;
			}
			MobState currentState = component.CurrentState;
			return currentState == MobState.Critical || currentState == MobState.Dead;
		}

		// Token: 0x0600089D RID: 2205 RVA: 0x0001D0D7 File Offset: 0x0001B2D7
		[NullableContext(2)]
		public bool IsInvalidState(EntityUid target, MobStateComponent component = null)
		{
			return base.Resolve<MobStateComponent>(target, ref component, false) && component.CurrentState == MobState.Invalid;
		}

		// Token: 0x0600089E RID: 2206 RVA: 0x0001D0F0 File Offset: 0x0001B2F0
		private void OnHandleComponentState(EntityUid uid, MobStateComponent component, ref ComponentHandleState args)
		{
			MobStateComponentState state = args.Current as MobStateComponentState;
			if (state == null)
			{
				return;
			}
			component.CurrentState = state.CurrentState;
			component.AllowedStates = new HashSet<MobState>(state.AllowedStates);
		}

		// Token: 0x0600089F RID: 2207 RVA: 0x0001D12A File Offset: 0x0001B32A
		private void OnGetComponentState(EntityUid uid, MobStateComponent component, ref ComponentGetState args)
		{
			args.State = new MobStateComponentState(component.CurrentState, component.AllowedStates);
		}

		// Token: 0x060008A0 RID: 2208 RVA: 0x0001D143 File Offset: 0x0001B343
		[NullableContext(2)]
		public bool HasState(EntityUid entity, MobState mobState, MobStateComponent component = null)
		{
			return base.Resolve<MobStateComponent>(entity, ref component, false) && component.AllowedStates.Contains(mobState);
		}

		// Token: 0x060008A1 RID: 2209 RVA: 0x0001D160 File Offset: 0x0001B360
		[NullableContext(2)]
		public void UpdateMobState(EntityUid entity, MobStateComponent component = null, EntityUid? origin = null)
		{
			if (!base.Resolve<MobStateComponent>(entity, ref component, true))
			{
				return;
			}
			UpdateMobStateEvent ev = new UpdateMobStateEvent
			{
				Target = entity,
				Component = component,
				Origin = origin
			};
			base.RaiseLocalEvent<UpdateMobStateEvent>(entity, ref ev, false);
			this.ChangeState(entity, component, ev.State, null);
		}

		// Token: 0x060008A2 RID: 2210 RVA: 0x0001D1C0 File Offset: 0x0001B3C0
		[NullableContext(2)]
		public void ChangeMobState(EntityUid entity, MobState mobState, MobStateComponent component = null, EntityUid? origin = null)
		{
			if (!base.Resolve<MobStateComponent>(entity, ref component, true))
			{
				return;
			}
			UpdateMobStateEvent ev = new UpdateMobStateEvent
			{
				Target = entity,
				Component = component,
				Origin = origin
			};
			base.RaiseLocalEvent<UpdateMobStateEvent>(entity, ref ev, false);
			this.ChangeState(entity, component, ev.State, null);
		}

		// Token: 0x060008A3 RID: 2211 RVA: 0x0001D21F File Offset: 0x0001B41F
		protected virtual void OnEnterState(EntityUid entity, MobStateComponent component, MobState state)
		{
			this.OnStateEnteredSubscribers(entity, component, state);
		}

		// Token: 0x060008A4 RID: 2212 RVA: 0x0001D22A File Offset: 0x0001B42A
		protected virtual void OnStateChanged(EntityUid entity, MobStateComponent component, MobState oldState, MobState newState)
		{
		}

		// Token: 0x060008A5 RID: 2213 RVA: 0x0001D22C File Offset: 0x0001B42C
		protected virtual void OnExitState(EntityUid entity, MobStateComponent component, MobState state)
		{
			this.OnStateExitSubscribers(entity, component, state);
		}

		// Token: 0x060008A6 RID: 2214 RVA: 0x0001D238 File Offset: 0x0001B438
		private void ChangeState(EntityUid target, MobStateComponent component, MobState newState, EntityUid? origin = null)
		{
			MobState oldState = component.CurrentState;
			if (oldState == newState || !component.AllowedStates.Contains(newState))
			{
				return;
			}
			this.OnExitState(target, component, oldState);
			component.CurrentState = newState;
			this.OnEnterState(target, component, newState);
			MobStateChangedEvent ev = new MobStateChangedEvent(target, component, oldState, newState, origin);
			this.OnStateChanged(target, component, oldState, newState);
			base.RaiseLocalEvent<MobStateChangedEvent>(target, ev, true);
			ISharedAdminLogManager adminLogger = this._adminLogger;
			LogType type = LogType.Damaged;
			LogImpact impact = (oldState == MobState.Alive) ? LogImpact.Low : LogImpact.Medium;
			LogStringHandler logStringHandler = new LogStringHandler(24, 3);
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(component.Owner), "user", "ToPrettyString(component.Owner)");
			logStringHandler.AppendLiteral(" state changed from ");
			logStringHandler.AppendFormatted<MobState>(oldState, "oldState");
			logStringHandler.AppendLiteral(" to ");
			logStringHandler.AppendFormatted<MobState>(newState, "newState");
			adminLogger.Add(type, impact, ref logStringHandler);
			base.Dirty(component, null);
		}

		// Token: 0x060008A7 RID: 2215 RVA: 0x0001D310 File Offset: 0x0001B510
		private void SubscribeEvents()
		{
			base.SubscribeLocalEvent<MobStateComponent, BeforeGettingStrippedEvent>(new ComponentEventHandler<MobStateComponent, BeforeGettingStrippedEvent>(this.OnGettingStripped), null, null);
			base.SubscribeLocalEvent<MobStateComponent, ChangeDirectionAttemptEvent>(new ComponentEventHandler<MobStateComponent, ChangeDirectionAttemptEvent>(this.CheckAct), null, null);
			base.SubscribeLocalEvent<MobStateComponent, UseAttemptEvent>(new ComponentEventHandler<MobStateComponent, UseAttemptEvent>(this.CheckAct), null, null);
			base.SubscribeLocalEvent<MobStateComponent, AttackAttemptEvent>(new ComponentEventHandler<MobStateComponent, AttackAttemptEvent>(this.CheckAct), null, null);
			base.SubscribeLocalEvent<MobStateComponent, InteractionAttemptEvent>(new ComponentEventHandler<MobStateComponent, InteractionAttemptEvent>(this.CheckAct), null, null);
			base.SubscribeLocalEvent<MobStateComponent, ThrowAttemptEvent>(new ComponentEventHandler<MobStateComponent, ThrowAttemptEvent>(this.CheckAct), null, null);
			base.SubscribeLocalEvent<MobStateComponent, SpeakAttemptEvent>(new ComponentEventHandler<MobStateComponent, SpeakAttemptEvent>(this.CheckAct), null, null);
			base.SubscribeLocalEvent<MobStateComponent, IsEquippingAttemptEvent>(new ComponentEventHandler<MobStateComponent, IsEquippingAttemptEvent>(this.OnEquipAttempt), null, null);
			base.SubscribeLocalEvent<MobStateComponent, EmoteAttemptEvent>(new ComponentEventHandler<MobStateComponent, EmoteAttemptEvent>(this.CheckAct), null, null);
			base.SubscribeLocalEvent<MobStateComponent, IsUnequippingAttemptEvent>(new ComponentEventHandler<MobStateComponent, IsUnequippingAttemptEvent>(this.OnUnequipAttempt), null, null);
			base.SubscribeLocalEvent<MobStateComponent, DropAttemptEvent>(new ComponentEventHandler<MobStateComponent, DropAttemptEvent>(this.CheckAct), null, null);
			base.SubscribeLocalEvent<MobStateComponent, PickupAttemptEvent>(new ComponentEventHandler<MobStateComponent, PickupAttemptEvent>(this.CheckAct), null, null);
			base.SubscribeLocalEvent<MobStateComponent, StartPullAttemptEvent>(new ComponentEventHandler<MobStateComponent, StartPullAttemptEvent>(this.CheckAct), null, null);
			base.SubscribeLocalEvent<MobStateComponent, UpdateCanMoveEvent>(new ComponentEventHandler<MobStateComponent, UpdateCanMoveEvent>(this.CheckAct), null, null);
			base.SubscribeLocalEvent<MobStateComponent, StandAttemptEvent>(new ComponentEventHandler<MobStateComponent, StandAttemptEvent>(this.CheckAct), null, null);
			base.SubscribeLocalEvent<MobStateComponent, TryingToSleepEvent>(new ComponentEventRefHandler<MobStateComponent, TryingToSleepEvent>(this.OnSleepAttempt), null, null);
			base.SubscribeLocalEvent<MobStateComponent, AttemptSneezeCoughEvent>(new ComponentEventRefHandler<MobStateComponent, AttemptSneezeCoughEvent>(this.OnSneezeAttempt), null, null);
		}

		// Token: 0x060008A8 RID: 2216 RVA: 0x0001D474 File Offset: 0x0001B674
		private void OnStateExitSubscribers(EntityUid target, MobStateComponent component, MobState state)
		{
			switch (state)
			{
			case MobState.Invalid:
			case MobState.Alive:
				break;
			case MobState.Critical:
				this._standing.Stand(target, null, null, false);
				return;
			case MobState.Dead:
			{
				base.RemComp<CollisionWakeComponent>(target);
				this._standing.Stand(target, null, null, false);
				PhysicsComponent physics;
				if (!this._standing.IsDown(target, null) && base.TryComp<PhysicsComponent>(target, ref physics))
				{
					this._physics.SetCanCollide(target, true, true, false, null, physics);
					return;
				}
				break;
			}
			default:
				throw new NotImplementedException();
			}
		}

		// Token: 0x060008A9 RID: 2217 RVA: 0x0001D4F8 File Offset: 0x0001B6F8
		private void OnStateEnteredSubscribers(EntityUid target, MobStateComponent component, MobState state)
		{
			this._blocker.UpdateCanMove(target, null);
			switch (state)
			{
			case MobState.Invalid:
				return;
			case MobState.Alive:
				this._standing.Stand(target, null, null, false);
				this._appearance.SetData(target, MobStateVisuals.State, MobState.Alive, null);
				return;
			case MobState.Critical:
				this._standing.Down(target, true, true, null, null, null);
				this._appearance.SetData(target, MobStateVisuals.State, MobState.Critical, null);
				return;
			case MobState.Dead:
			{
				base.EnsureComp<CollisionWakeComponent>(target);
				this._standing.Down(target, true, true, null, null, null);
				PhysicsComponent physics;
				if (this._standing.IsDown(target, null) && base.TryComp<PhysicsComponent>(target, ref physics))
				{
					this._physics.SetCanCollide(target, false, true, false, null, physics);
				}
				this._appearance.SetData(target, MobStateVisuals.State, MobState.Dead, null);
				return;
			}
			default:
				throw new NotImplementedException();
			}
		}

		// Token: 0x060008AA RID: 2218 RVA: 0x0001D5EA File Offset: 0x0001B7EA
		private void OnSleepAttempt(EntityUid target, MobStateComponent component, ref TryingToSleepEvent args)
		{
			if (this.IsDead(target, component))
			{
				args.Cancelled = true;
			}
		}

		// Token: 0x060008AB RID: 2219 RVA: 0x0001D5FD File Offset: 0x0001B7FD
		private void OnSneezeAttempt(EntityUid target, MobStateComponent component, ref AttemptSneezeCoughEvent args)
		{
			if (this.IsDead(target, component))
			{
				args.Cancelled = true;
			}
		}

		// Token: 0x060008AC RID: 2220 RVA: 0x0001D610 File Offset: 0x0001B810
		private void OnGettingStripped(EntityUid target, MobStateComponent component, BeforeGettingStrippedEvent args)
		{
			if (this.IsDead(target, component))
			{
				args.Multiplier /= 3f;
				return;
			}
			if (this.IsCritical(target, component))
			{
				args.Multiplier /= 2f;
			}
		}

		// Token: 0x060008AD RID: 2221 RVA: 0x0001D64C File Offset: 0x0001B84C
		private void CheckAct(EntityUid target, MobStateComponent component, CancellableEntityEventArgs args)
		{
			MobState currentState = component.CurrentState;
			if (currentState - MobState.Critical <= 1)
			{
				args.Cancel();
			}
		}

		// Token: 0x060008AE RID: 2222 RVA: 0x0001D66C File Offset: 0x0001B86C
		private void OnEquipAttempt(EntityUid target, MobStateComponent component, IsEquippingAttemptEvent args)
		{
			if (args.Equipee == target)
			{
				this.CheckAct(target, component, args);
			}
		}

		// Token: 0x060008AF RID: 2223 RVA: 0x0001D685 File Offset: 0x0001B885
		private void OnUnequipAttempt(EntityUid target, MobStateComponent component, IsUnequippingAttemptEvent args)
		{
			if (args.Unequipee == target)
			{
				this.CheckAct(target, component, args);
			}
		}

		// Token: 0x040008BC RID: 2236
		[Dependency]
		private readonly ActionBlockerSystem _blocker;

		// Token: 0x040008BD RID: 2237
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;

		// Token: 0x040008BE RID: 2238
		[Dependency]
		private readonly SharedPhysicsSystem _physics;

		// Token: 0x040008BF RID: 2239
		[Dependency]
		private readonly StandingStateSystem _standing;

		// Token: 0x040008C0 RID: 2240
		[Dependency]
		private readonly ISharedAdminLogManager _adminLogger;

		// Token: 0x040008C1 RID: 2241
		[Dependency]
		private readonly ILogManager _logManager;

		// Token: 0x040008C2 RID: 2242
		private ISawmill _sawmill;
	}
}
