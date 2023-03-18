using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.RCD.Components
{
	// Token: 0x0200024D RID: 589
	[RegisterComponent]
	public sealed class RCDAmmoComponent : Component
	{
		// Token: 0x04000747 RID: 1863
		[ViewVariables]
		[DataField("refillAmmo", false, 1, false, false, null)]
		public int RefillAmmo = 5;
	}
}
