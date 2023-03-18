using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Damage
{
	// Token: 0x0200052E RID: 1326
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class DamageableComponentState : ComponentState
	{
		// Token: 0x0600100C RID: 4108 RVA: 0x00033ABF File Offset: 0x00031CBF
		public DamageableComponentState(Dictionary<string, FixedPoint2> damageDict, [Nullable(2)] string modifierSetId)
		{
			this.DamageDict = damageDict;
			this.ModifierSetId = modifierSetId;
		}

		// Token: 0x04000F36 RID: 3894
		public readonly Dictionary<string, FixedPoint2> DamageDict;

		// Token: 0x04000F37 RID: 3895
		[Nullable(2)]
		public readonly string ModifierSetId;
	}
}
