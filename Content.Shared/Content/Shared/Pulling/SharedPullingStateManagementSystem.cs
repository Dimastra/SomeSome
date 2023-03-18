using System;
using System.Runtime.CompilerServices;
using Content.Shared.Physics.Pull;
using Content.Shared.Pulling.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics.Joints;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Timing;

namespace Content.Shared.Pulling
{
	// Token: 0x02000236 RID: 566
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SharedPullingStateManagementSystem : EntitySystem
	{
		// Token: 0x06000635 RID: 1589 RVA: 0x00015DB0 File Offset: 0x00013FB0
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<SharedPullableComponent, ComponentShutdown>(new ComponentEventHandler<SharedPullableComponent, ComponentShutdown>(this.OnShutdown), null, null);
			base.SubscribeLocalEvent<SharedPullableComponent, ComponentGetState>(new ComponentEventRefHandler<SharedPullableComponent, ComponentGetState>(this.OnGetState), null, null);
			base.SubscribeLocalEvent<SharedPullableComponent, ComponentHandleState>(new ComponentEventRefHandler<SharedPullableComponent, ComponentHandleState>(this.OnHandleState), null, null);
		}

		// Token: 0x06000636 RID: 1590 RVA: 0x00015DFF File Offset: 0x00013FFF
		private void OnGetState(EntityUid uid, SharedPullableComponent component, ref ComponentGetState args)
		{
			args.State = new PullableComponentState(component.Puller);
		}

