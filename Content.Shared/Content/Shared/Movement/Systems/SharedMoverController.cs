using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Content.Shared.CCVar;
using Content.Shared.Friction;
using Content.Shared.Gravity;
using Content.Shared.Input;
using Content.Shared.Inventory;
using Content.Shared.Maps;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Events;
using Content.Shared.Parallax.Biomes;
using Content.Shared.Pulling.Components;
using Content.Shared.Tag;
using Robust.Shared.Audio;
using Robust.Shared.Configuration;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Controllers;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Players;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;

namespace Content.Shared.Movement.Systems
{
	// Token: 0x020002DC RID: 732
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedMoverController : VirtualController
	{
		// Token: 0x06000813 RID: 2067 RVA: 0x0001AC90 File Offset: 0x00018E90
		public override void Initialize()
		{
			base.Initialize();
			this.Sawmill = Logger.GetSawmill("mover");
			this.InitializeFootsteps();
			this.InitializeInput();
			this.InitializeMob();
			this.InitializeRelay();
			this._configManager.OnValueChanged<bool>(CCVars.RelativeMovement, new Action<bool>(this.SetRelativeMovement), true);
			this._configManager.OnValueChanged<float>(CCVars.StopSpeed, new Action<float>(this.SetStopSpeed), true);
			base.UpdatesBefore.Add(typeof(TileFrictionController));
		}

		// Token: 0x06000814 RID: 2068 RVA: 0x0001AD1A File Offset: 0x00018F1A
		private void SetRelativeMovement(bool value)
		{
			this._relativeMovement = value;
		}

		// Token: 0x06000815 RID: 2069 RVA: 0x0001AD23 File Offset: 0x00018F23
		private void SetStopSpeed(float value)
		{
			this._stopSpeed = value;
		}

		// Token: 0x06000816 RID: 2070 RVA: 0x0001AD2C File Offset: 0x00018F2C
		public override void Shutdown()
		{
			base.Shutdown();
			this.ShutdownInput();
			this._configManager.UnsubValueChanged<bool>(CCVars.RelativeMovement, new Action<bool>(this.SetRelativeMovement));
			this._configManager.UnsubValueChanged<float>(CCVars.StopSpeed, new Action<float>(this.SetStopSpeed));
		}

		// Token: 0x06000817 RID: 2071 RVA: 0x0001AD7D File Offset: 0x00018F7D
		public override void UpdateAfterSolve(bool prediction, float frameTime)
		{
			base.UpdateAfterSolve(prediction, frameTime);
			this.UsedMobMovement.Clear();
		}

		// Token: 0x06000818 RID: 2072 RVA: 0x0001AD94 File Offset: 0x00018F94
		protected void HandleMobMovement(EntityUid uid, InputMoverComponent mover, PhysicsComponent physicsComponent, TransformComponent xform, float frameTime, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<TransformComponent> xformQuery, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<InputMoverComponent> moverQuery, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<MovementRelayTargetComponent> relayTargetQuery)
		{
			bool canMove = mover.CanMove;
			MovementRelayTargetComponent relayTarget;
			if (relayTargetQuery.TryGetComponent(uid, ref relayTarget) && relayTarget.Entities.Count > 0)
			{
				bool found = false;
				foreach (EntityUid ent in relayTarget.Entities)
				{
					InputMoverComponent relayedMover;
					if (!this._mobState.IsIncapacitated(ent, null) && moverQuery.TryGetComponent(ent, ref relayedMover))
					{
						found = true;
						mover.RelativeEntity = relayedMover.RelativeEntity;
						mover.RelativeRotation = relayedMover.RelativeRotation;
						mover.TargetRelativeRotation = relayedMover.TargetRelativeRotation;
						break;
					}
				}
				canMove = (canMove && found);
			}
			if (mover.LerpAccumulator > 0f)
			{
				base.Dirty(mover, null);
				mover.LerpAccumulator -= frameTime;
				if (mover.LerpAccumulator <= 0f)
				{
					mover.LerpAccumulator = 0f;
					EntityUid? relative = xform.GridUid;
					EntityUid? entityUid = relative;
					if (entityUid == null)
					{
						relative = xform.MapUid;
					}
					if (!mover.RelativeEntity.Equals(relative))
					{
						Angle currentRotation = Angle.Zero;
						Angle targetRotation = Angle.Zero;
						TransformComponent oldRelativeXform;
						if (xformQuery.TryGetComponent(mover.RelativeEntity, ref oldRelativeXform))
						{
							currentRotation = oldRelativeXform.WorldRotation + mover.RelativeRotation;
						}
						TransformComponent relativeXform;
						if (xformQuery.TryGetComponent(relative, ref relativeXform))
						{
							mover.RelativeRotation = (currentRotation - relativeXform.WorldRotation).FlipPositive();
						}
						if (relative != null && this._mapManager.IsMap(relative.Value))
						{
							targetRotation = currentRotation.FlipPositive().Reduced();
						}
						else if (relative != null && this._mapManager.IsGrid(relative.Value))
						{
							if (this.CameraRotationLocked)
							{
								targetRotation = Angle.Zero;
							}
							else
							{
								targetRotation = DirectionExtensions.ToAngle(mover.RelativeRotation.GetCardinalDir()).Reduced();
							}
						}
						mover.RelativeEntity = relative;
						mover.TargetRelativeRotation = targetRotation;
					}
				}
			}
			Angle angleDiff = Angle.ShortestDistance(ref mover.RelativeRotation, ref mover.TargetRelativeRotation);
			if (!angleDiff.EqualsApprox(Angle.Zero, 0.001))
			{
				double adjustment = angleDiff * 5.0 * (double)frameTime;
				double minAdjustment = 0.01 * (double)frameTime;
				if (angleDiff < 0.0)
				{
					adjustment = Math.Min(adjustment, -minAdjustment);
					adjustment = Math.Clamp(adjustment, angleDiff, -angleDiff);
				}
				else
				{
					adjustment = Math.Max(adjustment, minAdjustment);
					adjustment = Math.Clamp(adjustment, -angleDiff, angleDiff);
				}
				mover.RelativeRotation += adjustment;
				mover.RelativeRotation.FlipPositive();
				base.Dirty(mover, null);
			}
			else if (!angleDiff.Equals(Angle.Zero))
			{
				mover.TargetRelativeRotation.FlipPositive();
				mover.RelativeRotation = mover.TargetRelativeRotation;
				base.Dirty(mover, null);
			}
			SharedPullableComponent pullable;
			if (!canMove || physicsComponent.BodyStatus != null || (base.TryComp<SharedPullableComponent>(uid, ref pullable) && pullable.BeingPulled))
			{
				this.UsedMobMovement[uid] = false;
				return;
			}
			this.UsedMobMovement[uid] = true;
			bool weightless = this._gravity.IsWeightless(uid, physicsComponent, xform);
			ValueTuple<Vector2, Vector2> velocityInput = this.GetVelocityInput(mover);
			Vector2 walkDir = velocityInput.Item1;
			Vector2 sprintDir = velocityInput.Item2;
			bool touching = false;
			if (weightless)
			{
				if (xform.GridUid != null)
				{
					touching = true;
				}
				if (!touching)
				{
					CanWeightlessMoveEvent ev = new CanWeightlessMoveEvent();
					base.RaiseLocalEvent<CanWeightlessMoveEvent>(uid, ref ev, false);
					touching = ev.CanMove;
					MobMoverComponent mobMover;
					if (!touching && base.TryComp<MobMoverComponent>(uid, ref mobMover))
					{
						touching |= this.IsAroundCollider(this.PhysicsSystem, xform, mobMover, physicsComponent);
					}
				}
			}
			MovementSpeedModifierComponent moveSpeedComponent = base.CompOrNull<MovementSpeedModifierComponent>(uid);
			float walkSpeed = (moveSpeedComponent != null) ? moveSpeedComponent.CurrentWalkSpeed : 2.5f;
			float sprintSpeed = (moveSpeedComponent != null) ? moveSpeedComponent.CurrentSprintSpeed : 4.5f;
			Vector2 total = walkDir * walkSpeed + sprintDir * sprintSpeed;
			Angle parentRotation = this.GetParentGridAngle(mover, xformQuery);
			Vector2 worldTotal = this._relativeMovement ? parentRotation.RotateVec(ref total) : total;
			Vector2 velocity = physicsComponent.LinearVelocity;
			float friction;
			float weightlessModifier;
			float accel;
			if (weightless)
			{
				if (worldTotal != Vector2.Zero && touching)
				{
					friction = ((moveSpeedComponent != null) ? moveSpeedComponent.WeightlessFriction : 1f);
				}
				else
				{
					friction = ((moveSpeedComponent != null) ? moveSpeedComponent.WeightlessFrictionNoInput : 0.2f);
				}
				weightlessModifier = ((moveSpeedComponent != null) ? moveSpeedComponent.WeightlessModifier : 0.7f);
				accel = ((moveSpeedComponent != null) ? moveSpeedComponent.WeightlessAcceleration : 1f);
			}
			else
			{
				if (worldTotal != Vector2.Zero || (moveSpeedComponent == null || moveSpeedComponent.FrictionNoInput == null))
				{
					friction = ((moveSpeedComponent != null) ? moveSpeedComponent.Friction : 20f);
				}
				else
				{
					friction = (moveSpeedComponent.FrictionNoInput ?? 20f);
				}
				weightlessModifier = 1f;
				accel = ((moveSpeedComponent != null) ? moveSpeedComponent.Acceleration : 20f);
			}
			float minimumFrictionSpeed = (moveSpeedComponent != null) ? moveSpeedComponent.MinimumFrictionSpeed : 0.005f;
			this.Friction(minimumFrictionSpeed, frameTime, friction, ref velocity);
			if (worldTotal != Vector2.Zero)
			{
				Angle worldRot = this._transform.GetWorldRotation(xform);
				this._transform.SetLocalRotation(xform, xform.LocalRotation + DirectionExtensions.ToWorldAngle(worldTotal) - worldRot);
				MobMoverComponent mobMover2;
				SoundSpecifier sound;
				if (!weightless && base.TryComp<MobMoverComponent>(mover.Owner, ref mobMover2) && this.TryGetSound(weightless, mover, mobMover2, xform, out sound))
				{
					float soundModifier = mover.Sprinting ? 1f : 0f;
					AudioParams audioParams = sound.Params.WithVolume(3f * soundModifier).WithVariation(new float?(sound.Params.Variation.GetValueOrDefault()));
					if (relayTarget != null)
					{
						using (List<EntityUid>.Enumerator enumerator = relayTarget.Entities.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								EntityUid ent2 = enumerator.Current;
								this._audio.PlayPredicted(sound, uid, new EntityUid?(ent2), new AudioParams?(audioParams));
							}
							goto IL_689;
						}
					}
					this._audio.PlayPredicted(sound, uid, new EntityUid?(uid), new AudioParams?(audioParams));
				}
			}
			IL_689:
			worldTotal *= weightlessModifier;
			if (!weightless || touching)
			{
				this.Accelerate(ref velocity, worldTotal, accel, frameTime);
			}
			this.PhysicsSystem.SetLinearVelocity(uid, velocity, true, true, null, physicsComponent);
			this.PhysicsSystem.SetAngularVelocity(uid, 0f, true, null, physicsComponent);
		}

