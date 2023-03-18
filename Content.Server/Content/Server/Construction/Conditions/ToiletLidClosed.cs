using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Construction;
using Content.Shared.Examine;
using Content.Shared.Toilet;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Construction.Conditions
{
	// Token: 0x02000601 RID: 1537
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	public sealed class ToiletLidClosed : IGraphCondition
	{
		// Token: 0x06002104 RID: 8452 RVA: 0x000AD730 File Offset: 0x000AB930
		public bool Condition(EntityUid uid, IEntityManager entityManager)
		{
			ToiletComponent toilet;
			return entityManager.TryGetComponent<ToiletComponent>(uid, ref toilet) && !toilet.LidOpen;
		}

		// Token: 0x06002105 RID: 8453 RVA: 0x000AD754 File Offset: 0x000AB954
		public bool DoExamine(ExaminedEvent args)
		{
			EntityUid entity = args.Examined;
			ToiletComponent toilet;
			if (!IoCManager.Resolve<IEntityManager>().TryGetComponent<ToiletComponent>(entity, ref toilet))
			{
				return false;
			}
			if (!toilet.LidOpen)
			{
				return false;
			}
			args.PushMarkup(Loc.GetString("construction-examine-condition-toilet-lid-closed") + "\n");
			return true;
		}

		// Token: 0x06002106 RID: 8454 RVA: 0x000AD79E File Offset: 0x000AB99E
		public IEnumerable<ConstructionGuideEntry> GenerateGuideEntry()
		{
			yield return new ConstructionGuideEntry
			{
				Localization = "construction-step-condition-toilet-lid-closed"
			};
			yield break;
		}
	}
}
