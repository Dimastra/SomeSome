using System;
using System.Runtime.CompilerServices;
using Content.Shared.Pulling;
using Content.Shared.Pulling.Components;
using Content.Shared.Rotatable;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Controllers;

namespace Content.Server.Physics.Controllers
{
	// Token: 0x020002DB RID: 731
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PullController : VirtualController
	{
		// Token: 0x06000EE9 RID: 3817 RVA: 0x0004C36A File Offset: 0x0004A56A
		public override void Initialize()
		{
			base.UpdatesAfter.Add(typeof(MoverController));
			base.SubscribeLocalEvent<SharedPullerComponent, MoveEvent>(new ComponentEventRefHandler<SharedPullerComponent, MoveEvent>(this.OnPullerMove), null, null);
			base.Initialize();
		}

		// Token: 0x06000EEA RID: 3818 RVA: 0x0004C39C File Offset: 0x0004A59C
		private void OnPullerMove(EntityUid uid, SharedPullerComponent component, ref MoveEvent args)
		{
			SharedPullableComponent pullable;
			if (component.Pulling == null || !base.TryComp<SharedPullableComponent>(component.Pulling.Value, ref pullable))
			{
				return;
			}
			this.UpdatePulledRotation(uid, pullable.Owner);
			if (args.NewPosition.EntityId == args.OldPosition.EntityId && (args.NewPosition.Position - args.OldPosition.Position).LengthSquared < 2.5E-05f)
			{
				return;
			}
			PhysicsComponent physics;
			if (base.TryComp<PhysicsComponent>(pullable.Owner, ref physics))
			{
				this.PhysicsSystem.WakeBody(pullable.Owner, false, null, physics);
			}
			this._pullableSystem.StopMoveTo(pullable);
		}

		// Token: 0x06000EEB RID: 3819 RVA: 0x0004C458 File Offset: 0x0004A658
		private void UpdatePulledRotation(EntityUid puller, EntityUid pulled)
		{
			RotatableComponent rotatable;
			if (!base.TryComp<RotatableComponent>(pulled, ref rotatable))
			{
				return;
			}
			if (!rotatable.RotateWhilePulling)
			{
				return;
			}
			EntityQuery<TransformComponent> xforms = base.GetEntityQuery<TransformComponent>();
			TransformComponent pulledXform = xforms.GetComponent(pulled);
			TransformComponent pullerXform = xforms.GetComponent(puller);
			ref ValueTuple<Vector2, Angle> worldPositionRotation = this.TransformSystem.GetWorldPositionRotation(pullerXform, xforms);
			ValueTuple<Vector2, Angle> pulledData = this.TransformSystem.GetWorldPositionRotation(pulledXform, xforms);
			Vector2 dir = worldPositionRotation.Item1 - pulledData.Item1;
			if (dir.LengthSquared > 1f)
			{
				Angle oldAngle = pulledData.Item2;
				Angle newAngle = Angle.FromWorldVec(dir);
				if (Math.Abs((newAngle - oldAngle).Degrees) > 11.25)
				{
					Angle baseRotation = pulledData.Item2 - pulledXform.LocalRotation;
					Angle localRotationSnapped = Angle.FromDegrees(Math.Floor((newAngle - baseRotation).Degrees / 22.5 + 0.5) * 22.5);
					this.TransformSystem.SetLocalRotation(pulledXform, localRotationSnapped);
				}
			}
		}

		// Token: 0x06000EEC RID: 3820 RVA: 0x0004C568 File Offset: 0x0004A768
		public override void UpdateBeforeSolve(bool prediction, float frameTime)
		{
			base.UpdateBeforeSolve(prediction, frameTime);
			foreach (SharedPullableComponent pullable in this._pullableSystem.Moving)
			{
				if (!pullable.Deleted && pullable.MovingTo != null)
				{
					EntityUid? puller2 = pullable.Puller;
					if (puller2 != null)
					{
						EntityUid puller = puller2.GetValueOrDefault();
						if (puller.Valid)
						{
							MapCoordinates pullerPosition = this.EntityManager.GetComponent<TransformComponent>(puller).MapPosition;
							MapCoordinates movingTo = pullable.MovingTo.Value.ToMap(this.EntityManager);
							PhysicsComponent physics;
							if (movingTo.MapId != pullerPosition.MapId)
							{
								this._pullableSystem.StopMoveTo(pullable);
							}
							else if (!this.EntityManager.TryGetComponent<PhysicsComponent>(pullable.Owner, ref physics) || physics.BodyType == 4 || movingTo.MapId != this.EntityManager.GetComponent<TransformComponent>(pullable.Owner).MapID)
							{
								this._pullableSystem.StopMoveTo(pullable);
							}
							else
							{
								Vector2 position = movingTo.Position;
								Vector2 ownerPosition = this.EntityManager.GetComponent<TransformComponent>(pullable.Owner).MapPosition.Position;
								Vector2 diff = position - ownerPosition;
								float diffLength = diff.Length;
								if (diffLength < 0.1f && physics.LinearVelocity.Length < 0.1f)
								{
									this.PhysicsSystem.SetLinearVelocity(pullable.Owner, Vector2.Zero, true, true, null, physics);
									this._pullableSystem.StopMoveTo(pullable);
								}
								else
								{
									float impulseModifierLerp = Math.Min(1f, Math.Max(0f, (physics.Mass - 5f) / 65f));
									float impulseModifier = MathHelper.Lerp(60f, 15f, impulseModifierLerp);
									float multiplier = (diffLength < 1f) ? (impulseModifier * diffLength) : impulseModifier;
									Vector2 accel = diff.Normalized * multiplier;
									if (diffLength < 1f && physics.LinearVelocity.Length >= 0.25f)
									{
										float scaling = (1f - diffLength) / 1f;
										accel -= physics.LinearVelocity * 20f * scaling;
									}
									this.PhysicsSystem.WakeBody(pullable.Owner, false, null, physics);
									Vector2 impulse = accel * physics.Mass * frameTime;
									this.PhysicsSystem.ApplyLinearImpulse(pullable.Owner, impulse, null, physics);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x040008B7 RID: 2231
		private const float AccelModifierHigh = 15f;

		// Token: 0x040008B8 RID: 2232
		private const float AccelModifierLow = 60f;

		// Token: 0x040008B9 RID: 2233
		private const float AccelModifierHighMass = 70f;

		// Token: 0x040008BA RID: 2234
		private const float AccelModifierLowMass = 5f;

		// Token: 0x040008BB RID: 2235
		private const float MaximumSettleVelocity = 0.1f;

		// Token: 0x040008BC RID: 2236
		private const float MaximumSettleDistance = 0.1f;

		// Token: 0x040008BD RID: 2237
		private const float SettleMinimumShutdownVelocity = 0.25f;

		// Token: 0x040008BE RID: 2238
		private const float SettleShutdownDistance = 1f;

		// Token: 0x040008BF RID: 2239
		private const float SettleShutdownMultiplier = 20f;

		// Token: 0x040008C0 RID: 2240
		private const float MinimumMovementDistance = 0.005f;

		// Token: 0x040008C1 RID: 2241
		[Dependency]
		private readonly SharedPullingSystem _pullableSystem;

		// Token: 0x040008C2 RID: 2242
		private const float ThresholdRotDistance = 1f;

		// Token: 0x040008C3 RID: 2243
		private const float ThresholdRotAngle = 22.5f;
	}
}
