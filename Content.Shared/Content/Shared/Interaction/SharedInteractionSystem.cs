using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.ActionBlocker;
using Content.Shared.Administration.Logs;
using Content.Shared.CombatMode;
using Content.Shared.Database;
using Content.Shared.Ghost;
using Content.Shared.Hands.Components;
using Content.Shared.Input;
using Content.Shared.Interaction.Components;
using Content.Shared.Interaction.Events;
using Content.Shared.Item;
using Content.Shared.Movement.Components;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.Pulling;
using Content.Shared.Pulling.Components;
using Content.Shared.Throwing;
using Content.Shared.Timing;
using Content.Shared.Verbs;
using Content.Shared.Wall;
using Content.Shared.White.MeatyOre;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Players;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Shared.Interaction
{
	// Token: 0x020003CE RID: 974
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedInteractionSystem : EntitySystem
	{
		// Token: 0x06000B32 RID: 2866 RVA: 0x0002490C File Offset: 0x00022B0C
		public bool RollClumsy(ClumsyComponent component, float chance)
		{
			return component.Running && RandomExtensions.Prob(this._random, chance);
		}

		// Token: 0x06000B33 RID: 2867 RVA: 0x00024924 File Offset: 0x00022B24
		[NullableContext(2)]
		public bool TryRollClumsy(EntityUid entity, float chance, ClumsyComponent component = null)
		{
			return base.Resolve<ClumsyComponent>(entity, ref component, false) && this.RollClumsy(component, chance);
		}

		// Token: 0x06000B34 RID: 2868 RVA: 0x0002493C File Offset: 0x00022B3C
		public override void Initialize()
		{
			base.SubscribeLocalEvent<BoundUserInterfaceMessageAttempt>(new EntityEventHandler<BoundUserInterfaceMessageAttempt>(this.OnBoundInterfaceInteractAttempt), null, null);
			base.SubscribeAllEvent<InteractInventorySlotEvent>(new EntitySessionEventHandler<InteractInventorySlotEvent>(this.HandleInteractInventorySlotEvent), null, null);
			base.SubscribeLocalEvent<UnremoveableComponent, ContainerGettingRemovedAttemptEvent>(new ComponentEventHandler<UnremoveableComponent, ContainerGettingRemovedAttemptEvent>(this.OnRemoveAttempt), null, null);
			CommandBinds.Builder.Bind(ContentKeyFunctions.AltActivateItemInWorld, new PointerInputCmdHandler(new PointerInputCmdDelegate(this.HandleAltUseInteraction), true, false)).Bind(EngineKeyFunctions.Use, new PointerInputCmdHandler(new PointerInputCmdDelegate(this.HandleUseInteraction), true, false)).Bind(ContentKeyFunctions.ActivateItemInWorld, new PointerInputCmdHandler(new PointerInputCmdDelegate(this.HandleActivateItemInWorld), true, false)).Bind(ContentKeyFunctions.TryPullObject, new PointerInputCmdHandler(new PointerInputCmdDelegate(this.HandleTryPullObject), true, false)).Register<SharedInteractionSystem>();
			this.InitializeRelay();
		}

		// Token: 0x06000B35 RID: 2869 RVA: 0x00024A0A File Offset: 0x00022C0A
		public override void Shutdown()
		{
			CommandBinds.Unregister<SharedInteractionSystem>();
			base.Shutdown();
		}

		// Token: 0x06000B36 RID: 2870 RVA: 0x00024A18 File Offset: 0x00022C18
		private void OnBoundInterfaceInteractAttempt(BoundUserInterfaceMessageAttempt ev)
		{
			EntityUid? attachedEntity = ev.Sender.AttachedEntity;
			if (attachedEntity != null)
			{
				EntityUid user = attachedEntity.GetValueOrDefault();
				if (this._actionBlockerSystem.CanInteract(user, new EntityUid?(ev.Target)))
				{
					if (!this._containerSystem.IsInSameOrParentContainer(user, ev.Target) && !this.CanAccessViaStorage(user, ev.Target))
					{
						ev.Cancel();
						return;
					}
					if (base.CompOrNull<IgnorBUIInteractionRangeComponent>(ev.Target) != null)
					{
						return;
					}
					if (!this.InRangeUnobstructed(user, ev.Target, 1.5f, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, false))
					{
						ev.Cancel();
					}
					return;
				}
			}
			ev.Cancel();
		}

		// Token: 0x06000B37 RID: 2871 RVA: 0x00024ABC File Offset: 0x00022CBC
		private void OnRemoveAttempt(EntityUid uid, UnremoveableComponent item, ContainerGettingRemovedAttemptEvent args)
		{
			args.Cancel();
		}

		// Token: 0x06000B38 RID: 2872 RVA: 0x00024AC4 File Offset: 0x00022CC4
		[NullableContext(2)]
		private bool HandleTryPullObject(ICommonSession session, EntityCoordinates coords, EntityUid uid)
		{
			EntityUid? userEntity;
			if (!this.ValidateClientInput(session, coords, uid, out userEntity))
			{
				Logger.InfoS("system.interaction", "TryPullObject input validation failed");
				return true;
			}
			if (userEntity.Value == uid)
			{
				return false;
			}
			if (base.Deleted(uid, null))
			{
				return false;
			}
			if (!this.InRangeUnobstructed(userEntity.Value, uid, 1.5f, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, true))
			{
				return false;
			}
			SharedPullableComponent pull;
			if (!base.TryComp<SharedPullableComponent>(uid, ref pull))
			{
				return false;
			}
			this._pullSystem.TogglePull(userEntity.Value, pull);
			return false;
		}

		// Token: 0x06000B39 RID: 2873 RVA: 0x00024B4C File Offset: 0x00022D4C
		private void HandleInteractInventorySlotEvent(InteractInventorySlotEvent msg, EntitySessionEventArgs args)
		{
			TransformComponent itemXform;
			EntityUid? user;
			if (!base.TryComp<TransformComponent>(msg.ItemUid, ref itemXform) || !this.ValidateClientInput(args.SenderSession, itemXform.Coordinates, msg.ItemUid, out user))
			{
				string text = "system.interaction";
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(50, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Inventory interaction validation failed.  Session=");
				defaultInterpolatedStringHandler.AppendFormatted<ICommonSession>(args.SenderSession);
				Logger.InfoS(text, defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			if (msg.AltInteract)
			{
				this.UserInteraction(user.Value, itemXform.Coordinates, new EntityUid?(msg.ItemUid), msg.AltInteract, true, true, true);
				return;
			}
			this.InteractionActivate(user.Value, msg.ItemUid, true, true, true);
		}

		// Token: 0x06000B3A RID: 2874 RVA: 0x00024C04 File Offset: 0x00022E04
		[NullableContext(2)]
		public bool HandleAltUseInteraction(ICommonSession session, EntityCoordinates coords, EntityUid uid)
		{
			EntityUid? user;
			if (!this.ValidateClientInput(session, coords, uid, out user))
			{
				Logger.InfoS("system.interaction", "Alt-use input validation failed");
				return true;
			}
			this.UserInteraction(user.Value, coords, new EntityUid?(uid), true, true, this.ShouldCheckAccess(user.Value), true);
			return false;
		}

		// Token: 0x06000B3B RID: 2875 RVA: 0x00024C54 File Offset: 0x00022E54
		[NullableContext(2)]
		public bool HandleUseInteraction(ICommonSession session, EntityCoordinates coords, EntityUid uid)
		{
			EntityUid? userEntity;
			if (!this.ValidateClientInput(session, coords, uid, out userEntity))
			{
				Logger.InfoS("system.interaction", "Use input validation failed");
				return true;
			}
			this.UserInteraction(userEntity.Value, coords, (!base.Deleted(uid, null)) ? new EntityUid?(uid) : null, false, true, this.ShouldCheckAccess(userEntity.Value), true);
			return false;
		}

		// Token: 0x06000B3C RID: 2876 RVA: 0x00024CB9 File Offset: 0x00022EB9
		private bool ShouldCheckAccess(EntityUid user)
		{
			return !base.HasComp<SharedGhostComponent>(user);
		}

		// Token: 0x06000B3D RID: 2877 RVA: 0x00024CC8 File Offset: 0x00022EC8
		public void UserInteraction(EntityUid user, EntityCoordinates coordinates, EntityUid? target, bool altInteract = false, bool checkCanInteract = true, bool checkAccess = true, bool checkCanUse = true)
		{
			InteractionRelayComponent relay;
			EntityUid? entityUid;
			if (base.TryComp<InteractionRelayComponent>(user, ref relay))
			{
				entityUid = relay.RelayEntity;
				if (entityUid != null)
				{
					this.UserInteraction(relay.RelayEntity.Value, coordinates, target, altInteract, checkCanInteract, checkAccess, checkCanUse);
				}
			}
			if (target != null && base.Deleted(target.Value, null))
			{
				return;
			}
			SharedCombatModeComponent combatMode;
			if (!altInteract && base.TryComp<SharedCombatModeComponent>(user, ref combatMode) && combatMode.IsInCombatMode)
			{
				return;
			}
			if (!this.ValidateInteractAndFace(user, coordinates))
			{
				return;
			}
			if (altInteract && target != null)
			{
				this.AltInteract(user, target.Value);
				return;
			}
			if (checkCanInteract && !this._actionBlockerSystem.CanInteract(user, target))
			{
				return;
			}
			if (checkAccess && target != null && !this._containerSystem.IsInSameOrParentContainer(user, target.Value) && !this.CanAccessViaStorage(user, target.Value))
			{
				return;
			}
			bool inRangeUnobstructed = (target == null) ? (!checkAccess || this.InRangeUnobstructed(user, coordinates, 1.5f, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, false)) : (!checkAccess || this.InRangeUnobstructed(user, target.Value, 1.5f, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, false));
			SharedHandsComponent hands;
			if (!base.TryComp<SharedHandsComponent>(user, ref hands) || hands.ActiveHand == null)
			{
				InteractNoHandEvent ev = new InteractNoHandEvent(user, target, coordinates);
				base.RaiseLocalEvent<InteractNoHandEvent>(user, ev, false);
				if (target != null)
				{
					InteractedNoHandEvent interactedEv = new InteractedNoHandEvent(target.Value, user, coordinates);
					base.RaiseLocalEvent<InteractedNoHandEvent>(target.Value, interactedEv, false);
					this.DoContactInteraction(user, new EntityUid?(target.Value), ev);
				}
				return;
			}
			entityUid = hands.ActiveHandEntity;
			if (entityUid == null)
			{
				if (inRangeUnobstructed && target != null)
				{
					this.InteractHand(user, target.Value);
				}
				return;
			}
			EntityUid held = entityUid.GetValueOrDefault();
			if (checkCanUse && !this._actionBlockerSystem.CanUseHeldEntity(user))
			{
				return;
			}
			entityUid = target;
			EntityUid entityUid2 = held;
			if (entityUid != null && (entityUid == null || entityUid.GetValueOrDefault() == entityUid2))
			{
				this.UseInHandInteraction(user, target.Value, false, false, true);
				return;
			}
			if (inRangeUnobstructed && target != null)
			{
				this.InteractUsing(user, held, target.Value, coordinates, false, false);
				return;
			}
			this.InteractUsingRanged(user, held, target, coordinates, inRangeUnobstructed);
		}

		// Token: 0x06000B3E RID: 2878 RVA: 0x00024F14 File Offset: 0x00023114
		public void InteractHand(EntityUid user, EntityUid target)
		{
			InteractHandEvent message = new InteractHandEvent(user, target);
			base.RaiseLocalEvent<InteractHandEvent>(target, message, true);
			ISharedAdminLogManager adminLogger = this._adminLogger;
			LogType type = LogType.InteractHand;
			LogImpact impact = LogImpact.Low;
			LogStringHandler logStringHandler = new LogStringHandler(17, 2);
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(user), "user", "ToPrettyString(user)");
			logStringHandler.AppendLiteral(" interacted with ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(target), "target", "ToPrettyString(target)");
			adminLogger.Add(type, impact, ref logStringHandler);
			this.DoContactInteraction(user, new EntityUid?(target), message);
			if (message.Handled)
			{
				return;
			}
			this.InteractionActivate(user, target, false, true, false);
		}

		// Token: 0x06000B3F RID: 2879 RVA: 0x00024FAC File Offset: 0x000231AC
		public void InteractUsingRanged(EntityUid user, EntityUid used, EntityUid? target, EntityCoordinates clickLocation, bool inRangeUnobstructed)
		{
			if (this.RangedInteractDoBefore(user, used, target, clickLocation, inRangeUnobstructed))
			{
				return;
			}
			if (target != null)
			{
				RangedInteractEvent rangedMsg = new RangedInteractEvent(user, used, target.Value, clickLocation);
				base.RaiseLocalEvent<RangedInteractEvent>(target.Value, rangedMsg, true);
				this.DoContactInteraction(user, new EntityUid?(used), rangedMsg);
				if (rangedMsg.Handled)
				{
					return;
				}
			}
			this.InteractDoAfter(user, used, target, clickLocation, inRangeUnobstructed);
		}

		// Token: 0x06000B40 RID: 2880 RVA: 0x00025016 File Offset: 0x00023216
		protected bool ValidateInteractAndFace(EntityUid user, EntityCoordinates coordinates)
		{
			if (coordinates.GetMapId(this.EntityManager) != base.Transform(user).MapID)
			{
				return false;
			}
			this._rotateToFaceSystem.TryFaceCoordinates(user, coordinates.ToMapPos(this.EntityManager), null);
			return true;
		}

		// Token: 0x06000B41 RID: 2881 RVA: 0x00025058 File Offset: 0x00023258
		[NullableContext(2)]
		public float UnobstructedDistance(MapCoordinates origin, MapCoordinates other, int collisionMask = 130, SharedInteractionSystem.Ignored predicate = null)
		{
			Vector2 dir = other.Position - origin.Position;
			if (dir.LengthSquared.Equals(0f))
			{
				return 0f;
			}
			if (predicate == null)
			{
				predicate = ((EntityUid _) => false);
			}
			CollisionRay ray;
			ray..ctor(origin.Position, dir.Normalized, collisionMask);
			List<RayCastResults> rayResults = this._sharedBroadphaseSystem.IntersectRayWithPredicate(origin.MapId, ray, dir.Length, new Func<EntityUid, bool>(predicate.Invoke), false).ToList<RayCastResults>();
			if (rayResults.Count == 0)
			{
				return dir.Length;
			}
			return (rayResults[0].HitPos - origin.Position).Length;
		}

		// Token: 0x06000B42 RID: 2882 RVA: 0x00025130 File Offset: 0x00023330
		[NullableContext(2)]
		public bool InRangeUnobstructed(MapCoordinates origin, MapCoordinates other, float range = 1.5f, CollisionGroup collisionMask = CollisionGroup.Impassable | CollisionGroup.InteractImpassable, SharedInteractionSystem.Ignored predicate = null)
		{
			if (other.MapId != origin.MapId)
			{
				return false;
			}
			Vector2 dir = other.Position - origin.Position;
			float length = dir.Length;
			if (range > 0f && length > range)
			{
				return false;
			}
			if (MathHelper.CloseTo(length, 0f, 1E-07f))
			{
				return true;
			}
			if (predicate == null)
			{
				predicate = ((EntityUid _) => false);
			}
			if (length > 100f)
			{
				Logger.Warning("InRangeUnobstructed check performed over extreme range. Limiting CollisionRay size.");
				length = 100f;
			}
			CollisionRay ray;
			ray..ctor(origin.Position, dir.Normalized, (int)collisionMask);
			return this._sharedBroadphaseSystem.IntersectRayWithPredicate(origin.MapId, ray, length, new Func<EntityUid, bool>(predicate.Invoke), false).ToList<RayCastResults>().Count == 0;
		}

		// Token: 0x06000B43 RID: 2883 RVA: 0x00025210 File Offset: 0x00023410
		[NullableContext(2)]
		public bool InRangeUnobstructed(EntityUid origin, EntityUid other, float range = 1.5f, CollisionGroup collisionMask = CollisionGroup.Impassable | CollisionGroup.InteractImpassable, SharedInteractionSystem.Ignored predicate = null, bool popup = false)
		{
			TransformComponent otherXform;
			return base.TryComp<TransformComponent>(other, ref otherXform) && this.InRangeUnobstructed(origin, other, otherXform.Coordinates, otherXform.LocalRotation, range, collisionMask, predicate, popup);
		}

		// Token: 0x06000B44 RID: 2884 RVA: 0x00025248 File Offset: 0x00023448
		[NullableContext(2)]
		public bool InRangeUnobstructed(EntityUid origin, EntityUid other, EntityCoordinates otherCoordinates, Angle otherAngle, float range = 1.5f, CollisionGroup collisionMask = CollisionGroup.Impassable | CollisionGroup.InteractImpassable, SharedInteractionSystem.Ignored predicate = null, bool popup = false)
		{
			SharedInteractionSystem.Ignored combinedPredicate = delegate(EntityUid e)
			{
				if (!(e == origin))
				{
					SharedInteractionSystem.Ignored predicate2 = predicate;
					return predicate2 != null && predicate2(e);
				}
				return true;
			};
			bool inRange = true;
			MapCoordinates originPos = default(MapCoordinates);
			MapCoordinates targetPos = otherCoordinates.ToMap(this.EntityManager);
			Angle targetRot = default(Angle);
			FixturesComponent fixtureA;
			FixturesComponent fixtureB;
			TransformComponent xformA;
			if (range > 0f && base.TryComp<FixturesComponent>(origin, ref fixtureA) && fixtureA.FixtureCount > 0 && base.TryComp<FixturesComponent>(other, ref fixtureB) && fixtureB.FixtureCount > 0 && base.TryComp<TransformComponent>(origin, ref xformA))
			{
				ValueTuple<Vector2, Angle> worldPositionRotation = xformA.GetWorldPositionRotation();
				Vector2 worldPosA = worldPositionRotation.Item1;
				Angle worldRotA = worldPositionRotation.Item2;
				Transform xfA;
				xfA..ctor(worldPosA, worldRotA);
				Angle parentRotB = this._transform.GetWorldRotation(otherCoordinates.EntityId);
				Transform xfB;
				xfB..ctor(targetPos.Position, parentRotB + otherAngle);
				Vector2 vector;
				Vector2 vector2;
				float distance;
				if (!this._sharedBroadphaseSystem.TryGetNearest(origin, other, ref vector, ref vector2, ref distance, xfA, xfB, fixtureA, fixtureB, null, null))
				{
					inRange = false;
				}
				else
				{
					if (distance.Equals(0f))
					{
						return true;
					}
					if (distance > range)
					{
						inRange = false;
					}
					else
					{
						originPos = xformA.MapPosition;
						vector2 = originPos.Position - targetPos.Position;
						range = vector2.Length;
					}
				}
			}
			else
			{
				originPos = base.Transform(origin).MapPosition;
				EntityUid otherParent = base.Transform(other).ParentUid;
				targetRot = (otherParent.IsValid() ? (base.Transform(otherParent).LocalRotation + otherAngle) : otherAngle);
			}
			if (inRange)
			{
				SharedInteractionSystem.Ignored rayPredicate = this.GetPredicate(originPos, other, targetPos, targetRot, collisionMask, combinedPredicate);
				inRange = this.InRangeUnobstructed(originPos, targetPos, range, collisionMask, rayPredicate);
			}
			if (!inRange && popup && this._gameTiming.IsFirstTimePredicted)
			{
				string message = Loc.GetString("interaction-system-user-interaction-cannot-reach");
				this._popupSystem.PopupEntity(message, origin, origin, PopupType.Small);
			}
			return inRange;
		}

		// Token: 0x06000B45 RID: 2885 RVA: 0x00025450 File Offset: 0x00023650
		[NullableContext(2)]
		public bool InRangeUnobstructed(MapCoordinates origin, EntityUid target, float range = 1.5f, CollisionGroup collisionMask = CollisionGroup.Impassable | CollisionGroup.InteractImpassable, SharedInteractionSystem.Ignored predicate = null)
		{
			TransformComponent transform = base.Transform(target);
			ValueTuple<Vector2, Angle> worldPositionRotation = transform.GetWorldPositionRotation();
			Vector2 position = worldPositionRotation.Item1;
			Angle rotation = worldPositionRotation.Item2;
			MapCoordinates mapPos;
			mapPos..ctor(position, transform.MapID);
			SharedInteractionSystem.Ignored combinedPredicate = this.GetPredicate(origin, target, mapPos, rotation, collisionMask, predicate);
			return this.InRangeUnobstructed(origin, mapPos, range, collisionMask, combinedPredicate);
		}

		// Token: 0x06000B46 RID: 2886 RVA: 0x000254A4 File Offset: 0x000236A4
		private SharedInteractionSystem.Ignored GetPredicate(MapCoordinates origin, EntityUid target, MapCoordinates targetCoords, Angle targetRotation, CollisionGroup collisionMask, [Nullable(2)] SharedInteractionSystem.Ignored predicate = null)
		{
			HashSet<EntityUid> ignored = new HashSet<EntityUid>();
			PhysicsComponent physics;
			WallMountComponent wallMount;
			if (base.HasComp<ItemComponent>(target) && base.TryComp<PhysicsComponent>(target, ref physics) && physics.CanCollide)
			{
				ignored.UnionWith(this._sharedBroadphaseSystem.GetEntitiesIntersectingBody(target, (int)collisionMask, false, physics, null, null));
			}
			else if (base.TryComp<WallMountComponent>(target, ref wallMount))
			{
				bool ignoreAnchored;
				if (wallMount.Arc >= 6.283185307179586)
				{
					ignoreAnchored = true;
				}
				else
				{
					Angle angle = Angle.FromWorldVec(origin.Position - targetCoords.Position);
					Angle angleDelta = (wallMount.Direction + targetRotation - angle).Reduced().FlipPositive();
					ignoreAnchored = (angleDelta < wallMount.Arc / 2.0 || 6.283185307179586 - angleDelta < wallMount.Arc / 2.0);
				}
				MapGridComponent grid;
				if (ignoreAnchored && this._mapManager.TryFindGridAt(targetCoords, ref grid))
				{
					ignored.UnionWith(grid.GetAnchoredEntities(targetCoords));
				}
			}
			return delegate(EntityUid e)
			{
				if (!(e == target))
				{
					SharedInteractionSystem.Ignored predicate2 = predicate;
					if (predicate2 == null || !predicate2(e))
					{
						return ignored.Contains(e);
					}
				}
				return true;
			};
		}

		// Token: 0x06000B47 RID: 2887 RVA: 0x00025613 File Offset: 0x00023813
		[NullableContext(2)]
		public bool InRangeUnobstructed(EntityUid origin, EntityCoordinates other, float range = 1.5f, CollisionGroup collisionMask = CollisionGroup.Impassable | CollisionGroup.InteractImpassable, SharedInteractionSystem.Ignored predicate = null, bool popup = false)
		{
			return this.InRangeUnobstructed(origin, other.ToMap(this.EntityManager), range, collisionMask, predicate, popup);
		}

		// Token: 0x06000B48 RID: 2888 RVA: 0x00025630 File Offset: 0x00023830
		[NullableContext(2)]
		public bool InRangeUnobstructed(EntityUid origin, MapCoordinates other, float range = 1.5f, CollisionGroup collisionMask = CollisionGroup.Impassable | CollisionGroup.InteractImpassable, SharedInteractionSystem.Ignored predicate = null, bool popup = false)
		{
			SharedInteractionSystem.Ignored combinedPredicate = delegate(EntityUid e)
			{
				if (!(e == origin))
				{
					SharedInteractionSystem.Ignored predicate2 = predicate;
					return predicate2 != null && predicate2(e);
				}
				return true;
			};
			MapCoordinates originPosition = base.Transform(origin).MapPosition;
			bool flag = this.InRangeUnobstructed(originPosition, other, range, collisionMask, combinedPredicate);
			if (!flag && popup && this._gameTiming.IsFirstTimePredicted)
			{
				string message = Loc.GetString("interaction-system-user-interaction-cannot-reach");
				this._popupSystem.PopupEntity(message, origin, origin, PopupType.Small);
			}
			return flag;
		}

		// Token: 0x06000B49 RID: 2889 RVA: 0x000256B8 File Offset: 0x000238B8
		public bool RangedInteractDoBefore(EntityUid user, EntityUid used, EntityUid? target, EntityCoordinates clickLocation, bool canReach)
		{
			BeforeRangedInteractEvent ev = new BeforeRangedInteractEvent(user, used, target, clickLocation, canReach);
			base.RaiseLocalEvent<BeforeRangedInteractEvent>(used, ev, false);
			this.DoContactInteraction(user, new EntityUid?(used), ev);
			return ev.Handled;
		}

		// Token: 0x06000B4A RID: 2890 RVA: 0x000256F0 File Offset: 0x000238F0
		public void InteractUsing(EntityUid user, EntityUid used, EntityUid target, EntityCoordinates clickLocation, bool checkCanInteract = true, bool checkCanUse = true)
		{
			if (checkCanInteract && !this._actionBlockerSystem.CanInteract(user, new EntityUid?(target)))
			{
				return;
			}
			if (checkCanUse && !this._actionBlockerSystem.CanUseHeldEntity(user))
			{
				return;
			}
			if (this.RangedInteractDoBefore(user, used, new EntityUid?(target), clickLocation, true))
			{
				return;
			}
			InteractUsingEvent interactUsingEvent = new InteractUsingEvent(user, used, target, clickLocation);
			base.RaiseLocalEvent<InteractUsingEvent>(target, interactUsingEvent, true);
			this.DoContactInteraction(user, new EntityUid?(used), interactUsingEvent);
			this.DoContactInteraction(user, new EntityUid?(target), interactUsingEvent);
			this.DoContactInteraction(used, new EntityUid?(target), interactUsingEvent);
			if (interactUsingEvent.Handled)
			{
				return;
			}
			this.InteractDoAfter(user, used, new EntityUid?(target), clickLocation, true);
		}

		// Token: 0x06000B4B RID: 2891 RVA: 0x00025798 File Offset: 0x00023998
		public void InteractDoAfter(EntityUid user, EntityUid used, EntityUid? target, EntityCoordinates clickLocation, bool canReach)
		{
			if (target != null && !target.GetValueOrDefault().Valid)
			{
				target = null;
			}
			AfterInteractEvent afterInteractEvent = new AfterInteractEvent(user, used, target, clickLocation, canReach);
			base.RaiseLocalEvent<AfterInteractEvent>(used, afterInteractEvent, false);
			this.DoContactInteraction(user, new EntityUid?(used), afterInteractEvent);
			if (canReach)
			{
				this.DoContactInteraction(user, target, afterInteractEvent);
				this.DoContactInteraction(used, target, afterInteractEvent);
			}
			if (afterInteractEvent.Handled)
			{
				return;
			}
			if (target == null)
			{
				return;
			}
			AfterInteractUsingEvent afterInteractUsingEvent = new AfterInteractUsingEvent(user, used, target, clickLocation, canReach);
			base.RaiseLocalEvent<AfterInteractUsingEvent>(target.Value, afterInteractUsingEvent, false);
			this.DoContactInteraction(user, new EntityUid?(used), afterInteractUsingEvent);
			if (canReach)
			{
				this.DoContactInteraction(user, target, afterInteractUsingEvent);
				this.DoContactInteraction(used, target, afterInteractUsingEvent);
			}
		}

		// Token: 0x06000B4C RID: 2892 RVA: 0x00025854 File Offset: 0x00023A54
		[NullableContext(2)]
		private bool HandleActivateItemInWorld(ICommonSession session, EntityCoordinates coords, EntityUid uid)
		{
			EntityUid? user;
			if (!this.ValidateClientInput(session, coords, uid, out user))
			{
				Logger.InfoS("system.interaction", "ActivateItemInWorld input validation failed");
				return false;
			}
			if (base.Deleted(uid, null))
			{
				return false;
			}
			this.InteractionActivate(user.Value, uid, true, true, this.ShouldCheckAccess(user.Value));
			return false;
		}

		// Token: 0x06000B4D RID: 2893 RVA: 0x000258AC File Offset: 0x00023AAC
		public bool InteractionActivate(EntityUid user, EntityUid used, bool checkCanInteract = true, bool checkUseDelay = true, bool checkAccess = true)
		{
			UseDelayComponent delayComponent = null;
			if (checkUseDelay && base.TryComp<UseDelayComponent>(used, ref delayComponent) && delayComponent.ActiveDelay)
			{
				return false;
			}
			if (checkCanInteract && !this._actionBlockerSystem.CanInteract(user, new EntityUid?(used)))
			{
				return false;
			}
			if (checkAccess && !this.InRangeUnobstructed(user, used, 1.5f, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, false))
			{
				return false;
			}
			if (checkAccess && !this._containerSystem.IsInSameOrParentContainer(user, used) && !this.CanAccessViaStorage(user, used))
			{
				return false;
			}
			if (!base.HasComp<SharedHandsComponent>(user))
			{
				return false;
			}
			ActivateInWorldEvent activateMsg = new ActivateInWorldEvent(user, used);
			base.RaiseLocalEvent<ActivateInWorldEvent>(used, activateMsg, true);
			if (!activateMsg.Handled)
			{
				return false;
			}
			this.DoContactInteraction(user, new EntityUid?(used), activateMsg);
			this._useDelay.BeginDelay(used, delayComponent);
			if (!activateMsg.WasLogged)
			{
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.InteractActivate;
				LogImpact impact = LogImpact.Low;
				LogStringHandler logStringHandler = new LogStringHandler(11, 2);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(user), "user", "ToPrettyString(user)");
				logStringHandler.AppendLiteral(" activated ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(used), "used", "ToPrettyString(used)");
				adminLogger.Add(type, impact, ref logStringHandler);
			}
			return true;
		}

		// Token: 0x06000B4E RID: 2894 RVA: 0x000259C8 File Offset: 0x00023BC8
		public bool UseInHandInteraction(EntityUid user, EntityUid used, bool checkCanUse = true, bool checkCanInteract = true, bool checkUseDelay = true)
		{
			UseDelayComponent delayComponent = null;
			if (checkUseDelay && base.TryComp<UseDelayComponent>(used, ref delayComponent) && delayComponent.ActiveDelay)
			{
				return true;
			}
			if (checkCanInteract && !this._actionBlockerSystem.CanInteract(user, new EntityUid?(used)))
			{
				return false;
			}
			if (checkCanUse && !this._actionBlockerSystem.CanUseHeldEntity(user))
			{
				return false;
			}
			UseInHandEvent useMsg = new UseInHandEvent(user);
			base.RaiseLocalEvent<UseInHandEvent>(used, useMsg, true);
			if (useMsg.Handled)
			{
				this.DoContactInteraction(user, new EntityUid?(used), useMsg);
				this._useDelay.BeginDelay(used, delayComponent);
				return true;
			}
			return this.InteractionActivate(user, used, false, false, false);
		}

		// Token: 0x06000B4F RID: 2895 RVA: 0x00025A60 File Offset: 0x00023C60
		public bool AltInteract(EntityUid user, EntityUid target)
		{
			SortedSet<Verb> verbs = this._verbSystem.GetLocalVerbs(target, user, typeof(AlternativeVerb), false);
			if (!verbs.Any<Verb>())
			{
				return false;
			}
			this._verbSystem.ExecuteVerb(verbs.First<Verb>(), user, target, false);
			return true;
		}

		// Token: 0x06000B50 RID: 2896 RVA: 0x00025AA8 File Offset: 0x00023CA8
		public void ThrownInteraction(EntityUid user, EntityUid thrown)
		{
			ThrownEvent throwMsg = new ThrownEvent(user, thrown);
			base.RaiseLocalEvent<ThrownEvent>(thrown, throwMsg, true);
			LogStringHandler logStringHandler;
			if (throwMsg.Handled)
			{
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.Throw;
				LogImpact impact = LogImpact.Low;
				logStringHandler = new LogStringHandler(7, 2);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(user), "user", "ToPrettyString(user)");
				logStringHandler.AppendLiteral(" threw ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(thrown), "entity", "ToPrettyString(thrown)");
				adminLogger.Add(type, impact, ref logStringHandler);
				return;
			}
			ISharedAdminLogManager adminLogger2 = this._adminLogger;
			LogType type2 = LogType.Throw;
			LogImpact impact2 = LogImpact.Low;
			logStringHandler = new LogStringHandler(7, 2);
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(user), "user", "ToPrettyString(user)");
			logStringHandler.AppendLiteral(" threw ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(thrown), "entity", "ToPrettyString(thrown)");
			adminLogger2.Add(type2, impact2, ref logStringHandler);
		}

		// Token: 0x06000B51 RID: 2897 RVA: 0x00025B7C File Offset: 0x00023D7C
		public void DroppedInteraction(EntityUid user, EntityUid item)
		{
			DroppedEvent dropMsg = new DroppedEvent(user);
			base.RaiseLocalEvent<DroppedEvent>(item, dropMsg, true);
			if (dropMsg.Handled)
			{
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.Drop;
				LogImpact impact = LogImpact.Low;
				LogStringHandler logStringHandler = new LogStringHandler(9, 2);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(user), "user", "ToPrettyString(user)");
				logStringHandler.AppendLiteral(" dropped ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(item), "entity", "ToPrettyString(item)");
				adminLogger.Add(type, impact, ref logStringHandler);
			}
			Angle rotation = Angle.Zero;
			InputMoverComponent mover;
			if (base.TryComp<InputMoverComponent>(user, ref mover))
			{
				rotation = mover.TargetRelativeRotation;
			}
			base.Transform(item).LocalRotation = rotation;
		}

		// Token: 0x06000B52 RID: 2898
		public abstract bool CanAccessViaStorage(EntityUid user, EntityUid target);

		// Token: 0x06000B53 RID: 2899 RVA: 0x00025C1C File Offset: 0x00023E1C
		[NullableContext(2)]
		protected bool ValidateClientInput(ICommonSession session, EntityCoordinates coords, EntityUid uid, [NotNullWhen(true)] out EntityUid? userEntity)
		{
			userEntity = null;
			if (!coords.IsValid(this.EntityManager))
			{
				string text = "system.interaction";
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(37, 2);
				defaultInterpolatedStringHandler.AppendLiteral("Invalid Coordinates: client=");
				defaultInterpolatedStringHandler.AppendFormatted<ICommonSession>(session);
				defaultInterpolatedStringHandler.AppendLiteral(", coords=");
				defaultInterpolatedStringHandler.AppendFormatted<EntityCoordinates>(coords);
				Logger.InfoS(text, defaultInterpolatedStringHandler.ToStringAndClear());
				return false;
			}
			if (uid.IsClientSide())
			{
				string text2 = "system.interaction";
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(63, 2);
				defaultInterpolatedStringHandler.AppendLiteral("Client sent interaction with client-side entity. Session=");
				defaultInterpolatedStringHandler.AppendFormatted<ICommonSession>(session);
				defaultInterpolatedStringHandler.AppendLiteral(", Uid=");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(uid);
				Logger.WarningS(text2, defaultInterpolatedStringHandler.ToStringAndClear());
				return false;
			}
			userEntity = ((session != null) ? session.AttachedEntity : null);
			if (userEntity == null || !userEntity.Value.Valid)
			{
				string text3 = "system.interaction";
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(57, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Client sent interaction with no attached entity. Session=");
				defaultInterpolatedStringHandler.AppendFormatted<ICommonSession>(session);
				Logger.WarningS(text3, defaultInterpolatedStringHandler.ToStringAndClear());
				return false;
			}
			if (!base.Exists(userEntity))
			{
				string text4 = "system.interaction";
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(84, 2);
				defaultInterpolatedStringHandler.AppendLiteral("Client attempted interaction with a non-existent attached entity. Session=");
				defaultInterpolatedStringHandler.AppendFormatted<ICommonSession>(session);
				defaultInterpolatedStringHandler.AppendLiteral(",  entity=");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid?>(userEntity);
				Logger.WarningS(text4, defaultInterpolatedStringHandler.ToStringAndClear());
				return false;
			}
			return true;
		}

		// Token: 0x06000B54 RID: 2900 RVA: 0x00025D94 File Offset: 0x00023F94
		[NullableContext(2)]
		public void DoContactInteraction(EntityUid uidA, EntityUid? uidB, HandledEntityEventArgs args = null)
		{
			if (uidB == null || (args != null && !args.Handled))
			{
				return;
			}
			if (!base.Exists(uidA) || !base.Exists(uidB))
			{
				return;
			}
			base.RaiseLocalEvent<ContactInteractionEvent>(uidA, new ContactInteractionEvent(uidB.Value), false);
			base.RaiseLocalEvent<ContactInteractionEvent>(uidB.Value, new ContactInteractionEvent(uidA), false);
		}

		// Token: 0x06000B55 RID: 2901 RVA: 0x00025DF1 File Offset: 0x00023FF1
		public void InitializeRelay()
		{
			base.SubscribeLocalEvent<InteractionRelayComponent, ComponentGetState>(new ComponentEventRefHandler<InteractionRelayComponent, ComponentGetState>(this.OnGetState), null, null);
			base.SubscribeLocalEvent<InteractionRelayComponent, ComponentHandleState>(new ComponentEventRefHandler<InteractionRelayComponent, ComponentHandleState>(this.OnHandleState), null, null);
		}

		// Token: 0x06000B56 RID: 2902 RVA: 0x00025E1B File Offset: 0x0002401B
		private void OnGetState(EntityUid uid, InteractionRelayComponent component, ref ComponentGetState args)
		{
			args.State = new InteractionRelayComponentState(component.RelayEntity);
		}

		// Token: 0x06000B57 RID: 2903 RVA: 0x00025E30 File Offset: 0x00024030
		private void OnHandleState(EntityUid uid, InteractionRelayComponent component, ref ComponentHandleState args)
		{
			InteractionRelayComponentState state = args.Current as InteractionRelayComponentState;
			if (state == null)
			{
				return;
			}
			component.RelayEntity = state.RelayEntity;
		}

		// Token: 0x06000B58 RID: 2904 RVA: 0x00025E59 File Offset: 0x00024059
		[NullableContext(2)]
		public void SetRelay(EntityUid uid, EntityUid? relayEntity, InteractionRelayComponent component = null)
		{
			if (!base.Resolve<InteractionRelayComponent>(uid, ref component, true))
			{
				return;
			}
			component.RelayEntity = relayEntity;
			base.Dirty(component, null);
		}

		// Token: 0x04000B1E RID: 2846
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x04000B1F RID: 2847
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x04000B20 RID: 2848
		[Dependency]
		private readonly ISharedAdminLogManager _adminLogger;

		// Token: 0x04000B21 RID: 2849
		[Dependency]
		private readonly ActionBlockerSystem _actionBlockerSystem;

		// Token: 0x04000B22 RID: 2850
		[Dependency]
		private readonly RotateToFaceSystem _rotateToFaceSystem;

		// Token: 0x04000B23 RID: 2851
		[Dependency]
		private readonly SharedContainerSystem _containerSystem;

		// Token: 0x04000B24 RID: 2852
		[Dependency]
		private readonly SharedPhysicsSystem _sharedBroadphaseSystem;

		// Token: 0x04000B25 RID: 2853
		[Dependency]
		private readonly SharedTransformSystem _transform;

		// Token: 0x04000B26 RID: 2854
		[Dependency]
		private readonly SharedVerbSystem _verbSystem;

		// Token: 0x04000B27 RID: 2855
		[Dependency]
		private readonly SharedPopupSystem _popupSystem;

		// Token: 0x04000B28 RID: 2856
		[Dependency]
		private readonly UseDelaySystem _useDelay;

		// Token: 0x04000B29 RID: 2857
		[Dependency]
		private readonly SharedPullingSystem _pullSystem;

		// Token: 0x04000B2A RID: 2858
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x04000B2B RID: 2859
		private const CollisionGroup InRangeUnobstructedMask = CollisionGroup.Impassable | CollisionGroup.InteractImpassable;

		// Token: 0x04000B2C RID: 2860
		public const float InteractionRange = 1.5f;

		// Token: 0x04000B2D RID: 2861
		public const float InteractionRangeSquared = 2.25f;

		// Token: 0x04000B2E RID: 2862
		public const float MaxRaycastRange = 100f;

		// Token: 0x020007EE RID: 2030
		// (Invoke) Token: 0x06001879 RID: 6265
		[NullableContext(0)]
		public delegate bool Ignored(EntityUid entity);
	}
}
