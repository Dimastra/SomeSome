using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Weapons.Ranged.Components
{
	// Token: 0x02000066 RID: 102
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class ProjectileBatteryAmmoProviderComponent : BatteryAmmoProviderComponent
	{
		// Token: 0x04000141 RID: 321
		[Nullable(1)]
		[ViewVariables]
		[DataField("proto", false, 1, true, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string Prototype;
	}
}
