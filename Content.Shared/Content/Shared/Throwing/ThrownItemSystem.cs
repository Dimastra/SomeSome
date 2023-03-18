using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Hands.Components;
using Content.Shared.Physics.Pull;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;

namespace Content.Shared.Throwing
{
	// Token: 0x020000D9 RID: 217
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ThrownItemSystem : EntitySystem
	{
		// Token: 0x0600025A RID: 602 RVA: 0x0000B5B4 File Offset: 0x000097B4
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ThrownItemComponent, PhysicsSleepEvent>(new ComponentEventRefHandler<ThrownItemComponent, PhysicsSleepEvent>(this.OnSleep), null, null);
			base.SubscribeLocalEvent<ThrownItemComponent, StartCollideEvent>(new ComponentEventRefHandler<ThrownItemComponent, StartCollideEvent>(this.HandleCollision), null, null);
			base.SubscribeLocalEvent<ThrownItemComponent, PreventCollideEvent>(new ComponentEventRefHandler<ThrownItemComponent, PreventCollideEvent>(this.PreventCollision), null, null);
			base.SubscribeLocalEvent<ThrownItemComponent, ThrownEvent>(new ComponentEventHandler<ThrownItemComponent, ThrownEvent>(this.ThrowItem), null, null);
			base.SubscribeLocalEvent<ThrownItemComponent, ComponentGetState>(new ComponentEventRefHandler<ThrownItemComponent, ComponentGetState>(this.OnGetState), null, null);
			base.SubscribeLocalEvent<ThrownItemComponent, ComponentHandleState>(new ComponentEventRefHandler<ThrownItemComponent, ComponentHandleState>(this.OnHandleState), null, null);
			base.SubscribeLocalEvent<PullStartedMessage>(new EntityEventHandler<PullStartedMessage>(this.HandlePullStarted), null, null);
		}

		// Token: 0x0600025B RID: 603 RVA: 0x0000B653 File Offset: 0x00009853
		private void OnGetState(EntityUid uid, ThrownItemComponent component, ref ComponentGetState args)
		{
			args.State = new ThrownItemComponentState(component.Thrower);
		}

		// Token: 0x0600025C RID: 604 RVA: 0x0000B668 File Offset: 0x00009868
		private void OnHandleState(EntityUid uid, ThrownItemComponent component, ref ComponentHandleState args)
		{
			ThrownItemComponentState state = args.Current as ThrownItemComponentState;
			if (state == null || state.Thrower == null || !state.Thrower.Value.IsValid())
			{
				return;
			}
			component.Thrower = new EntityUid?(state.Thrower.Value);
		}

		// Token: 0x0600025D RID: 605 RVA: 0x0000B6C8 File Offset: 0x000098C8
		private void ThrowItem(EntityUid uid, ThrownItemComponent component, ThrownEvent args)
		{
			FixturesComponent fixturesComponent;
			PhysicsComponent body;
			if (!this.EntityManager.TryGetComponent<FixturesComponent>(uid, ref fixturesComponent) || fixturesComponent.Fixtures.Count != 1 || !base.TryComp<PhysicsComponent>(uid, ref body))
			{
				return;
			}
			IPhysShape shape = fixturesComponent.Fixtures.Values.First<Fixture>().Shape;
			this._fixtures.TryCreateFixture(uid, shape, "throw-fixture", 1f, false, 0, 74, 0.4f, 0f, true, fixturesComponent, body, null);
		}

		// Token: 0x0600025E RID: 606 RVA: 0x0000B740 File Offset: 0x00009940
		private void HandleCollision(EntityUid uid, ThrownItemComponent component, ref StartCollideEvent args)
		{
			if (!args.OtherFixture.Hard)
			{
				return;
			}
			EntityUid? thrower = component.Thrower;
			PhysicsComponent otherBody = args.OtherFixture.Body;
			if (otherBody.Owner == thrower)
			{
				return;
			}
			this.ThrowCollideInteraction(thrower, args.OurFixture.Body, otherBody);
		}

		// Token: 0x0600025F RID: 607 RVA: 0x0000B7A8 File Offset: 0x000099A8
		private void PreventCollision(EntityUid uid, ThrownItemComponent component, ref PreventCollideEvent args)
		{
			if (args.BodyB.Owner == component.Thrower)
			{
				args.Cancelled = true;
			}
		}

		// Token: 0x06000260 RID: 608 RVA: 0x0000B7EA File Offset: 0x000099EA
		private void OnSleep(EntityUid uid, ThrownItemComponent thrownItem, ref PhysicsSleepEvent @event)
		{
			this.StopThrow(uid, thrownItem);
		}

