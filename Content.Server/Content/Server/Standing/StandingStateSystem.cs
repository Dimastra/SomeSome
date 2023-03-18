using System;
using System.Runtime.CompilerServices;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Standing;
using Content.Shared.Throwing;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Random;

namespace Content.Server.Standing
{
	// Token: 0x020001A6 RID: 422
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class StandingStateSystem : EntitySystem
	{
		// Token: 0x0600085A RID: 2138 RVA: 0x0002AA54 File Offset: 0x00028C54
		private void FallOver(EntityUid uid, StandingStateComponent component, DropHandItemsEvent args)
		{
			PhysicsComponent comp;
			Vector2 direction = this.EntityManager.TryGetComponent<PhysicsComponent>(uid, ref comp) ? (comp.LinearVelocity / 50f) : Vector2.Zero;
			float dropAngle = this._random.NextFloat(0.8f, 1.2f);
			FellDownEvent fellEvent = new FellDownEvent(uid);
			base.RaiseLocalEvent<FellDownEvent>(uid, fellEvent, false);
			SharedHandsComponent handsComp;
			if (!base.TryComp<SharedHandsComponent>(uid, ref handsComp))
			{
				return;
			}
			Vector2 worldRotation = this.EntityManager.GetComponent<TransformComponent>(uid).WorldRotation.ToVec();
			foreach (Hand hand in handsComp.Hands.Values)
			{
				EntityUid? heldEntity = hand.HeldEntity;
				if (heldEntity != null)
				{
					EntityUid held = heldEntity.GetValueOrDefault();
					if (this._handsSystem.TryDrop(uid, hand, null, false, true, handsComp))
					{
						ThrowingSystem throwingSystem = this._throwingSystem;
						EntityUid uid2 = held;
						Angle angle = this._random.NextAngle();
						Vector2 vector = direction / dropAngle + worldRotation / 50f;
						throwingSystem.TryThrow(uid2, angle.RotateVec(ref vector), 0.5f * dropAngle * this._random.NextFloat(-0.9f, 1.1f), new EntityUid?(uid), 0f, null, null, null, null);
					}
				}
			}
		}

		// Token: 0x0600085B RID: 2139 RVA: 0x0002ABDC File Offset: 0x00028DDC
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<StandingStateComponent, DropHandItemsEvent>(new ComponentEventHandler<StandingStateComponent, DropHandItemsEvent>(this.FallOver), null, null);
		}

		// Token: 0x04000522 RID: 1314
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x04000523 RID: 1315
		[Dependency]
		private readonly SharedHandsSystem _handsSystem;

		// Token: 0x04000524 RID: 1316
		[Dependency]
		private readonly ThrowingSystem _throwingSystem;
	}
}
