using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Dragon
{
	// Token: 0x020004D6 RID: 1238
	[NetSerializable]
	[Serializable]
	public sealed class DragonRiftComponentState : ComponentState
	{
		// Token: 0x04000E13 RID: 3603
		public DragonRiftState State;
	}
}
