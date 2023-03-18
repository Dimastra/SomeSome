using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Construction;
using Content.Shared.Examine;
using Content.Shared.Lock;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Construction.Conditions
{
	// Token: 0x020005FE RID: 1534
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	public sealed class Locked : IGraphCondition
	{
		// Token: 0x170004EA RID: 1258
		// (get) Token: 0x060020F2 RID: 8434 RVA: 0x000AD17F File Offset: 0x000AB37F
		// (set) Token: 0x060020F3 RID: 8435 RVA: 0x000AD187 File Offset: 0x000AB387
		[DataField("locked", false, 1, false, false, null)]
		public bool IsLocked { get; private set; } = true;

		// Token: 0x060020F4 RID: 8436 RVA: 0x000AD190 File Offset: 0x000AB390
		public bool Condition(EntityUid uid, IEntityManager entityManager)
		{
			LockComponent lockcomp;
			return entityManager.TryGetComponent<LockComponent>(uid, ref lockcomp) && lockcomp.Locked == this.IsLocked;
		}

		// Token: 0x060020F5 RID: 8437 RVA: 0x000AD1B8 File Offset: 0x000AB3B8
		public bool DoExamine(ExaminedEvent args)
		{
			IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
			EntityUid entity = args.Examined;
			LockComponent lockcomp;
			if (!entityManager.TryGetComponent<LockComponent>(entity, ref lockcomp))
			{
				return false;
			}
			if (this.IsLocked)
			{
				if (!lockcomp.Locked)
				{
					args.PushMarkup(Loc.GetString("construction-examine-condition-lock"));
					return true;
				}
			}
			else if (lockcomp.Locked)
			{
				args.PushMarkup(Loc.GetString("construction-examine-condition-unlock"));
				return true;
			}
			return false;
		}

		// Token: 0x060020F6 RID: 8438 RVA: 0x000AD21A File Offset: 0x000AB41A
		public IEnumerable<ConstructionGuideEntry> GenerateGuideEntry()
		{
			yield return new ConstructionGuideEntry
			{
				Localization = (this.IsLocked ? "construction-step-condition-wire-panel-lock" : "construction-step-condition-wire-panel-unlock")
			};
			yield break;
		}
	}
}
