using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Mech
{
	// Token: 0x02000319 RID: 793
	[NetSerializable]
	[Serializable]
	public sealed class MechBoundUiState : BoundUserInterfaceState
	{
		// Token: 0x0400090B RID: 2315
		[Nullable(1)]
		public Dictionary<EntityUid, BoundUserInterfaceState> EquipmentStates = new Dictionary<EntityUid, BoundUserInterfaceState>();
	}
}
