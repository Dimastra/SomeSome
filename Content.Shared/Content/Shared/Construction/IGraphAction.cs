using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Construction
{
	// Token: 0x0200056A RID: 1386
	[NullableContext(1)]
	[ImplicitDataDefinitionForInheritors]
	public interface IGraphAction
	{
		// Token: 0x060010EE RID: 4334
		void PerformAction(EntityUid uid, EntityUid? userUid, IEntityManager entityManager);
	}
}
