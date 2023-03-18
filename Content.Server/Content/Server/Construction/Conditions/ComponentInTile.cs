using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Construction;
using Content.Shared.Examine;
using Content.Shared.Maps;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Server.Construction.Conditions
{
	// Token: 0x020005F9 RID: 1529
	[NullableContext(2)]
	[Nullable(0)]
	[DataDefinition]
	public sealed class ComponentInTile : IGraphCondition
	{
		// Token: 0x170004DB RID: 1243
		// (get) Token: 0x060020CA RID: 8394 RVA: 0x000ACC71 File Offset: 0x000AAE71
		// (set) Token: 0x060020CB RID: 8395 RVA: 0x000ACC79 File Offset: 0x000AAE79
		[DataField("hasEntity", false, 1, false, false, null)]
		public bool HasEntity { get; private set; }

		// Token: 0x170004DC RID: 1244
		// (get) Token: 0x060020CC RID: 8396 RVA: 0x000ACC82 File Offset: 0x000AAE82
		[DataField("examineText", false, 1, false, false, null)]
		public string ExamineText { get; }

		// Token: 0x170004DD RID: 1245
		// (get) Token: 0x060020CD RID: 8397 RVA: 0x000ACC8A File Offset: 0x000AAE8A
		[DataField("guideText", false, 1, false, false, null)]
		public string GuideText { get; }

		// Token: 0x170004DE RID: 1246
		// (get) Token: 0x060020CE RID: 8398 RVA: 0x000ACC92 File Offset: 0x000AAE92
		[DataField("guideIcon", false, 1, false, false, null)]
		public SpriteSpecifier GuideIcon { get; }

		// Token: 0x170004DF RID: 1247
		// (get) Token: 0x060020CF RID: 8399 RVA: 0x000ACC9A File Offset: 0x000AAE9A
		// (set) Token: 0x060020D0 RID: 8400 RVA: 0x000ACCA2 File Offset: 0x000AAEA2
		[Nullable(1)]
		[DataField("component", false, 1, false, false, null)]
		public string Component { [NullableContext(1)] get; [NullableContext(1)] private set; } = string.Empty;

		// Token: 0x060020D1 RID: 8401 RVA: 0x000ACCAC File Offset: 0x000AAEAC
		[NullableContext(1)]
		public bool Condition(EntityUid uid, IEntityManager entityManager)
		{
			if (string.IsNullOrEmpty(this.Component))
			{
				return false;
			}
			Type type = IoCManager.Resolve<IComponentFactory>().GetRegistration(this.Component, false).Type;
			TransformComponent transform = entityManager.GetComponent<TransformComponent>(uid);
			if (transform.GridUid == null)
			{
				return false;
			}
			Vector2i indices = transform.Coordinates.ToVector2i(entityManager, IoCManager.Resolve<IMapManager>());
			EntityLookupSystem lookup = entityManager.EntitySysManager.GetEntitySystem<EntityLookupSystem>();
			foreach (EntityUid ent in indices.GetEntitiesInTile(transform.GridUid.Value, 5, lookup))
			{
				if (entityManager.HasComponent(ent, type))
				{
					return this.HasEntity;
				}
			}
			return !this.HasEntity;
		}

		// Token: 0x060020D2 RID: 8402 RVA: 0x000ACD8C File Offset: 0x000AAF8C
		[NullableContext(1)]
		public bool DoExamine(ExaminedEvent args)
		{
			if (string.IsNullOrEmpty(this.ExamineText))
			{
				return false;
			}
			args.PushMarkup(Loc.GetString(this.ExamineText));
			return true;
		}

		// Token: 0x060020D3 RID: 8403 RVA: 0x000ACDAF File Offset: 0x000AAFAF
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
