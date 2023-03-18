using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Mind;
using Content.Server.Mind.Components;
using Content.Server.Objectives.Interfaces;
using Content.Shared.Mobs.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Random;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Objectives.Conditions
{
	// Token: 0x020002FF RID: 767
	[DataDefinition]
	public sealed class KillRandomPersonCondition : KillPersonCondition
	{
		// Token: 0x06000FC4 RID: 4036 RVA: 0x00050724 File Offset: 0x0004E924
		[NullableContext(1)]
		public override IObjectiveCondition GetAssigned(Mind mind)
		{
			List<Mind> allHumans = (from mc in base.EntityManager.EntityQuery<MindComponent>(true).Where(delegate(MindComponent mc)
			{
				Mind mind2 = mc.Mind;
				EntityUid? entity = (mind2 != null) ? mind2.OwnedEntity : null;
				MobStateComponent mobState;
				return entity != null && (this.EntityManager.TryGetComponent<MobStateComponent>(entity, ref mobState) && this.MobStateSystem.IsAlive(entity.Value, mobState)) && mc.Mind != mind;
			})
			select mc.Mind).ToList<Mind>();
			if (allHumans.Count == 0)
			{
				return new DieCondition();
			}
			return new KillRandomPersonCondition
			{
				Target = RandomExtensions.Pick<Mind>(IoCManager.Resolve<IRobustRandom>(), allHumans)
			};
		}
	}
}
