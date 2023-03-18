using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Server.NPC.HTN.PrimitiveTasks.Operators.Ranged
{
	// Token: 0x0200035B RID: 859
	public sealed class PickRangedTargetOperator : NPCCombatOperator
	{
		// Token: 0x1700027F RID: 639
		// (get) Token: 0x060011DA RID: 4570 RVA: 0x0005E0C4 File Offset: 0x0005C2C4
		protected override bool IsRanged
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060011DB RID: 4571 RVA: 0x0005E0C8 File Offset: 0x0005C2C8
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
			return rating + 1f / distance * 4f;
		}
	}
}
