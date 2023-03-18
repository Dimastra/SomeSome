using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;

namespace Content.Shared.Throwing
{
	// Token: 0x020000D7 RID: 215
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class ThrownItemComponent : Component
	{
		// Token: 0x17000075 RID: 117
		// (get) Token: 0x06000255 RID: 597 RVA: 0x0000B582 File Offset: 0x00009782
		// (set) Token: 0x06000256 RID: 598 RVA: 0x0000B58A File Offset: 0x0000978A
		public EntityUid? Thrower { get; set; }
	}
}
