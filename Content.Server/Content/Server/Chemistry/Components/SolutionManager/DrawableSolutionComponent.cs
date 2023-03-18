using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Chemistry.Components.SolutionManager
{
	// Token: 0x020006B5 RID: 1717
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class DrawableSolutionComponent : Component
	{
		// Token: 0x17000551 RID: 1361
		// (get) Token: 0x060023BF RID: 9151 RVA: 0x000BA627 File Offset: 0x000B8827
		// (set) Token: 0x060023C0 RID: 9152 RVA: 0x000BA62F File Offset: 0x000B882F
		[ViewVariables]
		[DataField("solution", false, 1, false, false, null)]
		public string Solution { get; set; } = "default";
	}
}
