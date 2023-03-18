using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Construction.Steps
{
	// Token: 0x02000573 RID: 1395
	[NullableContext(1)]
	[Nullable(0)]
	[ImplicitDataDefinitionForInheritors]
	public abstract class EntityInsertConstructionGraphStep : ConstructionGraphStep
	{
		// Token: 0x17000361 RID: 865
		// (get) Token: 0x0600110D RID: 4365 RVA: 0x00038369 File Offset: 0x00036569
		[DataField("store", false, 1, false, false, null)]
		public string Store { get; } = string.Empty;

		// Token: 0x0600110E RID: 4366
		public abstract bool EntityValid(EntityUid uid, IEntityManager entityManager, IComponentFactory compFactory);
	}
}
