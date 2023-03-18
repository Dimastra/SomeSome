using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Cargo.Components;
using Content.Server.Shuttles.Components;
using Content.Server.Shuttles.Systems;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.Shuttles.Components;
using Content.Shared.Shuttles.Systems;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;

namespace Content.Server.Physics.Controllers
{
	// Token: 0x020002DA RID: 730
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MoverController : SharedMoverController
	{
		// Token: 0x06000EDB RID: 3803 RVA: 0x0004B450 File Offset: 0x00049650
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<RelayInputMoverComponent, PlayerAttachedEvent>(new ComponentEventHandler<RelayInputMoverComponent, PlayerAttachedEvent>(this.OnRelayPlayerAttached), null, null);
			base.SubscribeLocalEvent<RelayInputMoverComponent, PlayerDetachedEvent>(new ComponentEventHandler<RelayInputMoverComponent, PlayerDetachedEvent>(this.OnRelayPlayerDetached), null, null);
			base.SubscribeLocalEvent<InputMoverComponent, PlayerAttachedEvent>(new ComponentEventHandler<InputMoverComponent, PlayerAttachedEvent>(this.OnPlayerAttached), null, null);
			base.SubscribeLocalEvent<InputMoverComponent, PlayerDetachedEvent>(new ComponentEventHandler<InputMoverComponent, PlayerDetachedEvent>(this.OnPlayerDetached), null, null);
		}

		// Token: 0x06000EDC RID: 3804 RVA: 0x0004B4B4 File Offset: 0x000496B4
		private void OnRelayPlayerAttached(EntityUid uid, RelayInputMoverComponent component, PlayerAttachedEvent args)
		{
			InputMoverComponent inputMover;
			if (base.TryComp<InputMoverComponent>(component.RelayEntity, ref inputMover))
			{
				base.SetMoveInput(inputMover, MoveButtons.None);
			}
		}

		// Token: 0x06000EDD RID: 3805 RVA: 0x0004B4DC File Offset: 0x000496DC
		private void OnRelayPlayerDetached(EntityUid uid, RelayInputMoverComponent component, PlayerDetachedEvent args)
		{
			InputMoverComponent inputMover;
			if (base.TryComp<InputMoverComponent>(component.RelayEntity, ref inputMover))
			{
				base.SetMoveInput(inputMover, MoveButtons.None);
			}
		}

		// Token: 0x06000EDE RID: 3806 RVA: 0x0004B501 File Offset: 0x00049701
		private void OnPlayerAttached(EntityUid uid, InputMoverComponent component, PlayerAttachedEvent args)
		{
			base.SetMoveInput(component, MoveButtons.None);
		}

		// Token: 0x06000EDF RID: 3807 RVA: 0x0004B50B File Offset: 0x0004970B
		private void OnPlayerDetached(EntityUid uid, InputMoverComponent component, PlayerDetachedEvent args)
		{
			base.SetMoveInput(component, MoveButtons.None);
		}

		// Token: 0x06000EE0 RID: 3808 RVA: 0x0004B515 File Offset: 0x00049715
		protected override bool CanSound()
		{
			return true;
		}