		// Token: 0x06000819 RID: 2073 RVA: 0x0001B490 File Offset: 0x00019690
		private void Friction(float minimumFrictionSpeed, float frameTime, float friction, ref Vector2 velocity)
		{
			float speed = velocity.Length;
			if (speed < minimumFrictionSpeed)
			{
				return;
			}
			float drop = 0f;
			float control = MathF.Max(this._stopSpeed, speed);
			drop += control * friction * frameTime;
			float newSpeed = MathF.Max(0f, speed - drop);
			if (newSpeed.Equals(speed))
			{
				return;
			}
			newSpeed /= speed;
			velocity *= newSpeed;
		}

		// Token: 0x0600081A RID: 2074 RVA: 0x0001B4F8 File Offset: 0x000196F8
		private void Accelerate(ref Vector2 currentVelocity, in Vector2 velocity, float accel, float frameTime)
		{
			Vector2 wishDir = (velocity != Vector2.Zero) ? velocity.Normalized : Vector2.Zero;
			float wishSpeed = velocity.Length;
			float currentSpeed = Vector2.Dot(currentVelocity, wishDir);
			float addSpeed = wishSpeed - currentSpeed;
			if (addSpeed <= 0f)
			{
				return;
			}
			float accelSpeed = accel * frameTime * wishSpeed;
			accelSpeed = MathF.Min(accelSpeed, addSpeed);
			currentVelocity += wishDir * accelSpeed;
		}

		// Token: 0x0600081B RID: 2075 RVA: 0x0001B574 File Offset: 0x00019774
		public bool UseMobMovement(EntityUid uid)
		{
			bool used;
			return this.UsedMobMovement.TryGetValue(uid, out used) && used;
		}

