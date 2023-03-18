using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Construction;
using Content.Shared.Examine;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Construction.Conditions
{
	// Token: 0x020005F5 RID: 1525
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	public sealed class AllConditions : IGraphCondition
	{
		// Token: 0x170004D7 RID: 1239
		// (get) Token: 0x060020B4 RID: 8372 RVA: 0x000AC99E File Offset: 0x000AAB9E
		[DataField("conditions", false, 1, false, false, null)]
		public IGraphCondition[] Conditions { get; } = Array.Empty<IGraphCondition>();

		// Token: 0x060020B5 RID: 8373 RVA: 0x000AC9A8 File Offset: 0x000AABA8
		public bool Condition(EntityUid uid, IEntityManager entityManager)
		{
			IGraphCondition[] conditions = this.Conditions;
			for (int i = 0; i < conditions.Length; i++)
			{
				if (!conditions[i].Condition(uid, entityManager))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x060020B6 RID: 8374 RVA: 0x000AC9DC File Offset: 0x000AABDC
		public bool DoExamine(ExaminedEvent args)
		{
			bool ret = false;
			foreach (IGraphCondition condition in this.Conditions)
			{
				ret |= condition.DoExamine(args);
			}
			return ret;
		}

		// Token: 0x060020B7 RID: 8375 RVA: 0x000ACA0F File Offset: 0x000AAC0F
		public IEnumerable<ConstructionGuideEntry> GenerateGuideEntry()
		{
			foreach (IGraphCondition condition in this.Conditions)
			{
				foreach (ConstructionGuideEntry entry in condition.GenerateGuideEntry())
				{
					yield return entry;
				}
				IEnumerator<ConstructionGuideEntry> enumerator = null;
			}
			IGraphCondition[] array = null;
			yield break;
			yield break;
		}
	}
}
