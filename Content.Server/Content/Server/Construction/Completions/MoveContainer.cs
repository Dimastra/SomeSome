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
	// Token: 0x0200061C RID: 1564
	[NullableContext(2)]
	[Nullable(0)]
	[DataDefinition]
	public sealed class MoveContainer : IGraphAction
	{
		// Token: 0x17000506 RID: 1286
		// (get) Token: 0x0600215C RID: 8540 RVA: 0x000AE829 File Offset: 0x000ACA29
		[DataField("from", false, 1, false, false, null)]
		public string FromContainer { get; }

		// Token: 0x17000507 RID: 1287
		// (get) Token: 0x0600215D RID: 8541 RVA: 0x000AE831 File Offset: 0x000ACA31
		[DataField("to", false, 1, false, false, null)]
		public string ToContainer { get; }

		// Token: 0x0600215E RID: 8542 RVA: 0x000AE83C File Offset: 0x000ACA3C
		[NullableContext(1)]
		public void PerformAction(EntityUid uid, EntityUid? userUid, IEntityManager entityManager)
		{
			if (string.IsNullOrEmpty(this.FromContainer) || string.IsNullOrEmpty(this.ToContainer))
			{
				return;
			}
			ContainerSystem entitySystem = entityManager.EntitySysManager.GetEntitySystem<ContainerSystem>();
			ContainerManagerComponent containerManager = entityManager.EnsureComponent<ContainerManagerComponent>(uid);
			Container from = entitySystem.EnsureContainer<Container>(uid, this.FromContainer, containerManager);
			Container to = entitySystem.EnsureContainer<Container>(uid, this.ToContainer, containerManager);
			foreach (EntityUid contained in from.ContainedEntities.ToArray<EntityUid>())
			{
				if (from.Remove(contained, null, null, null, true, false, null, null))
				{
					to.Insert(contained, null, null, null, null, null);
				}
			}
		}
	}
}
