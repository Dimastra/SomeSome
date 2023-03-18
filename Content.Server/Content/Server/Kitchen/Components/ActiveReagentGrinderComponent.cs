using System;
using Content.Server.Kitchen.EntitySystems;
using Content.Shared.Kitchen;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Server.Kitchen.Components
{
	// Token: 0x02000438 RID: 1080
	[Access(new Type[]
	{
		typeof(ReagentGrinderSystem)
	})]
	[RegisterComponent]
	public sealed class ActiveReagentGrinderComponent : Component
	{
		// Token: 0x04000DBF RID: 3519
		[ViewVariables]
		public TimeSpan EndTime;

		// Token: 0x04000DC0 RID: 3520
		[ViewVariables]
		public GrinderProgram Program;
	}
}
