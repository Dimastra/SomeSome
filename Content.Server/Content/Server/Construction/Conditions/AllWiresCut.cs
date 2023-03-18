using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Wires;
using Content.Shared.Construction;
using Content.Shared.Examine;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Construction.Conditions
{
	// Token: 0x020005F6 RID: 1526
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	public sealed class AllWiresCut : IGraphCondition
	{
		// Token: 0x170004D8 RID: 1240
		// (get) Token: 0x060020B9 RID: 8377 RVA: 0x000ACA32 File Offset: 0x000AAC32
		// (set) Token: 0x060020BA RID: 8378 RVA: 0x000ACA3A File Offset: 0x000AAC3A
		[DataField("value", false, 1, false, false, null)]
		public bool Value { get; private set; } = true;

		// Token: 0x060020BB RID: 8379 RVA: 0x000ACA44 File Offset: 0x000AAC44
		public bool Condition(EntityUid uid, IEntityManager entityManager)
		{
			WiresComponent wires;
			if (!entityManager.TryGetComponent<WiresComponent>(uid, ref wires))
			{
				return true;
			}
			foreach (Wire wire in wires.WiresList)
			{
				if (this.Value)
				{
					if (wire.IsCut)
					{
						continue;
					}
				}
				else if (!wire.IsCut)
				{
					continue;
				}
				return false;
			}
			return true;
		}

		// Token: 0x060020BC RID: 8380 RVA: 0x000ACAC0 File Offset: 0x000AACC0
		public bool DoExamine(ExaminedEvent args)
		{
			if (this.Condition(args.Examined, IoCManager.Resolve<IEntityManager>()))
			{
				return false;
			}
			args.PushMarkup(Loc.GetString(this.Value ? "construction-examine-condition-all-wires-cut" : "construction-examine-condition-all-wires-intact"));
			return true;
		}

		// Token: 0x060020BD RID: 8381 RVA: 0x000ACAF7 File Offset: 0x000AACF7
		public IEnumerable<ConstructionGuideEntry> GenerateGuideEntry()
		{
			yield return new ConstructionGuideEntry
			{
				Localization = (this.Value ? "construction-guide-condition-all-wires-cut" : "construction-guide-condition-all-wires-intact")
			};
			yield break;
		}
	}
}
