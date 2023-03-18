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
	// Token: 0x020005FB RID: 1531
	[NullableContext(2)]
	[Nullable(0)]
	[DataDefinition]
	public sealed class ContainerNotEmpty : IGraphCondition
	{
		// Token: 0x170004E4 RID: 1252
		// (get) Token: 0x060020DD RID: 8413 RVA: 0x000ACEB8 File Offset: 0x000AB0B8
		// (set) Token: 0x060020DE RID: 8414 RVA: 0x000ACEC0 File Offset: 0x000AB0C0
		[Nullable(1)]
		[DataField("container", false, 1, false, false, null)]
		public string Container { [NullableContext(1)] get; [NullableContext(1)] private set; } = string.Empty;

		// Token: 0x170004E5 RID: 1253
		// (get) Token: 0x060020DF RID: 8415 RVA: 0x000ACEC9 File Offset: 0x000AB0C9
		[DataField("examineText", false, 1, false, false, null)]
		public string ExamineText { get; }

		// Token: 0x170004E6 RID: 1254
		// (get) Token: 0x060020E0 RID: 8416 RVA: 0x000ACED1 File Offset: 0x000AB0D1
		[DataField("guideText", false, 1, false, false, null)]
		public string GuideText { get; }

		// Token: 0x170004E7 RID: 1255
		// (get) Token: 0x060020E1 RID: 8417 RVA: 0x000ACED9 File Offset: 0x000AB0D9
		[DataField("guideIcon", false, 1, false, false, null)]
		public SpriteSpecifier GuideIcon { get; }

		// Token: 0x060020E2 RID: 8418 RVA: 0x000ACEE4 File Offset: 0x000AB0E4
		[NullableContext(1)]
		public bool Condition(EntityUid uid, IEntityManager entityManager)
		{
			IContainer container;
			return entityManager.EntitySysManager.GetEntitySystem<ContainerSystem>().TryGetContainer(uid, this.Container, ref container, null) && container.ContainedEntities.Count != 0;
		}

		// Token: 0x060020E3 RID: 8419 RVA: 0x000ACF20 File Offset: 0x000AB120
		[NullableContext(1)]
		public bool DoExamine(ExaminedEvent args)
		{
			if (this.ExamineText == null)
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
			if (container.ContainedEntities.Count != 0)
			{
				return false;
			}
			args.PushMarkup(Loc.GetString(this.ExamineText));
			return true;
		}

		// Token: 0x060020E4 RID: 8420 RVA: 0x000ACF80 File Offset: 0x000AB180
		[NullableContext(1)]
		public IEnumerable<ConstructionGuideEntry> GenerateGuideEntry()
		{
			if (this.GuideText == null)
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
