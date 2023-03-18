using System;
using System.Runtime.CompilerServices;
using Content.Shared.Construction.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Client.Construction
{
	// Token: 0x0200038D RID: 909
	[NullableContext(2)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class ConstructionGhostComponent : Component
	{
		// Token: 0x17000475 RID: 1141
		// (get) Token: 0x06001650 RID: 5712 RVA: 0x00083693 File Offset: 0x00081893
		// (set) Token: 0x06001651 RID: 5713 RVA: 0x0008369B File Offset: 0x0008189B
		[ViewVariables]
		public ConstructionPrototype Prototype { get; set; }

		// Token: 0x17000476 RID: 1142
		// (get) Token: 0x06001652 RID: 5714 RVA: 0x000836A4 File Offset: 0x000818A4
		// (set) Token: 0x06001653 RID: 5715 RVA: 0x000836AC File Offset: 0x000818AC
		[ViewVariables]
		public int GhostId { get; set; }
	}
}
