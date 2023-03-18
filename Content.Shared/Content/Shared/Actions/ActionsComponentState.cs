using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Actions.ActionTypes;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Actions
{
	// Token: 0x02000761 RID: 1889
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class ActionsComponentState : ComponentState
	{
		// Token: 0x0600172F RID: 5935 RVA: 0x0004AEE4 File Offset: 0x000490E4
		public ActionsComponentState(List<ActionType> actions)
		{
			this.Actions = actions;
		}

		// Token: 0x0400170A RID: 5898
		public readonly List<ActionType> Actions;
	}
}
