using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Chemistry.Components.SolutionManager
{
	// Token: 0x020006B6 RID: 1718
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class ExaminableSolutionComponent : Component
	{
		// Token: 0x17000552 RID: 1362
		// (get) Token: 0x060023C2 RID: 9154 RVA: 0x000BA64B File Offset: 0x000B884B
		// (set) Token: 0x060023C3 RID: 9155 RVA: 0x000BA653 File Offset: 0x000B8853
		[ViewVariables]
		[DataField("solution", false, 1, false, false, null)]
		public string Solution { get; set; } = "default";
	}
}
