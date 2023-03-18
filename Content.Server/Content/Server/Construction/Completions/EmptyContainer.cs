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
	// Token: 0x02000619 RID: 1561
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	public sealed class EmptyContainer : IGraphAction
	{
		// Token: 0x17000503 RID: 1283
		// (get) Token: 0x06002150 RID: 8528 RVA: 0x000AE674 File Offset: 0x000AC874
		// (set) Token: 0x06002151 RID: 8529 RVA: 0x000AE67C File Offset: 0x000AC87C
		[DataField("container", false, 1, false, false, null)]
		public string Container { get; private set; } = string.Empty;

		// Token: 0x06002152 RID: 8530 RVA: 0x000AE688 File Offset: 0x000AC888
		public void PerformAction(EntityUid uid, EntityUid? userUid, IEntityManager entityManager)
		{
			ContainerManagerComponent containerManager;
			IContainer container;
			if (!entityManager.TryGetComponent<ContainerManagerComponent>(uid, ref containerManager) || !containerManager.TryGetContainer(this.Container, ref container))
			{
				return;
			}
			SharedContainerSystem entitySystem = entityManager.EntitySysManager.GetEntitySystem<ContainerSystem>();
			TransformComponent transform = entityManager.GetComponent<TransformComponent>(uid);
			entitySystem.EmptyContainer(container, true, new EntityCoordinates?(transform.Coordinates), true, null);
		}
	}
}
