using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Storage.Components;
using Content.Shared.Construction;
using Content.Shared.Examine;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Construction.Conditions
{
	// Token: 0x02000600 RID: 1536
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	public sealed class StorageWelded : IGraphCondition
	{
		// Token: 0x170004ED RID: 1261
		// (get) Token: 0x060020FE RID: 8446 RVA: 0x000AD61C File Offset: 0x000AB81C
		// (set) Token: 0x060020FF RID: 8447 RVA: 0x000AD624 File Offset: 0x000AB824
		[DataField("welded", false, 1, false, false, null)]
		public bool Welded { get; private set; } = true;

		// Token: 0x06002100 RID: 8448 RVA: 0x000AD630 File Offset: 0x000AB830
		public bool Condition(EntityUid uid, IEntityManager entityManager)
		{
			EntityStorageComponent entityStorageComponent;
			return entityManager.TryGetComponent<EntityStorageComponent>(uid, ref entityStorageComponent) && entityStorageComponent.IsWeldedShut == this.Welded;
		}

		// Token: 0x06002101 RID: 8449 RVA: 0x000AD658 File Offset: 0x000AB858
		public bool DoExamine(ExaminedEvent args)
		{
			IEntityManager entMan = IoCManager.Resolve<IEntityManager>();
			EntityUid entity = args.Examined;
			EntityStorageComponent entityStorage;
			if (!entMan.TryGetComponent<EntityStorageComponent>(entity, ref entityStorage))
			{
				return false;
			}
			MetaDataComponent metaData = entMan.GetComponent<MetaDataComponent>(entity);
			if (entityStorage.IsWeldedShut != this.Welded)
			{
				if (this.Welded)
				{
					args.PushMarkup(Loc.GetString("construction-examine-condition-door-weld", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("entityName", metaData.EntityName)
					}) + "\n");
				}
				else
				{
					args.PushMarkup(Loc.GetString("construction-examine-condition-door-unweld", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("entityName", metaData.EntityName)
					}) + "\n");
				}
				return true;
			}
			return false;
		}

		// Token: 0x06002102 RID: 8450 RVA: 0x000AD710 File Offset: 0x000AB910
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
