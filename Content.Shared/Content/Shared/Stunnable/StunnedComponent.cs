using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;

namespace Content.Shared.Stunnable
{
	// Token: 0x02000111 RID: 273
	[Access(new Type[]
	{
		typeof(SharedStunSystem)
	})]
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class StunnedComponent : Component
	{
	}
}
