using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Content.Server.Administration.Managers;
using Content.Server.CombatMode;
using Content.Server.Destructible;
using Content.Server.Doors.Systems;
using Content.Server.NPC.Components;
using Content.Server.NPC.Events;
using Content.Server.NPC.Pathfinding;
using Content.Shared.CCVar;
using Content.Shared.Doors.Components;
using Content.Shared.Interaction;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.NPC;
using Content.Shared.NPC.Events;
using Content.Shared.Weapons.Melee;
using Robust.Server.Player;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Players;
using Robust.Shared.Random;
using Robust.Shared.Threading;
using Robust.Shared.Timing;

namespace Content.Server.NPC.Systems
{
	// Token: 0x02000335 RID: 821
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class NPCSteeringSystem : SharedNPCSteeringSystem
	{
		// Token: 0x06001103 RID: 4355 RVA: 0x00057BE8 File Offset: 0x00055DE8
		private void ApplySeek(float[] interest, Vector2 direction, float weight)
		{
			if (weight == 0f || direction == Vector2.Zero)
			{
				return;
			}
			float directionAngle = (float)DirectionExtensions.ToAngle(direction).Theta;
			for (int i = 0; i < 12; i++)
			{
				if (!interest[i].Equals(-1f))
				{
					float angle = (float)i * 0.5235988f;
					float dot = MathF.Cos(directionAngle - angle);
					dot = (dot + 1f) * 0.5f;
					interest[i] += dot * weight;
				}
			}
		}

		// Token: 0x06001104 RID: 4356 RVA: 0x00057C68 File Offset: 0x00055E68
		private bool TrySeek(EntityUid uid, InputMoverComponent mover, NPCSteeringComponent steering, PhysicsComponent body, TransformComponent xform, Angle offsetRot, float moveSpeed, float[] interest, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<PhysicsComponent> bodyQuery, float frameTime)
		{
			EntityCoordinates ourCoordinates = xform.Coordinates;
			EntityCoordinates destinationCoordinates = steering.Coordinates;
			float distance;
			if (xform.Coordinates.TryDistance(this.EntityManager, destinationCoordinates, ref distance) && distance <= steering.Range)
			{
				steering.Status = SteeringStatus.InRange;
				return true;
			}
			EntityCoordinates targetCoordinates = this.GetTargetCoordinates(steering);
			bool needsPath = false;
			PathPoly poly;
			if (!targetCoordinates.IsValid(this.EntityManager) && steering.CurrentPath.TryPeek(out poly) && (poly.Data.Flags & PathfindingBreadcrumbFlag.Invalid) != PathfindingBreadcrumbFlag.None)
			{
				steering.CurrentPath.Dequeue();
				targetCoordinates = this.GetTargetCoordinates(steering);
				needsPath = true;
			}
			float arrivalDistance;
			if (targetCoordinates.Equals(steering.Coordinates))
			{
				arrivalDistance = steering.Range;
			}
			else
			{
				arrivalDistance = 0.85f;
			}
			MapCoordinates targetMap = targetCoordinates.ToMap(this.EntityManager);
			MapCoordinates ourMap = ourCoordinates.ToMap(this.EntityManager);
			if (targetMap.MapId != ourMap.MapId)
			{
				steering.Status = SteeringStatus.NoPath;
				return false;
			}
			Vector2 direction = targetMap.Position - ourMap.Position;
			if (direction.Length <= arrivalDistance)
			{
				PathPoly node;
				if (steering.CurrentPath.TryPeek(out node) && !node.Data.IsFreeSpace)
				{
					object obstacles = this._obstacles;
					NPCSteeringSystem.SteeringObstacleStatus status;
					lock (obstacles)
					{
						status = this.TryHandleFlags(steering, node, bodyQuery);
					}
					switch (status)
					{
					case NPCSteeringSystem.SteeringObstacleStatus.Completed:
						break;
					case NPCSteeringSystem.SteeringObstacleStatus.Failed:
						steering.Status = SteeringStatus.NoPath;
						return false;
					case NPCSteeringSystem.SteeringObstacleStatus.Continuing:
						this.CheckPath(steering, xform, needsPath, distance);
						return true;
					default:
						throw new ArgumentOutOfRangeException();
					}
				}
				if (direction.Length <= 0.4f)
				{
					if (steering.CurrentPath.Count <= 0)
					{
						steering.Status = SteeringStatus.NoPath;
						return false;
					}
					steering.CurrentPath.Dequeue();
					targetMap = this.GetTargetCoordinates(steering).ToMap(this.EntityManager);
					if (ourMap.MapId != targetMap.MapId)
					{
						this.SetDirection(mover, steering, Vector2.Zero, true);
						steering.Status = SteeringStatus.NoPath;
						return false;
					}
					direction = targetMap.Position - ourMap.Position;
				}
			}
			if (!needsPath)
			{
				needsPath = (steering.CurrentPath.Count == 0 || (steering.CurrentPath.Peek().Data.Flags & PathfindingBreadcrumbFlag.Invalid) > PathfindingBreadcrumbFlag.None);
			}
			this.CheckPath(steering, xform, needsPath, distance);
			if (steering != null && steering.Pathfind)
			{
				Queue<PathPoly> currentPath = steering.CurrentPath;
				if (currentPath != null && currentPath.Count == 0)
				{
					return true;
				}
			}
			if (moveSpeed == 0f || direction == Vector2.Zero)
			{
				steering.Status = SteeringStatus.NoPath;
				return false;
			}
			Vector2 input = direction.Normalized;
			float tickMovement = moveSpeed * frameTime;
			input = offsetRot.RotateVec(ref input);
			Vector2 norm = input.Normalized;
			float weight = this.MapValue(direction.Length, tickMovement * 0.5f, tickMovement * 0.75f);
			this.ApplySeek(interest, norm, weight);
			if (weight > 0f && body.LinearVelocity.LengthSquared > 0f)
			{
				norm = body.LinearVelocity.Normalized;
				this.ApplySeek(interest, norm, 0.1f);
			}
			return true;
		}

		// Token: 0x06001105 RID: 4357 RVA: 0x00057FA0 File Offset: 0x000561A0
		private void CheckPath(NPCSteeringComponent steering, TransformComponent xform, bool needsPath, float targetDistance)
		{
			if (!this._pathfinding)
			{
				steering.CurrentPath.Clear();
				CancellationTokenSource pathfindToken = steering.PathfindToken;
				if (pathfindToken != null)
				{
					pathfindToken.Cancel();
				}
				steering.PathfindToken = null;
				return;
			}
			float lastDistance;
			if (!needsPath && this.GetCoordinates(steering.CurrentPath.Last<PathPoly>()).TryDistance(this.EntityManager, steering.Coordinates, ref lastDistance) && lastDistance > steering.RepathRange)
			{
				needsPath = true;
			}
			if (needsPath)
			{
				this.RequestPath(steering, xform, targetDistance);
			}
		}

		// Token: 0x06001106 RID: 4358 RVA: 0x00058020 File Offset: 0x00056220
		public void PrunePath(MapCoordinates mapCoordinates, Vector2 direction, Queue<PathPoly> nodes)
		{
			if (nodes.Count == 0)
			{
				return;
			}
			nodes.Dequeue();
			PathPoly node;
			while (nodes.TryPeek(out node) && node.Data.IsFreeSpace)
			{
				MapCoordinates nodeMap = node.Coordinates.ToMap(this.EntityManager);
				if (!(nodeMap.MapId == mapCoordinates.MapId) || Vector2.Dot(direction, nodeMap.Position - mapCoordinates.Position) >= 0f)
				{
					break;
				}
				nodes.Dequeue();
			}
		}

		// Token: 0x06001107 RID: 4359 RVA: 0x000580A4 File Offset: 0x000562A4
		private EntityCoordinates GetTargetCoordinates(NPCSteeringComponent steering)
		{
			PathPoly nextTarget;
			if (this._pathfinding && steering.CurrentPath.Count >= 1 && steering.CurrentPath.TryPeek(out nextTarget))
			{
				return this.GetCoordinates(nextTarget);
			}
			return steering.Coordinates;
		}

		// Token: 0x06001108 RID: 4360 RVA: 0x000580E4 File Offset: 0x000562E4
		private float MapValue(float value, float minValue, float maxValue)
		{
			if (maxValue > minValue)
			{
				return Math.Clamp((value - minValue) / (maxValue - minValue), 0f, 1f);
			}
			if (value < minValue)
			{
				return 0f;
			}
			return 1f;
		}

		// Token: 0x06001109 RID: 4361 RVA: 0x00058110 File Offset: 0x00056310
		private void CollisionAvoidance(EntityUid uid, Angle offsetRot, Vector2 worldPos, float agentRadius, float moveSpeed, int layer, int mask, TransformComponent xform, float[] danger, List<Vector2> dangerPoints, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<PhysicsComponent> bodyQuery, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<TransformComponent> xformQuery)
		{
			float detectionRadius = MathF.Max(1f, agentRadius);
			foreach (EntityUid ent in this._lookup.GetEntitiesInRange(uid, detectionRadius, 4))
			{
				PhysicsComponent otherBody;
				Vector2 pointA;
				Vector2 pointB;
				if (!(ent == uid) && bodyQuery.TryGetComponent(ent, ref otherBody) && otherBody.Hard && otherBody.CanCollide && ((mask & otherBody.CollisionLayer) != 0 || (layer & otherBody.CollisionMask) != 0) && this._physics.TryGetNearestPoints(uid, ent, ref pointA, ref pointB, xform, xformQuery.GetComponent(ent), null, null, null, null))
				{
					Vector2 obstacleDirection = pointB - pointA;
					float obstableDistance = obstacleDirection.Length;
					if (obstableDistance <= detectionRadius)
					{
						if (obstableDistance == 0f)
						{
							obstacleDirection = pointB - worldPos;
							obstableDistance = obstacleDirection.Length;
							if (obstableDistance == 0f)
							{
								continue;
							}
							obstableDistance = agentRadius;
						}
						dangerPoints.Add(pointB);
						obstacleDirection = offsetRot.RotateVec(ref obstacleDirection);
						Vector2 norm = obstacleDirection.Normalized;
						float weight = (obstableDistance <= agentRadius) ? 1f : ((detectionRadius - obstableDistance) / detectionRadius);
						for (int i = 0; i < 12; i++)
						{
							float dot = Vector2.Dot(norm, NPCSteeringSystem.Directions[i]);
							danger[i] = MathF.Max(dot * weight * 0.9f, danger[i]);
						}
					}
				}
			}
		}

		// Token: 0x0600110A RID: 4362 RVA: 0x000582A8 File Offset: 0x000564A8
		private void Separation(EntityUid uid, Angle offsetRot, Vector2 worldPos, float agentRadius, int layer, int mask, PhysicsComponent body, TransformComponent xform, float[] danger, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<PhysicsComponent> bodyQuery, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<TransformComponent> xformQuery)
		{
			float detectionRadius = agentRadius + 0.1f;
			Vector2 ourVelocity = body.LinearVelocity;
			EntityQuery<FactionComponent> factionQuery = base.GetEntityQuery<FactionComponent>();
			FactionComponent ourFaction;
			factionQuery.TryGetComponent(uid, ref ourFaction);
			foreach (EntityUid ent in this._lookup.GetEntitiesInRange(uid, detectionRadius, 2))
			{
				PhysicsComponent otherBody;
				FactionComponent otherFaction;
				if (!(ent == uid) && bodyQuery.TryGetComponent(ent, ref otherBody) && otherBody.Hard && otherBody.CanCollide && ((mask & otherBody.CollisionLayer) != 0 || (layer & otherBody.CollisionMask) != 0) && factionQuery.TryGetComponent(ent, ref otherFaction) && this._faction.IsFriendly(uid, ent, ourFaction, otherFaction) && Vector2.Dot(otherBody.LinearVelocity, ourVelocity) > 0f)
				{
					TransformComponent xformB = xformQuery.GetComponent(ent);
					Vector2 pointA;
					Vector2 pointB;
					if (this._physics.TryGetNearestPoints(uid, ent, ref pointA, ref pointB, xform, xformB, null, null, null, null))
					{
						Vector2 obstacleDirection = pointB - worldPos;
						float obstableDistance = obstacleDirection.Length;
						if (obstableDistance <= detectionRadius && obstableDistance != 0f)
						{
							obstacleDirection = offsetRot.RotateVec(ref obstacleDirection);
							Vector2 norm = obstacleDirection.Normalized;
							float weight = (obstableDistance <= agentRadius) ? 1f : ((detectionRadius - obstableDistance) / detectionRadius);
							weight *= 1f;
							for (int i = 0; i < 12; i++)
							{
								float dot = Vector2.Dot(norm, NPCSteeringSystem.Directions[i]);
								danger[i] = MathF.Max(dot * weight, danger[i]);
							}
						}
					}
				}
			}
		}

		// Token: 0x0600110B RID: 4363 RVA: 0x00058478 File Offset: 0x00056678
		public override void Initialize()
		{
			base.Initialize();
			for (int i = 0; i < 12; i++)
			{
				NPCSteeringSystem.Directions[i] = new Angle((double)(0.5235988f * (float)i)).ToVec();
			}
			base.UpdatesBefore.Add(typeof(SharedPhysicsSystem));
			this._configManager.OnValueChanged<bool>(CCVars.NPCEnabled, new Action<bool>(this.SetNPCEnabled), true);
			this._configManager.OnValueChanged<bool>(CCVars.NPCPathfinding, new Action<bool>(this.SetNPCPathfinding), true);
			base.SubscribeLocalEvent<NPCSteeringComponent, ComponentShutdown>(new ComponentEventHandler<NPCSteeringComponent, ComponentShutdown>(this.OnSteeringShutdown), null, null);
			base.SubscribeNetworkEvent<RequestNPCSteeringDebugEvent>(new EntitySessionEventHandler<RequestNPCSteeringDebugEvent>(this.OnDebugRequest), null, null);
		}

		// Token: 0x0600110C RID: 4364 RVA: 0x00058530 File Offset: 0x00056730
		private void SetNPCEnabled(bool obj)
		{
			if (!obj)
			{
				foreach (ValueTuple<NPCSteeringComponent, InputMoverComponent> valueTuple in base.EntityQuery<NPCSteeringComponent, InputMoverComponent>(false))
				{
					NPCSteeringComponent comp = valueTuple.Item1;
					valueTuple.Item2.CurTickSprintMovement = Vector2.Zero;
					CancellationTokenSource pathfindToken = comp.PathfindToken;
					if (pathfindToken != null)
					{
						pathfindToken.Cancel();
					}
					comp.PathfindToken = null;
				}
			}
			this._enabled = obj;
		}

		// Token: 0x0600110D RID: 4365 RVA: 0x000585B0 File Offset: 0x000567B0
		private void SetNPCPathfinding(bool value)
		{
			this._pathfinding = value;
			if (!this._pathfinding)
			{
				foreach (NPCSteeringComponent npcsteeringComponent in base.EntityQuery<NPCSteeringComponent>(true))
				{
					CancellationTokenSource pathfindToken = npcsteeringComponent.PathfindToken;
					if (pathfindToken != null)
					{
						pathfindToken.Cancel();
					}
					npcsteeringComponent.PathfindToken = null;
				}
			}
		}

		// Token: 0x0600110E RID: 4366 RVA: 0x0005861C File Offset: 0x0005681C
		public override void Shutdown()
		{
			base.Shutdown();
			this._configManager.UnsubValueChanged<bool>(CCVars.NPCEnabled, new Action<bool>(this.SetNPCEnabled));
			this._configManager.UnsubValueChanged<bool>(CCVars.NPCPathfinding, new Action<bool>(this.SetNPCPathfinding));
		}

		// Token: 0x0600110F RID: 4367 RVA: 0x0005865C File Offset: 0x0005685C
		private void OnDebugRequest(RequestNPCSteeringDebugEvent msg, EntitySessionEventArgs args)
		{
			if (!this._admin.IsAdmin((IPlayerSession)args.SenderSession, false))
			{
				return;
			}
			if (msg.Enabled)
			{
				this._subscribedSessions.Add(args.SenderSession);
				return;
			}
			this._subscribedSessions.Remove(args.SenderSession);
		}

		// Token: 0x06001110 RID: 4368 RVA: 0x000586B3 File Offset: 0x000568B3
		private void OnSteeringShutdown(EntityUid uid, NPCSteeringComponent component, ComponentShutdown args)
		{
			CancellationTokenSource pathfindToken = component.PathfindToken;
			if (pathfindToken != null)
			{
				pathfindToken.Cancel();
			}
			component.PathfindToken = null;
		}

		// Token: 0x06001111 RID: 4369 RVA: 0x000586D0 File Offset: 0x000568D0
		public NPCSteeringComponent Register(EntityUid uid, EntityCoordinates coordinates, [Nullable(2)] NPCSteeringComponent component = null)
		{
			if (base.Resolve<NPCSteeringComponent>(uid, ref component, false))
			{
				CancellationTokenSource pathfindToken = component.PathfindToken;
				if (pathfindToken != null)
				{
					pathfindToken.Cancel();
				}
				component.PathfindToken = null;
				component.CurrentPath.Clear();
			}
			else
			{
				component = base.AddComp<NPCSteeringComponent>(uid);
				component.Flags = this._pathfindingSystem.GetFlags(uid);
			}
			component.Coordinates = coordinates;
			return component;
		}

		// Token: 0x06001112 RID: 4370 RVA: 0x00058731 File Offset: 0x00056931
		[NullableContext(2)]
		public bool TryRegister(EntityUid uid, EntityCoordinates coordinates, NPCSteeringComponent component = null)
		{
			if (base.Resolve<NPCSteeringComponent>(uid, ref component, false) && component.Coordinates.Equals(coordinates))
			{
				return false;
			}
			this.Register(uid, coordinates, component);
			return true;
		}

		// Token: 0x06001113 RID: 4371 RVA: 0x0005875C File Offset: 0x0005695C
		[NullableContext(2)]
		public void Unregister(EntityUid uid, NPCSteeringComponent component = null)
		{
			if (!base.Resolve<NPCSteeringComponent>(uid, ref component, false))
			{
				return;
			}
			InputMoverComponent controller;
			if (this.EntityManager.TryGetComponent<InputMoverComponent>(component.Owner, ref controller))
			{
				controller.CurTickSprintMovement = Vector2.Zero;
			}
			CancellationTokenSource pathfindToken = component.PathfindToken;
			if (pathfindToken != null)
			{
				pathfindToken.Cancel();
			}
			component.PathfindToken = null;
			base.RemComp<NPCSteeringComponent>(uid);
		}

		// Token: 0x06001114 RID: 4372 RVA: 0x000587B8 File Offset: 0x000569B8
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			if (!this._enabled)
			{
				return;
			}
			EntityQuery<PhysicsComponent> bodyQuery = base.GetEntityQuery<PhysicsComponent>();
			EntityQuery<MovementSpeedModifierComponent> modifierQuery = base.GetEntityQuery<MovementSpeedModifierComponent>();
			EntityQuery<TransformComponent> xformQuery = base.GetEntityQuery<TransformComponent>();
			ValueTuple<ActiveNPCComponent, NPCSteeringComponent, InputMoverComponent, TransformComponent>[] npcs = base.EntityQuery<ActiveNPCComponent, NPCSteeringComponent, InputMoverComponent, TransformComponent>(false).ToArray<ValueTuple<ActiveNPCComponent, NPCSteeringComponent, InputMoverComponent, TransformComponent>>();
			ParallelOptions options = new ParallelOptions
			{
				MaxDegreeOfParallelism = 1
			};
			TimeSpan curTime = this._timing.CurTime;
			Parallel.For(0, npcs.Length, options, delegate(int i)
			{
				ValueTuple<ActiveNPCComponent, NPCSteeringComponent, InputMoverComponent, TransformComponent> valueTuple2 = npcs[i];
				NPCSteeringComponent steering2 = valueTuple2.Item2;
				InputMoverComponent mover2 = valueTuple2.Item3;
				TransformComponent xform = valueTuple2.Item4;
				this.Steer(steering2, mover2, xform, modifierQuery, bodyQuery, xformQuery, frameTime, curTime);
			});
			if (this._subscribedSessions.Count > 0)
			{
				List<NPCSteeringDebugData> data = new List<NPCSteeringDebugData>(npcs.Length);
				foreach (ValueTuple<ActiveNPCComponent, NPCSteeringComponent, InputMoverComponent, TransformComponent> valueTuple in npcs)
				{
					NPCSteeringComponent steering = valueTuple.Item2;
					InputMoverComponent mover = valueTuple.Item3;
					data.Add(new NPCSteeringDebugData(mover.Owner, mover.CurTickSprintMovement, steering.Interest, steering.Danger, steering.DangerPoints));
				}
				Filter filter = Filter.Empty();
				filter.AddPlayers(this._subscribedSessions);
				base.RaiseNetworkEvent(new NPCSteeringDebugEvent(data), filter, true);
			}
		}

