using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Server.Body.Systems;
using Content.Server.Climbing.Components;
using Content.Server.DoAfter;
using Content.Server.Interaction;
using Content.Server.Popups;
using Content.Server.Stunnable;
using Content.Shared.ActionBlocker;
using Content.Shared.Body.Components;
using Content.Shared.Body.Part;
using Content.Shared.Buckle.Components;
using Content.Shared.Climbing;
using Content.Shared.Climbing.Events;
using Content.Shared.Damage;
using Content.Shared.DoAfter;
using Content.Shared.DragDrop;
using Content.Shared.GameTicking;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;

namespace Content.Server.Climbing
{
	// Token: 0x02000646 RID: 1606
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ClimbSystem : SharedClimbSystem
	{
		// Token: 0x06002215 RID: 8725 RVA: 0x000B20D0 File Offset: 0x000B02D0
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<RoundRestartCleanupEvent>(new EntityEventHandler<RoundRestartCleanupEvent>(this.Reset), null, null);
			base.SubscribeLocalEvent<ClimbableComponent, GetVerbsEvent<AlternativeVerb>>(new ComponentEventHandler<ClimbableComponent, GetVerbsEvent<AlternativeVerb>>(this.AddClimbableVerb), null, null);
			base.SubscribeLocalEvent<ClimbableComponent, DragDropTargetEvent>(new ComponentEventRefHandler<ClimbableComponent, DragDropTargetEvent>(this.OnClimbableDragDrop), null, null);
			base.SubscribeLocalEvent<ClimbingComponent, DoAfterEvent<ClimbSystem.ClimbExtraEvent>>(new ComponentEventHandler<ClimbingComponent, DoAfterEvent<ClimbSystem.ClimbExtraEvent>>(this.OnDoAfter), null, null);
			base.SubscribeLocalEvent<ClimbingComponent, EndCollideEvent>(new ComponentEventRefHandler<ClimbingComponent, EndCollideEvent>(this.OnClimbEndCollide), null, null);
			base.SubscribeLocalEvent<ClimbingComponent, BuckleChangeEvent>(new ComponentEventHandler<ClimbingComponent, BuckleChangeEvent>(this.OnBuckleChange), null, null);
			ComponentEventRefHandler<ClimbingComponent, ComponentGetState> componentEventRefHandler;
			if ((componentEventRefHandler = ClimbSystem.<>O.<0>__OnClimbingGetState) == null)
			{
				componentEventRefHandler = (ClimbSystem.<>O.<0>__OnClimbingGetState = new ComponentEventRefHandler<ClimbingComponent, ComponentGetState>(ClimbSystem.OnClimbingGetState));
			}
			base.SubscribeLocalEvent<ClimbingComponent, ComponentGetState>(componentEventRefHandler, null, null);
			base.SubscribeLocalEvent<GlassTableComponent, ClimbedOnEvent>(new ComponentEventHandler<GlassTableComponent, ClimbedOnEvent>(this.OnGlassClimbed), null, null);
		}

		// Token: 0x06002216 RID: 8726 RVA: 0x000B2194 File Offset: 0x000B0394
		protected override void OnCanDragDropOn(EntityUid uid, ClimbableComponent component, ref CanDropTargetEvent args)
		{
			base.OnCanDragDropOn(uid, component, ref args);
			if (!args.CanDrop)
			{
				return;
			}
			string reason;
			bool canVault = (args.User == args.Dragged) ? this.CanVault(component, args.User, uid, out reason) : this.CanVault(component, args.User, args.Dragged, uid, out reason);
			if (!canVault)
			{
				this._popupSystem.PopupEntity(reason, args.User, args.User, PopupType.Small);
			}
			args.CanDrop = canVault;
			args.Handled = true;
		}

		// Token: 0x06002217 RID: 8727 RVA: 0x000B221C File Offset: 0x000B041C
		private void AddClimbableVerb(EntityUid uid, ClimbableComponent component, GetVerbsEvent<AlternativeVerb> args)
		{
			if (!args.CanAccess || !args.CanInteract || !this._actionBlockerSystem.CanMove(args.User, null))
			{
				return;
			}
			ClimbingComponent climbingComponent;
			if (!base.TryComp<ClimbingComponent>(args.User, ref climbingComponent) || climbingComponent.IsClimbing)
			{
				return;
			}
			args.Verbs.Add(new AlternativeVerb
			{
				Act = delegate()
				{
					this.TryMoveEntity(component, args.User, args.User, args.Target);
				},
				Text = Loc.GetString("comp-climbable-verb-climb")
			});
		}

