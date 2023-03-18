using System;
using System.Runtime.CompilerServices;
using Content.Shared.Power;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Power.Components
{
	// Token: 0x020002B3 RID: 691
	[RegisterComponent]
	public sealed class ChargerComponent : Component
	{
		// Token: 0x0400083C RID: 2108
		[ViewVariables]
		public CellChargerStatus Status;

		// Token: 0x0400083D RID: 2109
		[DataField("chargeRate", false, 1, false, false, null)]
		public int ChargeRate = 20;

		// Token: 0x0400083E RID: 2110
		[Nullable(1)]
		[DataField("slotId", false, 1, true, false, null)]
		public string SlotId = string.Empty;
	}
}