		// Token: 0x06000EE1 RID: 3809 RVA: 0x0004B518 File Offset: 0x00049718
		public override void UpdateBeforeSolve(bool prediction, float frameTime)
		{
			base.UpdateBeforeSolve(prediction, frameTime);
			EntityQuery<PhysicsComponent> bodyQuery = base.GetEntityQuery<PhysicsComponent>();
			EntityQuery<RelayInputMoverComponent> relayQuery = base.GetEntityQuery<RelayInputMoverComponent>();
			EntityQuery<MovementRelayTargetComponent> relayTargetQuery = base.GetEntityQuery<MovementRelayTargetComponent>();
			EntityQuery<TransformComponent> xformQuery = base.GetEntityQuery<TransformComponent>();
			EntityQuery<InputMoverComponent> moverQuery = base.GetEntityQuery<InputMoverComponent>();
			foreach (InputMoverComponent mover in base.EntityQuery<InputMoverComponent>(true))
			{
				EntityUid uid = mover.Owner;
				TransformComponent xform;
				if (!relayQuery.HasComponent(uid) && xformQuery.TryGetComponent(uid, ref xform))
				{
					TransformComponent xformMover = xform;
					PhysicsComponent body;
					if (mover.ToParent && relayQuery.HasComponent(xform.ParentUid))
					{
						if (!bodyQuery.TryGetComponent(xform.ParentUid, ref body))
						{
							continue;
						}
						if (!base.TryComp<TransformComponent>(xform.ParentUid, ref xformMover))
						{
							continue;
						}
					}
					else if (!bodyQuery.TryGetComponent(uid, ref body))
					{
						continue;
					}
					base.HandleMobMovement(uid, mover, body, xformMover, frameTime, xformQuery, moverQuery, relayTargetQuery);
				}
			}
			this.HandleShuttleMovement(frameTime);
		}

		// Token: 0x06000EE2 RID: 3810 RVA: 0x0004B624 File Offset: 0x00049824
		[NullableContext(0)]
		[return: TupleElementNames(new string[]
		{
			"Strafe",
			"Rotation",
			"Brakes"
		})]
		public ValueTuple<Vector2, float, float> GetPilotVelocityInput([Nullable(1)] PilotComponent component)
		{
			if (!this.Timing.InSimulation)
			{
				this.ResetSubtick(component);
				this.ApplyTick(component, 1f);
				return new ValueTuple<Vector2, float, float>(component.CurTickStrafeMovement, component.CurTickRotationMovement, component.CurTickBraking);
			}
			float remainingFraction;
			if (this.Timing.CurTick > component.LastInputTick)
			{
				component.CurTickStrafeMovement = Vector2.Zero;
				component.CurTickRotationMovement = 0f;
				component.CurTickBraking = 0f;
				remainingFraction = 1f;
			}
			else
			{
				remainingFraction = (float)(ushort.MaxValue - component.LastInputSubTick) / 65535f;
			}
			this.ApplyTick(component, remainingFraction);
			return new ValueTuple<Vector2, float, float>(component.CurTickStrafeMovement, component.CurTickRotationMovement, component.CurTickBraking);
		}

		// Token: 0x06000EE3 RID: 3811 RVA: 0x0004B6E0 File Offset: 0x000498E0
		private void ResetSubtick(PilotComponent component)
		{
			if (this.Timing.CurTick <= component.LastInputTick)
			{
				return;
			}
			component.CurTickStrafeMovement = Vector2.Zero;
			component.CurTickRotationMovement = 0f;
			component.CurTickBraking = 0f;
			component.LastInputTick = this.Timing.CurTick;
			component.LastInputSubTick = 0;
		}

		// Token: 0x06000EE4 RID: 3812 RVA: 0x0004B740 File Offset: 0x00049940
		protected override void HandleShuttleInput(EntityUid uid, ShuttleButtons button, ushort subTick, bool state)
		{
			PilotComponent pilot;
			if (!base.TryComp<PilotComponent>(uid, ref pilot) || pilot.Console == null)
			{
				return;
			}
			this.ResetSubtick(pilot);
			if (subTick >= pilot.LastInputSubTick)
			{
				float fraction = (float)(subTick - pilot.LastInputSubTick) / 65535f;
				this.ApplyTick(pilot, fraction);
				pilot.LastInputSubTick = subTick;
			}
			ShuttleButtons buttons = pilot.HeldButtons;
			if (state)
			{
				buttons |= button;
			}
			else
			{
				buttons &= ~button;
			}
			pilot.HeldButtons = buttons;
		}

