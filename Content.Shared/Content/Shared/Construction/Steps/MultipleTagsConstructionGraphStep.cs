using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Tag;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Construction.Steps
{
	// Token: 0x02000575 RID: 1397
	public sealed class MultipleTagsConstructionGraphStep : ArbitraryInsertConstructionGraphStep
	{
		// Token: 0x06001117 RID: 4375 RVA: 0x00038521 File Offset: 0x00036721
		[NullableContext(2)]
		private static bool IsNullOrEmpty<T>([Nullable(new byte[]
		{
			2,
			1
		})] ICollection<T> list)
		{
			return list == null || list.Count == 0;
		}

		// Token: 0x06001118 RID: 4376 RVA: 0x00038534 File Offset: 0x00036734
		[NullableContext(1)]
		public override bool EntityValid(EntityUid uid, IEntityManager entityManager, IComponentFactory compFactory)
		{
			if (MultipleTagsConstructionGraphStep.IsNullOrEmpty<string>(this._allTags) && MultipleTagsConstructionGraphStep.IsNullOrEmpty<string>(this._anyTags))
			{
				return false;
			}
			TagComponent tags;
			if (!entityManager.TryGetComponent<TagComponent>(uid, ref tags))
			{
				return false;
			}
			TagSystem tagSystem = entityManager.EntitySysManager.GetEntitySystem<TagSystem>();
			return (this._allTags == null || tagSystem.HasAllTags(tags, this._allTags)) && (this._anyTags == null || tagSystem.HasAnyTag(tags, this._anyTags));
		}

		// Token: 0x04000FE1 RID: 4065
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[DataField("allTags", false, 1, false, false, null)]
		private List<string> _allTags;

		// Token: 0x04000FE2 RID: 4066
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[DataField("anyTags", false, 1, false, false, null)]
		private List<string> _anyTags;
	}
}
