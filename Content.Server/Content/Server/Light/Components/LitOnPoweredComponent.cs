using System;
using Content.Server.Light.EntitySystems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;

namespace Content.Server.Light.Components
{
	// Token: 0x0200041C RID: 1052
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(PoweredLightSystem)
	})]
	public sealed class LitOnPoweredComponent : Component
	{
	}
}
