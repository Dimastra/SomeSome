using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Weapons.Ranged.Components
{
	// Token: 0x0200005A RID: 90
	[RegisterComponent]
	[NetworkedComponent]
	[ComponentReference(typeof(AmmoComponent))]
	public sealed class CartridgeAmmoComponent : AmmoComponent
	{
		// Token: 0x0400010C RID: 268
		[Nullable(1)]
		[ViewVariables]
		[DataField("proto", false, 1, true, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string Prototype;

		// Token: 0x0400010D RID: 269
		[ViewVariables]
		[DataField("spent", false, 1, false, false, null)]
		public bool Spent;

		// Token: 0x0400010E RID: 270
		[ViewVariables]
		[DataField("spread", false, 1, false, false, null)]
		public Angle Spread = Angle.FromDegrees(5.0);

		// Token: 0x0400010F RID: 271
		[ViewVariables]
		[DataField("count", false, 1, false, false, null)]
		public int Count = 1;

		// Token: 0x04000110 RID: 272
		[DataField("deleteOnSpawn", false, 1, false, false, null)]
		public bool DeleteOnSpawn;

		// Token: 0x04000111 RID: 273
		[Nullable(2)]
		[DataField("soundEject", false, 1, false, false, null)]
		public SoundSpecifier EjectSound = new SoundCollectionSpecifier("CasingEject", null);
	}
}