		// Token: 0x06001115 RID: 4373 RVA: 0x00058900 File Offset: 0x00056B00
		private void SetDirection(InputMoverComponent component, NPCSteeringComponent steering, Vector2 value, bool clear = true)
		{
			if (clear && value.Equals(Vector2.Zero))
			{
				steering.CurrentPath.Clear();
			}
			component.CurTickSprintMovement = value;
			component.LastInputTick = this._timing.CurTick;
			component.LastInputSubTick = ushort.MaxValue;
		}

		// Token: 0x06001116 RID: 4374 RVA: 0x00058950 File Offset: 0x00056B50
		private void Steer(NPCSteeringComponent steering, InputMoverComponent mover, TransformComponent xform, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<MovementSpeedModifierComponent> modifierQuery, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<PhysicsComponent> bodyQuery, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<TransformComponent> xformQuery, float frameTime, TimeSpan curTime)
		{
			if (base.Deleted(steering.Coordinates.EntityId, null))
			{
				this.SetDirection(mover, steering, Vector2.Zero, true);
				steering.Status = SteeringStatus.NoPath;
				return;
			}
			if (steering.Status == SteeringStatus.NoPath)
			{
				this.SetDirection(mover, steering, Vector2.Zero, true);
				return;
			}
			if (!mover.CanMove)
			{
				this.SetDirection(mover, steering, Vector2.Zero, true);
				steering.Status = SteeringStatus.NoPath;
				return;
			}
			EntityUid uid = mover.Owner;
			float[] interest = steering.Interest;
			float[] danger = steering.Danger;
			float agentRadius = steering.Radius;
			Vector2 worldPos = xform.WorldPosition;
			ValueTuple<int, int> hardCollision = this._physics.GetHardCollision(uid, null);
			int layer = hardCollision.Item1;
			int mask = hardCollision.Item2;
			Angle offsetRot = -this._mover.GetParentGridAngle(mover);
			MovementSpeedModifierComponent modifier;
			modifierQuery.TryGetComponent(uid, ref modifier);
			float moveSpeed = this.GetSprintSpeed(steering.Owner, modifier);
			PhysicsComponent body = bodyQuery.GetComponent(uid);
			List<Vector2> dangerPoints = steering.DangerPoints;
			dangerPoints.Clear();
			for (int i = 0; i < 12; i++)
			{
				steering.Interest[i] = 0f;
				steering.Danger[i] = 0f;
			}
			NPCSteeringEvent ev = new NPCSteeringEvent(steering, interest, danger, agentRadius, offsetRot, worldPos);
			base.RaiseLocalEvent<NPCSteeringEvent>(uid, ref ev, false);
			if (steering.CanSeek && !this.TrySeek(uid, mover, steering, body, xform, offsetRot, moveSpeed, interest, bodyQuery, frameTime))
			{
				this.SetDirection(mover, steering, Vector2.Zero, true);
				return;
			}
			this.CollisionAvoidance(uid, offsetRot, worldPos, agentRadius, moveSpeed, layer, mask, xform, danger, dangerPoints, bodyQuery, xformQuery);
			this.Separation(uid, offsetRot, worldPos, agentRadius, layer, mask, body, xform, danger, bodyQuery, xformQuery);
			int desiredDirection = -1;
			float desiredValue = 0f;
			for (int j = 0; j < 12; j++)
			{
				float adjustedValue = Math.Clamp(interest[j] - danger[j], 0f, 1f);
				if (adjustedValue > desiredValue)
				{
					desiredDirection = j;
					desiredValue = adjustedValue;
				}
			}
			Vector2 resultDirection = Vector2.Zero;
			if (desiredDirection != -1)
			{
				resultDirection = new Angle((double)((float)desiredDirection * 0.5235988f)).ToVec();
			}
			if (steering.NextSteer > curTime)
			{
				this.SetDirection(mover, steering, steering.LastSteerDirection, false);
				return;
			}
			steering.NextSteer = curTime + TimeSpan.FromSeconds(0.10000000149011612);
			steering.LastSteerDirection = resultDirection;
			this.SetDirection(mover, steering, resultDirection, false);
		}

