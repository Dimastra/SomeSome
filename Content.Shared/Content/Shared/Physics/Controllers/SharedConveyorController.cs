using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Conveyor;
using Content.Shared.Gravity;
using Content.Shared.Movement.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Controllers;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;

namespace Content.Shared.Physics.Controllers
{
	// Token: 0x02000281 RID: 641
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedConveyorController : VirtualController
	{
		// Token: 0x06000746 RID: 1862 RVA: 0x00018B58 File Offset: 0x00016D58
		public override void Initialize()
		{
			base.UpdatesAfter.Add(typeof(SharedMoverController));
			base.SubscribeLocalEvent<ConveyorComponent, ComponentGetState>(new ComponentEventRefHandler<ConveyorComponent, ComponentGetState>(this.OnConveyorGetState), null, null);
			base.SubscribeLocalEvent<ConveyorComponent, ComponentHandleState>(new ComponentEventRefHandler<ConveyorComponent, ComponentHandleState>(this.OnConveyorHandleState), null, null);
			base.SubscribeLocalEvent<ConveyorComponent, StartCollideEvent>(new ComponentEventRefHandler<ConveyorComponent, StartCollideEvent>(this.OnConveyorStartCollide), null, null);
			base.SubscribeLocalEvent<ConveyorComponent, EndCollideEvent>(new ComponentEventRefHandler<ConveyorComponent, EndCollideEvent>(this.OnConveyorEndCollide), null, null);
			base.Initialize();
		}

		// Token: 0x06000747 RID: 1863 RVA: 0x00018BD0 File Offset: 0x00016DD0
		private void OnConveyorGetState(EntityUid uid, ConveyorComponent component, ref ComponentGetState args)
		{
			args.State = new ConveyorComponentState(component.Angle, component.Speed, component.State, component.Powered);
		}

		// Token: 0x06000748 RID: 1864 RVA: 0x00018BF8 File Offset: 0x00016DF8
		private void OnConveyorHandleState(EntityUid uid, ConveyorComponent component, ref ComponentHandleState args)
		{
			ConveyorComponentState state = args.Current as ConveyorComponentState;
			if (state == null)
			{
				return;
			}
			component.Powered = state.Powered;
			component.Angle = state.Angle;
			component.Speed = state.Speed;
			component.State = state.State;
		}

		// Token: 0x06000749 RID: 1865 RVA: 0x00018C48 File Offset: 0x00016E48
		private void OnConveyorStartCollide(EntityUid uid, ConveyorComponent component, ref StartCollideEvent args)
		{
			EntityUid otherUid = args.OtherFixture.Body.Owner;
			if (args.OtherFixture.Body.BodyType == 4 || component.State == ConveyorState.Off)
			{
				return;
			}
			component.Intersecting.Add(otherUid);
			base.EnsureComp<ActiveConveyorComponent>(uid);
		}

		// Token: 0x0600074A RID: 1866 RVA: 0x00018C97 File Offset: 0x00016E97
		private void OnConveyorEndCollide(EntityUid uid, ConveyorComponent component, ref EndCollideEvent args)
		{
			component.Intersecting.Remove(args.OtherFixture.Body.Owner);
			if (component.Intersecting.Count == 0)
			{
				base.RemComp<ActiveConveyorComponent>(uid);
			}
		}

		// Token: 0x0600074B RID: 1867 RVA: 0x00018CCC File Offset: 0x00016ECC
		public override void UpdateBeforeSolve(bool prediction, float frameTime)
		{
			base.UpdateBeforeSolve(prediction, frameTime);
			HashSet<EntityUid> conveyed = new HashSet<EntityUid>();
			EntityQuery<TransformComponent> xformQuery = base.GetEntityQuery<TransformComponent>();
			EntityQuery<PhysicsComponent> bodyQuery = base.GetEntityQuery<PhysicsComponent>();
			foreach (ValueTuple<ActiveConveyorComponent, ConveyorComponent> valueTuple in base.EntityQuery<ActiveConveyorComponent, ConveyorComponent>(false))
			{
				ConveyorComponent comp = valueTuple.Item2;
				EntityUid uid = comp.Owner;
				this.Convey(uid, comp, xformQuery, bodyQuery, conveyed, frameTime, prediction);
			}
		}

		// Token: 0x0600074C RID: 1868 RVA: 0x00018D50 File Offset: 0x00016F50
		private void Convey(EntityUid uid, ConveyorComponent comp, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<TransformComponent> xformQuery, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<PhysicsComponent> bodyQuery, HashSet<EntityUid> conveyed, float frameTime, bool prediction)
		{
			if (!this.CanRun(comp))
			{
				return;
			}
			float speed = comp.Speed;
			TransformComponent xform;
			if (speed <= 0f || !xformQuery.TryGetComponent(uid, ref xform) || xform.GridUid == null)
			{
				return;
			}
			Vector2 conveyorPos = xform.LocalPosition;
			Angle conveyorRot = xform.LocalRotation;
			conveyorRot += comp.Angle;
			if (comp.State == ConveyorState.Reverse)
			{
				conveyorRot += 3.1415927f;
			}
			Vector2 direction = conveyorRot.ToWorldVec();
			foreach (ValueTuple<EntityUid, TransformComponent, PhysicsComponent> valueTuple in this.GetEntitiesToMove(comp, xform, xformQuery, bodyQuery))
			{
				EntityUid entity = valueTuple.Item1;
				TransformComponent transform = valueTuple.Item2;
				PhysicsComponent body = valueTuple.Item3;
				if (conveyed.Add(entity) && (!prediction || body.Predict))
				{
					Vector2 localPos = transform.LocalPosition;
					Vector2 itemRelative = conveyorPos - localPos;
					localPos += SharedConveyorController.Convey(direction, speed, frameTime, itemRelative);
					transform.LocalPosition = localPos;
					this._physics.SetAwake(entity, body, true, true);
					this._physics.SetSleepTime(body, 0f);
				}
			}
			base.Dirty(comp, null);
		}

		// Token: 0x0600074D RID: 1869 RVA: 0x00018EA8 File Offset: 0x000170A8
		private static Vector2 Convey(Vector2 direction, float speed, float frameTime, Vector2 itemRelative)
		{
			if (speed == 0f || direction.Length == 0f)
			{
				return Vector2.Zero;
			}
			Vector2 p = direction * (Vector2.Dot(itemRelative, direction) / Vector2.Dot(direction, direction));
			Vector2 r = itemRelative - p;
			if ((double)r.Length < 0.1)
			{
				return direction * speed * frameTime;
			}
			return (r + direction * 0.2f).Normalized * speed * frameTime;
		}

		// Token: 0x0600074E RID: 1870 RVA: 0x00018F35 File Offset: 0x00017135
		[return: Nullable(new byte[]
		{
			1,
			0,
			1,
			1
		})]
		private IEnumerable<ValueTuple<EntityUid, TransformComponent, PhysicsComponent>> GetEntitiesToMove(ConveyorComponent comp, TransformComponent xform, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<TransformComponent> xformQuery, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<PhysicsComponent> bodyQuery)
		{
			MapGridComponent grid = this._mapManager.GetGrid(xform.GridUid.Value);
			TileRef tile = grid.GetTileRef(xform.Coordinates);
			Box2 conveyorBounds = this._lookup.GetLocalBounds(tile, grid.TileSize);
			foreach (EntityUid entity in comp.Intersecting)
			{
				TransformComponent entityXform;
				PhysicsComponent physics;
				if (xformQuery.TryGetComponent(entity, ref entityXform) && !(entityXform.ParentUid != grid.Owner) && bodyQuery.TryGetComponent(entity, ref physics) && physics.BodyType != 4 && physics.BodyStatus != 1 && !this._gravity.IsWeightless(entity, physics, entityXform))
				{
					Box2 gridAABB;
					gridAABB..ctor(entityXform.LocalPosition - 0.1f, entityXform.LocalPosition + 0.1f);
					if (conveyorBounds.Intersects(ref gridAABB))
					{
						yield return new ValueTuple<EntityUid, TransformComponent, PhysicsComponent>(entity, entityXform, physics);
					}
				}
			}
			HashSet<EntityUid>.Enumerator enumerator = default(HashSet<EntityUid>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x0600074F RID: 1871 RVA: 0x00018F62 File Offset: 0x00017162
		public bool CanRun(ConveyorComponent component)
		{
			return component.State != ConveyorState.Off && component.Powered;
		}

		// Token: 0x0400074F RID: 1871
		[Dependency]
		protected readonly IMapManager _mapManager;

		// Token: 0x04000750 RID: 1872
		[Dependency]
		protected readonly EntityLookupSystem _lookup;

		// Token: 0x04000751 RID: 1873
		[Dependency]
		protected readonly SharedPhysicsSystem _physics;

		// Token: 0x04000752 RID: 1874
		[Dependency]
		private readonly SharedGravitySystem _gravity;

		// Token: 0x04000753 RID: 1875
		protected const string ConveyorFixture = "conveyor";
	}
}
