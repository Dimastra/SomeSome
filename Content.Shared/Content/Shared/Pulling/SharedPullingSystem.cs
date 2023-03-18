using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Content.Shared.ActionBlocker;
using Content.Shared.Administration.Logs;
using Content.Shared.Alert;
using Content.Shared.Buckle.Components;
using Content.Shared.Database;
using Content.Shared.GameTicking;
using Content.Shared.Gravity;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Input;
using Content.Shared.Interaction;
using Content.Shared.Physics.Pull;
using Content.Shared.Pulling.Components;
using Content.Shared.Pulling.Events;
using Content.Shared.Verbs;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics.Joints;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Players;

namespace Content.Shared.Pulling
{
	// Token: 0x02000237 RID: 567
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedPullingSystem : EntitySystem
	{
		// Token: 0x0600063F RID: 1599 RVA: 0x0001632C File Offset: 0x0001452C
		public bool CanPull(EntityUid puller, EntityUid pulled)
		{
			SharedPullerComponent comp;
			if (!this.EntityManager.TryGetComponent<SharedPullerComponent>(puller, ref comp))
			{
				return false;
			}
			Hand hand;
			if (comp.NeedsHands && !this._handsSystem.TryGetEmptyHand(puller, out hand, null))
			{
				return false;
			}
			if (!this._blocker.CanInteract(puller, new EntityUid?(pulled)))
			{
				return false;
			}
			PhysicsComponent physics;
			if (!this.EntityManager.TryGetComponent<PhysicsComponent>(pulled, ref physics))
			{
				return false;
			}
			if (physics.BodyType == 4)
			{
				return false;
			}
			if (puller == pulled)
			{
				return false;
			}
			if (!this._containerSystem.IsInSameOrNoContainer(puller, pulled))
			{
				return false;
			}
			BuckleComponent buckle;
			if (this.EntityManager.TryGetComponent<BuckleComponent>(puller, ref buckle) && buckle.Buckled)
			{
				EntityUid? lastEntityBuckledTo = buckle.LastEntityBuckledTo;
				if (lastEntityBuckledTo != null && (lastEntityBuckledTo == null || lastEntityBuckledTo.GetValueOrDefault() == pulled))
				{
					return false;
				}
			}
			BeingPulledAttemptEvent getPulled = new BeingPulledAttemptEvent(puller, pulled);
			base.RaiseLocalEvent<BeingPulledAttemptEvent>(pulled, getPulled, true);
			StartPullAttemptEvent startPull = new StartPullAttemptEvent(puller, pulled);
			base.RaiseLocalEvent<StartPullAttemptEvent>(puller, startPull, true);
			return !startPull.Cancelled && !getPulled.Cancelled;
		}

		// Token: 0x06000640 RID: 1600 RVA: 0x0001643C File Offset: 0x0001463C
		public bool TogglePull(EntityUid puller, SharedPullableComponent pullable)
		{
			EntityUid? puller2 = pullable.Puller;
			if (puller2 != null && (puller2 == null || puller2.GetValueOrDefault() == puller))
			{
				return this.TryStopPull(pullable, null);
			}
			return this.TryStartPull(puller, pullable.Owner);
		}

		// Token: 0x06000641 RID: 1601 RVA: 0x00016498 File Offset: 0x00014698
		public bool TryStopPull(SharedPullableComponent pullable, EntityUid? user = null)
		{
			if (!pullable.BeingPulled)
			{
				return false;
			}
			StopPullingEvent msg = new StopPullingEvent(user);
			base.RaiseLocalEvent<StopPullingEvent>(pullable.Owner, msg, true);
			if (msg.Cancelled)
			{
				return false;
			}
			PhysicsComponent pullablePhysics;
			if (base.TryComp<PhysicsComponent>(pullable.Owner, ref pullablePhysics))
			{
				this._physics.SetFixedRotation(pullable.Owner, pullable.PrevFixedRotation, true, null, pullablePhysics);
			}
			this._pullSm.ForceRelationship(null, pullable);
			return true;
		}

		// Token: 0x06000642 RID: 1602 RVA: 0x00016508 File Offset: 0x00014708
		public bool TryStartPull(EntityUid puller, EntityUid pullable)
		{
			SharedPullerComponent pullerComp;
			SharedPullableComponent pullableComp;
			return this.EntityManager.TryGetComponent<SharedPullerComponent>(puller, ref pullerComp) && this.EntityManager.TryGetComponent<SharedPullableComponent>(pullable, ref pullableComp) && this.TryStartPull(pullerComp, pullableComp);
		}

		// Token: 0x06000643 RID: 1603 RVA: 0x00016544 File Offset: 0x00014744
		public bool TryStartPull(SharedPullerComponent puller, SharedPullableComponent pullable)
		{
			EntityUid? pulling = puller.Pulling;
			EntityUid owner = pullable.Owner;
			if (pulling != null && (pulling == null || pulling.GetValueOrDefault() == owner))
			{
				return true;
			}
			if (!this.CanPull(puller.Owner, pullable.Owner))
			{
				return false;
			}
			PhysicsComponent pullerPhysics;
			if (!this.EntityManager.TryGetComponent<PhysicsComponent>(puller.Owner, ref pullerPhysics))
			{
				return false;
			}
			PhysicsComponent pullablePhysics;
			if (!this.EntityManager.TryGetComponent<PhysicsComponent>(pullable.Owner, ref pullablePhysics))
			{
				return false;
			}
			EntityUid? oldPullable = puller.Pulling;
			if (oldPullable != null)
			{
				SharedPullableComponent oldPullableComp;
				if (!this.EntityManager.TryGetComponent<SharedPullableComponent>(oldPullable.Value, ref oldPullableComp))
				{
					Logger.WarningS("c.go.c.pulling", "Well now you've done it, haven't you? Someone transferred pulling (onto {0}) while presently pulling something that has no Pullable component (on {1})!", new object[]
					{
						pullable.Owner,
						oldPullable
					});
					return false;
				}
				if (!this.TryStopPull(oldPullableComp, null))
				{
					return false;
				}
			}
			if (pullable.Puller != null && !this.TryStopPull(pullable, null))
			{
				return false;
			}
			PullAttemptEvent pullAttempt = new PullAttemptEvent(pullerPhysics, pullablePhysics);
			base.RaiseLocalEvent<PullAttemptEvent>(puller.Owner, pullAttempt, false);
			if (pullAttempt.Cancelled)
			{
				return false;
			}
			base.RaiseLocalEvent<PullAttemptEvent>(pullable.Owner, pullAttempt, true);
			if (pullAttempt.Cancelled)
			{
				return false;
			}
			this._interaction.DoContactInteraction(pullable.Owner, new EntityUid?(puller.Owner), null);
			this._pullSm.ForceRelationship(puller, pullable);
			pullable.PrevFixedRotation = pullablePhysics.FixedRotation;
			this._physics.SetFixedRotation(pullable.Owner, pullable.FixedRotationOnPull, true, null, pullablePhysics);
			ISharedAdminLogManager adminLogger = this._adminLogger;
			LogType type = LogType.Action;
			LogImpact impact = LogImpact.Low;
			LogStringHandler logStringHandler = new LogStringHandler(17, 2);
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(puller.Owner), "user", "ToPrettyString(puller.Owner)");
			logStringHandler.AppendLiteral(" started pulling ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(pullable.Owner), "target", "ToPrettyString(pullable.Owner)");
			adminLogger.Add(type, impact, ref logStringHandler);
			return true;
		}

		// Token: 0x06000644 RID: 1604 RVA: 0x0001674C File Offset: 0x0001494C
		public bool TryMoveTo(SharedPullableComponent pullable, EntityCoordinates to)
		{
			if (pullable.Puller == null)
			{
				return false;
			}
			if (!this.EntityManager.HasComponent<PhysicsComponent>(pullable.Owner))
			{
				return false;
			}
			this._pullSm.ForceSetMovingTo(pullable, new EntityCoordinates?(to));
			return true;
		}

		// Token: 0x06000645 RID: 1605 RVA: 0x00016794 File Offset: 0x00014994
		public void StopMoveTo(SharedPullableComponent pullable)
		{
			this._pullSm.ForceSetMovingTo(pullable, null);
		}

		// Token: 0x1700012F RID: 303
		// (get) Token: 0x06000646 RID: 1606 RVA: 0x000167B6 File Offset: 0x000149B6
		public IReadOnlySet<SharedPullableComponent> Moving
		{
			get
			{
				return this._moving;
			}
		}

		// Token: 0x06000647 RID: 1607 RVA: 0x000167C0 File Offset: 0x000149C0
		public override void Initialize()
		{
			base.Initialize();
			base.UpdatesOutsidePrediction = true;
			base.SubscribeLocalEvent<RoundRestartCleanupEvent>(new EntityEventHandler<RoundRestartCleanupEvent>(this.Reset), null, null);
			base.SubscribeLocalEvent<PullStartedMessage>(new EntityEventHandler<PullStartedMessage>(this.OnPullStarted), null, null);
			base.SubscribeLocalEvent<PullStoppedMessage>(new EntityEventHandler<PullStoppedMessage>(this.OnPullStopped), null, null);
			base.SubscribeLocalEvent<EntInsertedIntoContainerMessage>(new EntityEventHandler<EntInsertedIntoContainerMessage>(this.HandleContainerInsert), null, null);
			base.SubscribeLocalEvent<SharedPullableComponent, JointRemovedEvent>(new ComponentEventHandler<SharedPullableComponent, JointRemovedEvent>(this.OnJointRemoved), null, null);
			base.SubscribeLocalEvent<SharedPullableComponent, PullStartedMessage>(new ComponentEventHandler<SharedPullableComponent, PullStartedMessage>(this.PullableHandlePullStarted), null, null);
			base.SubscribeLocalEvent<SharedPullableComponent, PullStoppedMessage>(new ComponentEventHandler<SharedPullableComponent, PullStoppedMessage>(this.PullableHandlePullStopped), null, null);
			base.SubscribeLocalEvent<SharedPullableComponent, GetVerbsEvent<Verb>>(new ComponentEventHandler<SharedPullableComponent, GetVerbsEvent<Verb>>(this.AddPullVerbs), null, null);
			CommandBinds.Builder.Bind(ContentKeyFunctions.MovePulledObject, new PointerInputCmdHandler(new PointerInputCmdDelegate(this.HandleMovePulledObject), true, false)).Register<SharedPullingSystem>();
		}

		// Token: 0x06000648 RID: 1608 RVA: 0x000168A4 File Offset: 0x00014AA4
		private void OnJointRemoved(EntityUid uid, SharedPullableComponent component, JointRemovedEvent args)
		{
			EntityUid? puller = component.Puller;
			EntityUid owner = args.OtherBody.Owner;
			if (puller == null || (puller != null && puller.GetValueOrDefault() != owner))
			{
				return;
			}
			JointComponent joints;
			if (base.TryComp<JointComponent>(uid, ref joints))
			{
				foreach (Joint jt in joints.GetJoints.Values)
				{
					if (jt.BodyAUid == component.Puller || jt.BodyBUid == component.Puller)
					{
						return;
					}
				}
			}
			this._pullSm.ForceDisconnectPullable(component);
		}

		// Token: 0x06000649 RID: 1609 RVA: 0x0001699C File Offset: 0x00014B9C
		private void AddPullVerbs(EntityUid uid, SharedPullableComponent component, GetVerbsEvent<Verb> args)
		{
			if (!args.CanAccess || !args.CanInteract)
			{
				return;
			}
			if (args.User == args.Target)
			{
				return;
			}
			EntityUid? puller = component.Puller;
			EntityUid user = args.User;
			if (puller != null && (puller == null || puller.GetValueOrDefault() == user))
			{
				Verb verb = new Verb();
				verb.Text = Loc.GetString("pulling-verb-get-data-text-stop-pulling");
				verb.Act = delegate()
				{
					this.TryStopPull(component, new EntityUid?(args.User));
				};
				verb.DoContactInteraction = new bool?(false);
				args.Verbs.Add(verb);
				return;
			}
			if (this.CanPull(args.User, args.Target))
			{
				Verb verb2 = new Verb();
				verb2.Text = Loc.GetString("pulling-verb-get-data-text");
				verb2.Act = delegate()
				{
					this.TryStartPull(args.User, args.Target);
				};
				verb2.DoContactInteraction = new bool?(false);
				args.Verbs.Add(verb2);
			}
		}

		// Token: 0x0600064A RID: 1610 RVA: 0x00016AEC File Offset: 0x00014CEC
		private void PullableHandlePullStarted(EntityUid uid, SharedPullableComponent component, PullStartedMessage args)
		{
			if (args.Pulled.Owner != uid)
			{
				return;
			}
			this._alertsSystem.ShowAlert(component.Owner, AlertType.Pulled, null, null);
		}

		// Token: 0x0600064B RID: 1611 RVA: 0x00016B32 File Offset: 0x00014D32
		private void PullableHandlePullStopped(EntityUid uid, SharedPullableComponent component, PullStoppedMessage args)
		{
			if (args.Pulled.Owner != uid)
			{
				return;
			}
			this._alertsSystem.ClearAlert(component.Owner, AlertType.Pulled);
		}

		// Token: 0x0600064C RID: 1612 RVA: 0x00016B5B File Offset: 0x00014D5B
		[NullableContext(2)]
		public bool IsPulled(EntityUid uid, SharedPullableComponent component = null)
		{
			return base.Resolve<SharedPullableComponent>(uid, ref component, false) && component.BeingPulled;
		}

		// Token: 0x0600064D RID: 1613 RVA: 0x00016B71 File Offset: 0x00014D71
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			this._moving.ExceptWith(this._stoppedMoving);
			this._stoppedMoving.Clear();
		}

