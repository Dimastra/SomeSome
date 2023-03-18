using System;
using Content.Server.Body.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;

namespace Content.Server.Body.Components
{
	// Token: 0x02000712 RID: 1810
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(BrainSystem)
	})]
	public sealed class BrainComponent : Component
	{
	}
}
