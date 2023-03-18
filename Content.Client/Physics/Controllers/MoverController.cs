using System;
using System.Runtime.CompilerServices;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.Pulling.Components;
using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics.Joints;
using Robust.Shared.Timing;

namespace Content.Client.Physics.Controllers
{
	// Token: 0x020001BB RID: 443
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MoverController : SharedMoverController
	{
		// Token: 0x06000B5F RID: 2911 RVA: 0x00042078 File Offset: 0x00040278
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<RelayInputMoverComponent, PlayerAttachedEvent>(new ComponentEventHandler<RelayInputMoverComponent, PlayerAttachedEvent>(this.OnRelayPlayerAttached), null, null);
			base.SubscribeLocalEvent<RelayInputMoverComponent, PlayerDetachedEvent>(new ComponentEventHandler<RelayInputMoverComponent, PlayerDetachedEvent>(this.OnRelayPlayerDetached), null, null);
			base.SubscribeLocalEvent<InputMoverComponent, PlayerAttachedEvent>(new ComponentEventHandler<InputMoverComponent, PlayerAttachedEvent>(this.OnPlayerAttached), null, null);
			base.SubscribeLocalEvent<InputMoverComponent, PlayerDetachedEvent>(new ComponentEventHandler<InputMoverComponent, PlayerDetachedEvent>(this.OnPlayerDetached), null, null);
		}

		// Token: 0x06000B60 RID: 2912 RVA: 0x000420DC File Offset: 0x000402DC
		private void OnRelayPlayerAttached(EntityUid uid, RelayInputMoverComponent component, PlayerAttachedEvent args)
		{
			InputMoverComponent component2;
			if (base.TryComp<InputMoverComponent>(component.RelayEntity, ref component2))
			{
				base.SetMoveInput(component2, MoveButtons.None);
			}
		}

		// Token: 0x06000B61 RID: 2913 RVA: 0x00042104 File Offset: 0x00040304
		private void OnRelayPlayerDetached(EntityUid uid, RelayInputMoverComponent component, PlayerDetachedEvent args)
		{
			InputMoverComponent component2;
			if (base.TryComp<InputMoverComponent>(component.RelayEntity, ref component2))
			{
				base.SetMoveInput(component2, MoveButtons.None);
			}
		}

		// Token: 0x06000B62 RID: 2914 RVA: 0x00042129 File Offset: 0x00040329
		private void OnPlayerAttached(EntityUid uid, InputMoverComponent component, PlayerAttachedEvent args)
		{
			base.SetMoveInput(component, MoveButtons.None);
		}

		// Token: 0x06000B63 RID: 2915 RVA: 0x00042129 File Offset: 0x00040329
		private void OnPlayerDetached(EntityUid uid, InputMoverComponent component, PlayerDetachedEvent args)
		{
			base.SetMoveInput(component, MoveButtons.None);
		}

		// Token: 0x06000B64 RID: 2916 RVA: 0x00042134 File Offset: 0x00040334
		public override void UpdateBeforeSolve(bool prediction, float frameTime)
		{
			base.UpdateBeforeSolve(prediction, frameTime);
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			EntityUid? entityUid = (localPlayer != null) ? localPlayer.ControlledEntity : null;
			if (entityUid != null)
			{
				EntityUid valueOrDefault = entityUid.GetValueOrDefault();
				if (valueOrDefault.Valid)
				{
					RelayInputMoverComponent relayInputMoverComponent;
					MovementRelayTargetComponent movementRelayTargetComponent;
					if (base.TryComp<RelayInputMoverComponent>(valueOrDefault, ref relayInputMoverComponent) && base.TryComp<MovementRelayTargetComponent>(relayInputMoverComponent.RelayEntity, ref movementRelayTargetComponent))
					{
						this.HandleClientsideMovement(relayInputMoverComponent.RelayEntity.Value, frameTime);
					}
					this.HandleClientsideMovement(valueOrDefault, frameTime);
					return;
				}
			}
		}

		// Token: 0x06000B65 RID: 2917 RVA: 0x000421BC File Offset: 0x000403BC
		private void HandleClientsideMovement(EntityUid player, float frameTime)
		{
			EntityQuery<TransformComponent> entityQuery = base.GetEntityQuery<TransformComponent>();
			EntityQuery<InputMoverComponent> entityQuery2 = base.GetEntityQuery<InputMoverComponent>();
			EntityQuery<MovementRelayTargetComponent> entityQuery3 = base.GetEntityQuery<MovementRelayTargetComponent>();
			InputMoverComponent inputMoverComponent;
			TransformComponent transformComponent;
			if (!base.TryComp<InputMoverComponent>(player, ref inputMoverComponent) || !entityQuery.TryGetComponent(player, ref transformComponent))
			{
				return;
			}
			TransformComponent xform = transformComponent;
			PhysicsComponent physicsComponent;
			if (inputMoverComponent.ToParent && base.HasComp<RelayInputMoverComponent>(transformComponent.ParentUid))
			{
				if (!base.TryComp<PhysicsComponent>(transformComponent.ParentUid, ref physicsComponent) || !base.TryComp<TransformComponent>(transformComponent.ParentUid, ref xform))
				{
					return;
				}
			}
			else if (!base.TryComp<PhysicsComponent>(player, ref physicsComponent))
			{
				return;
			}
			physicsComponent.Predict = true;
			JointComponent jointComponent;
			if (base.TryComp<JointComponent>(player, ref jointComponent))
			{
				foreach (Joint joint in jointComponent.GetJoints.Values)
				{
					PhysicsComponent physicsComponent2;
					if (base.TryComp<PhysicsComponent>(joint.BodyAUid, ref physicsComponent2))
					{
						physicsComponent2.Predict = true;
					}
					if (base.TryComp<PhysicsComponent>(joint.BodyBUid, ref physicsComponent2))
					{
						physicsComponent2.Predict = true;
					}
				}
			}
			SharedPullableComponent sharedPullableComponent;
			if (base.TryComp<SharedPullableComponent>(player, ref sharedPullableComponent))
			{
				EntityUid? puller = sharedPullableComponent.Puller;
				if (puller != null)
				{
					EntityUid valueOrDefault = puller.GetValueOrDefault();
					PhysicsComponent physicsComponent3;
					if (valueOrDefault.Valid && base.TryComp<PhysicsComponent>(valueOrDefault, ref physicsComponent3))
					{
						physicsComponent3.Predict = false;
						physicsComponent.Predict = false;
						SharedPullerComponent sharedPullerComponent;
						PhysicsComponent physicsComponent4;
						if (base.TryComp<SharedPullerComponent>(player, ref sharedPullerComponent) && sharedPullerComponent.Pulling != null && base.TryComp<PhysicsComponent>(sharedPullerComponent.Pulling, ref physicsComponent4))
						{
							physicsComponent4.Predict = false;
						}
					}
				}
			}
			base.HandleMobMovement(player, inputMoverComponent, physicsComponent, xform, frameTime, entityQuery, entityQuery2, entityQuery3);
		}

		// Token: 0x06000B66 RID: 2918 RVA: 0x0004235C File Offset: 0x0004055C
		protected override bool CanSound()
		{
			IGameTiming timing = this._timing;
			return timing != null && timing.IsFirstTimePredicted && timing.InSimulation;
		}

		// Token: 0x0400058F RID: 1423
		[Dependency]
		private readonly IGameTiming _timing;

		// Token: 0x04000590 RID: 1424
		[Dependency]
		private readonly IPlayerManager _playerManager;
	}
}
