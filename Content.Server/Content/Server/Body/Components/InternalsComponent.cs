using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Body.Components
{
	// Token: 0x02000713 RID: 1811
	[RegisterComponent]
	public sealed class InternalsComponent : Component
	{
		// Token: 0x1700059F RID: 1439
		// (get) Token: 0x06002620 RID: 9760 RVA: 0x000C96A6 File Offset: 0x000C78A6
		// (set) Token: 0x06002621 RID: 9761 RVA: 0x000C96AE File Offset: 0x000C78AE
		[ViewVariables]
		public EntityUid? GasTankEntity { get; set; }

		// Token: 0x170005A0 RID: 1440
		// (get) Token: 0x06002622 RID: 9762 RVA: 0x000C96B7 File Offset: 0x000C78B7
		// (set) Token: 0x06002623 RID: 9763 RVA: 0x000C96BF File Offset: 0x000C78BF
		[ViewVariables]
		public EntityUid? BreathToolEntity { get; set; }

		// Token: 0x040017A0 RID: 6048
		[ViewVariables]
		[DataField("delay", false, 1, false, false, null)]
		public float Delay = 3f;
	}
}
