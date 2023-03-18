using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Actions.ActionTypes;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Actions
{
	// Token: 0x02000760 RID: 1888
	[NetworkedComponent]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(SharedActionsSystem)
	})]
	public sealed class ActionsComponent : Component
	{
		// Token: 0x170004DF RID: 1247
		// (get) Token: 0x0600172D RID: 5933 RVA: 0x0004AECE File Offset: 0x000490CE
		public override bool SendOnlyToOwner
		{
			get
			{
				return true;
			}
		}

		// Token: 0x04001709 RID: 5897
		[Nullable(1)]
		[ViewVariables]
		[Access]
		public SortedSet<ActionType> Actions = new SortedSet<ActionType>();
	}
}