		// Token: 0x06001117 RID: 4375 RVA: 0x00058B9F File Offset: 0x00056D9F
		private EntityCoordinates GetCoordinates(PathPoly poly)
		{
			if (!poly.IsValid())
			{
				return EntityCoordinates.Invalid;
			}
			return new EntityCoordinates(poly.GraphUid, poly.Box.Center);
		}

		// Token: 0x06001118 RID: 4376 RVA: 0x00058BC8 File Offset: 0x00056DC8
		private void RequestPath(NPCSteeringComponent steering, TransformComponent xform, float targetDistance)
		{
			NPCSteeringSystem.<RequestPath>d__41 <RequestPath>d__;
			<RequestPath>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<RequestPath>d__.<>4__this = this;
			<RequestPath>d__.steering = steering;
			<RequestPath>d__.xform = xform;
			<RequestPath>d__.targetDistance = targetDistance;
			<RequestPath>d__.<>1__state = -1;
			<RequestPath>d__.<>t__builder.Start<NPCSteeringSystem.<RequestPath>d__41>(ref <RequestPath>d__);
		}

		// Token: 0x06001119 RID: 4377 RVA: 0x00058C17 File Offset: 0x00056E17
		[NullableContext(2)]
		private float GetSprintSpeed(EntityUid uid, MovementSpeedModifierComponent modifier = null)
		{
			if (!base.Resolve<MovementSpeedModifierComponent>(uid, ref modifier, false))
			{
				return 4.5f;
			}
			return modifier.CurrentSprintSpeed;
		}

