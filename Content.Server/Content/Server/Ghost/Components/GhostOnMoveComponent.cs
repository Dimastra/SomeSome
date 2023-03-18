using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Ghost.Components
{
	// Token: 0x020004A0 RID: 1184
	[RegisterComponent]
	public sealed class GhostOnMoveComponent : Component
	{
		// Token: 0x17000344 RID: 836
		// (get) Token: 0x060017CF RID: 6095 RVA: 0x0007C3C6 File Offset: 0x0007A5C6
		// (set) Token: 0x060017D0 RID: 6096 RVA: 0x0007C3CE File Offset: 0x0007A5CE
		[DataField("canReturn", false, 1, false, false, null)]
		public bool CanReturn { get; set; } = true;

		// Token: 0x04000EC3 RID: 3779
		[DataField("mustBeDead", false, 1, false, false, null)]
		public bool MustBeDead;
	}
}
