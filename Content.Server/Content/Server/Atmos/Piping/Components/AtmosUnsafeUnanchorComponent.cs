using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Atmos.Piping.Components
{
	// Token: 0x02000767 RID: 1895
	[RegisterComponent]
	public sealed class AtmosUnsafeUnanchorComponent : Component
	{
		// Token: 0x1700061B RID: 1563
		// (get) Token: 0x0600281D RID: 10269 RVA: 0x000D1F46 File Offset: 0x000D0146
		// (set) Token: 0x0600281E RID: 10270 RVA: 0x000D1F4E File Offset: 0x000D014E
		[ViewVariables]
		[DataField("enabled", false, 1, false, false, null)]
		public bool Enabled { get; set; } = true;
	}
}
