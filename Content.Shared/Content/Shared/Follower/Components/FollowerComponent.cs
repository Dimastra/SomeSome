using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;

namespace Content.Shared.Follower.Components
{
	// Token: 0x0200047B RID: 1147
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(FollowerSystem)
	})]
	[NetworkedComponent]
	public sealed class FollowerComponent : Component
	{
		// Token: 0x04000D2A RID: 3370
		public EntityUid Following;
	}
}
