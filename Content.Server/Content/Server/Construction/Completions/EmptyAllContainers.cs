using System;
using System.Runtime.CompilerServices;
using Content.Shared.Construction;
using Robust.Server.Containers;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Construction.Completions
{
	// Token: 0x02000618 RID: 1560
	[DataDefinition]
	public sealed class EmptyAllContainers : IGraphAction
	{
		// Token: 0x0600214E RID: 8526 RVA: 0x000AE5E4 File Offset: 0x000AC7E4
		[NullableContext(1)]
		public void PerformAction(EntityUid uid, EntityUid? userUid, IEntityManager entityManager)
		{
			ContainerManagerComponent containerManager;
			if (!entityManager.TryGetComponent<ContainerManagerComponent>(uid, ref containerManager))
			{
				return;
			}
			TransformComponent transform = entityManager.GetComponent<TransformComponent>(uid);
			ContainerSystem containerSys = entityManager.EntitySysManager.GetEntitySystem<ContainerSystem>();
			foreach (IContainer container in containerManager.GetAllContainers())
			{
				containerSys.EmptyContainer(container, true, new EntityCoordinates?(transform.Coordinates), false, null);
			}
		}
	}
}
