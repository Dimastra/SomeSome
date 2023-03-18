using System;
using System.Runtime.CompilerServices;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Chemistry.Components.SolutionManager
{
	// Token: 0x020006B8 RID: 1720
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class RefillableSolutionComponent : Component
	{
		// Token: 0x17000554 RID: 1364
		// (get) Token: 0x060023C8 RID: 9160 RVA: 0x000BA693 File Offset: 0x000B8893
		// (set) Token: 0x060023C9 RID: 9161 RVA: 0x000BA69B File Offset: 0x000B889B
		[ViewVariables]
		[DataField("solution", false, 1, false, false, null)]
		public string Solution { get; set; } = "default";

		// Token: 0x17000555 RID: 1365
		// (get) Token: 0x060023CA RID: 9162 RVA: 0x000BA6A4 File Offset: 0x000B88A4
		// (set) Token: 0x060023CB RID: 9163 RVA: 0x000BA6AC File Offset: 0x000B88AC
		[DataField("maxRefill", false, 1, false, false, null)]
		[ViewVariables]
		public FixedPoint2? MaxRefill { get; set; }
	}
}
