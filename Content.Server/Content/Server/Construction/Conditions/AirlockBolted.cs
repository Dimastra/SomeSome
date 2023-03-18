using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Construction;
using Content.Shared.Doors.Components;
using Content.Shared.Examine;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Construction.Conditions
{
	// Token: 0x020005F4 RID: 1524
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	public sealed class AirlockBolted : IGraphCondition
	{
		// Token: 0x170004D6 RID: 1238
		// (get) Token: 0x060020AE RID: 8366 RVA: 0x000AC887 File Offset: 0x000AAA87
		// (set) Token: 0x060020AF RID: 8367 RVA: 0x000AC88F File Offset: 0x000AAA8F
		[DataField("value", false, 1, false, false, null)]
		public bool Value { get; private set; } = true;

		// Token: 0x060020B0 RID: 8368 RVA: 0x000AC898 File Offset: 0x000AAA98
		public bool Condition(EntityUid uid, IEntityManager entityManager)
		{
			AirlockComponent airlock;
			return !entityManager.TryGetComponent<AirlockComponent>(uid, ref airlock) || airlock.BoltsDown == this.Value;
		}

		// Token: 0x060020B1 RID: 8369 RVA: 0x000AC8C0 File Offset: 0x000AAAC0
		public bool DoExamine(ExaminedEvent args)
		{
			EntityUid entity = args.Examined;
			IEntityManager entMan = IoCManager.Resolve<IEntityManager>();
			AirlockComponent airlock;
			if (!entMan.TryGetComponent<AirlockComponent>(entity, ref airlock))
			{
				return false;
			}
			if (airlock.BoltsDown != this.Value)
			{
				if (this.Value)
				{
					args.PushMarkup(Loc.GetString("construction-examine-condition-airlock-bolt", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("entityName", entMan.GetComponent<MetaDataComponent>(entity).EntityName)
					}) + "\n");
				}
				else
				{
					args.PushMarkup(Loc.GetString("construction-examine-condition-airlock-unbolt", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("entityName", entMan.GetComponent<MetaDataComponent>(entity).EntityName)
					}) + "\n");
				}
				return true;
			}
			return false;
		}

		// Token: 0x060020B2 RID: 8370 RVA: 0x000AC97F File Offset: 0x000AAB7F
		public IEnumerable<ConstructionGuideEntry> GenerateGuideEntry()
		{
			yield return new ConstructionGuideEntry
			{
				Localization = (this.Value ? "construction-step-condition-airlock-bolt" : "construction-step-condition-airlock-unbolt")
			};
			yield break;
		}
	}
}
