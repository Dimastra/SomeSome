using System;
using System.Runtime.CompilerServices;
using Content.Server.Storage.Components;
using Content.Shared.Storage.Components;
using Content.Shared.Storage.EntitySystems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;

namespace Content.Server.Storage.EntitySystems
{
	// Token: 0x02000161 RID: 353
	public sealed class ItemCounterSystem : SharedItemCounterSystem
	{
		// Token: 0x060006DA RID: 1754 RVA: 0x00021E90 File Offset: 0x00020090
		[NullableContext(1)]
		protected override int? GetCount(ContainerModifiedMessage msg, ItemCounterComponent itemCounter)
		{
			ServerStorageComponent component;
			if (!this.EntityManager.TryGetComponent<ServerStorageComponent>(msg.Container.Owner, ref component) || component.StoredEntities == null)
			{
				return null;
			}
			int count = 0;
			foreach (EntityUid entity in component.StoredEntities)
			{
				if (itemCounter.Count.IsValid(entity, null))
				{
					count++;
				}
			}
			return new int?(count);
		}
	}
}