		// Token: 0x06000637 RID: 1591 RVA: 0x00015E14 File Offset: 0x00014014
		private void OnHandleState(EntityUid uid, SharedPullableComponent component, ref ComponentHandleState args)
		{
			PullableComponentState state = args.Current as PullableComponentState;
			if (state == null)
			{
				return;
			}
			if (state.Puller == null)
			{
				this.ForceDisconnectPullable(component);
				return;
			}
			if (component.Puller == state.Puller)
			{
				return;
			}
			SharedPullerComponent comp;
			if (!base.TryComp<SharedPullerComponent>(state.Puller.Value, ref comp))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(53, 2);
				defaultInterpolatedStringHandler.AppendLiteral("Pullable state for entity ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid));
				defaultInterpolatedStringHandler.AppendLiteral(" had invalid puller entity ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(state.Puller.Value));
				Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
				this.ForceDisconnectPullable(component);
				return;
			}
			this.ForceRelationship(comp, component);
		}

		// Token: 0x06000638 RID: 1592 RVA: 0x00015F04 File Offset: 0x00014104
		private void OnShutdown(EntityUid uid, SharedPullableComponent component, ComponentShutdown args)
		{
			if (component.Puller != null)
			{
				this.ForceRelationship(null, component);
			}
		}

		// Token: 0x06000639 RID: 1593 RVA: 0x00015F2C File Offset: 0x0001412C
		private void ForceDisconnect(SharedPullerComponent puller, SharedPullableComponent pullable)
		{
			PhysicsComponent component = this.EntityManager.GetComponent<PhysicsComponent>(puller.Owner);
			PhysicsComponent pullablePhysics = this.EntityManager.GetComponent<PhysicsComponent>(pullable.Owner);
			this.ForceSetMovingTo(pullable, null);
			JointComponent jointComp;
			Joint i;
			if (!this._timing.ApplyingState && pullable.PullJointId != null && base.TryComp<JointComponent>(puller.Owner, ref jointComp) && jointComp.GetJoints.TryGetValue(pullable.PullJointId, out i))
			{
				this._jointSystem.RemoveJoint(i);
			}
			pullable.PullJointId = null;
			puller.Pulling = null;
			pullable.Puller = null;
			PullStoppedMessage message = new PullStoppedMessage(component, pullablePhysics);
			base.RaiseLocalEvent<PullStoppedMessage>(puller.Owner, message, false);
			if (base.Initialized(pullable.Owner, null))
			{
				base.RaiseLocalEvent<PullStoppedMessage>(pullable.Owner, message, true);
			}
			base.Dirty(puller, null);
			base.Dirty(pullable, null);
		}

		// Token: 0x0600063A RID: 1594 RVA: 0x0001601C File Offset: 0x0001421C
		[NullableContext(2)]
		public void ForceRelationship(SharedPullerComponent puller, SharedPullableComponent pullable)
		{
			if (pullable != null && puller != null)
			{
				EntityUid? pulling = puller.Pulling;
				EntityUid owner = pullable.Owner;
				if (pulling != null && (pulling == null || pulling.GetValueOrDefault() == owner))
				{
					return;
				}
			}
			EntityUid? pullableOldPullerE = (pullable != null) ? pullable.Puller : null;
			if (pullableOldPullerE != null)
			{
				this.ForceDisconnect(this.EntityManager.GetComponent<SharedPullerComponent>(pullableOldPullerE.Value), pullable);
			}
			EntityUid? pullerOldPullableE = (puller != null) ? puller.Pulling : null;
			if (pullerOldPullableE != null)
			{
				this.ForceDisconnect(puller, this.EntityManager.GetComponent<SharedPullableComponent>(pullerOldPullableE.Value));
			}
			if (puller != null && pullable != null)
			{
				PhysicsComponent pullerPhysics = this.EntityManager.GetComponent<PhysicsComponent>(puller.Owner);
				PhysicsComponent pullablePhysics = this.EntityManager.GetComponent<PhysicsComponent>(pullable.Owner);
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(11, 1);
				defaultInterpolatedStringHandler.AppendLiteral("pull-joint-");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(pullable.Owner);
				pullable.PullJointId = defaultInterpolatedStringHandler.ToStringAndClear();
				puller.Pulling = new EntityUid?(pullable.Owner);
				pullable.Puller = new EntityUid?(puller.Owner);
				if (!this._timing.ApplyingState)
				{
					Box2 hardAABB = this._physics.GetHardAABB(puller.Owner, null, null, null);
					Box2 hardAABB2 = this._physics.GetHardAABB(pullable.Owner, null, pullablePhysics, null);
					Box2 union = hardAABB.Union(ref hardAABB2);
					float length = Math.Max(union.Size.X, union.Size.Y) * 0.75f;
					SharedJointSystem jointSystem = this._jointSystem;
					EntityUid owner2 = pullablePhysics.Owner;
					EntityUid owner3 = pullerPhysics.Owner;
					string pullJointId = pullable.PullJointId;
					DistanceJoint distanceJoint = jointSystem.CreateDistanceJoint(owner2, owner3, null, null, pullJointId, null, null);
					distanceJoint.CollideConnected = false;
					distanceJoint.MaxLength = Math.Max(1f, length);
					distanceJoint.Length = length * 0.75f;
					distanceJoint.MinLength = 0f;
					distanceJoint.Stiffness = 1f;
				}
				PullStartedMessage message = new PullStartedMessage(pullerPhysics, pullablePhysics);
				base.RaiseLocalEvent<PullStartedMessage>(puller.Owner, message, false);
				base.RaiseLocalEvent<PullStartedMessage>(pullable.Owner, message, true);
				base.Dirty(puller, null);
				base.Dirty(pullable, null);
			}
		}

		// Token: 0x0600063B RID: 1595 RVA: 0x00016272 File Offset: 0x00014472
		public void ForceDisconnectPuller(SharedPullerComponent puller)
		{
			this.ForceRelationship(puller, null);
		}

		// Token: 0x0600063C RID: 1596 RVA: 0x0001627C File Offset: 0x0001447C
		public void ForceDisconnectPullable(SharedPullableComponent pullable)
		{
			this.ForceRelationship(null, pullable);
		}

		// Token: 0x0600063D RID: 1597 RVA: 0x00016288 File Offset: 0x00014488
		public void ForceSetMovingTo(SharedPullableComponent pullable, EntityCoordinates? movingTo)
		{
			if (pullable.MovingTo == movingTo)
			{
				return;
			}
			if (pullable.Puller == null && movingTo != null)
			{
				return;
			}
			pullable.MovingTo = movingTo;
			if (movingTo == null)
			{
				base.RaiseLocalEvent<PullableStopMovingMessage>(pullable.Owner, new PullableStopMovingMessage(), true);
				return;
			}
			base.RaiseLocalEvent<PullableMoveMessage>(pullable.Owner, new PullableMoveMessage(), true);
		}

		// Token: 0x0400064F RID: 1615
		[Dependency]
		private readonly SharedJointSystem _jointSystem;

		// Token: 0x04000650 RID: 1616
		[Dependency]
		private readonly SharedPhysicsSystem _physics;

		// Token: 0x04000651 RID: 1617
		[Dependency]
		private readonly IGameTiming _timing;
	}
}