		// Token: 0x0600081C RID: 2076 RVA: 0x0001B594 File Offset: 0x00019794
		private bool IsAroundCollider(SharedPhysicsSystem broadPhaseSystem, TransformComponent transform, MobMoverComponent mover, PhysicsComponent collider)
		{
			Box2 enlargedAABB = this._lookup.GetWorldAABB(collider.Owner, transform).Enlarged(mover.GrabRangeVV);
			foreach (PhysicsComponent otherCollider in broadPhaseSystem.GetCollidingEntities(transform.MapID, ref enlargedAABB))
			{
				SharedPullableComponent pullable;
				if (otherCollider != collider && otherCollider.BodyType == 4 && otherCollider.CanCollide && ((collider.CollisionMask & otherCollider.CollisionLayer) != 0 || (otherCollider.CollisionMask & collider.CollisionLayer) != 0) && (!base.TryComp<SharedPullableComponent>(otherCollider.Owner, ref pullable) || !pullable.BeingPulled))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600081D RID: 2077
		protected abstract bool CanSound();

		// Token: 0x0600081E RID: 2078 RVA: 0x0001B65C File Offset: 0x0001985C
		private bool TryGetSound(bool weightless, InputMoverComponent mover, MobMoverComponent mobMover, TransformComponent xform, [Nullable(2)] [NotNullWhen(true)] out SoundSpecifier sound)
		{
			sound = null;
			if (!this.CanSound() || !this._tags.HasTag(mover.Owner, "FootstepSound"))
			{
				return false;
			}
			EntityCoordinates coordinates = xform.Coordinates;
			float distanceNeeded = mover.Sprinting ? 2f : 1.5f;
			if (weightless)
			{
				return false;
			}
			float distance;
			if (!coordinates.TryDistance(this.EntityManager, mobMover.LastPosition, ref distance) || distance > distanceNeeded)
			{
				mobMover.StepSoundDistance = distanceNeeded;
			}
			else
			{
				mobMover.StepSoundDistance += distance;
			}
			mobMover.LastPosition = coordinates;
			if (mobMover.StepSoundDistance < distanceNeeded)
			{
				return false;
			}
			mobMover.StepSoundDistance -= distanceNeeded;
			FootstepModifierComponent moverModifier;
			if (base.TryComp<FootstepModifierComponent>(mover.Owner, ref moverModifier))
			{
				sound = moverModifier.Sound;
				return true;
			}
			EntityUid? shoes;
			FootstepModifierComponent modifier;
			if (this._inventory.TryGetSlotEntity(mover.Owner, "shoes", out shoes, null, null) && base.TryComp<FootstepModifierComponent>(shoes, ref modifier))
			{
				sound = modifier.Sound;
				return true;
			}
			return this.TryGetFootstepSound(xform, shoes != null, out sound);
		}

		// Token: 0x0600081F RID: 2079 RVA: 0x0001B768 File Offset: 0x00019968
		private bool TryGetFootstepSound(TransformComponent xform, bool haveShoes, [Nullable(2)] [NotNullWhen(true)] out SoundSpecifier sound)
		{
			sound = null;
			if (xform.GridUid == null)
			{
				FootstepModifierComponent modifier;
				if (base.TryComp<FootstepModifierComponent>(xform.MapUid, ref modifier))
				{
					sound = modifier.Sound;
					return true;
				}
				return false;
			}
			else
			{
				MapGridComponent grid = this._mapManager.GetGrid(xform.GridUid.Value);
				Vector2i position = grid.LocalToTile(xform.Coordinates);
				EntityUid? maybeFootstep;
				while (grid.GetAnchoredEntitiesEnumerator(position).MoveNext(ref maybeFootstep))
				{
					FootstepModifierComponent footstep;
					if (base.TryComp<FootstepModifierComponent>(maybeFootstep, ref footstep))
					{
						sound = footstep.Sound;
						return true;
					}
				}
				TileRef tileRef;
				if (!grid.TryGetTileRef(position, ref tileRef))
				{
					sound = null;
					return false;
				}
				ContentTileDefinition def = (ContentTileDefinition)this._tileDefinitionManager[(int)tileRef.Tile.TypeId];
				sound = (haveShoes ? def.FootstepSounds : def.BarestepSounds);
				return sound != null;
			}
		}

		// Token: 0x06000820 RID: 2080 RVA: 0x0001B83F File Offset: 0x00019A3F
		private void InitializeFootsteps()
		{
			base.SubscribeLocalEvent<FootstepModifierComponent, ComponentGetState>(new ComponentEventRefHandler<FootstepModifierComponent, ComponentGetState>(this.OnFootGetState), null, null);
			base.SubscribeLocalEvent<FootstepModifierComponent, ComponentHandleState>(new ComponentEventRefHandler<FootstepModifierComponent, ComponentHandleState>(this.OnFootHandleState), null, null);
		}

		// Token: 0x06000821 RID: 2081 RVA: 0x0001B86C File Offset: 0x00019A6C
		private void OnFootHandleState(EntityUid uid, FootstepModifierComponent component, ref ComponentHandleState args)
		{
			SharedMoverController.FootstepModifierComponentState state = args.Current as SharedMoverController.FootstepModifierComponentState;
			if (state == null)
			{
				return;
			}
			component.Sound = state.Sound;
		}

		// Token: 0x06000822 RID: 2082 RVA: 0x0001B895 File Offset: 0x00019A95
		private void OnFootGetState(EntityUid uid, FootstepModifierComponent component, ref ComponentGetState args)
		{
			args.State = new SharedMoverController.FootstepModifierComponentState
			{
				Sound = component.Sound
			};
		}

		// Token: 0x1700018D RID: 397
		// (get) Token: 0x06000823 RID: 2083 RVA: 0x0001B8AE File Offset: 0x00019AAE
		// (set) Token: 0x06000824 RID: 2084 RVA: 0x0001B8B6 File Offset: 0x00019AB6
		public bool CameraRotationLocked { get; set; }

		// Token: 0x06000825 RID: 2085 RVA: 0x0001B8C0 File Offset: 0x00019AC0
		private void InitializeInput()
		{
			SharedMoverController.MoverDirInputCmdHandler moveUpCmdHandler = new SharedMoverController.MoverDirInputCmdHandler(this, 4);
			SharedMoverController.MoverDirInputCmdHandler moveLeftCmdHandler = new SharedMoverController.MoverDirInputCmdHandler(this, 6);
			SharedMoverController.MoverDirInputCmdHandler moveRightCmdHandler = new SharedMoverController.MoverDirInputCmdHandler(this, 2);
			SharedMoverController.MoverDirInputCmdHandler moveDownCmdHandler = new SharedMoverController.MoverDirInputCmdHandler(this, 0);
			CommandBinds.Builder.Bind(EngineKeyFunctions.MoveUp, moveUpCmdHandler).Bind(EngineKeyFunctions.MoveLeft, moveLeftCmdHandler).Bind(EngineKeyFunctions.MoveRight, moveRightCmdHandler).Bind(EngineKeyFunctions.MoveDown, moveDownCmdHandler).Bind(EngineKeyFunctions.Walk, new SharedMoverController.WalkInputCmdHandler(this)).Bind(EngineKeyFunctions.CameraRotateLeft, new SharedMoverController.CameraRotateInputCmdHandler(this, 2)).Bind(EngineKeyFunctions.CameraRotateRight, new SharedMoverController.CameraRotateInputCmdHandler(this, 6)).Bind(EngineKeyFunctions.CameraReset, new SharedMoverController.CameraResetInputCmdHandler(this)).Bind(ContentKeyFunctions.ShuttleStrafeUp, new SharedMoverController.ShuttleInputCmdHandler(this, ShuttleButtons.StrafeUp)).Bind(ContentKeyFunctions.ShuttleStrafeLeft, new SharedMoverController.ShuttleInputCmdHandler(this, ShuttleButtons.StrafeLeft)).Bind(ContentKeyFunctions.ShuttleStrafeRight, new SharedMoverController.ShuttleInputCmdHandler(this, ShuttleButtons.StrafeRight)).Bind(ContentKeyFunctions.ShuttleStrafeDown, new SharedMoverController.ShuttleInputCmdHandler(this, ShuttleButtons.StrafeDown)).Bind(ContentKeyFunctions.ShuttleRotateLeft, new SharedMoverController.ShuttleInputCmdHandler(this, ShuttleButtons.RotateLeft)).Bind(ContentKeyFunctions.ShuttleRotateRight, new SharedMoverController.ShuttleInputCmdHandler(this, ShuttleButtons.RotateRight)).Bind(ContentKeyFunctions.ShuttleBrake, new SharedMoverController.ShuttleInputCmdHandler(this, ShuttleButtons.Brake)).Register<SharedMoverController>();
			base.SubscribeLocalEvent<InputMoverComponent, ComponentInit>(new ComponentEventHandler<InputMoverComponent, ComponentInit>(this.OnInputInit), null, null);
			base.SubscribeLocalEvent<InputMoverComponent, ComponentGetState>(new ComponentEventRefHandler<InputMoverComponent, ComponentGetState>(this.OnInputGetState), null, null);
			base.SubscribeLocalEvent<InputMoverComponent, ComponentHandleState>(new ComponentEventRefHandler<InputMoverComponent, ComponentHandleState>(this.OnInputHandleState), null, null);
			base.SubscribeLocalEvent<InputMoverComponent, EntParentChangedMessage>(new ComponentEventRefHandler<InputMoverComponent, EntParentChangedMessage>(this.OnInputParentChange), null, null);
			this._configManager.OnValueChanged<bool>(CCVars.CameraRotationLocked, new Action<bool>(this.SetCameraRotationLocked), true);
			this._configManager.OnValueChanged<bool>(CCVars.GameDiagonalMovement, new Action<bool>(this.SetDiagonalMovement), true);
		}

		// Token: 0x06000826 RID: 2086 RVA: 0x0001BA6A File Offset: 0x00019C6A
		private void SetCameraRotationLocked(bool obj)
		{
			this.CameraRotationLocked = obj;
		}

		// Token: 0x06000827 RID: 2087 RVA: 0x0001BA73 File Offset: 0x00019C73
		protected void SetMoveInput(InputMoverComponent component, MoveButtons buttons)
		{
			if (component.HeldMoveButtons == buttons)
			{
				return;
			}
			component.HeldMoveButtons = buttons;
			base.Dirty(component, null);
		}

		// Token: 0x06000828 RID: 2088 RVA: 0x0001BA90 File Offset: 0x00019C90
		private void OnInputHandleState(EntityUid uid, InputMoverComponent component, ref ComponentHandleState args)
		{
			SharedMoverController.InputMoverComponentState state = args.Current as SharedMoverController.InputMoverComponentState;
			if (state == null)
			{
				return;
			}
			component.HeldMoveButtons = state.Buttons;
			component.LastInputTick = GameTick.Zero;
			component.LastInputSubTick = 0;
			component.CanMove = state.CanMove;
			component.RelativeRotation = state.RelativeRotation;
			component.TargetRelativeRotation = state.TargetRelativeRotation;
			component.RelativeEntity = state.RelativeEntity;
			component.LerpAccumulator = state.LerpAccumulator;
		}

		// Token: 0x06000829 RID: 2089 RVA: 0x0001BB07 File Offset: 0x00019D07
		private void OnInputGetState(EntityUid uid, InputMoverComponent component, ref ComponentGetState args)
		{
			args.State = new SharedMoverController.InputMoverComponentState(component.HeldMoveButtons, component.CanMove, component.RelativeRotation, component.TargetRelativeRotation, component.RelativeEntity, component.LerpAccumulator);
		}

		// Token: 0x0600082A RID: 2090 RVA: 0x0001BB38 File Offset: 0x00019D38
		private void ShutdownInput()
		{
			CommandBinds.Unregister<SharedMoverController>();
			this._configManager.UnsubValueChanged<bool>(CCVars.CameraRotationLocked, new Action<bool>(this.SetCameraRotationLocked));
			this._configManager.UnsubValueChanged<bool>(CCVars.GameDiagonalMovement, new Action<bool>(this.SetDiagonalMovement));
		}

		// Token: 0x1700018E RID: 398
		// (get) Token: 0x0600082B RID: 2091 RVA: 0x0001BB77 File Offset: 0x00019D77
		// (set) Token: 0x0600082C RID: 2092 RVA: 0x0001BB7F File Offset: 0x00019D7F
		public bool DiagonalMovementEnabled { get; private set; }

		// Token: 0x0600082D RID: 2093 RVA: 0x0001BB88 File Offset: 0x00019D88
		private void SetDiagonalMovement(bool value)
		{
			this.DiagonalMovementEnabled = value;
		}

		// Token: 0x0600082E RID: 2094 RVA: 0x0001BB91 File Offset: 0x00019D91
		protected virtual void HandleShuttleInput(EntityUid uid, ShuttleButtons button, ushort subTick, bool state)
		{
		}

		// Token: 0x0600082F RID: 2095 RVA: 0x0001BB94 File Offset: 0x00019D94
		public void RotateCamera(EntityUid uid, Angle angle)
		{
			InputMoverComponent mover;
			if (this.CameraRotationLocked || !base.TryComp<InputMoverComponent>(uid, ref mover))
			{
				return;
			}
			mover.TargetRelativeRotation += angle;
			base.Dirty(mover, null);
		}

		// Token: 0x06000830 RID: 2096 RVA: 0x0001BBD0 File Offset: 0x00019DD0
		public void ResetCamera(EntityUid uid)
		{
			InputMoverComponent mover;
			if (this.CameraRotationLocked || !base.TryComp<InputMoverComponent>(uid, ref mover) || mover.TargetRelativeRotation.Equals(Angle.Zero))
			{
				return;
			}
			mover.TargetRelativeRotation = Angle.Zero;
			base.Dirty(mover, null);
		}

		// Token: 0x06000831 RID: 2097 RVA: 0x0001BC18 File Offset: 0x00019E18
		public Angle GetParentGridAngle(InputMoverComponent mover, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<TransformComponent> xformQuery)
		{
			Angle rotation = mover.RelativeRotation;
			TransformComponent relativeXform;
			if (xformQuery.TryGetComponent(mover.RelativeEntity, ref relativeXform))
			{
				return this._transform.GetWorldRotation(relativeXform, xformQuery) + rotation;
			}
			return rotation;
		}

		// Token: 0x06000832 RID: 2098 RVA: 0x0001BC54 File Offset: 0x00019E54
		public Angle GetParentGridAngle(InputMoverComponent mover)
		{
			Angle rotation = mover.RelativeRotation;
			TransformComponent relativeXform;
			if (base.TryComp<TransformComponent>(mover.RelativeEntity, ref relativeXform))
			{
				return relativeXform.WorldRotation + rotation;
			}
			return rotation;
		}

		// Token: 0x06000833 RID: 2099 RVA: 0x0001BC88 File Offset: 0x00019E88
		private void OnInputParentChange(EntityUid uid, InputMoverComponent component, ref EntParentChangedMessage args)
		{
			EntityUid? relative = args.Transform.GridUid;
			EntityUid? entityUid = relative;
			if (entityUid == null)
			{
				relative = args.Transform.MapUid;
			}
			if (component.LifeStage < 6)
			{
				component.RelativeEntity = relative;
				base.Dirty(component, null);
				return;
			}
			MapId oldMapId = args.OldMapId;
			MapId mapId = args.Transform.MapID;
			if (oldMapId != mapId)
			{
				component.RelativeEntity = relative;
				component.TargetRelativeRotation = Angle.Zero;
				component.RelativeRotation = Angle.Zero;
				component.LerpAccumulator = 0f;
				base.Dirty(component, null);
				return;
			}
			if (relative == component.RelativeEntity)
			{
				if (component.LerpAccumulator != 0f)
				{
					component.LerpAccumulator = 0f;
					base.Dirty(component, null);
				}
				return;
			}
			component.LerpAccumulator = 1f;
			base.Dirty(component, null);
		}

		// Token: 0x06000834 RID: 2100 RVA: 0x0001BD90 File Offset: 0x00019F90
		private void HandleDirChange(EntityUid entity, Direction dir, ushort subTick, bool state)
		{
			RelayInputMoverComponent relayMover;
			if (base.TryComp<RelayInputMoverComponent>(entity, ref relayMover))
			{
				InputMoverComponent mover;
				if (base.TryComp<InputMoverComponent>(entity, ref mover))
				{
					this.SetMoveInput(mover, MoveButtons.None);
				}
				if (relayMover.RelayEntity != null && !this._mobState.IsIncapacitated(entity, null))
				{
					this.HandleDirChange(relayMover.RelayEntity.Value, dir, subTick, state);
				}
				return;
			}
			InputMoverComponent moverComp;
			if (!base.TryComp<InputMoverComponent>(entity, ref moverComp))
			{
				return;
			}
			EntityUid owner = moverComp.Owner;
			MoveInputEvent moveEvent = new MoveInputEvent(entity);
			base.RaiseLocalEvent<MoveInputEvent>(owner, ref moveEvent, false);
			TransformComponent xform;
			if (this._container.IsEntityInContainer(owner, null) && base.TryComp<TransformComponent>(owner, ref xform) && xform.ParentUid.IsValid() && this._mobState.IsAlive(owner, null))
			{
				ContainerRelayMovementEntityEvent relayMoveEvent = new ContainerRelayMovementEntityEvent(owner);
				base.RaiseLocalEvent<ContainerRelayMovementEntityEvent>(xform.ParentUid, ref relayMoveEvent, false);
			}
			this.SetVelocityDirection(moverComp, dir, subTick, state);
		}

		// Token: 0x06000835 RID: 2101 RVA: 0x0001BE74 File Offset: 0x0001A074
		private void OnInputInit(EntityUid uid, InputMoverComponent component, ComponentInit args)
		{
			TransformComponent xform = base.Transform(uid);
			if (!xform.ParentUid.IsValid())
			{
				return;
			}
			EntityUid? gridUid = xform.GridUid;
			component.RelativeEntity = ((gridUid != null) ? gridUid : xform.MapUid);
			component.TargetRelativeRotation = Angle.Zero;
		}

		// Token: 0x06000836 RID: 2102 RVA: 0x0001BEC4 File Offset: 0x0001A0C4
		private void HandleRunChange(EntityUid uid, ushort subTick, bool walking)
		{
			InputMoverComponent moverComp;
			base.TryComp<InputMoverComponent>(uid, ref moverComp);
			RelayInputMoverComponent relayMover;
			if (base.TryComp<RelayInputMoverComponent>(uid, ref relayMover))
			{
				if (moverComp != null)
				{
					this.SetMoveInput(moverComp, MoveButtons.None);
				}
				if (relayMover.RelayEntity == null)
				{
					return;
				}
				this.HandleRunChange(relayMover.RelayEntity.Value, subTick, walking);
				return;
			}
			else
			{
				if (moverComp == null)
				{
					return;
				}
				this.SetSprinting(moverComp, subTick, walking);
				return;
			}
		}

		// Token: 0x06000837 RID: 2103 RVA: 0x0001BF20 File Offset: 0x0001A120
		[NullableContext(0)]
		[return: TupleElementNames(new string[]
		{
			"Walking",
			"Sprinting"
		})]
		public ValueTuple<Vector2, Vector2> GetVelocityInput([Nullable(1)] InputMoverComponent mover)
		{
			if (this.Timing.InSimulation)
			{
				Vector2 walk;
				Vector2 sprint;
				float remainingFraction;
				if (this.Timing.CurTick > mover.LastInputTick)
				{
					walk = Vector2.Zero;
					sprint = Vector2.Zero;
					remainingFraction = 1f;
				}
				else
				{
					walk = mover.CurTickWalkMovement;
					sprint = mover.CurTickSprintMovement;
					remainingFraction = (float)(ushort.MaxValue - mover.LastInputSubTick) / 65535f;
				}
				Vector2 curDir = this.DirVecForButtons(mover.HeldMoveButtons) * remainingFraction;
				if (mover.Sprinting)
				{
					sprint += curDir;
				}
				else
				{
					walk += curDir;
				}
				return new ValueTuple<Vector2, Vector2>(walk, sprint);
			}
			Vector2 immediateDir = this.DirVecForButtons(mover.HeldMoveButtons);
			if (!mover.Sprinting)
			{
				return new ValueTuple<Vector2, Vector2>(immediateDir, Vector2.Zero);
			}
			return new ValueTuple<Vector2, Vector2>(Vector2.Zero, immediateDir);
		}

		// Token: 0x06000838 RID: 2104 RVA: 0x0001BFEC File Offset: 0x0001A1EC
		public void SetVelocityDirection(InputMoverComponent component, Direction direction, ushort subTick, bool enabled)
		{
			MoveButtons moveButtons;
			switch (direction)
			{
			case 0:
				moveButtons = MoveButtons.Down;
				goto IL_3F;
			case 2:
				moveButtons = MoveButtons.Right;
				goto IL_3F;
			case 4:
				moveButtons = MoveButtons.Up;
				goto IL_3F;
			case 6:
				moveButtons = MoveButtons.Left;
				goto IL_3F;
			}
			throw new ArgumentException("direction");
			IL_3F:
			MoveButtons bit = moveButtons;
			this.SetMoveInput(component, subTick, enabled, bit);
		}

		// Token: 0x06000839 RID: 2105 RVA: 0x0001C048 File Offset: 0x0001A248
		private unsafe void SetMoveInput(InputMoverComponent component, ushort subTick, bool enabled, MoveButtons bit)
		{
			this.ResetSubtick(component);
			if (subTick >= component.LastInputSubTick)
			{
				float fraction = (float)(subTick - component.LastInputSubTick) / 65535f;
				*(ref component.Sprinting ? ref component.CurTickSprintMovement : ref component.CurTickWalkMovement) += this.DirVecForButtons(component.HeldMoveButtons) * fraction;
				component.LastInputSubTick = subTick;
			}
			MoveButtons buttons = component.HeldMoveButtons;
			if (enabled)
			{
				buttons |= bit;
			}
			else
			{
				buttons &= ~bit;
			}
			this.SetMoveInput(component, buttons);
		}

		// Token: 0x0600083A RID: 2106 RVA: 0x0001C0D4 File Offset: 0x0001A2D4
		private void ResetSubtick(InputMoverComponent component)
		{
			if (this.Timing.CurTick <= component.LastInputTick)
			{
				return;
			}
			component.CurTickWalkMovement = Vector2.Zero;
			component.CurTickSprintMovement = Vector2.Zero;
			component.LastInputTick = this.Timing.CurTick;
			component.LastInputSubTick = 0;
		}

		// Token: 0x0600083B RID: 2107 RVA: 0x0001C128 File Offset: 0x0001A328
		public void SetSprinting(InputMoverComponent component, ushort subTick, bool walking)
		{
			this.SetMoveInput(component, subTick, walking, MoveButtons.Walk);
		}

		// Token: 0x0600083C RID: 2108 RVA: 0x0001C138 File Offset: 0x0001A338
		private Vector2 DirVecForButtons(MoveButtons buttons)
		{
			int x = 0;
			x -= (SharedMoverController.HasFlag(buttons, MoveButtons.Left) ? 1 : 0);
			x += (SharedMoverController.HasFlag(buttons, MoveButtons.Right) ? 1 : 0);
			int y = 0;
			if (this.DiagonalMovementEnabled || x == 0)
			{
				y -= (SharedMoverController.HasFlag(buttons, MoveButtons.Down) ? 1 : 0);
				y += (SharedMoverController.HasFlag(buttons, MoveButtons.Up) ? 1 : 0);
			}
			Vector2 vec;
			vec..ctor((float)x, (float)y);
			if ((double)vec.LengthSquared > 1E-06)
			{
				vec = vec.Normalized;
			}
			return vec;
		}

		// Token: 0x0600083D RID: 2109 RVA: 0x0001C1BB File Offset: 0x0001A3BB
		private static bool HasFlag(MoveButtons buttons, MoveButtons flag)
		{
			return (buttons & flag) == flag;
		}

		// Token: 0x0600083E RID: 2110 RVA: 0x0001C1C3 File Offset: 0x0001A3C3
		private void InitializeMob()
		{
			base.SubscribeLocalEvent<MobMoverComponent, ComponentGetState>(new ComponentEventRefHandler<MobMoverComponent, ComponentGetState>(this.OnMobGetState), null, null);
			base.SubscribeLocalEvent<MobMoverComponent, ComponentHandleState>(new ComponentEventRefHandler<MobMoverComponent, ComponentHandleState>(this.OnMobHandleState), null, null);
		}

		// Token: 0x0600083F RID: 2111 RVA: 0x0001C1F0 File Offset: 0x0001A3F0
		private void OnMobHandleState(EntityUid uid, MobMoverComponent component, ref ComponentHandleState args)
		{
			SharedMoverController.MobMoverComponentState state = args.Current as SharedMoverController.MobMoverComponentState;
			if (state == null)
			{
				return;
			}
			component.GrabRangeVV = state.GrabRange;
			component.PushStrengthVV = state.PushStrength;
		}

		// Token: 0x06000840 RID: 2112 RVA: 0x0001C225 File Offset: 0x0001A425
		private void OnMobGetState(EntityUid uid, MobMoverComponent component, ref ComponentGetState args)
		{
			args.State = new SharedMoverController.MobMoverComponentState(component.GrabRange, component.PushStrength);
		}

		// Token: 0x06000841 RID: 2113 RVA: 0x0001C240 File Offset: 0x0001A440
		private void InitializeRelay()
		{
			base.SubscribeLocalEvent<RelayInputMoverComponent, ComponentGetState>(new ComponentEventRefHandler<RelayInputMoverComponent, ComponentGetState>(this.OnRelayGetState), null, null);
			base.SubscribeLocalEvent<RelayInputMoverComponent, ComponentHandleState>(new ComponentEventRefHandler<RelayInputMoverComponent, ComponentHandleState>(this.OnRelayHandleState), null, null);
			base.SubscribeLocalEvent<RelayInputMoverComponent, ComponentShutdown>(new ComponentEventHandler<RelayInputMoverComponent, ComponentShutdown>(this.OnRelayShutdown), null, null);
			base.SubscribeLocalEvent<MovementRelayTargetComponent, ComponentGetState>(new ComponentEventRefHandler<MovementRelayTargetComponent, ComponentGetState>(this.OnTargetRelayGetState), null, null);
			base.SubscribeLocalEvent<MovementRelayTargetComponent, ComponentHandleState>(new ComponentEventRefHandler<MovementRelayTargetComponent, ComponentHandleState>(this.OnTargetRelayHandleState), null, null);
			base.SubscribeLocalEvent<MovementRelayTargetComponent, ComponentShutdown>(new ComponentEventHandler<MovementRelayTargetComponent, ComponentShutdown>(this.OnTargetRelayShutdown), null, null);
		}

		// Token: 0x06000842 RID: 2114 RVA: 0x0001C2C8 File Offset: 0x0001A4C8
		[NullableContext(2)]
		public void SetRelay(EntityUid uid, EntityUid relayEntity, RelayInputMoverComponent component = null)
		{
			if (base.Resolve<RelayInputMoverComponent>(uid, ref component, true))
			{
				EntityUid? relayEntity2 = component.RelayEntity;
				if (relayEntity2 == null || (relayEntity2 != null && !(relayEntity2.GetValueOrDefault() == relayEntity)))
				{
					if (uid == relayEntity)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(56, 1);
						defaultInterpolatedStringHandler.AppendLiteral("An entity attempted to relay movement to itself. Entity:");
						defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid));
						Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
						return;
					}
					MovementRelayTargetComponent targetComp;
					if (base.TryComp<MovementRelayTargetComponent>(relayEntity, ref targetComp))
					{
						targetComp.Entities.Remove(uid);
						if (targetComp.Entities.Count == 0)
						{
							base.RemComp<MovementRelayTargetComponent>(relayEntity);
						}
					}
					component.RelayEntity = new EntityUid?(relayEntity);
					targetComp = base.EnsureComp<MovementRelayTargetComponent>(relayEntity);
					targetComp.Entities.Add(uid);
					base.Dirty(component, null);
					base.Dirty(targetComp, null);
					return;
				}
			}
		}

		// Token: 0x06000843 RID: 2115 RVA: 0x0001C3AC File Offset: 0x0001A5AC
		private void OnRelayShutdown(EntityUid uid, RelayInputMoverComponent component, ComponentShutdown args)
		{
			InputMoverComponent inputMover;
			if (!base.TryComp<InputMoverComponent>(component.RelayEntity, ref inputMover))
			{
				return;
			}
			MovementRelayTargetComponent targetComp;
			if (base.TryComp<MovementRelayTargetComponent>(component.RelayEntity, ref targetComp) && targetComp.LifeStage < 7)
			{
				targetComp.Entities.Remove(uid);
				if (targetComp.Entities.Count == 0)
				{
					base.RemCompDeferred<MovementRelayTargetComponent>(component.RelayEntity.Value);
				}
				else
				{
					base.Dirty(targetComp, null);
				}
			}
			this.SetMoveInput(inputMover, MoveButtons.None);
		}

		// Token: 0x06000844 RID: 2116 RVA: 0x0001C424 File Offset: 0x0001A624
		private void OnRelayHandleState(EntityUid uid, RelayInputMoverComponent component, ref ComponentHandleState args)
		{
			SharedMoverController.RelayInputMoverComponentState state = args.Current as SharedMoverController.RelayInputMoverComponentState;
			if (state == null)
			{
				return;
			}
			component.RelayEntity = state.Entity;
		}

		// Token: 0x06000845 RID: 2117 RVA: 0x0001C44D File Offset: 0x0001A64D
		private void OnRelayGetState(EntityUid uid, RelayInputMoverComponent component, ref ComponentGetState args)
		{
			args.State = new SharedMoverController.RelayInputMoverComponentState
			{
				Entity = component.RelayEntity
			};
		}

		// Token: 0x06000846 RID: 2118 RVA: 0x0001C468 File Offset: 0x0001A668
		private void OnTargetRelayShutdown(EntityUid uid, MovementRelayTargetComponent component, ComponentShutdown args)
		{
			if (component.Entities.Count == 0)
			{
				return;
			}
			EntityQuery<RelayInputMoverComponent> relayQuery = base.GetEntityQuery<RelayInputMoverComponent>();
			foreach (EntityUid ent in component.Entities)
			{
				RelayInputMoverComponent relay;
				if (relayQuery.TryGetComponent(ent, ref relay))
				{
					EntityUid? relayEntity = relay.RelayEntity;
					if (relayEntity != null && (relayEntity == null || !(relayEntity.GetValueOrDefault() != uid)))
					{
						base.RemCompDeferred<RelayInputMoverComponent>(ent);
					}
				}
			}
		}

		// Token: 0x06000847 RID: 2119 RVA: 0x0001C510 File Offset: 0x0001A710
		private void OnTargetRelayHandleState(EntityUid uid, MovementRelayTargetComponent component, ref ComponentHandleState args)
		{
			SharedMoverController.MovementRelayTargetComponentState state = args.Current as SharedMoverController.MovementRelayTargetComponentState;
			if (state == null)
			{
				return;
			}
			component.Entities.Clear();
			component.Entities.AddRange(state.Entities);
		}

		// Token: 0x06000848 RID: 2120 RVA: 0x0001C549 File Offset: 0x0001A749
		private void OnTargetRelayGetState(EntityUid uid, MovementRelayTargetComponent component, ref ComponentGetState args)
		{
			args.State = new SharedMoverController.MovementRelayTargetComponentState(component.Entities);
		}

		// Token: 0x04000839 RID: 2105
		[Dependency]
		private readonly IConfigurationManager _configManager;

		// Token: 0x0400083A RID: 2106
		[Dependency]
		protected readonly IGameTiming Timing;

		// Token: 0x0400083B RID: 2107
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x0400083C RID: 2108
		[Dependency]
		private readonly ITileDefinitionManager _tileDefinitionManager;

		// Token: 0x0400083D RID: 2109
		[Dependency]
		private readonly SharedBiomeSystem _biome;

		// Token: 0x0400083E RID: 2110
		[Dependency]
		private readonly InventorySystem _inventory;

		// Token: 0x0400083F RID: 2111
		[Dependency]
		private readonly SharedContainerSystem _container;

		// Token: 0x04000840 RID: 2112
		[Dependency]
		private readonly EntityLookupSystem _lookup;

		// Token: 0x04000841 RID: 2113
		[Dependency]
		private readonly SharedGravitySystem _gravity;

		// Token: 0x04000842 RID: 2114
		[Dependency]
		private readonly MobStateSystem _mobState;

		// Token: 0x04000843 RID: 2115
		[Dependency]
		private readonly SharedAudioSystem _audio;

		// Token: 0x04000844 RID: 2116
		[Dependency]
		private readonly SharedTransformSystem _transform;

		// Token: 0x04000845 RID: 2117
		[Dependency]
		private readonly TagSystem _tags;

		// Token: 0x04000846 RID: 2118
		private const float StepSoundMoveDistanceRunning = 2f;

		// Token: 0x04000847 RID: 2119
		private const float StepSoundMoveDistanceWalking = 1.5f;

		// Token: 0x04000848 RID: 2120
		private const float FootstepVariation = 0f;

		// Token: 0x04000849 RID: 2121
		private const float FootstepVolume = 3f;

		// Token: 0x0400084A RID: 2122
		private const float FootstepWalkingAddedVolumeMultiplier = 0f;

		// Token: 0x0400084B RID: 2123
		protected ISawmill Sawmill;

		// Token: 0x0400084C RID: 2124
		private float _stopSpeed;

		// Token: 0x0400084D RID: 2125
		private bool _relativeMovement;

		// Token: 0x0400084E RID: 2126
		public Dictionary<EntityUid, bool> UsedMobMovement = new Dictionary<EntityUid, bool>();

		// Token: 0x020007CC RID: 1996
		[NullableContext(0)]
		[NetSerializable]
		[Serializable]
		private sealed class FootstepModifierComponentState : ComponentState
		{
			// Token: 0x04001818 RID: 6168
			[Nullable(1)]
			public SoundSpecifier Sound;
		}

		// Token: 0x020007CD RID: 1997
		[Nullable(0)]
		private sealed class CameraRotateInputCmdHandler : InputCmdHandler
		{
			// Token: 0x0600182E RID: 6190 RVA: 0x0004D976 File Offset: 0x0004BB76
			public CameraRotateInputCmdHandler(SharedMoverController controller, Direction direction)
			{
				this._controller = controller;
				this._angle = DirectionExtensions.ToAngle(direction);
			}

			// Token: 0x0600182F RID: 6191 RVA: 0x0004D994 File Offset: 0x0004BB94
			public override bool HandleCmdMessage([Nullable(2)] ICommonSession session, InputCmdMessage message)
			{
				FullInputCmdMessage full = message as FullInputCmdMessage;
				if (full == null || (session == null || session.AttachedEntity == null))
				{
					return false;
				}
				if (full.State != null)
				{
					return false;
				}
				this._controller.RotateCamera(session.AttachedEntity.Value, this._angle);
				return false;
			}

			// Token: 0x04001819 RID: 6169
			private readonly SharedMoverController _controller;

			// Token: 0x0400181A RID: 6170
			private readonly Angle _angle;
		}

		// Token: 0x020007CE RID: 1998
		[Nullable(0)]
		private sealed class CameraResetInputCmdHandler : InputCmdHandler
		{
			// Token: 0x06001830 RID: 6192 RVA: 0x0004D9F0 File Offset: 0x0004BBF0
			public CameraResetInputCmdHandler(SharedMoverController controller)
			{
				this._controller = controller;
			}

			// Token: 0x06001831 RID: 6193 RVA: 0x0004DA00 File Offset: 0x0004BC00
			public override bool HandleCmdMessage([Nullable(2)] ICommonSession session, InputCmdMessage message)
			{
				FullInputCmdMessage full = message as FullInputCmdMessage;
				if (full == null || (session == null || session.AttachedEntity == null))
				{
					return false;
				}
				if (full.State != null)
				{
					return false;
				}
				this._controller.ResetCamera(session.AttachedEntity.Value);
				return false;
			}

			// Token: 0x0400181B RID: 6171
			private readonly SharedMoverController _controller;
		}

		// Token: 0x020007CF RID: 1999
		[Nullable(0)]
		private sealed class MoverDirInputCmdHandler : InputCmdHandler
		{
			// Token: 0x06001832 RID: 6194 RVA: 0x0004DA56 File Offset: 0x0004BC56
			public MoverDirInputCmdHandler(SharedMoverController controller, Direction dir)
			{
				this._controller = controller;
				this._dir = dir;
			}

			// Token: 0x06001833 RID: 6195 RVA: 0x0004DA6C File Offset: 0x0004BC6C
			public override bool HandleCmdMessage([Nullable(2)] ICommonSession session, InputCmdMessage message)
			{
				FullInputCmdMessage full = message as FullInputCmdMessage;
				if (full == null || (session == null || session.AttachedEntity == null))
				{
					return false;
				}
				this._controller.HandleDirChange(session.AttachedEntity.Value, this._dir, message.SubTick, full.State == 1);
				return false;
			}

			// Token: 0x0400181C RID: 6172
			private readonly SharedMoverController _controller;

			// Token: 0x0400181D RID: 6173
			private readonly Direction _dir;
		}

		// Token: 0x020007D0 RID: 2000
		[Nullable(0)]
		private sealed class WalkInputCmdHandler : InputCmdHandler
		{
			// Token: 0x06001834 RID: 6196 RVA: 0x0004DACD File Offset: 0x0004BCCD
			public WalkInputCmdHandler(SharedMoverController controller)
			{
				this._controller = controller;
			}

			// Token: 0x06001835 RID: 6197 RVA: 0x0004DADC File Offset: 0x0004BCDC
			public override bool HandleCmdMessage([Nullable(2)] ICommonSession session, InputCmdMessage message)
			{
				FullInputCmdMessage full = message as FullInputCmdMessage;
				if (full == null || (session == null || session.AttachedEntity == null))
				{
					return false;
				}
				this._controller.HandleRunChange(session.AttachedEntity.Value, full.SubTick, full.State == 1);
				return false;
			}

			// Token: 0x0400181E RID: 6174
			private SharedMoverController _controller;
		}

		// Token: 0x020007D1 RID: 2001
		[NullableContext(0)]
		[NetSerializable]
		[Serializable]
		private sealed class InputMoverComponentState : ComponentState
		{
			// Token: 0x170004FB RID: 1275
			// (get) Token: 0x06001836 RID: 6198 RVA: 0x0004DB37 File Offset: 0x0004BD37
			public MoveButtons Buttons { get; }

			// Token: 0x06001837 RID: 6199 RVA: 0x0004DB3F File Offset: 0x0004BD3F
			public InputMoverComponentState(MoveButtons buttons, bool canMove, Angle relativeRotation, Angle targetRelativeRotation, EntityUid? relativeEntity, float lerpAccumulator)
			{
				this.Buttons = buttons;
				this.CanMove = canMove;
				this.RelativeRotation = relativeRotation;
				this.TargetRelativeRotation = targetRelativeRotation;
				this.RelativeEntity = relativeEntity;
				this.LerpAccumulator = lerpAccumulator;
			}

			// Token: 0x04001820 RID: 6176
			public readonly bool CanMove;

			// Token: 0x04001821 RID: 6177
			public Angle RelativeRotation;

			// Token: 0x04001822 RID: 6178
			public Angle TargetRelativeRotation;

			// Token: 0x04001823 RID: 6179
			public EntityUid? RelativeEntity;

			// Token: 0x04001824 RID: 6180
			public float LerpAccumulator;
		}

		// Token: 0x020007D2 RID: 2002
		[Nullable(0)]
		private sealed class ShuttleInputCmdHandler : InputCmdHandler
		{
			// Token: 0x06001838 RID: 6200 RVA: 0x0004DB74 File Offset: 0x0004BD74
			public ShuttleInputCmdHandler(SharedMoverController controller, ShuttleButtons button)
			{
				this._controller = controller;
				this._button = button;
			}

			// Token: 0x06001839 RID: 6201 RVA: 0x0004DB8C File Offset: 0x0004BD8C
			public override bool HandleCmdMessage([Nullable(2)] ICommonSession session, InputCmdMessage message)
			{
				FullInputCmdMessage full = message as FullInputCmdMessage;
				if (full == null || (session == null || session.AttachedEntity == null))
				{
					return false;
				}
				this._controller.HandleShuttleInput(session.AttachedEntity.Value, this._button, full.SubTick, full.State == 1);
				return false;
			}

			// Token: 0x04001825 RID: 6181
			private readonly SharedMoverController _controller;

			// Token: 0x04001826 RID: 6182
			private readonly ShuttleButtons _button;
		}

		// Token: 0x020007D3 RID: 2003
		[NullableContext(0)]
		[NetSerializable]
		[Serializable]
		private sealed class MobMoverComponentState : ComponentState
		{
			// Token: 0x0600183A RID: 6202 RVA: 0x0004DBED File Offset: 0x0004BDED
			public MobMoverComponentState(float grabRange, float pushStrength)
			{
				this.GrabRange = grabRange;
				this.PushStrength = pushStrength;
			}

			// Token: 0x04001827 RID: 6183
			public float GrabRange;

			// Token: 0x04001828 RID: 6184
			public float PushStrength;
		}

		// Token: 0x020007D4 RID: 2004
		[NullableContext(0)]
		[NetSerializable]
		[Serializable]
		private sealed class RelayInputMoverComponentState : ComponentState
		{
			// Token: 0x04001829 RID: 6185
			public EntityUid? Entity;
		}

		// Token: 0x020007D5 RID: 2005
		[Nullable(0)]
		[NetSerializable]
		[Serializable]
		private sealed class MovementRelayTargetComponentState : ComponentState
		{
			// Token: 0x0600183C RID: 6204 RVA: 0x0004DC0B File Offset: 0x0004BE0B
			public MovementRelayTargetComponentState(List<EntityUid> entities)
			{
				this.Entities = entities;
			}

			// Token: 0x0400182A RID: 6186
			public List<EntityUid> Entities;
		}
	}
}
