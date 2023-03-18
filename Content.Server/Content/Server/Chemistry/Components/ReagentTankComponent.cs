using System;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Chemistry.Components
{
	// Token: 0x020006AA RID: 1706
	[RegisterComponent]
	public sealed class ReagentTankComponent : Component
	{
		// Token: 0x17000546 RID: 1350
		// (get) Token: 0x06002390 RID: 9104 RVA: 0x000B9B48 File Offset: 0x000B7D48
		// (set) Token: 0x06002391 RID: 9105 RVA: 0x000B9B50 File Offset: 0x000B7D50
		[DataField("transferAmount", false, 1, false, false, null)]
		[ViewVariables]
		public FixedPoint2 TransferAmount { get; set; } = FixedPoint2.New(10);

		// Token: 0x17000547 RID: 1351
		// (get) Token: 0x06002392 RID: 9106 RVA: 0x000B9B59 File Offset: 0x000B7D59
		// (set) Token: 0x06002393 RID: 9107 RVA: 0x000B9B61 File Offset: 0x000B7D61
		[DataField("tankType", false, 1, false, false, null)]
		[ViewVariables]
		public ReagentTankType TankType { get; set; }
	}
}
