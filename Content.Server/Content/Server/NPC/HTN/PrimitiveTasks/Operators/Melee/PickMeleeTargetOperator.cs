using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Server.NPC.HTN.PrimitiveTasks.Operators.Melee
{
	// Token: 0x0200035E RID: 862
	public sealed class PickMeleeTargetOperator : NPCCombatOperator
	{
		// Token: 0x060011E7 RID: 4583 RVA: 0x0005E414 File Offset: 0x0005C614
		[NullableContext(1)]
		protected override float GetRating(NPCBlackboard blackboard, EntityUid uid, EntityUid existingTarget, float distance, bool canMove, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<TransformComponent> xformQuery)
		{
			float rating = 0f;
			if (existingTarget == uid)
			{
				rating += 2f;
			}
			if (distance > 0f)
			{
				rating += 50f / distance;
			}
			return rating;
		}
	}
}
