using System;
using System.Runtime.CompilerServices;
using Content.Shared.Construction;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Construction.Completions
{
	// Token: 0x0200060D RID: 1549
	[NullableContext(2)]
	[Nullable(0)]
	[DataDefinition]
	public sealed class AddContainer : IGraphAction
	{
		// Token: 0x170004FC RID: 1276
		// (get) Token: 0x06002130 RID: 8496 RVA: 0x000ADA77 File Offset: 0x000ABC77
		// (set) Token: 0x06002131 RID: 8497 RVA: 0x000ADA7F File Offset: 0x000ABC7F
		[DataField("container", false, 1, false, false, null)]
		public string Container { get; private set; }

		// Token: 0x06002132 RID: 8498 RVA: 0x000ADA88 File Offset: 0x000ABC88
		[NullableContext(1)]
		public void PerformAction(EntityUid uid, EntityUid? userUid, IEntityManager entityManager)
		{
			if (string.IsNullOrEmpty(this.Container))
			{
				return;
			}
			entityManager.EntitySysManager.GetEntitySystem<ConstructionSystem>().AddContainer(uid, this.Container, null);
		}
	}
}
