using System;
using System.Runtime.CompilerServices;
using Content.Shared.Construction;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Construction.Completions
{
	// Token: 0x02000616 RID: 1558
	[DataDefinition]
	public sealed class DeleteEntity : IGraphAction
	{
		// Token: 0x0600214A RID: 8522 RVA: 0x000AE590 File Offset: 0x000AC790
		[NullableContext(1)]
		public void PerformAction(EntityUid uid, EntityUid? userUid, IEntityManager entityManager)
		{
			ConstructionBeforeDeleteEvent ev = new ConstructionBeforeDeleteEvent(userUid);
			entityManager.EventBus.RaiseLocalEvent<ConstructionBeforeDeleteEvent>(uid, ev, false);
			if (!ev.Cancelled)
			{
				entityManager.DeleteEntity(uid);
			}
		}
	}
}