		// Token: 0x0600064E RID: 1614 RVA: 0x00016B96 File Offset: 0x00014D96
		public void Reset(RoundRestartCleanupEvent ev)
		{
			this._pullers.Clear();
			this._moving.Clear();
			this._stoppedMoving.Clear();
		}

		// Token: 0x0600064F RID: 1615 RVA: 0x00016BB9 File Offset: 0x00014DB9
		private void OnPullStarted(PullStartedMessage message)
		{
			this.SetPuller(message.Puller.Owner, message.Pulled.Owner);
		}

		// Token: 0x06000650 RID: 1616 RVA: 0x00016BD7 File Offset: 0x00014DD7
		private void OnPullStopped(PullStoppedMessage message)
		{
			this.RemovePuller(message.Puller.Owner);
		}

		// Token: 0x06000651 RID: 1617 RVA: 0x00016BEB File Offset: 0x00014DEB
		protected void OnPullableMove(EntityUid uid, SharedPullableComponent component, PullableMoveMessage args)
		{
			this._moving.Add(component);
		}

		// Token: 0x06000652 RID: 1618 RVA: 0x00016BFA File Offset: 0x00014DFA
		protected void OnPullableStopMove(EntityUid uid, SharedPullableComponent component, PullableStopMovingMessage args)
		{
			this._stoppedMoving.Add(component);
		}

