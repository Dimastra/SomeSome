using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Server.Zombies
{
	// Token: 0x02000018 RID: 24
	[RegisterComponent]
	public sealed class ActiveZombieComponent : Component
	{
		// Token: 0x04000029 RID: 41
		[ViewVariables]
		public float GroanChance = 0.2f;

		// Token: 0x0400002A RID: 42
		[ViewVariables]
		public float GroanCooldown = 2f;

		// Token: 0x0400002B RID: 43
		[ViewVariables]
		public float RandomGroanAttempt = 5f;

		// Token: 0x0400002C RID: 44
		[Nullable(1)]
		[ViewVariables]
		public string GroanEmoteId = "Scream";

		// Token: 0x0400002D RID: 45
		[ViewVariables]
		public float LastDamageGroanCooldown;

		// Token: 0x0400002E RID: 46
		[ViewVariables]
		public float Accumulator;
	}
}
