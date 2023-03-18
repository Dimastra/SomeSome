using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;

namespace Content.Shared.Follower.Components
{
	// Token: 0x0200047A RID: 1146
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(FollowerSystem)
	})]
	[NetworkedComponent]
	public sealed class FollowedComponent : Component
	{
		// Token: 0x04000D29 RID: 3369
		[Nullable(1)]
		public HashSet<EntityUid> Following = new HashSet<EntityUid>();
	}
}
