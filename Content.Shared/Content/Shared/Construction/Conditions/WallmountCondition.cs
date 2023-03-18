using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.Tag;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Shared.Construction.Conditions
{
	// Token: 0x02000583 RID: 1411
	[DataDefinition]
	public sealed class WallmountCondition : IConstructionCondition
	{
		// Token: 0x06001156 RID: 4438 RVA: 0x00038F40 File Offset: 0x00037140
		public bool Condition(EntityUid user, EntityCoordinates location, Direction direction)
		{
			IEntityManager entManager = IoCManager.Resolve<IEntityManager>();
			Vector2 userWorldPosition = entManager.GetComponent<TransformComponent>(user).WorldPosition;
			Vector2 objWorldPosition = location.ToMap(entManager).Position;
			Vector2 userToObject = objWorldPosition - userWorldPosition;
			Angle gridRotation = entManager.GetComponent<TransformComponent>(location.EntityId).WorldRotation;
			Vector2 vector = DirectionExtensions.ToVec(direction);
			Vector2 directionWithOffset = gridRotation.RotateVec(ref vector);
			if (Vector2.Dot(directionWithOffset.Normalized, userToObject.Normalized) > 0f)
			{
				return false;
			}
			SharedPhysicsSystem physics = entManager.System<SharedPhysicsSystem>();
			CollisionRay rUserToObj;
			rUserToObj..ctor(userWorldPosition, userToObject.Normalized, 2);
			float length = userToObject.Length;
			TagSystem tagSystem = entManager.System<TagSystem>();
			IEnumerable<RayCastResults> userToObjRaycastResults = physics.IntersectRayWithPredicate(entManager.GetComponent<TransformComponent>(user).MapID, rUserToObj, length, (EntityUid e) => !tagSystem.HasTag(e, "Wall"), true);
			RayCastResults? targetWall = Extensions.FirstOrNull<RayCastResults>(userToObjRaycastResults);
			if (targetWall == null)
			{
				return false;
			}
			CollisionRay rAdjWall;
			rAdjWall..ctor(objWorldPosition, directionWithOffset.Normalized, 2);
			return !physics.IntersectRayWithPredicate(entManager.GetComponent<TransformComponent>(user).MapID, rAdjWall, 0.5f, (EntityUid e) => e == targetWall.Value.HitEntity || !tagSystem.HasTag(e, "Wall"), true).Any<RayCastResults>();
		}

		// Token: 0x06001157 RID: 4439 RVA: 0x0003906D File Offset: 0x0003726D
		[NullableContext(1)]
		public ConstructionGuideEntry GenerateGuideEntry()
		{
			return new ConstructionGuideEntry
			{
				Localization = "construction-step-condition-wallmount"
			};
		}
	}
}