		// Token: 0x06000653 RID: 1619 RVA: 0x00016C0C File Offset: 0x00014E0C
		private void HandleContainerInsert(EntInsertedIntoContainerMessage message)
		{
			SharedPullableComponent pullable;
			if (this.EntityManager.TryGetComponent<SharedPullableComponent>(message.Entity, ref pullable))
			{
				this.TryStopPull(pullable, null);
			}
			SharedPullerComponent puller;
			if (this.EntityManager.TryGetComponent<SharedPullerComponent>(message.Entity, ref puller))
			{
				if (puller.Pulling == null)
				{
					return;
				}
				SharedPullableComponent pulling;
				if (!this.EntityManager.TryGetComponent<SharedPullableComponent>(puller.Pulling.Value, ref pulling))
				{
					return;
				}
				this.TryStopPull(pulling, null);
			}
		}

		// Token: 0x06000654 RID: 1620 RVA: 0x00016C94 File Offset: 0x00014E94
		[NullableContext(2)]
		private bool HandleMovePulledObject(ICommonSession session, EntityCoordinates coords, EntityUid uid)
		{
			EntityUid? entityUid = (session != null) ? session.AttachedEntity : null;
			if (entityUid != null)
			{
				EntityUid player = entityUid.GetValueOrDefault();
				if (player.IsValid())
				{
					EntityUid? pulled;
					if (!this.TryGetPulled(player, out pulled))
					{
						return false;
					}
					SharedPullableComponent pullable;
					if (!this.EntityManager.TryGetComponent<SharedPullableComponent>(pulled.Value, ref pullable))
					{
						return false;
					}
					if (this._containerSystem.IsEntityInContainer(player, null) || this._gravity.IsWeightless(player, null, null))
					{
						return false;
					}
					this.TryMoveTo(pullable, coords);
					return false;
				}
			}
			return false;
		}

