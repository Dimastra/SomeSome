using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Weapons.Ranged.Components
{
	// Token: 0x02000065 RID: 101
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class HitscanBatteryAmmoProviderComponent : BatteryAmmoProviderComponent
	{
		// Token: 0x04000140 RID: 320
		[Nullable(1)]
		[ViewVariables]
		[DataField("proto", false, 1, true, false, typeof(PrototypeIdSerializer<HitscanPrototype>))]
		public string Prototype;
	}
}
