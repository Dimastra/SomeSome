using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Mech.Components
{
	// Token: 0x02000329 RID: 809
	[NetSerializable]
	[Serializable]
	public sealed class MechPilotComponentState : ComponentState
	{
		// Token: 0x04000926 RID: 2342
		public EntityUid Mech;
	}
}
