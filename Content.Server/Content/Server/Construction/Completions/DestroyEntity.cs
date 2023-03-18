using System;
using System.Runtime.CompilerServices;
using Content.Server.Destructible;
using Content.Shared.Construction;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Construction.Completions
{
	// Token: 0x02000617 RID: 1559
	[DataDefinition]
	public sealed class DestroyEntity : IGraphAction
	{
		// Token: 0x0600214C RID: 8524 RVA: 0x000AE5C9 File Offset: 0x000AC7C9
		[NullableContext(1)]
		public void PerformAction(EntityUid uid, EntityUid? userUid, IEntityManager entityManager)
		{
			entityManager.EntitySysManager.GetEntitySystem<DestructibleSystem>().DestroyEntity(uid);
		}
	}
}
