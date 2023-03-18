using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.StatusEffect
{
	// Token: 0x02000156 RID: 342
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[NetworkedComponent]
	[Access(new Type[]
	{
		typeof(StatusEffectsSystem)
	})]
	public sealed class StatusEffectsComponent : Component
	{
		// Token: 0x040003F4 RID: 1012
		[ViewVariables]
		public Dictionary<string, StatusEffectState> ActiveEffects = new Dictionary<string, StatusEffectState>();

		// Token: 0x040003F5 RID: 1013
		[DataField("allowed", false, 1, true, false, null)]
		[Access]
		public List<string> AllowedEffects;
	}
}
