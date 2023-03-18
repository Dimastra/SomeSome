using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Mobs.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Mobs.Components
{
	// Token: 0x02000303 RID: 771
	[RegisterComponent]
	[NetworkedComponent]
	[Access(new Type[]
	{
		typeof(MobStateSystem),
		typeof(MobThresholdSystem)
	})]
	public sealed class MobStateComponent : Component
	{
		// Token: 0x170001AC RID: 428
		// (get) Token: 0x060008E8 RID: 2280 RVA: 0x0001E288 File Offset: 0x0001C488
		// (set) Token: 0x060008E9 RID: 2281 RVA: 0x0001E290 File Offset: 0x0001C490
		[ViewVariables]
		public MobState CurrentState { get; set; } = MobState.Alive;

		// Token: 0x040008CE RID: 2254
		[Nullable(1)]
		[DataField("allowedStates", false, 1, false, false, null)]
		public HashSet<MobState> AllowedStates = new HashSet<MobState>
		{
			MobState.Alive,
			MobState.Critical,
			MobState.Dead
		};
	}
}
