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
using Robust.Shared.ViewVariables;

namespace Content.Shared.Weapons.Ranged.Components
{
	// Token: 0x0200005C RID: 92
	[NullableContext(2)]
	[Nullable(0)]
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class BallisticAmmoProviderComponent : Component
	{
		// Token: 0x04000112 RID: 274
		[ViewVariables]
		[DataField("soundRack", false, 1, false, false, null)]
		public SoundSpecifier SoundRack = new SoundPathSpecifier("/Audio/Weapons/Guns/Cock/smg_cock.ogg", null);

		// Token: 0x04000113 RID: 275
		[ViewVariables]
		[DataField("soundInsert", false, 1, false, false, null)]
		public SoundSpecifier SoundInsert = new SoundPathSpecifier("/Audio/Weapons/Guns/MagIn/bullet_insert.ogg", null);

		// Token: 0x04000114 RID: 276
		[ViewVariables]
		[DataField("proto", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string FillProto;

		// Token: 0x04000115 RID: 277
		[ViewVariables]
		[DataField("capacity", false, 1, false, false, null)]
		public int Capacity = 30;

		// Token: 0x04000116 RID: 278
		[ViewVariables]
		[DataField("unspawnedCount", false, 1, false, false, null)]
		public int UnspawnedCount;

		// Token: 0x04000117 RID: 279
		[ViewVariables]
		[DataField("whitelist", false, 1, false, false, null)]
		public EntityWhitelist Whitelist;

		// Token: 0x04000118 RID: 280
		[Nullable(1)]
		public Container Container;

		// Token: 0x04000119 RID: 281
		[Nullable(1)]
		[DataField("entities", false, 1, false, false, null)]
		public List<EntityUid> Entities = new List<EntityUid>();

		// Token: 0x0400011A RID: 282
		[ViewVariables]
		[DataField("autoCycle", false, 1, false, false, null)]
		public bool AutoCycle = true;

		// Token: 0x0400011B RID: 283
		[ViewVariables]
		[DataField("cycled", false, 1, false, false, null)]
		public bool Cycled = true;

		// Token: 0x0400011C RID: 284
		[ViewVariables]
		[DataField("mayTransfer", false, 1, false, false, null)]
		public bool MayTransfer;
	}
}
