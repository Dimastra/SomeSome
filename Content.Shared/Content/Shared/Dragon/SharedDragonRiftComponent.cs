using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Dragon
{
	// Token: 0x020004D7 RID: 1239
	[NetworkedComponent]
	public abstract class SharedDragonRiftComponent : Component
	{
		// Token: 0x04000E14 RID: 3604
		[DataField("state", false, 1, false, false, null)]
		public DragonRiftState State;
	}
}
