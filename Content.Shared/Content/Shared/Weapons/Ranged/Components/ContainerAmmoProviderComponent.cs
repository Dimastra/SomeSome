using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Weapons.Ranged.Components
{
	// Token: 0x02000061 RID: 97
	[RegisterComponent]
	public sealed class ContainerAmmoProviderComponent : AmmoProviderComponent
	{
		// Token: 0x04000125 RID: 293
		[Nullable(1)]
		[DataField("container", false, 1, true, false, null)]
		public string Container;
	}
}
