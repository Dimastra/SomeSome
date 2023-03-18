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
	// Token: 0x02000602 RID: 1538
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	public sealed class WirePanel : IGraphCondition
	{
		// Token: 0x170004EE RID: 1262
		// (get) Token: 0x06002108 RID: 8456 RVA: 0x000AD7AF File Offset: 0x000AB9AF
		// (set) Token: 0x06002109 RID: 8457 RVA: 0x000AD7B7 File Offset: 0x000AB9B7
		[DataField("open", false, 1, false, false, null)]
		public bool Open { get; private set; } = true;

		// Token: 0x0600210A RID: 8458 RVA: 0x000AD7C0 File Offset: 0x000AB9C0
		public bool Condition(EntityUid uid, IEntityManager entityManager)
		{
			WiresComponent wires;
			return !entityManager.TryGetComponent<WiresComponent>(uid, ref wires) || wires.IsPanelOpen == this.Open;
		}

		// Token: 0x0600210B RID: 8459 RVA: 0x000AD7E8 File Offset: 0x000AB9E8
		public bool DoExamine(ExaminedEvent args)
		{
			EntityUid entity = args.Examined;
			WiresComponent wires;
			if (!IoCManager.Resolve<IEntityManager>().TryGetComponent<WiresComponent>(entity, ref wires))
			{
				return false;
			}
			if (this.Open)
			{
				if (!wires.IsPanelOpen)
				{
					args.PushMarkup(Loc.GetString("construction-examine-condition-wire-panel-open"));
					return true;
				}
			}
			else if (wires.IsPanelOpen)
			{
				args.PushMarkup(Loc.GetString("construction-examine-condition-wire-panel-close"));
				return true;
			}
			return false;
		}

		// Token: 0x0600210C RID: 8460 RVA: 0x000AD84A File Offset: 0x000ABA4A
		public IEnumerable<ConstructionGuideEntry> GenerateGuideEntry()
		{
			yield return new ConstructionGuideEntry
			{
				Localization = (this.Open ? "construction-step-condition-wire-panel-open" : "construction-step-condition-wire-panel-close")
			};
			yield break;
		}
	}
}
