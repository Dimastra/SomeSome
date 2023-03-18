using System;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Weapons.Ranged.Components
{
	// Token: 0x0200005F RID: 95
	public abstract class BatteryAmmoProviderComponent : AmmoProviderComponent
	{
		// Token: 0x04000122 RID: 290
		[DataField("fireCost", false, 1, false, false, null)]
		public float FireCost = 100f;

		// Token: 0x04000123 RID: 291
		[ViewVariables]
		public int Shots;

		// Token: 0x04000124 RID: 292
		[ViewVariables]
		public int Capacity;
	}
}
