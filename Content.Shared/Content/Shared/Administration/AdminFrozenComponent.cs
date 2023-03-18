using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;

namespace Content.Shared.Administration
{
	// Token: 0x02000732 RID: 1842
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(AdminFrozenSystem)
	})]
	[NetworkedComponent]
	public sealed class AdminFrozenComponent : Component
	{
	}
}
