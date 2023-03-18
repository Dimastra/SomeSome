using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.Construction;
using Robust.Server.Containers;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Construction.Completions
{
	// Token: 0x02000614 RID: 1556
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	public sealed class DeleteEntitiesInContainer : IGraphAction
	{
		// Token: 0x17000502 RID: 1282
		// (get) Token: 0x06002146 RID: 8518 RVA: 0x000AE4DD File Offset: 0x000AC6DD
		[DataField("container", false, 1, false, false, null)]
		public string Container { get; } = string.Empty;

		// Token: 0x06002147 RID: 8519 RVA: 0x000AE4E8 File Offset: 0x000AC6E8
		public void PerformAction(EntityUid uid, EntityUid? userUid, IEntityManager entityManager)
		{
			if (string.IsNullOrEmpty(this.Container))
			{
				return;
			}
			IContainer container;
			if (!entityManager.EntitySysManager.GetEntitySystem<ContainerSystem>().TryGetContainer(uid, this.Container, ref container, null))
			{
				return;
			}
			foreach (EntityUid contained in container.ContainedEntities.ToArray<EntityUid>())
			{
				if (container.Remove(contained, null, null, null, true, false, null, null))
				{
					entityManager.QueueDeleteEntity(contained);
				}
			}
		}
	}
}
