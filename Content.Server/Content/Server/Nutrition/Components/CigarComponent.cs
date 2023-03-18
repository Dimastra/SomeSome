using System;
using Content.Server.Nutrition.EntitySystems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;

namespace Content.Server.Nutrition.Components
{
	// Token: 0x02000317 RID: 791
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(SmokingSystem)
	})]
	public sealed class CigarComponent : Component
	{
	}
}
