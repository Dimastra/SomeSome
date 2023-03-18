using System;
using System.Runtime.CompilerServices;
using Content.Client.Animations;
using Content.Shared.Storage;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;

namespace Content.Client.Storage.Systems
{
	// Token: 0x0200012D RID: 301
	public sealed class StorageSystem : EntitySystem
	{
		// Token: 0x06000821 RID: 2081 RVA: 0x0002F5A8 File Offset: 0x0002D7A8
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeNetworkEvent<AnimateInsertingEntitiesEvent>(new EntityEventHandler<AnimateInsertingEntitiesEvent>(this.HandleAnimatingInsertingEntities), null, null);
		}

		// Token: 0x06000822 RID: 2082 RVA: 0x0002F5C4 File Offset: 0x0002D7C4
		[NullableContext(1)]
		public void HandleAnimatingInsertingEntities(AnimateInsertingEntitiesEvent msg)
		{
			ClientStorageComponent clientStorageComponent;
			if (!base.TryComp<ClientStorageComponent>(msg.Storage, ref clientStorageComponent))
			{
				return;
			}
			TransformComponent transformComponent;
			base.TryComp<TransformComponent>(msg.Storage, ref transformComponent);
			int num = 0;
			while (msg.StoredEntities.Count > num)
			{
				EntityUid entityUid = msg.StoredEntities[num];
				EntityCoordinates initialPosition = msg.EntityPositions[num];
				if (this.EntityManager.EntityExists(entityUid) && transformComponent != null)
				{
					ReusableAnimations.AnimateEntityPickup(entityUid, initialPosition, transformComponent.LocalPosition, this.EntityManager);
				}
				num++;
			}
		}
	}
}
