using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Weapons.Melee
{
	// Token: 0x0200006F RID: 111
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class MeleeWeaponComponentState : ComponentState
	{
		// Token: 0x0600014F RID: 335 RVA: 0x00007259 File Offset: 0x00005459
		public MeleeWeaponComponentState(float attackRate, bool attacking, TimeSpan nextAttack, TimeSpan? windupStart, string clickAnimation, string wideAnimation, float range)
		{
			this.AttackRate = attackRate;
			this.Attacking = attacking;
			this.NextAttack = nextAttack;
			this.WindUpStart = windupStart;
			this.ClickAnimation = clickAnimation;
			this.WideAnimation = wideAnimation;
			this.Range = range;
		}

		// Token: 0x0400016D RID: 365
		public float AttackRate;

		// Token: 0x0400016E RID: 366
		public bool Attacking;

		// Token: 0x0400016F RID: 367
		public TimeSpan NextAttack;

		// Token: 0x04000170 RID: 368
		public TimeSpan? WindUpStart;

		// Token: 0x04000171 RID: 369
		public string ClickAnimation;

		// Token: 0x04000172 RID: 370
		public string WideAnimation;

		// Token: 0x04000173 RID: 371
		public float Range;
	}
}
