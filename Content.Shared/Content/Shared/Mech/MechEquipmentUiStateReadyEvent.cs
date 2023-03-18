using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Shared.Mech
{
	// Token: 0x02000314 RID: 788
	public sealed class MechEquipmentUiStateReadyEvent : EntityEventArgs
	{
		// Token: 0x04000906 RID: 2310
		[Nullable(1)]
		public Dictionary<EntityUid, BoundUserInterfaceState> States = new Dictionary<EntityUid, BoundUserInterfaceState>();
	}
}
