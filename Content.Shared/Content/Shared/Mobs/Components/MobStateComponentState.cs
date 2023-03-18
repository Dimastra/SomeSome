using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Mobs.Components
{
	// Token: 0x02000304 RID: 772
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class MobStateComponentState : ComponentState
	{
		// Token: 0x060008EB RID: 2283 RVA: 0x0001E2CB File Offset: 0x0001C4CB
		public MobStateComponentState(MobState currentState, HashSet<MobState> allowedStates)
		{
			this.CurrentState = currentState;
			this.AllowedStates = allowedStates;
		}

		// Token: 0x040008CF RID: 2255
		public readonly MobState CurrentState;

		// Token: 0x040008D0 RID: 2256
		public readonly HashSet<MobState> AllowedStates;
	}
}