		// Token: 0x06000655 RID: 1621 RVA: 0x00016D23 File Offset: 0x00014F23
		private void SetPuller(EntityUid puller, EntityUid pulled)
		{
			this._pullers[puller] = pulled;
		}

		// Token: 0x06000656 RID: 1622 RVA: 0x00016D32 File Offset: 0x00014F32
		private bool RemovePuller(EntityUid puller)
		{
			return this._pullers.Remove(puller);
		}

		// Token: 0x06000657 RID: 1623 RVA: 0x00016D40 File Offset: 0x00014F40
		public EntityUid GetPulled(EntityUid by)
		{
			return this._pullers.GetValueOrDefault(by);
		}

		// Token: 0x06000658 RID: 1624 RVA: 0x00016D50 File Offset: 0x00014F50
		public bool TryGetPulled(EntityUid by, [NotNullWhen(true)] out EntityUid? pulled)
		{
			EntityUid? entityUid = pulled = new EntityUid?(this.GetPulled(by));
			return entityUid != null;
		}

		// Token: 0x06000659 RID: 1625 RVA: 0x00016D7A File Offset: 0x00014F7A
		public bool IsPulling(EntityUid puller)
		{
			return this._pullers.ContainsKey(puller);
		}

		// Token: 0x04000652 RID: 1618
		[Dependency]
		private readonly ActionBlockerSystem _blocker;

