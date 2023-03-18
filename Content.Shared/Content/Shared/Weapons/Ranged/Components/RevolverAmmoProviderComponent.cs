using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.Weapons.Ranged.Components
{
	// Token: 0x02000067 RID: 103
	[NullableContext(2)]
	[Nullable(0)]
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class RevolverAmmoProviderComponent : AmmoProviderComponent
	{
		// Token: 0x04000142 RID: 322
		[DataField("whitelist", false, 1, false, false, null)]
		public EntityWhitelist Whitelist;

		// Token: 0x04000143 RID: 323
		[Nullable(1)]
		public Container AmmoContainer;

		// Token: 0x04000144 RID: 324
		[DataField("currentSlot", false, 1, false, false, null)]
		public int CurrentIndex;

		// Token: 0x04000145 RID: 325
		[DataField("capacity", false, 1, false, false, null)]
		public int Capacity = 6;

		// Token: 0x04000146 RID: 326
		[Nullable(1)]
		[DataField("ammoSlots", false, 1, false, false, null)]
		public List<EntityUid?> AmmoSlots = new List<EntityUid?>();

		// Token: 0x04000147 RID: 327
		[Nullable(1)]
		[DataField("chambers", false, 1, false, false, null)]
		public bool?[] Chambers = Array.Empty<bool?>();

		// Token: 0x04000148 RID: 328
		[DataField("proto", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string FillPrototype = "CartridgeMagnum";

		// Token: 0x04000149 RID: 329
		[DataField("soundEject", false, 1, false, false, null)]
		public SoundSpecifier SoundEject = new SoundPathSpecifier("/Audio/Weapons/Guns/MagOut/revolver_magout.ogg", null);

		// Token: 0x0400014A RID: 330
		[DataField("soundInsert", false, 1, false, false, null)]
		public SoundSpecifier SoundInsert = new SoundPathSpecifier("/Audio/Weapons/Guns/MagIn/revolver_magin.ogg", null);

		// Token: 0x0400014B RID: 331
		[DataField("soundSpin", false, 1, false, false, null)]
		public SoundSpecifier SoundSpin = new SoundPathSpecifier("/Audio/Weapons/Guns/Misc/revolver_spin.ogg", null);
	}
}