		// Token: 0x0600111A RID: 4378 RVA: 0x00058C34 File Offset: 0x00056E34
		private NPCSteeringSystem.SteeringObstacleStatus TryHandleFlags(NPCSteeringComponent component, PathPoly poly, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<PhysicsComponent> bodyQuery)
		{
			FixturesComponent manager;
			if (!base.TryComp<FixturesComponent>(component.Owner, ref manager))
			{
				return NPCSteeringSystem.SteeringObstacleStatus.Failed;
			}
			ValueTuple<int, int> hardCollision = this._physics.GetHardCollision(component.Owner, manager);
			int layer = hardCollision.Item1;
			int mask = hardCollision.Item2;
			if ((poly.Data.CollisionLayer & mask) != 0 || (poly.Data.CollisionMask & layer) != 0)
			{
				List<EntityUid> obstacleEnts = new List<EntityUid>();
				this.GetObstacleEntities(poly, mask, layer, bodyQuery, obstacleEnts);
				bool isDoor = (poly.Data.Flags & PathfindingBreadcrumbFlag.Door) > PathfindingBreadcrumbFlag.None;
				bool isAccess = (poly.Data.Flags & PathfindingBreadcrumbFlag.Access) > PathfindingBreadcrumbFlag.None;
				if (isDoor && !isAccess)
				{
					EntityQuery<DoorComponent> doorQuery = base.GetEntityQuery<DoorComponent>();
					foreach (EntityUid ent in obstacleEnts)
					{
						DoorComponent door;
						if (doorQuery.TryGetComponent(ent, ref door))
						{
							if (door.BumpOpen || (component.Flags & PathFlags.Interact) == PathFlags.None)
							{
								return NPCSteeringSystem.SteeringObstacleStatus.Failed;
							}
							if (door.State != DoorState.Opening)
							{
								this._interaction.InteractionActivate(component.Owner, ent, true, true, true);
								return NPCSteeringSystem.SteeringObstacleStatus.Continuing;
							}
						}
					}
					return NPCSteeringSystem.SteeringObstacleStatus.Completed;
				}
				if ((component.Flags & PathFlags.Prying) > PathFlags.None && isAccess && isDoor)
				{
					EntityQuery<DoorComponent> doorQuery2 = base.GetEntityQuery<DoorComponent>();
					foreach (EntityUid ent2 in obstacleEnts)
					{
						DoorComponent door2;
						if (doorQuery2.TryGetComponent(ent2, ref door2) && door2.State != DoorState.Open)
						{
							if (door2.State != DoorState.Opening && !door2.BeingPried)
							{
								this._doors.TryPryDoor(ent2, component.Owner, component.Owner, door2, true);
							}
							return NPCSteeringSystem.SteeringObstacleStatus.Continuing;
						}
					}
					if (obstacleEnts.Count == 0)
					{
						return NPCSteeringSystem.SteeringObstacleStatus.Completed;
					}
				}
				else if ((component.Flags & PathFlags.Smashing) != PathFlags.None)
				{
					MeleeWeaponComponent meleeWeapon = this._melee.GetWeapon(component.Owner);
					CombatModeComponent combatMode;
					if (meleeWeapon != null && meleeWeapon.NextAttack <= this._timing.CurTime && base.TryComp<CombatModeComponent>(component.Owner, ref combatMode))
					{
						combatMode.IsInCombatMode = true;
						EntityQuery<DestructibleComponent> destructibleQuery = base.GetEntityQuery<DestructibleComponent>();
						this._random.Shuffle<EntityUid>(obstacleEnts);
						foreach (EntityUid ent3 in obstacleEnts)
						{
							if (destructibleQuery.HasComponent(ent3))
							{
								this._melee.AttemptLightAttack(component.Owner, component.Owner, meleeWeapon, ent3);
								break;
							}
						}
						combatMode.IsInCombatMode = false;
						if (obstacleEnts.Count == 0)
						{
							return NPCSteeringSystem.SteeringObstacleStatus.Completed;
						}
						return NPCSteeringSystem.SteeringObstacleStatus.Continuing;
					}
				}
				return NPCSteeringSystem.SteeringObstacleStatus.Failed;
			}
			return NPCSteeringSystem.SteeringObstacleStatus.Completed;
		}