		// Token: 0x04000653 RID: 1619
		[Dependency]
		private readonly SharedContainerSystem _containerSystem;

		// Token: 0x04000654 RID: 1620
		[Dependency]
		private readonly SharedHandsSystem _handsSystem;

		// Token: 0x04000655 RID: 1621
		[Dependency]
		private readonly SharedInteractionSystem _interaction;

		// Token: 0x04000656 RID: 1622
		[Dependency]
		private readonly SharedPhysicsSystem _physics;

		// Token: 0x04000657 RID: 1623
		[Dependency]
		private readonly ISharedAdminLogManager _adminLogger;

		// Token: 0x04000658 RID: 1624
		[Dependency]
		private readonly SharedPullingStateManagementSystem _pullSm;

		// Token: 0x04000659 RID: 1625
		[Dependency]
		private readonly SharedGravitySystem _gravity;

		// Token: 0x0400065A RID: 1626
		[Dependency]
		private readonly AlertsSystem _alertsSystem;

		// Token: 0x0400065B RID: 1627
		private readonly Dictionary<EntityUid, EntityUid> _pullers = new Dictionary<EntityUid, EntityUid>();

		// Token: 0x0400065C RID: 1628
		private readonly HashSet<SharedPullableComponent> _moving = new HashSet<SharedPullableComponent>();

		// Token: 0x0400065D RID: 1629
		private readonly HashSet<SharedPullableComponent> _stoppedMoving = new HashSet<SharedPullableComponent>();
	}
}