		// Token: 0x06000261 RID: 609 RVA: 0x0000B7F4 File Offset: 0x000099F4
		private void HandlePullStarted(PullStartedMessage message)
		{
			ThrownItemComponent thrownItemComponent;
			if (this.EntityManager.TryGetComponent<ThrownItemComponent>(message.Pulled.Owner, ref thrownItemComponent))
			{
				this.StopThrow(message.Pulled.Owner, thrownItemComponent);
			}
		}

		// Token: 0x06000262 RID: 610 RVA: 0x0000B830 File Offset: 0x00009A30
		private void StopThrow(EntityUid uid, ThrownItemComponent thrownItemComponent)
		{
			FixturesComponent manager;
			if (this.EntityManager.TryGetComponent<FixturesComponent>(uid, ref manager))
			{
				Fixture fixture = this._fixtures.GetFixtureOrNull(uid, "throw-fixture", manager);
				if (fixture != null)
				{
					this._fixtures.DestroyFixture(uid, fixture, true, null, manager, null);
				}
			}
			this.EntityManager.EventBus.RaiseLocalEvent<StopThrowEvent>(uid, new StopThrowEvent
			{
				User = thrownItemComponent.Thrower
			}, true);
			this.EntityManager.RemoveComponent<ThrownItemComponent>(uid);
		}

		// Token: 0x06000263 RID: 611 RVA: 0x0000B8A4 File Offset: 0x00009AA4
		public void LandComponent(ThrownItemComponent thrownItem, PhysicsComponent physics)
		{
			this._physics.SetBodyStatus(physics, 0, true);
			if (thrownItem.Deleted || base.Deleted(thrownItem.Owner, null) || this._containerSystem.IsEntityInContainer(thrownItem.Owner, null))
			{
				return;
			}
			EntityUid landing = thrownItem.Owner;
			IContainerManager containerManager;
			if (ContainerHelpers.TryGetContainerMan(thrownItem.Owner, ref containerManager, null) && this.EntityManager.HasComponent<SharedHandsComponent>(containerManager.Owner))
			{
				this.EntityManager.RemoveComponent(landing, thrownItem);
				return;
			}
			if (thrownItem.Thrower != null)
			{
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.Landed;
				LogImpact impact = LogImpact.Low;
				LogStringHandler logStringHandler = new LogStringHandler(19, 2);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(landing), "entity", "ToPrettyString(landing)");
				logStringHandler.AppendLiteral(" thrown by ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(thrownItem.Thrower.Value), "thrower", "ToPrettyString(thrownItem.Thrower.Value)");
				logStringHandler.AppendLiteral(" landed.");
				adminLogger.Add(type, impact, ref logStringHandler);
			}
			this._broadphase.RegenerateContacts(physics, null, null);
			LandEvent landEvent = new LandEvent(thrownItem.Thrower);
			base.RaiseLocalEvent<LandEvent>(landing, ref landEvent, false);
		}

		// Token: 0x06000264 RID: 612 RVA: 0x0000B9C8 File Offset: 0x00009BC8
		public void ThrowCollideInteraction(EntityUid? user, PhysicsComponent thrown, PhysicsComponent target)
		{
			if (user != null)
			{
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.ThrowHit;
				LogImpact impact = LogImpact.Low;
				LogStringHandler logStringHandler = new LogStringHandler(17, 3);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(thrown.Owner), "thrown", "ToPrettyString(thrown.Owner)");
				logStringHandler.AppendLiteral(" thrown by ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(user.Value), "thrower", "ToPrettyString(user.Value)");
				logStringHandler.AppendLiteral(" hit ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(target.Owner), "target", "ToPrettyString(target.Owner)");
				logStringHandler.AppendLiteral(".");
				adminLogger.Add(type, impact, ref logStringHandler);
			}
			base.RaiseLocalEvent<ThrowHitByEvent>(target.Owner, new ThrowHitByEvent(user, thrown.Owner, target.Owner), true);
			base.RaiseLocalEvent<ThrowDoHitEvent>(thrown.Owner, new ThrowDoHitEvent(user, thrown.Owner, target.Owner), true);
		}

		// Token: 0x040002C5 RID: 709
		[Dependency]
		private readonly ISharedAdminLogManager _adminLogger;

		// Token: 0x040002C6 RID: 710
		[Dependency]
		private readonly SharedBroadphaseSystem _broadphase;

		// Token: 0x040002C7 RID: 711
		[Dependency]
		private readonly SharedContainerSystem _containerSystem;

		// Token: 0x040002C8 RID: 712
		[Dependency]
		private readonly FixtureSystem _fixtures;

		// Token: 0x040002C9 RID: 713
		[Dependency]
		private readonly SharedPhysicsSystem _physics;

		// Token: 0x040002CA RID: 714
		private const string ThrowingFixture = "throw-fixture";
	}
}
