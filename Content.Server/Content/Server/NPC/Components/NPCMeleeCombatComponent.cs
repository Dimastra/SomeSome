using System;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Server.NPC.Components
{
	// Token: 0x0200036D RID: 877
	[RegisterComponent]
	public sealed class NPCMeleeCombatComponent : Component
	{
		// Token: 0x04000B04 RID: 2820
		[ViewVariables]
		public EntityUid Weapon;

		// Token: 0x04000B05 RID: 2821
		[ViewVariables]
		public float MissChance;

		// Token: 0x04000B06 RID: 2822
		[ViewVariables]
		public EntityUid Target;

		// Token: 0x04000B07 RID: 2823
		[ViewVariables]
		public CombatStatus Status = CombatStatus.Normal;
	}
}
