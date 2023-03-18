using System;
using System.Runtime.CompilerServices;
using Content.Shared.Tag;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Construction.Steps
{
	// Token: 0x02000576 RID: 1398
	[DataDefinition]
	public sealed class TagConstructionGraphStep : ArbitraryInsertConstructionGraphStep
	{
		// Token: 0x0600111A RID: 4378 RVA: 0x000385B4 File Offset: 0x000367B4
		[NullableContext(1)]
		public override bool EntityValid(EntityUid uid, IEntityManager entityManager, IComponentFactory compFactory)
		{
			TagSystem tagSystem = entityManager.EntitySysManager.GetEntitySystem<TagSystem>();
			return !string.IsNullOrEmpty(this._tag) && tagSystem.HasTag(uid, this._tag);
		}

		// Token: 0x04000FE3 RID: 4067
		[Nullable(2)]
		[DataField("tag", false, 1, false, false, null)]
		private string _tag;
	}
}
