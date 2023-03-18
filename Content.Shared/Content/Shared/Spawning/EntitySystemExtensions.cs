using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Content.Shared.Physics;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;

namespace Content.Shared.Spawning
{
	// Token: 0x02000182 RID: 386
	[NullableContext(2)]
	[Nullable(0)]
	public static class EntitySystemExtensions
	{
		// Token: 0x060004A6 RID: 1190 RVA: 0x00012118 File Offset: 0x00010318
		public static EntityUid? SpawnIfUnobstructed([Nullable(1)] this IEntityManager entityManager, string prototypeName, EntityCoordinates coordinates, CollisionGroup collisionLayer, in Box2? box = null, SharedPhysicsSystem physicsManager = null)
		{
			if (physicsManager == null)
			{
				physicsManager = entityManager.System<SharedPhysicsSystem>();
			}
			MapCoordinates mapCoordinates = coordinates.ToMap(entityManager);
			return entityManager.SpawnIfUnobstructed(prototypeName, mapCoordinates, collisionLayer, box, physicsManager);
		}

		// Token: 0x060004A7 RID: 1191 RVA: 0x00012148 File Offset: 0x00010348
		public static EntityUid? SpawnIfUnobstructed([Nullable(1)] this IEntityManager entityManager, string prototypeName, MapCoordinates coordinates, CollisionGroup collisionLayer, in Box2? box = null, SharedPhysicsSystem collision = null)
		{
			Box2 boxOrDefault = box.GetValueOrDefault(Box2.UnitCentered).Translated(coordinates.Position);
			if (collision == null)
			{
				collision = entityManager.System<SharedPhysicsSystem>();
			}
			foreach (PhysicsComponent body in collision.GetCollidingEntities(coordinates.MapId, ref boxOrDefault))
			{
				if (body.Hard && collisionLayer != CollisionGroup.None && (body.CollisionMask & (int)collisionLayer) != 0)
				{
					return null;
				}
			}
			return new EntityUid?(entityManager.SpawnEntity(prototypeName, coordinates));
		}

		// Token: 0x060004A8 RID: 1192 RVA: 0x000121F4 File Offset: 0x000103F4
		public static bool TrySpawnIfUnobstructed([Nullable(1)] this IEntityManager entityManager, string prototypeName, EntityCoordinates coordinates, CollisionGroup collisionLayer, [NotNullWhen(true)] out EntityUid? entity, Box2? box = null, SharedPhysicsSystem physicsManager = null)
		{
			entity = entityManager.SpawnIfUnobstructed(prototypeName, coordinates, collisionLayer, box, physicsManager);
			return entity != null;
		}

		// Token: 0x060004A9 RID: 1193 RVA: 0x00012211 File Offset: 0x00010411
		public static bool TrySpawnIfUnobstructed([Nullable(1)] this IEntityManager entityManager, string prototypeName, MapCoordinates coordinates, CollisionGroup collisionLayer, [NotNullWhen(true)] out EntityUid? entity, in Box2? box = null, SharedPhysicsSystem physicsManager = null)
		{
			entity = entityManager.SpawnIfUnobstructed(prototypeName, coordinates, collisionLayer, box, physicsManager);
			return entity != null;
		}
	}
}
