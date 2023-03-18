using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Weapons.Ranged.Components
{
	// Token: 0x0200006A RID: 106
	[NullableContext(2)]
	[Nullable(0)]
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class TwoModeEnergyAmmoProviderComponent : BatteryAmmoProviderComponent
	{
		// Token: 0x1700002D RID: 45
		// (get) Token: 0x06000140 RID: 320 RVA: 0x0000705A File Offset: 0x0000525A
		// (set) Token: 0x06000141 RID: 321 RVA: 0x00007062 File Offset: 0x00005262
		[ViewVariables]
		[DataField("currentMode", false, 1, false, false, null)]
		public EnergyModes CurrentMode { get; set; }

		// Token: 0x0400014C RID: 332
		[Nullable(1)]
		[ViewVariables]
		[DataField("projProto", false, 1, true, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string ProjectilePrototype;

		// Token: 0x0400014D RID: 333
		[Nullable(1)]
		[ViewVariables]
		[DataField("hitscanProto", false, 1, true, false, typeof(PrototypeIdSerializer<HitscanPrototype>))]
		public string HitscanPrototype;

		// Token: 0x0400014E RID: 334
		[ViewVariables]
		[DataField("projFireCost", false, 1, false, false, null)]
		public float ProjFireCost = 50f;

		// Token: 0x0400014F RID: 335
		[ViewVariables]
		[DataField("hitscanFireCost", false, 1, false, false, null)]
		public float HitscanFireCost = 100f;

		// Token: 0x04000151 RID: 337
		[ViewVariables]
		[DataField("projSound", false, 1, false, false, null)]
		public SoundSpecifier ProjSound = new SoundPathSpecifier("/Audio/Weapons/Guns/Gunshots/taser2.ogg", null);

		// Token: 0x04000152 RID: 338
		[ViewVariables]
		[DataField("hitscanSound", false, 1, false, false, null)]
		public SoundSpecifier HitscanSound = new SoundPathSpecifier("/Audio/Weapons/Guns/Gunshots/laser_cannon.ogg", null);

		// Token: 0x04000153 RID: 339
		public SoundSpecifier ToggleSound = new SoundPathSpecifier("/Audio/Weapons/Guns/Misc/egun_toggle.ogg", null);

		// Token: 0x04000154 RID: 340
		[ViewVariables]
		public bool InStun = true;
	}
}
