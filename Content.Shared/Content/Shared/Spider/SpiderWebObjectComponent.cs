using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;

namespace Content.Shared.Spider
{
	// Token: 0x02000178 RID: 376
	[RegisterComponent]
	[NetworkedComponent]
	[Access(new Type[]
	{
		typeof(SharedSpiderSystem)
	})]
	public sealed class SpiderWebObjectComponent : Component
	{
	}
}
