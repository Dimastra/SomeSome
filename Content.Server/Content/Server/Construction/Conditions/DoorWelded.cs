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
	// Token: 0x020005FC RID: 1532
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	public sealed class DoorWelded : IGraphCondition
	{
		// Token: 0x170004E8 RID: 1256
		// (get) Token: 0x060020E6 RID: 8422 RVA: 0x000ACFA3 File Offset: 0x000AB1A3
		// (set) Token: 0x060020E7 RID: 8423 RVA: 0x000ACFAB File Offset: 0x000AB1AB
		[DataField("welded", false, 1, false, false, null)]
		public bool Welded { get; private set; } = true;

		// Token: 0x060020E8 RID: 8424 RVA: 0x000ACFB4 File Offset: 0x000AB1B4
		public bool Condition(EntityUid uid, IEntityManager entityManager)
		{
			DoorComponent doorComponent;
			return entityManager.TryGetComponent<DoorComponent>(uid, ref doorComponent) && doorComponent.State == DoorState.Welded;
		}

		// Token: 0x060020E9 RID: 8425 RVA: 0x000ACFD8 File Offset: 0x000AB1D8
		public bool DoExamine(ExaminedEvent args)
		{
			EntityUid entity = args.Examined;
			IEntityManager entMan = IoCManager.Resolve<IEntityManager>();
			DoorComponent door;
			if (!entMan.TryGetComponent<DoorComponent>(entity, ref door))
			{
				return false;
			}
			if (door.State == DoorState.Welded != this.Welded)
			{
				if (this.Welded)
				{
					args.PushMarkup(Loc.GetString("construction-examine-condition-door-weld", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("entityName", entMan.GetComponent<MetaDataComponent>(entity).EntityName)
					}) + "\n");
				}
				else
				{
					args.PushMarkup(Loc.GetString("construction-examine-condition-door-unweld", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("entityName", entMan.GetComponent<MetaDataComponent>(entity).EntityName)
					}) + "\n");
				}
				return true;
			}
			return false;
		}

		// Token: 0x060020EA RID: 8426 RVA: 0x000AD09A File Offset: 0x000AB29A
		public IEnumerable<ConstructionGuideEntry> GenerateGuideEntry()
		{
			yield return new ConstructionGuideEntry
			{
				Localization = (this.Welded ? "construction-guide-condition-door-weld" : "construction-guide-condition-door-unweld")
			};
			yield break;
		}
	}
}
