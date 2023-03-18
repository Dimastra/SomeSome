using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Construction;
using Content.Shared.Examine;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Server.Construction.Conditions
{
	// Token: 0x020005FD RID: 1533
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	public sealed class EntityAnchored : IGraphCondition
	{
		// Token: 0x170004E9 RID: 1257
		// (get) Token: 0x060020EC RID: 8428 RVA: 0x000AD0B9 File Offset: 0x000AB2B9
		// (set) Token: 0x060020ED RID: 8429 RVA: 0x000AD0C1 File Offset: 0x000AB2C1
		[DataField("anchored", false, 1, false, false, null)]
		public bool Anchored { get; private set; } = true;

		// Token: 0x060020EE RID: 8430 RVA: 0x000AD0CC File Offset: 0x000AB2CC
		public bool Condition(EntityUid uid, IEntityManager entityManager)
		{
			TransformComponent transform = entityManager.GetComponent<TransformComponent>(uid);
			return (transform.Anchored && this.Anchored) || (!transform.Anchored && !this.Anchored);
		}

		// Token: 0x060020EF RID: 8431 RVA: 0x000AD108 File Offset: 0x000AB308
		public bool DoExamine(ExaminedEvent args)
		{
			EntityUid entity = args.Examined;
			bool anchored = IoCManager.Resolve<IEntityManager>().GetComponent<TransformComponent>(entity).Anchored;
			if (this.Anchored)
			{
				if (!anchored)
				{
					args.PushMarkup(Loc.GetString("construction-examine-condition-entity-anchored"));
					return true;
				}
			}
			else if (anchored)
			{
				args.PushMarkup(Loc.GetString("construction-examine-condition-entity-unanchored"));
				return true;
			}
			return false;
		}

		// Token: 0x060020F0 RID: 8432 RVA: 0x000AD160 File Offset: 0x000AB360
		public IEnumerable<ConstructionGuideEntry> GenerateGuideEntry()
		{
			yield return new ConstructionGuideEntry
			{
				Localization = (this.Anchored ? "construction-step-condition-entity-anchored" : "construction-step-condition-entity-unanchored"),
				Icon = new SpriteSpecifier.Rsi(new ResourcePath("Objects/Tools/wrench.rsi", "/"), "icon")
			};
			yield break;
		}
	}
}
