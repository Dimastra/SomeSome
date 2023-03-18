using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Power.Components;
using Content.Shared.Construction;
using Content.Shared.Examine;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Construction.Conditions
{
	// Token: 0x020005F8 RID: 1528
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	public sealed class ApcPanel : IGraphCondition
	{
		// Token: 0x170004DA RID: 1242
		// (get) Token: 0x060020C4 RID: 8388 RVA: 0x000ACBB4 File Offset: 0x000AADB4
		// (set) Token: 0x060020C5 RID: 8389 RVA: 0x000ACBBC File Offset: 0x000AADBC
		[DataField("open", false, 1, false, false, null)]
		public bool Open { get; private set; } = true;

		// Token: 0x060020C6 RID: 8390 RVA: 0x000ACBC8 File Offset: 0x000AADC8
		public bool Condition(EntityUid uid, IEntityManager entityManager)
		{
			ApcComponent apc;
			return !entityManager.TryGetComponent<ApcComponent>(uid, ref apc) || apc.IsApcOpen == this.Open;
		}

		// Token: 0x060020C7 RID: 8391 RVA: 0x000ACBF0 File Offset: 0x000AADF0
		public bool DoExamine(ExaminedEvent args)
		{
			EntityUid entity = args.Examined;
			ApcComponent apc;
			if (!IoCManager.Resolve<IEntityManager>().TryGetComponent<ApcComponent>(entity, ref apc))
			{
				return false;
			}
			if (this.Open)
			{
				if (!apc.IsApcOpen)
				{
					args.PushMarkup(Loc.GetString("construction-examine-condition-apc-open"));
					return true;
				}
			}
			else if (apc.IsApcOpen)
			{
				args.PushMarkup(Loc.GetString("construction-examine-condition-apc-close"));
				return true;
			}
			return false;
		}

		// Token: 0x060020C8 RID: 8392 RVA: 0x000ACC52 File Offset: 0x000AAE52
		public IEnumerable<ConstructionGuideEntry> GenerateGuideEntry()
		{
			yield return new ConstructionGuideEntry
			{
				Localization = (this.Open ? "construction-step-condition-apc-open" : "construction-step-condition-apc-close")
			};
			yield break;
		}
	}
}
