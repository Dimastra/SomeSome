using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.PowerCell.Components
{
	// Token: 0x02000252 RID: 594
	[RegisterComponent]
	public sealed class PowerCellSlotComponent : Component
	{
		// Token: 0x040006AC RID: 1708
		[Nullable(1)]
		[DataField("cellSlotId", false, 1, true, false, null)]
		public string CellSlotId = string.Empty;

		// Token: 0x040006AD RID: 1709
		[DataField("fitsInCharger", false, 1, false, false, null)]
		public bool FitsInCharger = true;
	}
}
