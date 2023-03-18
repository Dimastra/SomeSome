using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Destructible.Thresholds.Behaviors
{
	// Token: 0x020005A7 RID: 1447
	[DataDefinition]
	public sealed class EmptyAllContainersBehaviour : IThresholdBehavior
	{
		// Token: 0x06001E14 RID: 7700 RVA: 0x0009F318 File Offset: 0x0009D518
		[NullableContext(1)]
		public void Execute(EntityUid owner, DestructibleSystem system, EntityUid? cause = null)
		{
			ContainerManagerComponent containerManager;
			if (!system.EntityManager.TryGetComponent<ContainerManagerComponent>(owner, ref containerManager))
			{
				return;
			}
			foreach (IContainer container in containerManager.GetAllContainers())
			{
				ContainerHelpers.EmptyContainer(container, true, new EntityCoordinates?(system.EntityManager.GetComponent<TransformComponent>(owner).Coordinates), false, null);
			}
		}
	}
}
