using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.StatusEffect
{
	// Token: 0x02000159 RID: 345
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class StatusEffectsComponentState : ComponentState
	{
		// Token: 0x06000424 RID: 1060 RVA: 0x0001074B File Offset: 0x0000E94B
		public StatusEffectsComponentState(Dictionary<string, StatusEffectState> activeEffects, List<string> allowedEffects)
		{
			this.ActiveEffects = activeEffects;
			this.AllowedEffects = allowedEffects;
		}

		// Token: 0x040003F9 RID: 1017
		public Dictionary<string, StatusEffectState> ActiveEffects;

		// Token: 0x040003FA RID: 1018
		public List<string> AllowedEffects;
	}
}
