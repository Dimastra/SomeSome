using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Power.Components
{
	// Token: 0x020002AE RID: 686
	[RegisterComponent]
	public sealed class BatterySelfRechargerComponent : Component
	{
		// Token: 0x170001E8 RID: 488
		// (get) Token: 0x06000DE8 RID: 3560 RVA: 0x00047AFD File Offset: 0x00045CFD
		// (set) Token: 0x06000DE9 RID: 3561 RVA: 0x00047B05 File Offset: 0x00045D05
		[ViewVariables]
		[DataField("autoRecharge", false, 1, false, false, null)]
		public bool AutoRecharge { get; set; }

		// Token: 0x170001E9 RID: 489
		// (get) Token: 0x06000DEA RID: 3562 RVA: 0x00047B0E File Offset: 0x00045D0E
		// (set) Token: 0x06000DEB RID: 3563 RVA: 0x00047B16 File Offset: 0x00045D16
		[ViewVariables]
		[DataField("autoRechargeRate", false, 1, false, false, null)]
		public float AutoRechargeRate { get; set; }
	}
}