		// Token: 0x0600111B RID: 4379 RVA: 0x00058F14 File Offset: 0x00057114
		private void GetObstacleEntities(PathPoly poly, int mask, int layer, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<PhysicsComponent> bodyQuery, List<EntityUid> ents)
		{
			MapGridComponent grid;
			if (!this._mapManager.TryGetGrid(new EntityUid?(poly.GraphUid), ref grid))
			{
				return;
			}
			foreach (EntityUid ent in grid.GetLocalAnchoredEntities(poly.Box))
			{
				PhysicsComponent body;
				if (bodyQuery.TryGetComponent(ent, ref body) && body.Hard && body.CanCollide && ((body.CollisionMask & layer) != 0 || (body.CollisionLayer & mask) != 0))
				{
					ents.Add(ent);
				}
			}
		}

		// Token: 0x04000A1C RID: 2588
		[Dependency]
		private readonly IAdminManager _admin;

		// Token: 0x04000A1D RID: 2589
		[Dependency]
		private readonly IConfigurationManager _configManager;

		// Token: 0x04000A1E RID: 2590
		[Dependency]
		private readonly IGameTiming _timing;

		// Token: 0x04000A1F RID: 2591
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x04000A20 RID: 2592
		[Dependency]
		private readonly IParallelManager _parallel;

