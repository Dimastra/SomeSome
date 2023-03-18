using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Construction;
using Content.Shared.Examine;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Construction.Conditions
{
	// Token: 0x020005F7 RID: 1527
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	public sealed class AnyConditions : IGraphCondition
	{
		// Token: 0x170004D9 RID: 1241
		// (get) Token: 0x060020BF RID: 8383 RVA: 0x000ACB16 File Offset: 0x000AAD16
		[DataField("conditions", false, 1, false, false, null)]
		public IGraphCondition[] Conditions { get; } = Array.Empty<IGraphCondition>();

		// Token: 0x060020C0 RID: 8384 RVA: 0x000ACB20 File Offset: 0x000AAD20
		public bool Condition(EntityUid uid, IEntityManager entityManager)
		{
			IGraphCondition[] conditions = this.Conditions;
			for (int i = 0; i < conditions.Length; i++)
			{
				if (conditions[i].Condition(uid, entityManager))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060020C1 RID: 8385 RVA: 0x000ACB54 File Offset: 0x000AAD54
		public bool DoExamine(ExaminedEvent args)
		{
			args.PushMarkup(Loc.GetString("construction-examine-condition-any-conditions"));
			IGraphCondition[] conditions = this.Conditions;
			for (int i = 0; i < conditions.Length; i++)
			{
				conditions[i].DoExamine(args);
			}
			return true;
		}

		// Token: 0x060020C2 RID: 8386 RVA: 0x000ACB91 File Offset: 0x000AAD91
		public IEnumerable<ConstructionGuideEntry> GenerateGuideEntry()
		{
			yield return new ConstructionGuideEntry
			{
				Localization = "construction-guide-condition-any-conditions"
			};
			foreach (IGraphCondition condition in this.Conditions)
			{
				foreach (ConstructionGuideEntry entry in condition.GenerateGuideEntry())
				{
					entry.Padding += 4;
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
