using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Examine;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Construction
{
	// Token: 0x0200056B RID: 1387
	[NullableContext(1)]
	[ImplicitDataDefinitionForInheritors]
	public interface IGraphCondition
	{
		// Token: 0x060010EF RID: 4335
		bool Condition(EntityUid uid, IEntityManager entityManager);

		// Token: 0x060010F0 RID: 4336
		bool DoExamine(ExaminedEvent args);

		// Token: 0x060010F1 RID: 4337
		IEnumerable<ConstructionGuideEntry> GenerateGuideEntry();
	}
}