		// Token: 0x06000EE5 RID: 3813 RVA: 0x0004B7B0 File Offset: 0x000499B0
		private void ApplyTick(PilotComponent component, float fraction)
		{
			int x = 0;
			int y = 0;
			int rot = 0;
			if ((component.HeldButtons & ShuttleButtons.StrafeLeft) != ShuttleButtons.None)
			{
				x--;
			}
			if ((component.HeldButtons & ShuttleButtons.StrafeRight) != ShuttleButtons.None)
			{
				x++;
			}
			component.CurTickStrafeMovement.X = component.CurTickStrafeMovement.X + (float)x * fraction;
			if ((component.HeldButtons & ShuttleButtons.StrafeUp) != ShuttleButtons.None)
			{
				y++;
			}
			if ((component.HeldButtons & ShuttleButtons.StrafeDown) != ShuttleButtons.None)
			{
				y--;
			}
			component.CurTickStrafeMovement.Y = component.CurTickStrafeMovement.Y + (float)y * fraction;
			if ((component.HeldButtons & ShuttleButtons.RotateLeft) != ShuttleButtons.None)
			{
				rot--;
			}
			if ((component.HeldButtons & ShuttleButtons.RotateRight) != ShuttleButtons.None)
			{
				rot++;
			}
			component.CurTickRotationMovement += (float)rot * fraction;
			int brake;
			if ((component.HeldButtons & ShuttleButtons.Brake) != ShuttleButtons.None)
			{
				brake = 1;
			}
			else
			{
				brake = 0;
			}
			component.CurTickBraking += (float)brake * fraction;
		}

