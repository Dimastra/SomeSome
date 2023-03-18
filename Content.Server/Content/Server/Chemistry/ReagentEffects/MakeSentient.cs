using System;
using Content.Server.Ghost.Roles.Components;
using Content.Server.Mind.Components;
using Content.Server.Speech.Components;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;

namespace Content.Server.Chemistry.ReagentEffects
{
	// Token: 0x02000667 RID: 1639
	public sealed class MakeSentient : ReagentEffect
	{
		// Token: 0x06002273 RID: 8819 RVA: 0x000B3F40 File Offset: 0x000B2140
		public override void Effect(ReagentEffectArgs args)
		{
			IEntityManager entityManager = args.EntityManager;
			EntityUid uid = args.SolutionEntity;
			if (entityManager.HasComponent<MindComponent>(uid))
			{
				return;
			}
			entityManager.RemoveComponent<ReplacementAccentComponent>(uid);
			entityManager.RemoveComponent<MonkeyAccentComponent>(uid);
			GhostTakeoverAvailableComponent takeOver;
			if (entityManager.TryGetComponent<GhostTakeoverAvailableComponent>(uid, ref takeOver))
			{
				return;
			}
			takeOver = entityManager.AddComponent<GhostTakeoverAvailableComponent>(uid);
			MetaDataComponent entityData = entityManager.GetComponent<MetaDataComponent>(uid);
			takeOver.RoleName = entityData.EntityName;
			takeOver.RoleDescription = Loc.GetString("ghost-role-information-cognizine-description");
		}
	}
}
