using System;
using System.Runtime.CompilerServices;
using Content.Shared.ActionBlocker;
using Content.Shared.Buckle.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Rotatable;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Shared.Interaction
{
	// Token: 0x020003CD RID: 973
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RotateToFaceSystem : EntitySystem
	{
		// Token: 0x06000B2E RID: 2862 RVA: 0x00024774 File Offset: 0x00022974
		[NullableContext(2)]
		public bool TryRotateTo(EntityUid uid, Angle goalRotation, float frameTime, Angle tolerance, double rotationSpeed = 3.4028234663852886E+38, TransformComponent xform = null)
		{
			if (!base.Resolve<TransformComponent>(uid, ref xform, true))
			{
				return true;
			}
			if (rotationSpeed < 3.4028234663852886E+38)
			{
				Angle worldRot = this._transform.GetWorldRotation(xform);
				double rotationDiff = Angle.ShortestDistance(ref worldRot, ref goalRotation).Theta;
				double maxRotate = rotationSpeed * (double)frameTime;
				if (Math.Abs(rotationDiff) > maxRotate)
				{
					Angle goalTheta = worldRot + (double)Math.Sign(rotationDiff) * maxRotate;
					this._transform.SetWorldRotation(xform, goalTheta);
					rotationDiff = goalRotation - goalTheta;
					return Math.Abs(rotationDiff) <= tolerance;
				}
				this._transform.SetWorldRotation(xform, goalRotation);
			}
			else
			{
				this._transform.SetWorldRotation(xform, goalRotation);
			}
			return true;
		}

		// Token: 0x06000B2F RID: 2863 RVA: 0x0002482C File Offset: 0x00022A2C
		[NullableContext(2)]
		public bool TryFaceCoordinates(EntityUid user, Vector2 coordinates, TransformComponent xform = null)
		{
			if (!base.Resolve<TransformComponent>(user, ref xform, true))
			{
				return false;
			}
			Vector2 diff = coordinates - xform.MapPosition.Position;
			if (diff.LengthSquared <= 0.01f)
			{
				return true;
			}
			Angle diffAngle = Angle.FromWorldVec(diff);
			return this.TryFaceAngle(user, diffAngle, null);
		}

		// Token: 0x06000B30 RID: 2864 RVA: 0x0002487C File Offset: 0x00022A7C
		[NullableContext(2)]
		public bool TryFaceAngle(EntityUid user, Angle diffAngle, TransformComponent xform = null)
		{
			if (!this._actionBlockerSystem.CanChangeDirection(user))
			{
				BuckleComponent buckle;
				if (this.EntityManager.TryGetComponent<BuckleComponent>(user, ref buckle) && buckle.Buckled)
				{
					EntityUid? suid = buckle.LastEntityBuckledTo;
					RotatableComponent rotatable;
					if (suid != null && base.TryComp<RotatableComponent>(suid.Value, ref rotatable) && rotatable.RotateWhileAnchored)
					{
						base.Transform(rotatable.Owner).WorldRotation = diffAngle;
						return true;
					}
				}
				return false;
			}
			if (!base.Resolve<TransformComponent>(user, ref xform, true))
			{
				return false;
			}
			xform.WorldRotation = diffAngle;
			return true;
		}

		// Token: 0x04000B1B RID: 2843
		[Dependency]
		private readonly ActionBlockerSystem _actionBlockerSystem;

		// Token: 0x04000B1C RID: 2844
		[Dependency]
		private readonly MobStateSystem _mobState;

		// Token: 0x04000B1D RID: 2845
		[Dependency]
		private readonly SharedTransformSystem _transform;
	}
}
