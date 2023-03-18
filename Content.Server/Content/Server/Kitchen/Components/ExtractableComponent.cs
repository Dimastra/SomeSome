using System;
using System.Runtime.CompilerServices;
using Content.Server.Kitchen.EntitySystems;
using Content.Shared.Chemistry.Components;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Kitchen.Components
{
	// Token: 0x02000433 RID: 1075
	[NullableContext(2)]
	[Nullable(0)]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(ReagentGrinderSystem)
	})]
	public sealed class ExtractableComponent : Component
	{
		// Token: 0x04000D9B RID: 3483
		[DataField("juiceSolution", false, 1, false, false, null)]
		public Solution JuiceSolution;

		// Token: 0x04000D9C RID: 3484
		[DataField("grindableSolutionName", false, 1, false, false, null)]
		public string GrindableSolution;
	}
}
