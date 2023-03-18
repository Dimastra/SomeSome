using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Server.Ghost.Components;
using Content.Shared.Administration;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Server.Console;
using Robust.Server.Player;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics.Joints;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Players;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Server.Weapons.Ranged.Systems
{
	// Token: 0x020000B2 RID: 178
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class TetherGunSystem : SharedTetherGunSystem
	{
		// Token: 0x060002D9 RID: 729 RVA: 0x0000F8F0 File Offset: 0x0000DAF0
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeNetworkEvent<StartTetherEvent>(new EntitySessionEventHandler<StartTetherEvent>(this.OnStartTether), null, null);
			base.SubscribeNetworkEvent<StopTetherEvent>(new EntitySessionEventHandler<StopTetherEvent>(this.OnStopTether), null, null);
			base.SubscribeNetworkEvent<TetherMoveEvent>(new EntitySessionEventHandler<TetherMoveEvent>(this.OnMoveTether), null, null);
			this._playerManager.PlayerStatusChanged += this.OnStatusChange;
		}

		// Token: 0x060002DA RID: 730 RVA: 0x0000F956 File Offset: 0x0000DB56
		private void OnStatusChange([Nullable(2)] object sender, SessionStatusEventArgs e)
		{
			this.StopTether(e.Session);
		}

		// Token: 0x060002DB RID: 731 RVA: 0x0000F964 File Offset: 0x0000DB64
		public override void Shutdown()
		{
			base.Shutdown();
			this._playerManager.PlayerStatusChanged -= this.OnStatusChange;
		}

		// Token: 0x060002DC RID: 732 RVA: 0x0000F984 File Offset: 0x0000DB84
		[NullableContext(2)]
		public void Toggle(ICommonSession session)
		{
			if (session == null)
			{
				return;
			}
			if (this._draggers.Add(session))
			{
				base.RaiseNetworkEvent(new TetherGunToggleMessage
				{
					Enabled = true
				}, session.ConnectedClient);
				return;
			}
			this._draggers.Remove(session);
			base.RaiseNetworkEvent(new TetherGunToggleMessage
			{
				Enabled = false
			}, session.ConnectedClient);
		}

		// Token: 0x060002DD RID: 733 RVA: 0x0000F9E1 File Offset: 0x0000DBE1
		[NullableContext(2)]
		public bool IsEnabled(ICommonSession session)
		{
			return session != null && this._draggers.Contains(session);
		}

		// Token: 0x060002DE RID: 734 RVA: 0x0000F9F4 File Offset: 0x0000DBF4
		private void OnStartTether(StartTetherEvent msg, EntitySessionEventArgs args)
		{
			IPlayerSession playerSession = args.SenderSession as IPlayerSession;
			if (playerSession == null || !this._admin.CanCommand(playerSession, "tethergun") || !base.Exists(msg.Entity) || base.Deleted(msg.Entity, null) || msg.Coordinates == MapCoordinates.Nullspace || this._tethered.ContainsKey(args.SenderSession))
			{
				return;
			}
			EntityUid tether = base.Spawn("TetherEntity", msg.Coordinates);
			PhysicsComponent bodyA;
			PhysicsComponent bodyB;
			if (!base.TryComp<PhysicsComponent>(tether, ref bodyA) || !base.TryComp<PhysicsComponent>(msg.Entity, ref bodyB))
			{
				base.Del(tether);
				return;
			}
			base.EnsureComp<AdminFrozenComponent>(msg.Entity);
			TransformComponent xform;
			if (base.TryComp<TransformComponent>(msg.Entity, ref xform))
			{
				xform.Anchored = false;
			}
			if (this._container.IsEntityInContainer(msg.Entity, null) && xform != null)
			{
				xform.AttachToGridOrMap();
			}
			PhysicsComponent body;
			if (base.TryComp<PhysicsComponent>(msg.Entity, ref body))
			{
				this._physics.SetBodyStatus(body, 1, true);
			}
			this._physics.WakeBody(tether, false, null, bodyA);
			this._physics.WakeBody(msg.Entity, false, null, bodyB);
			MouseJoint joint = this._joints.CreateMouseJoint(tether, msg.Entity, null, null, "tether-joint");
			float stiffness;
			float damping;
			SharedJointSystem.LinearStiffness(5f, 0.7f, bodyA.Mass, bodyB.Mass, ref stiffness, ref damping);
			joint.Stiffness = stiffness;
			joint.Damping = damping;
			joint.MaxForce = 10000f * bodyB.Mass;
			this._tethered.Add(playerSession, new ValueTuple<EntityUid, EntityUid, Joint>(msg.Entity, tether, joint));
			base.RaiseNetworkEvent(new PredictTetherEvent
			{
				Entity = msg.Entity
			}, args.SenderSession.ConnectedClient);
		}

		// Token: 0x060002DF RID: 735 RVA: 0x0000FBD3 File Offset: 0x0000DDD3
		private void OnStopTether(StopTetherEvent msg, EntitySessionEventArgs args)
		{
			this.StopTether(args.SenderSession);
		}

		// Token: 0x060002E0 RID: 736 RVA: 0x0000FBE4 File Offset: 0x0000DDE4
		private void StopTether(ICommonSession session)
		{
			ValueTuple<EntityUid, EntityUid, Joint> weh;
			if (!this._tethered.TryGetValue(session, out weh))
			{
				return;
			}
			base.RemComp<AdminFrozenComponent>(weh.Item1);
			PhysicsComponent body;
			if (base.TryComp<PhysicsComponent>(weh.Item1, ref body) && !base.HasComp<GhostComponent>(weh.Item1))
			{
				Timer.Spawn(1000, delegate()
				{
					if (this.Deleted(weh.Item1, null))
					{
						return;
					}
					this._physics.SetBodyStatus(body, 0, true);
				}, default(CancellationToken));
			}
			this._joints.RemoveJoint(weh.Item3);
			base.Del(weh.Item2);
			this._tethered.Remove(session);
		}

		// Token: 0x060002E1 RID: 737 RVA: 0x0000FCA4 File Offset: 0x0000DEA4
		private void OnMoveTether(TetherMoveEvent msg, EntitySessionEventArgs args)
		{
			ValueTuple<EntityUid, EntityUid, Joint> tether;
			TransformComponent xform;
			if (!this._tethered.TryGetValue(args.SenderSession, out tether) || !base.TryComp<TransformComponent>(tether.Item2, ref xform) || xform.MapID != msg.Coordinates.MapId)
			{
				return;
			}
			xform.WorldPosition = msg.Coordinates.Position;
		}

		// Token: 0x060002E2 RID: 738 RVA: 0x0000FD04 File Offset: 0x0000DF04
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			RemQueue<ICommonSession> toRemove = default(RemQueue<ICommonSession>);
			EntityQuery<PhysicsComponent> bodyQuery = base.GetEntityQuery<PhysicsComponent>();
			foreach (KeyValuePair<ICommonSession, ValueTuple<EntityUid, EntityUid, Joint>> keyValuePair in this._tethered)
			{
				ICommonSession commonSession;
				ValueTuple<EntityUid, EntityUid, Joint> valueTuple;
				keyValuePair.Deconstruct(out commonSession, out valueTuple);
				ICommonSession session = commonSession;
				ValueTuple<EntityUid, EntityUid, Joint> entity = valueTuple;
				PhysicsComponent body;
				if (base.Deleted(entity.Item1, null) || base.Deleted(entity.Item2, null) || !entity.Item3.Enabled)
				{
					toRemove.Add(session);
				}
				else if (bodyQuery.TryGetComponent(entity.Item1, ref body))
				{
					this._physics.WakeBody(entity.Item1, false, null, body);
				}
			}
			foreach (ICommonSession session2 in toRemove)
			{
				this.StopTether(session2);
			}
		}

		// Token: 0x040001ED RID: 493
		[Dependency]
		private readonly IConGroupController _admin;

		// Token: 0x040001EE RID: 494
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x040001EF RID: 495
		[Dependency]
		private readonly SharedContainerSystem _container;

		// Token: 0x040001F0 RID: 496
		[Dependency]
		private readonly SharedJointSystem _joints;

		// Token: 0x040001F1 RID: 497
		[Dependency]
		private readonly SharedPhysicsSystem _physics;

		// Token: 0x040001F2 RID: 498
		[TupleElementNames(new string[]
		{
			"Entity",
			"Tether",
			"Joint"
		})]
		[Nullable(new byte[]
		{
			1,
			1,
			0,
			1
		})]
		private readonly Dictionary<ICommonSession, ValueTuple<EntityUid, EntityUid, Joint>> _tethered = new Dictionary<ICommonSession, ValueTuple<EntityUid, EntityUid, Joint>>();

		// Token: 0x040001F3 RID: 499
		private readonly HashSet<ICommonSession> _draggers = new HashSet<ICommonSession>();

		// Token: 0x040001F4 RID: 500
		private const string JointId = "tether-joint";
	}
}
