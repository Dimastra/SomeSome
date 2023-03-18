using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Tabletop.Components
{
	// Token: 0x02000134 RID: 308
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(TabletopSystem)
	})]
	public sealed class TabletopGamerComponent : Component
	{
		// Token: 0x17000101 RID: 257
		// (get) Token: 0x060005A1 RID: 1441 RVA: 0x0001BD3A File Offset: 0x00019F3A
		// (set) Token: 0x060005A2 RID: 1442 RVA: 0x0001BD42 File Offset: 0x00019F42
		[DataField("tabletop", false, 1, false, false, null)]
		public EntityUid Tabletop { get; set; } = EntityUid.Invalid;
	}
}
