using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Chemistry.Components.SolutionManager
{
	// Token: 0x020006B4 RID: 1716
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class DrainableSolutionComponent : Component
	{
		// Token: 0x17000550 RID: 1360
		// (get) Token: 0x060023BC RID: 9148 RVA: 0x000BA603 File Offset: 0x000B8803
		// (set) Token: 0x060023BD RID: 9149 RVA: 0x000BA60B File Offset: 0x000B880B
		[ViewVariables]
		[DataField("solution", false, 1, false, false, null)]
		public string Solution { get; set; } = "default";
	}
}