		// Token: 0x06002218 RID: 8728 RVA: 0x000B22CE File Offset: 0x000B04CE
		private void OnClimbableDragDrop(EntityUid uid, ClimbableComponent component, ref DragDropTargetEvent args)
		{
			this.TryMoveEntity(component, args.User, args.Dragged, uid);
		}

		// Token: 0x06002219 RID: 8729 RVA: 0x000B22E4 File Offset: 0x000B04E4
		private void TryMoveEntity(ClimbableComponent component, EntityUid user, EntityUid entityToMove, EntityUid climbable)
		{
			ClimbingComponent climbingComponent;
			if (!base.TryComp<ClimbingComponent>(entityToMove, ref climbingComponent) || climbingComponent.IsClimbing)
			{
				return;
			}
			if (this._bonkSystem.TryBonk(entityToMove, climbable, null))
			{
				return;
			}
			ClimbSystem.ClimbExtraEvent ev = new ClimbSystem.ClimbExtraEvent();
			float climbDelay = component.ClimbDelay;
			EntityUid? target = new EntityUid?(climbable);
			EntityUid? used = new EntityUid?(entityToMove);
			DoAfterEventArgs args = new DoAfterEventArgs(user, climbDelay, default(CancellationToken), target, used)
			{
				BreakOnTargetMove = true,
				BreakOnUserMove = true,
				BreakOnDamage = true,
				BreakOnStun = true,
				RaiseOnUser = (user == entityToMove),
				RaiseOnTarget = (user != entityToMove)
			};
			this._doAfterSystem.DoAfter<ClimbSystem.ClimbExtraEvent>(args, ev);
		}

		// Token: 0x0600221A RID: 8730 RVA: 0x000B238C File Offset: 0x000B058C
		private void OnDoAfter(EntityUid uid, ClimbingComponent component, DoAfterEvent<ClimbSystem.ClimbExtraEvent> args)
		{
			if (args.Handled || args.Cancelled || args.Args.Target == null || args.Args.Used == null)
			{
				return;
			}
			this.Climb(uid, args.Args.User, args.Args.Used.Value, args.Args.Target.Value, false, component, null, null);
			args.Handled = true;
		}

