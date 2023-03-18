using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Weapons.Ranged.Components
{
	// Token: 0x0200005D RID: 93
	[RegisterComponent]
	public sealed class BasicEntityAmmoProviderComponent : AmmoProviderComponent
	{
		// Token: 0x0400011D RID: 285
		[Nullable(1)]
		[ViewVariables]
		[DataField("proto", false, 1, true, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string Proto;

		// Token: 0x0400011E RID: 286
		[ViewVariables]
		[DataField("capacity", false, 1, false, false, null)]
		public int? Capacity;

		// Token: 0x0400011F RID: 287
		[ViewVariables]
		[DataField("count", false, 1, false, false, null)]
		public int? Count;
	}
}
