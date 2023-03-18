using System;
using System.Runtime.CompilerServices;
using Content.Server.Stack;
using Content.Shared.Construction;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Construction.Completions
{
	// Token: 0x02000622 RID: 1570
	[DataDefinition]
	public sealed class SetStackCount : IGraphAction
	{
		// Token: 0x17000510 RID: 1296
		// (get) Token: 0x06002174 RID: 8564 RVA: 0x000AEAF5 File Offset: 0x000ACCF5
		[DataField("amount", false, 1, false, false, null)]
		public int Amount { get; } = 1;

		// Token: 0x06002175 RID: 8565 RVA: 0x000AEAFD File Offset: 0x000ACCFD
		[NullableContext(1)]
		public void PerformAction(EntityUid uid, EntityUid? userUid, IEntityManager entityManager)
		{
			entityManager.EntitySysManager.GetEntitySystem<StackSystem>().SetCount(uid, this.Amount, null);
		}
	}
}