		// Token: 0x06000EE6 RID: 3814 RVA: 0x0004B874 File Offset: 0x00049A74
		private void HandleShuttleMovement(float frameTime)
		{
			Dictionary<ShuttleComponent, List<ValueTuple<PilotComponent, InputMoverComponent, TransformComponent>>> newPilots = new Dictionary<ShuttleComponent, List<ValueTuple<PilotComponent, InputMoverComponent, TransformComponent>>>();
			foreach (ValueTuple<PilotComponent, InputMoverComponent> valueTuple in this.EntityManager.EntityQuery<PilotComponent, InputMoverComponent>(false))
			{
				PilotComponent pilot = valueTuple.Item1;
				InputMoverComponent mover = valueTuple.Item2;
				SharedShuttleConsoleComponent console = pilot.Console;
				EntityUid? consoleEnt = (console != null) ? new EntityUid?(console.Owner) : null;
				CargoPilotConsoleComponent cargoConsole;
				if (base.TryComp<CargoPilotConsoleComponent>(consoleEnt, ref cargoConsole))
				{
					consoleEnt = cargoConsole.Entity;
				}
				TransformComponent xform;
				if (base.TryComp<TransformComponent>(consoleEnt, ref xform))
				{
					EntityUid? gridId = xform.GridUid;
					MapGridComponent grid;
					ShuttleComponent shuttleComponent;
					if (this._mapManager.TryGetGrid(gridId, ref grid) && this.EntityManager.TryGetComponent<ShuttleComponent>(grid.Owner, ref shuttleComponent) && shuttleComponent.Enabled)
					{
						List<ValueTuple<PilotComponent, InputMoverComponent, TransformComponent>> pilots;
						if (!newPilots.TryGetValue(shuttleComponent, out pilots))
						{
							pilots = new List<ValueTuple<PilotComponent, InputMoverComponent, TransformComponent>>();
							newPilots[shuttleComponent] = pilots;
						}
						pilots.Add(new ValueTuple<PilotComponent, InputMoverComponent, TransformComponent>(pilot, mover, xform));
					}
				}
			}
			foreach (KeyValuePair<ShuttleComponent, List<ValueTuple<PilotComponent, InputMoverComponent, TransformComponent>>> keyValuePair in this._shuttlePilots)
			{
				ShuttleComponent shuttleComponent2;
				List<ValueTuple<PilotComponent, InputMoverComponent, TransformComponent>> list;
				keyValuePair.Deconstruct(out shuttleComponent2, out list);
				ShuttleComponent shuttle = shuttleComponent2;
				if (!newPilots.ContainsKey(shuttle) && !this.FTLLocked(shuttle))
				{
					this._thruster.DisableLinearThrusters(shuttle);
				}
			}
			this._shuttlePilots = newPilots;
			foreach (KeyValuePair<ShuttleComponent, List<ValueTuple<PilotComponent, InputMoverComponent, TransformComponent>>> keyValuePair in this._shuttlePilots)
			{
				ShuttleComponent shuttleComponent2;
				List<ValueTuple<PilotComponent, InputMoverComponent, TransformComponent>> list;
				keyValuePair.Deconstruct(out shuttleComponent2, out list);
				ShuttleComponent shuttle2 = shuttleComponent2;
				List<ValueTuple<PilotComponent, InputMoverComponent, TransformComponent>> pilots2 = list;
				PhysicsComponent body;
				if (!base.Paused(shuttle2.Owner, null) && !this.FTLLocked(shuttle2) && base.TryComp<PhysicsComponent>(shuttle2.Owner, ref body))
				{
					Angle shuttleNorthAngle = base.Transform(body.Owner).WorldRotation;
					Vector2 linearInput = Vector2.Zero;
					float brakeInput = 0f;
					float angularInput = 0f;
					foreach (ValueTuple<PilotComponent, InputMoverComponent, TransformComponent> valueTuple2 in pilots2)
					{
						PilotComponent pilot2 = valueTuple2.Item1;
						TransformComponent consoleXform = valueTuple2.Item3;
						ValueTuple<Vector2, float, float> pilotInput = this.GetPilotVelocityInput(pilot2);
						if (pilotInput.Item3 > 0f)
						{
							brakeInput += pilotInput.Item3;
						}
						if (pilotInput.Item1.Length > 0f)
						{
							Angle offsetRotation = consoleXform.LocalRotation;
							linearInput += offsetRotation.RotateVec(ref pilotInput.Item1);
						}
						if (pilotInput.Item2 != 0f)
						{
							angularInput += pilotInput.Item2;
						}
					}
					int count = pilots2.Count;
					linearInput /= (float)count;
					angularInput /= (float)count;
					brakeInput /= (float)count;
					if (brakeInput > 0f)
					{
						if (body.LinearVelocity.Length > 0f)
						{
							Vector2 shuttleVelocity = (-shuttleNorthAngle).RotateVec(ref body.LinearVelocity);
							Vector2 force = Vector2.Zero;
							if (shuttleVelocity.X < 0f)
							{
								this._thruster.DisableLinearThrustDirection(shuttle2, 8);
								this._thruster.EnableLinearThrustDirection(shuttle2, 2);
								int index = (int)Math.Log2(2.0);
								force.X += shuttle2.LinearThrust[index];
							}
							else if (shuttleVelocity.X > 0f)
							{
								this._thruster.DisableLinearThrustDirection(shuttle2, 2);
								this._thruster.EnableLinearThrustDirection(shuttle2, 8);
								int index2 = (int)Math.Log2(8.0);
								force.X -= shuttle2.LinearThrust[index2];
							}
							if (shuttleVelocity.Y < 0f)
							{
								this._thruster.DisableLinearThrustDirection(shuttle2, 1);
								this._thruster.EnableLinearThrustDirection(shuttle2, 4);
								int index3 = (int)Math.Log2(4.0);
								force.Y += shuttle2.LinearThrust[index3];
							}
							else if (shuttleVelocity.Y > 0f)
							{
								this._thruster.DisableLinearThrustDirection(shuttle2, 4);
								this._thruster.EnableLinearThrustDirection(shuttle2, 1);
								int index4 = (int)Math.Log2(1.0);
								force.Y -= shuttle2.LinearThrust[index4];
							}
							Vector2 impulse = force * brakeInput;
							Vector2 wishDir = impulse.Normalized;
							float num = 20f;
							float currentSpeed = Vector2.Dot(shuttleVelocity, wishDir);
							float addSpeed = num - currentSpeed;
							if (addSpeed > 0f)
							{
								float accelSpeed = impulse.Length * frameTime;
								accelSpeed = MathF.Min(accelSpeed, addSpeed);
								impulse = impulse.Normalized * accelSpeed * body.InvMass;
								if (shuttleVelocity.X < 0f)
								{
									impulse.X = MathF.Min(impulse.X, -shuttleVelocity.X);
								}
								else if (shuttleVelocity.X > 0f)
								{
									impulse.X = MathF.Max(impulse.X, -shuttleVelocity.X);
								}
								if (shuttleVelocity.Y < 0f)
								{
									impulse.Y = MathF.Min(impulse.Y, -shuttleVelocity.Y);
								}
								else if (shuttleVelocity.Y > 0f)
								{
									impulse.Y = MathF.Max(impulse.Y, -shuttleVelocity.Y);
								}
								this.PhysicsSystem.SetLinearVelocity(shuttle2.Owner, body.LinearVelocity + shuttleNorthAngle.RotateVec(ref impulse), true, true, null, body);
							}
						}
						else
						{
							this._thruster.DisableLinearThrusters(shuttle2);
						}
						if (body.AngularVelocity != 0f)
						{
							float impulse2 = shuttle2.AngularThrust * brakeInput * ((body.AngularVelocity > 0f) ? -1f : 1f);
							float wishSpeed = 3.1415927f;
							if (impulse2 < 0f)
							{
								wishSpeed *= -1f;
							}
							float currentSpeed2 = body.AngularVelocity;
							float addSpeed2 = wishSpeed - currentSpeed2;
							if (!addSpeed2.Equals(0f))
							{
								float accelSpeed2 = impulse2 * body.InvI * frameTime;
								if (accelSpeed2 < 0f)
								{
									accelSpeed2 = MathF.Max(accelSpeed2, addSpeed2);
								}
								else
								{
									accelSpeed2 = MathF.Min(accelSpeed2, addSpeed2);
								}
								if (body.AngularVelocity < 0f && body.AngularVelocity + accelSpeed2 > 0f)
								{
									accelSpeed2 = -body.AngularVelocity;
								}
								else if (body.AngularVelocity > 0f && body.AngularVelocity + accelSpeed2 < 0f)
								{
									accelSpeed2 = -body.AngularVelocity;
								}
								this.PhysicsSystem.SetAngularVelocity(shuttle2.Owner, body.AngularVelocity + accelSpeed2, true, null, body);
								this._thruster.SetAngularThrust(shuttle2, true);
							}
						}
					}
					if (linearInput.Length.Equals(0f))
					{
						this.PhysicsSystem.SetSleepingAllowed(shuttle2.Owner, body, true, true);
						if (brakeInput.Equals(0f))
						{
							this._thruster.DisableLinearThrusters(shuttle2);
						}
						if ((double)body.LinearVelocity.Length < 0.08)
						{
							this.PhysicsSystem.SetLinearVelocity(shuttle2.Owner, Vector2.Zero, true, true, null, body);
						}
					}
					else
					{
						this.PhysicsSystem.SetSleepingAllowed(shuttle2.Owner, body, false, true);
						DirectionFlag dockFlag = DirectionExtensions.AsFlag(DirectionExtensions.ToWorldAngle(linearInput).GetDir());
						Vector2 totalForce = default(Vector2);
						foreach (object obj in Enum.GetValues(typeof(DirectionFlag)))
						{
							DirectionFlag dir = (DirectionFlag)obj;
							if (dir - 1 <= 1 || dir == 4 || dir == 8)
							{
								if ((dir & dockFlag) != null)
								{
									int index5 = (int)Math.Log2(dir);
									float thrust = shuttle2.LinearThrust[index5];
									switch (dir)
									{
									case 1:
										totalForce.Y -= thrust;
										break;
									case 2:
										totalForce.X += thrust;
										break;
									case 3:
										goto IL_831;
									case 4:
										totalForce.Y += thrust;
										break;
									default:
										if (dir != 8)
										{
											goto IL_831;
										}
										totalForce.X -= thrust;
										break;
									}
									this._thruster.EnableLinearThrustDirection(shuttle2, dir);
									continue;
									IL_831:
									throw new ArgumentOutOfRangeException();
								}
								this._thruster.DisableLinearThrustDirection(shuttle2, dir);
							}
						}
						Vector2 shuttleVelocity2 = (-shuttleNorthAngle).RotateVec(ref body.LinearVelocity);
						Vector2 wishDir2 = totalForce.Normalized;
						float num2 = 20f;
						float currentSpeed3 = Vector2.Dot(shuttleVelocity2, wishDir2);
						float addSpeed3 = num2 - currentSpeed3;
						if (addSpeed3 > 0f)
						{
							float accelSpeed3 = totalForce.Length * frameTime;
							accelSpeed3 = MathF.Min(accelSpeed3, addSpeed3);
							SharedPhysicsSystem physicsSystem = this.PhysicsSystem;
							EntityUid owner = shuttle2.Owner;
							Vector2 vector = totalForce.Normalized * accelSpeed3;
							physicsSystem.ApplyLinearImpulse(owner, shuttleNorthAngle.RotateVec(ref vector), null, body);
						}
					}
					if (MathHelper.CloseTo(angularInput, 0f, 1E-07f))
					{
						this._thruster.SetAngularThrust(shuttle2, false);
						this.PhysicsSystem.SetSleepingAllowed(shuttle2.Owner, body, true, true);
						if (Math.Abs(body.AngularVelocity) < 0.01f)
						{
							this.PhysicsSystem.SetAngularVelocity(shuttle2.Owner, 0f, true, null, body);
						}
					}
					else
					{
						this.PhysicsSystem.SetSleepingAllowed(shuttle2.Owner, body, false, true);
						float impulse3 = shuttle2.AngularThrust * -angularInput;
						float wishSpeed2 = 3.1415927f;
						if (impulse3 < 0f)
						{
							wishSpeed2 *= -1f;
						}
						float currentSpeed4 = body.AngularVelocity;
						float addSpeed4 = wishSpeed2 - currentSpeed4;
						if (!addSpeed4.Equals(0f))
						{
							float accelSpeed4 = impulse3 * body.InvI * frameTime;
							if (accelSpeed4 < 0f)
							{
								accelSpeed4 = MathF.Max(accelSpeed4, addSpeed4);
							}
							else
							{
								accelSpeed4 = MathF.Min(accelSpeed4, addSpeed4);
							}
							this.PhysicsSystem.SetAngularVelocity(shuttle2.Owner, body.AngularVelocity + accelSpeed4, true, null, body);
							this._thruster.SetAngularThrust(shuttle2, true);
						}
					}
				}
			}
		}

		// Token: 0x06000EE7 RID: 3815 RVA: 0x0004C32C File Offset: 0x0004A52C
		private bool FTLLocked(ShuttleComponent shuttle)
		{
			FTLComponent ftl;
			return base.TryComp<FTLComponent>(shuttle.Owner, ref ftl) && (ftl.State & (FTLState.Starting | FTLState.Travelling | FTLState.Arriving)) > FTLState.Invalid;
		}

		// Token: 0x040008B4 RID: 2228
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x040008B5 RID: 2229
		[Dependency]
		private readonly ThrusterSystem _thruster;

		// Token: 0x040008B6 RID: 2230
		[Nullable(new byte[]
		{
			1,
			1,
			1,
			0,
			1,
			1,
			1
		})]
		private Dictionary<ShuttleComponent, List<ValueTuple<PilotComponent, InputMoverComponent, TransformComponent>>> _shuttlePilots = new Dictionary<ShuttleComponent, List<ValueTuple<PilotComponent, InputMoverComponent, TransformComponent>>>();
	}
}
