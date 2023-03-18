using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Doors.Systems;
using Content.Server.NPC.Pathfinding;
using Content.Server.Shuttles.Components;
using Content.Server.Shuttles.Events;
using Content.Shared.Doors;
using Content.Shared.Doors.Components;
using Content.Shared.Shuttles.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Dynamics.Joints;
using Robust.Shared.Physics.Systems;

namespace Content.Server.Shuttles.Systems
{
	// Token: 0x020001F6 RID: 502
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DockingSystem : EntitySystem
	{
		// Token: 0x060009B9 RID: 2489 RVA: 0x000312A4 File Offset: 0x0002F4A4
		private void UpdateAutodock()
		{
			EntityQuery<DockingComponent> dockingQuery = base.GetEntityQuery<DockingComponent>();
			EntityQuery<TransformComponent> xformQuery = base.GetEntityQuery<TransformComponent>();
			EntityQuery<RecentlyDockedComponent> recentQuery = base.GetEntityQuery<RecentlyDockedComponent>();
			foreach (ValueTuple<AutoDockComponent, PhysicsComponent> valueTuple in base.EntityQuery<AutoDockComponent, PhysicsComponent>(false))
			{
				AutoDockComponent comp = valueTuple.Item1;
				PhysicsComponent body = valueTuple.Item2;
				DockingComponent dock;
				if (comp.Requesters.Count == 0 || !dockingQuery.TryGetComponent(comp.Owner, ref dock))
				{
					base.RemComp<AutoDockComponent>(comp.Owner);
				}
				else if (!dock.Docked && !recentQuery.HasComponent(comp.Owner))
				{
					DockingComponent dockable = this.GetDockable(body, xformQuery.GetComponent(comp.Owner));
					if (dockable != null)
					{
						this.TryDock(dock, dockable);
					}
				}
			}
			HashSet<EntityUid> checkedRecent = new HashSet<EntityUid>();
			foreach (ValueTuple<RecentlyDockedComponent, TransformComponent> valueTuple2 in base.EntityQuery<RecentlyDockedComponent, TransformComponent>(false))
			{
				RecentlyDockedComponent comp2 = valueTuple2.Item1;
				TransformComponent xform = valueTuple2.Item2;
				if (checkedRecent.Add(comp2.Owner))
				{
					DockingComponent dock2;
					TransformComponent otherXform;
					if (!dockingQuery.TryGetComponent(comp2.Owner, ref dock2))
					{
						base.RemComp<RecentlyDockedComponent>(comp2.Owner);
					}
					else if (!xformQuery.TryGetComponent(comp2.LastDocked, ref otherXform))
					{
						base.RemComp<RecentlyDockedComponent>(comp2.Owner);
					}
					else
					{
						Vector2 worldPosition = this._transformSystem.GetWorldPosition(xform, xformQuery);
						Vector2 otherWorldPos = this._transformSystem.GetWorldPosition(otherXform, xformQuery);
						if ((worldPosition - otherWorldPos).Length >= comp2.Radius)
						{
							ISawmill sawmill = this._sawmill;
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(33, 2);
							defaultInterpolatedStringHandler.AppendLiteral("Removed RecentlyDocked from ");
							defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(comp2.Owner));
							defaultInterpolatedStringHandler.AppendLiteral(" and ");
							defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(comp2.LastDocked));
							sawmill.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
							base.RemComp<RecentlyDockedComponent>(comp2.Owner);
							base.RemComp<RecentlyDockedComponent>(comp2.LastDocked);
						}
					}
				}
			}
		}

		// Token: 0x060009BA RID: 2490 RVA: 0x00031500 File Offset: 0x0002F700
		private void OnRequestUndock(EntityUid uid, ShuttleConsoleComponent component, UndockRequestMessage args)
		{
			ISawmill sawmill = this._sawmill;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(28, 1);
			defaultInterpolatedStringHandler.AppendLiteral("Received undock request for ");
			defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.DockEntity));
			sawmill.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
			DockingComponent dock;
			if (!base.TryComp<DockingComponent>(args.DockEntity, ref dock) || !dock.Docked)
			{
				return;
			}
			this.Undock(dock);
		}

		// Token: 0x060009BB RID: 2491 RVA: 0x00031568 File Offset: 0x0002F768
		private void OnRequestAutodock(EntityUid uid, ShuttleConsoleComponent component, AutodockRequestMessage args)
		{
			ISawmill sawmill = this._sawmill;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(30, 1);
			defaultInterpolatedStringHandler.AppendLiteral("Received autodock request for ");
			defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.DockEntity));
			sawmill.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
			EntityUid? player = args.Session.AttachedEntity;
			if (player == null || !base.HasComp<DockingComponent>(args.DockEntity))
			{
				return;
			}
			base.EnsureComp<AutoDockComponent>(args.DockEntity).Requesters.Add(player.Value);
		}

		// Token: 0x060009BC RID: 2492 RVA: 0x000315F4 File Offset: 0x0002F7F4
		private void OnRequestStopAutodock(EntityUid uid, ShuttleConsoleComponent component, StopAutodockRequestMessage args)
		{
			ISawmill sawmill = this._sawmill;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(35, 1);
			defaultInterpolatedStringHandler.AppendLiteral("Received stop autodock request for ");
			defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.DockEntity));
			sawmill.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
			EntityUid? player = args.Session.AttachedEntity;
			AutoDockComponent comp;
			if (player == null || !base.TryComp<AutoDockComponent>(args.DockEntity, ref comp))
			{
				return;
			}
			comp.Requesters.Remove(player.Value);
			if (comp.Requesters.Count == 0)
			{
				base.RemComp<AutoDockComponent>(args.DockEntity);
			}
		}

		// Token: 0x060009BD RID: 2493 RVA: 0x00031690 File Offset: 0x0002F890
		public override void Initialize()
		{
			base.Initialize();
			this._sawmill = Logger.GetSawmill("docking");
			base.SubscribeLocalEvent<DockingComponent, ComponentStartup>(new ComponentEventHandler<DockingComponent, ComponentStartup>(this.OnStartup), null, null);
			base.SubscribeLocalEvent<DockingComponent, ComponentShutdown>(new ComponentEventHandler<DockingComponent, ComponentShutdown>(this.OnShutdown), null, null);
			base.SubscribeLocalEvent<DockingComponent, AnchorStateChangedEvent>(new ComponentEventRefHandler<DockingComponent, AnchorStateChangedEvent>(this.OnAnchorChange), null, null);
			base.SubscribeLocalEvent<DockingComponent, ReAnchorEvent>(new ComponentEventRefHandler<DockingComponent, ReAnchorEvent>(this.OnDockingReAnchor), null, null);
			base.SubscribeLocalEvent<DockingComponent, BeforeDoorAutoCloseEvent>(new ComponentEventHandler<DockingComponent, BeforeDoorAutoCloseEvent>(this.OnAutoClose), null, null);
			base.SubscribeLocalEvent<ShuttleConsoleComponent, AutodockRequestMessage>(new ComponentEventHandler<ShuttleConsoleComponent, AutodockRequestMessage>(this.OnRequestAutodock), null, null);
			base.SubscribeLocalEvent<ShuttleConsoleComponent, StopAutodockRequestMessage>(new ComponentEventHandler<ShuttleConsoleComponent, StopAutodockRequestMessage>(this.OnRequestStopAutodock), null, null);
			base.SubscribeLocalEvent<ShuttleConsoleComponent, UndockRequestMessage>(new ComponentEventHandler<ShuttleConsoleComponent, UndockRequestMessage>(this.OnRequestUndock), null, null);
		}

		// Token: 0x060009BE RID: 2494 RVA: 0x00031753 File Offset: 0x0002F953
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			this.UpdateAutodock();
		}

		// Token: 0x060009BF RID: 2495 RVA: 0x00031762 File Offset: 0x0002F962
		private void OnAutoClose(EntityUid uid, DockingComponent component, BeforeDoorAutoCloseEvent args)
		{
			if (component.Docked)
			{
				args.Cancel();
			}
		}

		// Token: 0x060009C0 RID: 2496 RVA: 0x00031774 File Offset: 0x0002F974
		[return: Nullable(2)]
		private DockingComponent GetDockable(PhysicsComponent body, TransformComponent dockingXform)
		{
			MapGridComponent grid;
			if (!this._mapManager.TryGetGrid(dockingXform.GridUid, ref grid) || !base.HasComp<ShuttleComponent>(grid.Owner))
			{
				return null;
			}
			Transform transform = this._physics.GetPhysicsTransform(body.Owner, dockingXform, null);
			Fixture dockingFixture = this._fixtureSystem.GetFixtureOrNull(body.Owner, "docking", null);
			if (dockingFixture == null)
			{
				return null;
			}
			Box2? aabb = null;
			Box2 box;
			for (int i = 0; i < dockingFixture.Shape.ChildCount; i++)
			{
				Box2 value;
				if (aabb == null)
				{
					value = dockingFixture.Shape.ComputeAABB(transform, i);
				}
				else
				{
					Box2 valueOrDefault = aabb.GetValueOrDefault();
					box = dockingFixture.Shape.ComputeAABB(transform, i);
					value = valueOrDefault.Union(ref box);
				}
				aabb = new Box2?(value);
			}
			if (aabb == null)
			{
				return null;
			}
			box = aabb.Value;
			Box2 enlargedAABB = box.Enlarged(0.3f);
			foreach (MapGridComponent otherGrid in this._mapManager.FindGridsIntersecting(dockingXform.MapID, enlargedAABB, false))
			{
				if (!(otherGrid.Owner == dockingXform.GridUid))
				{
					foreach (EntityUid ent in otherGrid.GetAnchoredEntities(enlargedAABB))
					{
						DockingComponent otherDocking;
						FixturesComponent otherBody;
						if (base.TryComp<DockingComponent>(ent, ref otherDocking) && otherDocking.Enabled && base.TryComp<FixturesComponent>(ent, ref otherBody))
						{
							Transform otherTransform = this._physics.GetPhysicsTransform(ent, null, null);
							Fixture otherDockingFixture = this._fixtureSystem.GetFixtureOrNull(ent, "docking", otherBody);
							if (otherDockingFixture == null)
							{
								ISawmill sawmill = this._sawmill;
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(30, 1);
								defaultInterpolatedStringHandler.AppendLiteral("Found null docking fixture on ");
								defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(ent);
								sawmill.Error(defaultInterpolatedStringHandler.ToStringAndClear());
							}
							else
							{
								for (int j = 0; j < otherDockingFixture.Shape.ChildCount; j++)
								{
									Box2 otherAABB = otherDockingFixture.Shape.ComputeAABB(otherTransform, j);
									box = aabb.Value;
									if (box.Intersects(ref otherAABB))
									{
										return otherDocking;
									}
								}
							}
						}
					}
				}
			}
			return null;
		}

		// Token: 0x060009C1 RID: 2497 RVA: 0x00031A24 File Offset: 0x0002FC24
		private void OnShutdown(EntityUid uid, DockingComponent component, ComponentShutdown args)
		{
			if (component.DockedWith == null || this.EntityManager.GetComponent<MetaDataComponent>(uid).EntityLifeStage > 3)
			{
				return;
			}
			this.Cleanup(component);
		}

		// Token: 0x060009C2 RID: 2498 RVA: 0x00031A50 File Offset: 0x0002FC50
		private void Cleanup(DockingComponent dockA)
		{
			this._pathfinding.RemovePortal(dockA.PathfindHandle);
			this._jointSystem.RemoveJoint(dockA.DockJoint);
			EntityUid? dockBUid = dockA.DockedWith;
			DockingComponent dockB;
			if (dockBUid == null || dockA.DockJoint == null || !base.TryComp<DockingComponent>(dockBUid, ref dockB))
			{
				ISawmill sawmill = this._sawmill;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(33, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Tried to cleanup ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(dockA.Owner);
				defaultInterpolatedStringHandler.AppendLiteral(" but not docked?");
				sawmill.Error(defaultInterpolatedStringHandler.ToStringAndClear());
				dockA.DockedWith = null;
				if (dockA.DockJoint != null)
				{
					this._jointSystem.RemoveJoint(dockA.DockJoint);
				}
				return;
			}
			dockB.DockedWith = null;
			dockB.DockJoint = null;
			dockB.DockJointId = null;
			dockA.DockJoint = null;
			dockA.DockedWith = null;
			dockA.DockJointId = null;
			EntityUid? gridAUid = this.EntityManager.GetComponent<TransformComponent>(dockA.Owner).GridUid;
			EntityUid? gridBUid = this.EntityManager.GetComponent<TransformComponent>(dockB.Owner).GridUid;
			UndockEvent msg = new UndockEvent
			{
				DockA = dockA,
				DockB = dockB,
				GridAUid = gridAUid.Value,
				GridBUid = gridBUid.Value
			};
			this.EntityManager.EventBus.RaiseLocalEvent<UndockEvent>(dockA.Owner, msg, false);
			this.EntityManager.EventBus.RaiseLocalEvent<UndockEvent>(dockB.Owner, msg, false);
			this.EntityManager.EventBus.RaiseEvent<UndockEvent>(1, msg);
		}

		// Token: 0x060009C3 RID: 2499 RVA: 0x00031BE0 File Offset: 0x0002FDE0
		private void OnStartup(EntityUid uid, DockingComponent component, ComponentStartup args)
		{
			if (!this.EntityManager.GetComponent<TransformComponent>(uid).Anchored)
			{
				return;
			}
			this.EnableDocking(uid, component);
			if (component.DockedWith != null)
			{
				if (base.MetaData(component.DockedWith.Value).EntityLifeStage < 2)
				{
					return;
				}
				DockingComponent otherDock = this.EntityManager.GetComponent<DockingComponent>(component.DockedWith.Value);
				this.Dock(component, otherDock);
			}
		}

		// Token: 0x060009C4 RID: 2500 RVA: 0x00031C4F File Offset: 0x0002FE4F
		private void OnAnchorChange(EntityUid uid, DockingComponent component, ref AnchorStateChangedEvent args)
		{
			if (args.Anchored)
			{
				this.EnableDocking(uid, component);
			}
			else
			{
				this.DisableDocking(uid, component);
			}
			this._console.RefreshShuttleConsoles();
		}

		// Token: 0x060009C5 RID: 2501 RVA: 0x00031C78 File Offset: 0x0002FE78
		private void OnDockingReAnchor(EntityUid uid, DockingComponent component, ref ReAnchorEvent args)
		{
			if (!component.Docked)
			{
				return;
			}
			DockingComponent other = base.Comp<DockingComponent>(component.DockedWith.Value);
			this.Undock(component);
			this.Dock(component, other);
			this._console.RefreshShuttleConsoles();
		}

		// Token: 0x060009C6 RID: 2502 RVA: 0x00031CBC File Offset: 0x0002FEBC
		private void DisableDocking(EntityUid uid, DockingComponent component)
		{
			if (!component.Enabled)
			{
				return;
			}
			component.Enabled = false;
			if (component.DockedWith != null)
			{
				this.Undock(component);
			}
			PhysicsComponent physicsComponent;
			if (!base.TryComp<PhysicsComponent>(uid, ref physicsComponent))
			{
				return;
			}
			this._fixtureSystem.DestroyFixture(uid, "docking", true, physicsComponent, null, null);
		}

		// Token: 0x060009C7 RID: 2503 RVA: 0x00031D10 File Offset: 0x0002FF10
		private void EnableDocking(EntityUid uid, DockingComponent component)
		{
			if (component.Enabled)
			{
				return;
			}
			PhysicsComponent physicsComponent;
			if (!base.TryComp<PhysicsComponent>(uid, ref physicsComponent))
			{
				return;
			}
			component.Enabled = true;
			PhysShapeCircle shape = new PhysShapeCircle(0.2f, new Vector2(0f, -0.5f));
			this._fixtureSystem.TryCreateFixture(uid, shape, "docking", 1f, false, 0, 0, 0.4f, 0f, true, null, physicsComponent, null);
		}

		// Token: 0x060009C8 RID: 2504 RVA: 0x00031D7C File Offset: 0x0002FF7C
		public void Dock(DockingComponent dockA, DockingComponent dockB)
		{
			if (dockB.Owner.GetHashCode() < dockA.Owner.GetHashCode())
			{
				DockingComponent dockingComponent = dockB;
				dockB = dockA;
				dockA = dockingComponent;
			}
			ISawmill sawmill = this._sawmill;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(21, 2);
			defaultInterpolatedStringHandler.AppendLiteral("Docking between ");
			defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(dockA.Owner);
			defaultInterpolatedStringHandler.AppendLiteral(" and ");
			defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(dockB.Owner);
			sawmill.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
			TransformComponent dockAXform = this.EntityManager.GetComponent<TransformComponent>(dockA.Owner);
			TransformComponent dockBXform = this.EntityManager.GetComponent<TransformComponent>(dockB.Owner);
			EntityUid gridA = dockAXform.GridUid.Value;
			EntityUid gridB = dockBXform.GridUid.Value;
			float stiffness;
			float damping;
			SharedJointSystem.LinearStiffness(2f, 0.7f, this.EntityManager.GetComponent<PhysicsComponent>(gridA).Mass, this.EntityManager.GetComponent<PhysicsComponent>(gridB).Mass, ref stiffness, ref damping);
			WeldJoint joint;
			if (dockA.DockJointId != null)
			{
				joint = this._jointSystem.GetOrCreateWeldJoint(gridA, gridB, dockA.DockJointId);
			}
			else
			{
				joint = this._jointSystem.GetOrCreateWeldJoint(gridA, gridB, "docking" + dockA.Owner.ToString());
			}
			TransformComponent gridAXform = this.EntityManager.GetComponent<TransformComponent>(gridA);
			TransformComponent gridBXform = this.EntityManager.GetComponent<TransformComponent>(gridB);
			Vector2 anchorA = dockAXform.LocalPosition + dockAXform.LocalRotation.ToWorldVec() / 2f;
			Vector2 anchorB = dockBXform.LocalPosition + dockBXform.LocalRotation.ToWorldVec() / 2f;
			joint.LocalAnchorA = anchorA;
			joint.LocalAnchorB = anchorB;
			joint.ReferenceAngle = (float)(gridBXform.WorldRotation - gridAXform.WorldRotation);
			joint.CollideConnected = true;
			joint.Stiffness = stiffness;
			joint.Damping = damping;
			dockA.DockedWith = new EntityUid?(dockB.Owner);
			dockB.DockedWith = new EntityUid?(dockA.Owner);
			dockA.DockJoint = joint;
			dockA.DockJointId = joint.ID;
			dockB.DockJoint = joint;
			dockB.DockJointId = joint.ID;
			DoorComponent doorA;
			if (base.TryComp<DoorComponent>(dockA.Owner, ref doorA) && this._doorSystem.TryOpen(doorA.Owner, doorA, null, false, false))
			{
				doorA.ChangeAirtight = false;
				AirlockComponent airlockA;
				if (base.TryComp<AirlockComponent>(dockA.Owner, ref airlockA))
				{
					this._airlocks.SetBoltsWithAudio(dockA.Owner, airlockA, true);
				}
			}
			DoorComponent doorB;
			if (base.TryComp<DoorComponent>(dockB.Owner, ref doorB) && this._doorSystem.TryOpen(doorB.Owner, doorB, null, false, false))
			{
				doorB.ChangeAirtight = false;
				AirlockComponent airlockB;
				if (base.TryComp<AirlockComponent>(dockB.Owner, ref airlockB))
				{
					this._airlocks.SetBoltsWithAudio(dockB.Owner, airlockB, true);
				}
			}
			int handle;
			if (this._pathfinding.TryCreatePortal(dockAXform.Coordinates, dockBXform.Coordinates, out handle))
			{
				dockA.PathfindHandle = handle;
				dockB.PathfindHandle = handle;
			}
			DockEvent msg = new DockEvent
			{
				DockA = dockA,
				DockB = dockB,
				GridAUid = gridA,
				GridBUid = gridB
			};
			this.EntityManager.EventBus.RaiseLocalEvent<DockEvent>(dockA.Owner, msg, false);
			this.EntityManager.EventBus.RaiseLocalEvent<DockEvent>(dockB.Owner, msg, false);
			this.EntityManager.EventBus.RaiseEvent<DockEvent>(1, msg);
		}

		// Token: 0x060009C9 RID: 2505 RVA: 0x00032120 File Offset: 0x00030320
		private bool CanDock(DockingComponent dockA, DockingComponent dockB)
		{
			PhysicsComponent bodyA;
			PhysicsComponent bodyB;
			if (!base.TryComp<PhysicsComponent>(dockA.Owner, ref bodyA) || !base.TryComp<PhysicsComponent>(dockB.Owner, ref bodyB) || !dockA.Enabled || !dockB.Enabled || dockA.DockedWith != null || dockB.DockedWith != null)
			{
				return false;
			}
			Fixture fixtureA = this._fixtureSystem.GetFixtureOrNull(bodyA.Owner, "docking", null);
			Fixture fixtureB = this._fixtureSystem.GetFixtureOrNull(bodyB.Owner, "docking", null);
			if (fixtureA == null || fixtureB == null)
			{
				return false;
			}
			Transform transformA = this._physics.GetPhysicsTransform(dockA.Owner, null, null);
			Transform transformB = this._physics.GetPhysicsTransform(dockB.Owner, null, null);
			bool intersect = false;
			for (int i = 0; i < fixtureA.Shape.ChildCount; i++)
			{
				Box2 aabb = fixtureA.Shape.ComputeAABB(transformA, i);
				for (int j = 0; j < fixtureB.Shape.ChildCount; j++)
				{
					Box2 otherAABB = fixtureB.Shape.ComputeAABB(transformB, j);
					if (aabb.Intersects(ref otherAABB))
					{
						intersect = true;
						break;
					}
				}
				if (intersect)
				{
					break;
				}
			}
			return intersect;
		}

		// Token: 0x060009CA RID: 2506 RVA: 0x0003225C File Offset: 0x0003045C
		private void TryDock(DockingComponent dockA, DockingComponent dockB)
		{
			if (!this.CanDock(dockA, dockB))
			{
				return;
			}
			this.Dock(dockA, dockB);
		}

		// Token: 0x060009CB RID: 2507 RVA: 0x00032274 File Offset: 0x00030474
		public void Undock(DockingComponent dock)
		{
			if (dock.DockedWith == null)
			{
				return;
			}
			AirlockComponent airlockA;
			if (base.TryComp<AirlockComponent>(dock.Owner, ref airlockA))
			{
				this._airlocks.SetBoltsWithAudio(dock.Owner, airlockA, false);
			}
			AirlockComponent airlockB;
			if (base.TryComp<AirlockComponent>(dock.DockedWith, ref airlockB))
			{
				this._airlocks.SetBoltsWithAudio(dock.DockedWith.Value, airlockB, false);
			}
			DoorComponent doorA;
			if (base.TryComp<DoorComponent>(dock.Owner, ref doorA) && this._doorSystem.TryClose(doorA.Owner, doorA, null, false))
			{
				doorA.ChangeAirtight = true;
			}
			DoorComponent doorB;
			if (base.TryComp<DoorComponent>(dock.DockedWith, ref doorB) && this._doorSystem.TryClose(doorB.Owner, doorB, null, false))
			{
				doorB.ChangeAirtight = true;
			}
			if (base.LifeStage(dock.Owner, null) < 4)
			{
				base.EnsureComp<RecentlyDockedComponent>(dock.Owner).LastDocked = dock.DockedWith.Value;
			}
			MetaDataComponent meta;
			if (base.TryComp<MetaDataComponent>(dock.DockedWith.Value, ref meta) && meta.EntityLifeStage < 4)
			{
				base.EnsureComp<RecentlyDockedComponent>(dock.DockedWith.Value).LastDocked = dock.DockedWith.Value;
			}
			this.Cleanup(dock);
		}

		// Token: 0x040005D7 RID: 1495
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x040005D8 RID: 1496
		[Dependency]
		private readonly AirlockSystem _airlocks;

		// Token: 0x040005D9 RID: 1497
		[Dependency]
		private readonly DoorSystem _doorSystem;

		// Token: 0x040005DA RID: 1498
		[Dependency]
		private readonly FixtureSystem _fixtureSystem;

		// Token: 0x040005DB RID: 1499
		[Dependency]
		private readonly PathfindingSystem _pathfinding;

		// Token: 0x040005DC RID: 1500
		[Dependency]
		private readonly ShuttleConsoleSystem _console;

		// Token: 0x040005DD RID: 1501
		[Dependency]
		private readonly SharedJointSystem _jointSystem;

		// Token: 0x040005DE RID: 1502
		[Dependency]
		private readonly SharedPhysicsSystem _physics;

		// Token: 0x040005DF RID: 1503
		[Dependency]
		private readonly SharedTransformSystem _transformSystem;

		// Token: 0x040005E0 RID: 1504
		private ISawmill _sawmill;

		// Token: 0x040005E1 RID: 1505
		private const string DockingFixture = "docking";

		// Token: 0x040005E2 RID: 1506
		private const string DockingJoint = "docking";

		// Token: 0x040005E3 RID: 1507
		private const float DockingRadius = 0.2f;
	}
}