		// Token: 0x0600221B RID: 8731 RVA: 0x000B240C File Offset: 0x000B060C
		[NullableContext(2)]
		private void Climb(EntityUid uid, EntityUid user, EntityUid instigator, EntityUid climbable, bool silent = false, ClimbingComponent climbing = null, PhysicsComponent physics = null, FixturesComponent fixtures = null)
		{
			if (!base.Resolve<ClimbingComponent, PhysicsComponent, FixturesComponent>(uid, ref climbing, ref physics, ref fixtures, false))
			{
				return;
			}
			if (!this.ReplaceFixtures(climbing, fixtures))
			{
				return;
			}
			climbing.IsClimbing = true;
			base.Dirty(climbing, null);
			this.MoveEntityToward(uid, climbable, physics, climbing);
			base.RaiseLocalEvent<StartClimbEvent>(uid, new StartClimbEvent(climbable), false);
			base.RaiseLocalEvent<ClimbedOnEvent>(climbable, new ClimbedOnEvent(uid, user), false);
			if (silent)
			{
				return;
			}
			if (user == uid)
			{
				string othersMessage = Loc.GetString("comp-climbable-user-climbs-other", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("user", Identity.Entity(uid, this.EntityManager)),
					new ValueTuple<string, object>("climbable", climbable)
				});
				uid.PopupMessageOtherClients(othersMessage);
				string selfMessage = Loc.GetString("comp-climbable-user-climbs", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("climbable", climbable)
				});
				uid.PopupMessage(selfMessage);
				return;
			}
			string othersMessage2 = Loc.GetString("comp-climbable-user-climbs-force-other", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("user", Identity.Entity(user, this.EntityManager)),
				new ValueTuple<string, object>("moved-user", Identity.Entity(uid, this.EntityManager)),
				new ValueTuple<string, object>("climbable", climbable)
			});
			user.PopupMessageOtherClients(othersMessage2);
			string selfMessage2 = Loc.GetString("comp-climbable-user-climbs-force", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("moved-user", Identity.Entity(uid, this.EntityManager)),
				new ValueTuple<string, object>("climbable", climbable)
			});
			user.PopupMessage(selfMessage2);
		}

		// Token: 0x0600221C RID: 8732 RVA: 0x000B25D0 File Offset: 0x000B07D0
		private bool ReplaceFixtures(ClimbingComponent climbingComp, FixturesComponent fixturesComp)
		{
			EntityUid uid = climbingComp.Owner;
			foreach (KeyValuePair<string, Fixture> keyValuePair in fixturesComp.Fixtures)
			{
				string text;
				Fixture fixture2;
				keyValuePair.Deconstruct(out text, out fixture2);
				string name = text;
				Fixture fixture = fixture2;
				if (!climbingComp.DisabledFixtureMasks.ContainsKey(name) && fixture.Hard && (fixture.CollisionMask & 20) != 0)
				{
					climbingComp.DisabledFixtureMasks.Add(fixture.ID, fixture.CollisionMask & 20);
					this._physics.SetCollisionMask(uid, fixture, fixture.CollisionMask & -21, fixturesComp, null);
				}
			}
			return this._fixtureSystem.TryCreateFixture(uid, new PhysShapeCircle(0.35f), "climb", 1f, false, 0, 20, 0.4f, 0f, true, fixturesComp, null, null);
		}

		// Token: 0x0600221D RID: 8733 RVA: 0x000B26C0 File Offset: 0x000B08C0
		private void OnClimbEndCollide(EntityUid uid, ClimbingComponent component, ref EndCollideEvent args)
		{
			if (args.OurFixture.ID != "climb" || !component.IsClimbing || component.OwnerIsTransitioning)
			{
				return;
			}
			foreach (Fixture fixture in args.OurFixture.Contacts.Keys)
			{
				if (fixture != args.OtherFixture && base.HasComp<ClimbableComponent>(fixture.Body.Owner))
				{
					return;
				}
			}
			this.StopClimb(uid, component, null);
		}

		// Token: 0x0600221E RID: 8734 RVA: 0x000B2768 File Offset: 0x000B0968
		[NullableContext(2)]
		private void StopClimb(EntityUid uid, ClimbingComponent climbing = null, FixturesComponent fixtures = null)
		{
			if (!base.Resolve<ClimbingComponent, FixturesComponent>(uid, ref climbing, ref fixtures, false))
			{
				return;
			}
			foreach (KeyValuePair<string, int> keyValuePair in climbing.DisabledFixtureMasks)
			{
				string text;
				int num;
				keyValuePair.Deconstruct(out text, out num);
				string name = text;
				int fixtureMask = num;
				Fixture fixture;
				if (fixtures.Fixtures.TryGetValue(name, out fixture))
				{
					this._physics.SetCollisionMask(uid, fixture, fixture.CollisionMask | fixtureMask, fixtures, null);
				}
			}
			climbing.DisabledFixtureMasks.Clear();
			List<Fixture> removeQueue;
			if (!this._fixtureRemoveQueue.TryGetValue(uid, out removeQueue))
			{
				removeQueue = new List<Fixture>();
				this._fixtureRemoveQueue.Add(uid, removeQueue);
			}
			Fixture climbingFixture;
			if (fixtures.Fixtures.TryGetValue("climb", out climbingFixture))
			{
				removeQueue.Add(climbingFixture);
			}
			climbing.IsClimbing = false;
			climbing.OwnerIsTransitioning = false;
			EndClimbEvent ev = default(EndClimbEvent);
			base.RaiseLocalEvent<EndClimbEvent>(uid, ref ev, false);
			base.Dirty(climbing, null);
		}

		// Token: 0x0600221F RID: 8735 RVA: 0x000B2874 File Offset: 0x000B0A74
		private bool CanVault(ClimbableComponent component, EntityUid user, EntityUid target, out string reason)
		{
			if (!this._actionBlockerSystem.CanInteract(user, new EntityUid?(target)))
			{
				reason = Loc.GetString("comp-climbable-cant-interact");
				return false;
			}
			BodyComponent body;
			if (!base.HasComp<ClimbingComponent>(user) || !base.TryComp<BodyComponent>(user, ref body) || !this._bodySystem.BodyHasChildOfType(new EntityUid?(user), BodyPartType.Leg, body) || !this._bodySystem.BodyHasChildOfType(new EntityUid?(user), BodyPartType.Foot, body))
			{
				reason = Loc.GetString("comp-climbable-cant-climb");
				return false;
			}
			if (!this._interactionSystem.InRangeUnobstructed(user, target, component.Range, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, false))
			{
				reason = Loc.GetString("comp-climbable-cant-reach");
				return false;
			}
			reason = string.Empty;
			return true;
		}

		// Token: 0x06002220 RID: 8736 RVA: 0x000B2928 File Offset: 0x000B0B28
		private bool CanVault(ClimbableComponent component, EntityUid user, EntityUid dragged, EntityUid target, out string reason)
		{
			ClimbSystem.<>c__DisplayClass24_0 CS$<>8__locals1 = new ClimbSystem.<>c__DisplayClass24_0();
			CS$<>8__locals1.target = target;
			CS$<>8__locals1.user = user;
			CS$<>8__locals1.dragged = dragged;
			if (!this._actionBlockerSystem.CanInteract(CS$<>8__locals1.user, new EntityUid?(CS$<>8__locals1.dragged)) || !this._actionBlockerSystem.CanInteract(CS$<>8__locals1.user, new EntityUid?(CS$<>8__locals1.target)))
			{
				reason = Loc.GetString("comp-climbable-cant-interact");
				return false;
			}
			if (!base.HasComp<ClimbingComponent>(CS$<>8__locals1.dragged))
			{
				reason = Loc.GetString("comp-climbable-cant-climb");
				return false;
			}
			if (!this._interactionSystem.InRangeUnobstructed(CS$<>8__locals1.user, CS$<>8__locals1.target, component.Range, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, new SharedInteractionSystem.Ignored(CS$<>8__locals1.<CanVault>g__Ignored|0), false) || !this._interactionSystem.InRangeUnobstructed(CS$<>8__locals1.user, CS$<>8__locals1.dragged, component.Range, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, new SharedInteractionSystem.Ignored(CS$<>8__locals1.<CanVault>g__Ignored|0), false))
			{
				reason = Loc.GetString("comp-climbable-cant-reach");
				return false;
			}
			reason = string.Empty;
			return true;
		}

		// Token: 0x06002221 RID: 8737 RVA: 0x000B2A34 File Offset: 0x000B0C34
		[NullableContext(2)]
		public void ForciblySetClimbing(EntityUid uid, EntityUid climbable, ClimbingComponent component = null)
		{
			this.Climb(uid, uid, uid, climbable, true, component, null, null);
		}

		// Token: 0x06002222 RID: 8738 RVA: 0x000B2A4F File Offset: 0x000B0C4F
		private void OnBuckleChange(EntityUid uid, ClimbingComponent component, BuckleChangeEvent args)
		{
			if (!args.Buckling)
			{
				return;
			}
			this.StopClimb(uid, component, null);
		}

		// Token: 0x06002223 RID: 8739 RVA: 0x000B2A63 File Offset: 0x000B0C63
		private static void OnClimbingGetState(EntityUid uid, ClimbingComponent component, ref ComponentGetState args)
		{
			args.State = new ClimbingComponent.ClimbModeComponentState(component.IsClimbing, component.OwnerIsTransitioning);
		}

		// Token: 0x06002224 RID: 8740 RVA: 0x000B2A7C File Offset: 0x000B0C7C
		private void OnGlassClimbed(EntityUid uid, GlassTableComponent component, ClimbedOnEvent args)
		{
			PhysicsComponent physics;
			if (base.TryComp<PhysicsComponent>(args.Climber, ref physics) && physics.Mass <= component.MassLimit)
			{
				return;
			}
			this._damageableSystem.TryChangeDamage(new EntityUid?(args.Climber), component.ClimberDamage, false, true, null, new EntityUid?(args.Climber));
			this._damageableSystem.TryChangeDamage(new EntityUid?(uid), component.TableDamage, false, true, null, new EntityUid?(args.Climber));
			this._stunSystem.TryParalyze(args.Climber, TimeSpan.FromSeconds((double)component.StunTime), true, null);
			this._popupSystem.PopupEntity(Loc.GetString("glass-table-shattered-others", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("table", uid),
				new ValueTuple<string, object>("climber", Identity.Entity(args.Climber, this.EntityManager))
			}), args.Climber, Filter.PvsExcept(args.Climber, 2f, null), true, PopupType.Small);
		}

		// Token: 0x06002225 RID: 8741 RVA: 0x000B2B8C File Offset: 0x000B0D8C
		[NullableContext(2)]
		public void MoveEntityToward(EntityUid uid, EntityUid target, PhysicsComponent physics = null, ClimbingComponent climbing = null)
		{
			if (!base.Resolve<PhysicsComponent, ClimbingComponent>(uid, ref physics, ref climbing, false))
			{
				return;
			}
			Vector2 from = base.Transform(uid).WorldPosition;
			Vector2 to = base.Transform(target).WorldPosition;
			float num;
			float num2;
			(to - from).Normalized.Deconstruct(ref num, ref num2);
			float x = num;
			float y = num2;
			if (MathF.Abs(x) < 0.6f)
			{
				to..ctor(from.X, to.Y);
			}
			else if (MathF.Abs(y) < 0.6f)
			{
				to..ctor(to.X, from.Y);
			}
			float velocity = (to - from).Length;
			if (velocity <= 0f)
			{
				return;
			}
			this._physics.ApplyLinearImpulse(uid, (to - from).Normalized * velocity * physics.Mass * 10f, null, physics);
			this._physics.SetBodyType(uid, 8, null, physics, null);
			climbing.OwnerIsTransitioning = true;
			this._actionBlockerSystem.UpdateCanMove(uid, null);
			TimerExtensions.SpawnTimer(climbing.Owner, 300, delegate()
			{
				if (climbing.Deleted)
				{
					return;
				}
				this._physics.SetBodyType(uid, 2, null, null, null);
				climbing.OwnerIsTransitioning = false;
				this._actionBlockerSystem.UpdateCanMove(uid, null);
			}, default(CancellationToken));
		}

		// Token: 0x06002226 RID: 8742 RVA: 0x000B2D0C File Offset: 0x000B0F0C
		public override void Update(float frameTime)
		{
			foreach (KeyValuePair<EntityUid, List<Fixture>> keyValuePair in this._fixtureRemoveQueue)
			{
				EntityUid entityUid;
				List<Fixture> list;
				keyValuePair.Deconstruct(out entityUid, out list);
				EntityUid uid = entityUid;
				List<Fixture> fixtures = list;
				PhysicsComponent physicsComp;
				FixturesComponent fixturesComp;
				if (base.TryComp<PhysicsComponent>(uid, ref physicsComp) && base.TryComp<FixturesComponent>(uid, ref fixturesComp))
				{
					foreach (Fixture fixture in fixtures)
					{
						this._fixtureSystem.DestroyFixture(uid, fixture, true, physicsComp, fixturesComp, null);
					}
				}
			}
			this._fixtureRemoveQueue.Clear();
		}

		// Token: 0x06002227 RID: 8743 RVA: 0x000B2DD8 File Offset: 0x000B0FD8
		private void Reset(RoundRestartCleanupEvent ev)
		{
			this._fixtureRemoveQueue.Clear();
		}

		// Token: 0x04001506 RID: 5382
		[Dependency]
		private readonly ActionBlockerSystem _actionBlockerSystem;

		// Token: 0x04001507 RID: 5383
		[Dependency]
		private readonly BodySystem _bodySystem;

		// Token: 0x04001508 RID: 5384
		[Dependency]
		private readonly DamageableSystem _damageableSystem;

		// Token: 0x04001509 RID: 5385
		[Dependency]
		private readonly DoAfterSystem _doAfterSystem;

		// Token: 0x0400150A RID: 5386
		[Dependency]
		private readonly FixtureSystem _fixtureSystem;

		// Token: 0x0400150B RID: 5387
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x0400150C RID: 5388
		[Dependency]
		private readonly InteractionSystem _interactionSystem;

		// Token: 0x0400150D RID: 5389
		[Dependency]
		private readonly StunSystem _stunSystem;

		// Token: 0x0400150E RID: 5390
		[Dependency]
		private readonly SharedPhysicsSystem _physics;

		// Token: 0x0400150F RID: 5391
		[Dependency]
		private readonly BonkSystem _bonkSystem;

		// Token: 0x04001510 RID: 5392
		private const string ClimbingFixtureName = "climb";

		// Token: 0x04001511 RID: 5393
		private const int ClimbingCollisionGroup = 20;

		// Token: 0x04001512 RID: 5394
		private readonly Dictionary<EntityUid, List<Fixture>> _fixtureRemoveQueue = new Dictionary<EntityUid, List<Fixture>>();

		// Token: 0x02000AEB RID: 2795
		[NullableContext(0)]
		private sealed class ClimbExtraEvent : EntityEventArgs
		{
		}

		// Token: 0x02000AEC RID: 2796
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04002881 RID: 10369
			[Nullable(new byte[]
			{
				0,
				1
			})]
			public static ComponentEventRefHandler<ClimbingComponent, ComponentGetState> <0>__OnClimbingGetState;
		}
	}
}
