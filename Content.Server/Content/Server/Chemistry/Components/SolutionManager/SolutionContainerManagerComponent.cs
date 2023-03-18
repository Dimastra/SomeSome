using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Components;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Chemistry.Components.SolutionManager
{
	// Token: 0x020006B9 RID: 1721
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(SolutionContainerSystem)
	})]
	public sealed class SolutionContainerManagerComponent : Component
	{
		// Token: 0x04001626 RID: 5670
		[Nullable(1)]
		[DataField("solutions", false, 1, false, false, null)]
		[Access]
		public readonly Dictionary<string, Solution> Solutions = new Dictionary<string, Solution>();
	}
}