		// Token: 0x04000A21 RID: 2593
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x04000A22 RID: 2594
		[Dependency]
		private readonly DoorSystem _doors;

		// Token: 0x04000A23 RID: 2595
		[Dependency]
		private readonly EntityLookupSystem _lookup;

		// Token: 0x04000A24 RID: 2596
		[Dependency]
		private readonly FactionSystem _faction;

		// Token: 0x04000A25 RID: 2597
		[Dependency]
		private readonly PathfindingSystem _pathfindingSystem;

		// Token: 0x04000A26 RID: 2598
		[Dependency]
		private readonly SharedInteractionSystem _interaction;

		// Token: 0x04000A27 RID: 2599
		[Dependency]
		private readonly SharedMeleeWeaponSystem _melee;

		// Token: 0x04000A28 RID: 2600
		[Dependency]
		private readonly SharedMoverController _mover;

		// Token: 0x04000A29 RID: 2601
		[Dependency]
		private readonly SharedPhysicsSystem _physics;

		// Token: 0x04000A2A RID: 2602
		private const float TileTolerance = 0.4f;

		// Token: 0x04000A2B RID: 2603
		private bool _enabled;

		// Token: 0x04000A2C RID: 2604
		private bool _pathfinding = true;

		// Token: 0x04000A2D RID: 2605
		public static readonly Vector2[] Directions = new Vector2[12];

		// Token: 0x04000A2E RID: 2606
		private readonly HashSet<ICommonSession> _subscribedSessions = new HashSet<ICommonSession>();

		// Token: 0x04000A2F RID: 2607
		private object _obstacles = new object();

		// Token: 0x02000966 RID: 2406
		[NullableContext(0)]
		private enum SteeringObstacleStatus : byte
		{
			// Token: 0x04002024 RID: 8228
			Completed,
			// Token: 0x04002025 RID: 8229
			Failed,
			// Token: 0x04002026 RID: 8230
			Continuing
		}
	}
}
