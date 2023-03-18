using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Construction;
using Content.Shared.Examine;
using Robust.Server.Containers;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Server.Construction.Conditions
{
	// Token: 0x020005FA RID: 1530
	[NullableContext(2)]
	[Nullable(0)]
	[DataDefinition]
	public sealed class ContainerEmpty : IGraphCondition
	{
		// Token: 0x170004E0 RID: 1248
		// (get) Token: 0x060020D5 RID: 8405 RVA: 0x000ACDD2 File Offset: 0x000AAFD2
		[Nullable(1)]
		[DataField("container", false, 1, false, false, null)]
		public string Container { [NullableContext(1)] get; } = string.Empty;

		// Token: 0x170004E1 RID: 1249
		// (get) Token: 0x060020D6 RID: 8406 RVA: 0x000ACDDA File Offset: 0x000AAFDA
		[DataField("examineText", false, 1, false, false, null)]
		public string ExamineText { get; }

		// Token: 0x170004E2 RID: 1250
		// (get) Token: 0x060020D7 RID: 8407 RVA: 0x000ACDE2 File Offset: 0x000AAFE2
		[DataField("guideStep", false, 1, false, false, null)]
		public string GuideText { get; }

		// Token: 0x170004E3 RID: 1251
		// (get) Token: 0x060020D8 RID: 8408 RVA: 0x000ACDEA File Offset: 0x000AAFEA
		[DataField("guideIcon", false, 1, false, false, null)]
		public SpriteSpecifier GuideIcon { get; }

		// Token: 0x060020D9 RID: 8409 RVA: 0x000ACDF4 File Offset: 0x000AAFF4
		[NullableContext(1)]
		public bool Condition(EntityUid uid, IEntityManager entityManager)
		{
			IContainer container;
			return entityManager.EntitySysManager.GetEntitySystem<ContainerSystem>().TryGetContainer(uid, this.Container, ref container, null) && container.ContainedEntities.Count == 0;
		}

		// Token: 0x060020DA RID: 8410 RVA: 0x000ACE30 File Offset: 0x000AB030
		[NullableContext(1)]
		public bool DoExamine(ExaminedEvent args)
		{
			if (string.IsNullOrEmpty(this.ExamineText))
			{
				return false;
			}
			EntityUid entity = args.Examined;
			ContainerManagerComponent containerManager;
			IContainer container;
			if (!IoCManager.Resolve<IEntityManager>().TryGetComponent<ContainerManagerComponent>(entity, ref containerManager) || !containerManager.TryGetContainer(this.Container, ref container))
			{
				return false;
			}
			if (container.ContainedEntities.Count == 0)
			{
				return false;
			}
			args.PushMarkup(Loc.GetString(this.ExamineText));
			return true;
		}

		// Token: 0x060020DB RID: 8411 RVA: 0x000ACE95 File Offset: 0x000AB095
		[NullableContext(1)]
		public IEnumerable<ConstructionGuideEntry> GenerateGuideEntry()
		{
			if (string.IsNullOrEmpty(this.GuideText))
			{
				yield break;
			}
			yield return new ConstructionGuideEntry
			{
				Localization = this.GuideText,
				Icon = this.GuideIcon
			};
			yield break;
		}
	}
}
