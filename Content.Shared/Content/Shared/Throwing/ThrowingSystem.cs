using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Shared.Gravity;
using Content.Shared.Interaction;
using Content.Shared.Tag;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Timing;

namespace Content.Shared.Throwing
{
	// Token: 0x020000D6 RID: 214
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ThrowingSystem : EntitySystem
	{
		// Token: 0x06000253 RID: 595 RVA: 0x0000B294 File Offset: 0x00009494
		[NullableContext(2)]
		public void TryThrow(EntityUid uid, Vector2 direction, float strength = 1f, EntityUid? user = null, float pushbackRatio = 5f, PhysicsComponent physics = null, TransformComponent transform = null, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<PhysicsComponent>? physicsQuery = null, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<TransformComponent>? xformQuery = null)
		{
			if (strength <= 0f || direction == Vector2.Infinity || direction == Vector2.NaN || direction == Vector2.Zero)
			{
				return;
			}
			EntityQuery<PhysicsComponent> value = physicsQuery.GetValueOrDefault();
			if (physicsQuery == null)
			{
				value = base.GetEntityQuery<PhysicsComponent>();
				physicsQuery = new EntityQuery<PhysicsComponent>?(value);
			}
			if (physics == null && !physicsQuery.Value.TryGetComponent(uid, ref physics))
			{
				return;
			}
			if ((physics.BodyType & 10) == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(47, 2);
				defaultInterpolatedStringHandler.AppendLiteral("Tried to throw entity ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid));
				defaultInterpolatedStringHandler.AppendLiteral(" but can't throw ");
				defaultInterpolatedStringHandler.AppendFormatted<BodyType>(physics.BodyType);
				defaultInterpolatedStringHandler.AppendLiteral(" bodies!");
				Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			ThrownItemComponent comp = base.EnsureComp<ThrownItemComponent>(uid);
			comp.Thrower = user;
			if (!this._tagSystem.HasTag(uid, "NoSpinOnThrow"))
			{
				this._physics.ApplyAngularImpulse(uid, 1.5f, null, physics);
			}
			else
			{
				if (transform == null)
				{
					EntityQuery<TransformComponent> value2 = xformQuery.GetValueOrDefault();
					if (xformQuery == null)
					{
						value2 = base.GetEntityQuery<TransformComponent>();
						xformQuery = new EntityQuery<TransformComponent>?(value2);
					}
					transform = xformQuery.Value.GetComponent(uid);
				}
				transform.LocalRotation = DirectionExtensions.ToWorldAngle(direction) - 3.141592653589793;
			}
			if (user != null)
			{
				this._interactionSystem.ThrownInteraction(user.Value, uid);
			}
			Vector2 impulseVector = direction.Normalized * strength * physics.Mass;
			this._physics.ApplyLinearImpulse(uid, impulseVector, null, physics);
			float time = (direction / strength).Length;
			if (time < 0.15f)
			{
				this._thrownSystem.LandComponent(comp, physics);
			}
			else
			{
				this._physics.SetBodyStatus(physics, 1, true);
				Timer.Spawn(TimeSpan.FromSeconds((double)(time - 0.15f)), delegate()
				{
					if (physics.Deleted)
					{
						return;
					}
					this._thrownSystem.LandComponent(comp, physics);
				}, default(CancellationToken));
			}
			PhysicsComponent userPhysics;
			if (user != null && pushbackRatio > 0f && physicsQuery.Value.TryGetComponent(user.Value, ref userPhysics) && this._gravity.IsWeightless(user.Value, userPhysics, null))
			{
				ThrowPushbackAttemptEvent msg = new ThrowPushbackAttemptEvent();
				base.RaiseLocalEvent<ThrowPushbackAttemptEvent>(physics.Owner, msg, false);
				if (!msg.Cancelled)
				{
					this._physics.ApplyLinearImpulse(user.Value, -impulseVector * pushbackRatio, null, userPhysics);
				}
			}
		}

		// Token: 0x040002BC RID: 700
		public const float ThrowAngularImpulse = 1.5f;

		// Token: 0x040002BD RID: 701
		public const float FlyTime = 0.15f;

		// Token: 0x040002BE RID: 702
		[Dependency]
		private readonly SharedGravitySystem _gravity;

		// Token: 0x040002BF RID: 703
		[Dependency]
		private readonly SharedInteractionSystem _interactionSystem;

		// Token: 0x040002C0 RID: 704
		[Dependency]
		private readonly SharedPhysicsSystem _physics;

		// Token: 0x040002C1 RID: 705
		[Dependency]
		private readonly ThrownItemSystem _thrownSystem;

		// Token: 0x040002C2 RID: 706
		[Dependency]
		private readonly TagSystem _tagSystem;
	}
}
