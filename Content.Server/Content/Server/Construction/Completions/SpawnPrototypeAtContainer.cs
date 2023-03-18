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
	// Token: 0x02000625 RID: 1573
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	public sealed class SpawnPrototypeAtContainer : IGraphAction
	{
		// Token: 0x17000514 RID: 1300
		// (get) Token: 0x06002181 RID: 8577 RVA: 0x000AEC47 File Offset: 0x000ACE47
		[DataField("prototype", false, 1, false, false, null)]
		public string Prototype { get; } = string.Empty;

		// Token: 0x17000515 RID: 1301
		// (get) Token: 0x06002182 RID: 8578 RVA: 0x000AEC4F File Offset: 0x000ACE4F
		[DataField("container", false, 1, false, false, null)]
		public string Container { get; } = string.Empty;

		// Token: 0x17000516 RID: 1302
		// (get) Token: 0x06002183 RID: 8579 RVA: 0x000AEC57 File Offset: 0x000ACE57
		[DataField("amount", false, 1, false, false, null)]
		public int Amount { get; } = 1;

		// Token: 0x06002184 RID: 8580 RVA: 0x000AEC60 File Offset: 0x000ACE60
		public void PerformAction(EntityUid uid, EntityUid? userUid, IEntityManager entityManager)
		{
			if (string.IsNullOrEmpty(this.Container) || string.IsNullOrEmpty(this.Prototype))
			{
				return;
			}
			Container container = entityManager.EntitySysManager.GetEntitySystem<ContainerSystem>().EnsureContainer<Container>(uid, this.Container, null);
			EntityCoordinates coordinates = entityManager.GetComponent<TransformComponent>(uid).Coordinates;
			for (int i = 0; i < this.Amount; i++)
			{
				container.Insert(entityManager.SpawnEntity(this.Prototype, coordinates), null, null, null, null, null);
			}
		}
	}
}
