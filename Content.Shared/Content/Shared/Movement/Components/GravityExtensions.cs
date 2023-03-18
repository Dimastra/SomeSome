using System;
using System.Runtime.CompilerServices;
using Content.Shared.Clothing;
using Content.Shared.Gravity;
using Content.Shared.Inventory;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Physics.Components;

namespace Content.Shared.Movement.Components
{
	// Token: 0x020002EF RID: 751
	public static class GravityExtensions
	{
		// Token: 0x0600086D RID: 2157 RVA: 0x0001CA34 File Offset: 0x0001AC34
		[NullableContext(2)]
		[Obsolete("Use GravitySystem")]
		public static bool IsWeightless(this EntityUid entity, PhysicsComponent body = null, EntityCoordinates? coords = null, IMapManager mapManager = null, IEntityManager entityManager = null)
		{
			if (entityManager == null)
			{
				entityManager = IoCManager.Resolve<IEntityManager>();
			}
			if (body == null)
			{
				entityManager.TryGetComponent<PhysicsComponent>(entity, ref body);
			}
			if (body == null || (body.BodyType & 4) > 0)
			{
				return false;
			}
			MovementIgnoreGravityComponent ignoreGravityComponent;
			if (entityManager.TryGetComponent<MovementIgnoreGravityComponent>(entity, ref ignoreGravityComponent))
			{
				return ignoreGravityComponent.Weightless;
			}
			TransformComponent transform = entityManager.GetComponent<TransformComponent>(entity);
			EntityUid? gridId = transform.GridUid;
			GravityComponent gravity;
			if ((entityManager.TryGetComponent<GravityComponent>(transform.GridUid, ref gravity) || entityManager.TryGetComponent<GravityComponent>(transform.MapUid, ref gravity)) && gravity.EnabledVV)
			{
				return false;
			}
			if (gridId == null)
			{
				return true;
			}
			if (mapManager == null)
			{
				mapManager = IoCManager.Resolve<IMapManager>();
			}
			MapGridComponent grid = mapManager.GetGrid(gridId.Value);
			EntityUid? ent;
			MagbootsComponent boots;
			if (EntitySystem.Get<InventorySystem>().TryGetSlotEntity(entity, "shoes", out ent, null, null) && entityManager.TryGetComponent<MagbootsComponent>(ent, ref boots) && boots.On)
			{
				return false;
			}
			if (!entityManager.GetComponent<GravityComponent>(grid.Owner).EnabledVV)
			{
				return true;
			}
			EntityCoordinates value = coords.GetValueOrDefault();
			if (coords == null)
			{
				value = transform.Coordinates;
				coords = new EntityCoordinates?(value);
			}
			if (!coords.Value.IsValid(entityManager))
			{
				return true;
			}
			Tile tile = grid.GetTileRef(coords.Value).Tile;
			return tile.IsEmpty;
		}
	}
}
