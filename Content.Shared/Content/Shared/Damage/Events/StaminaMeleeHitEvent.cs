using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Damage.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Damage.Events
{
	// Token: 0x02000540 RID: 1344
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class StaminaMeleeHitEvent : HandledEntityEventArgs
	{
		// Token: 0x06001068 RID: 4200 RVA: 0x00035CD4 File Offset: 0x00033ED4
		public StaminaMeleeHitEvent(List<StaminaComponent> hitList)
		{
			this.HitList = hitList;
		}

		// Token: 0x04000F69 RID: 3945
		public List<StaminaComponent> HitList;

		// Token: 0x04000F6A RID: 3946
		public float Multiplier = 1f;

		// Token: 0x04000F6B RID: 3947
		public float FlatModifier;
	}
}
