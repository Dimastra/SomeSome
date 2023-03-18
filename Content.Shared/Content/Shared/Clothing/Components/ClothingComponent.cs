using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Clothing.EntitySystems;
using Content.Shared.Inventory;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Clothing.Components
{
	// Token: 0x020005B5 RID: 1461
	[NullableContext(2)]
	[Nullable(0)]
	[NetworkedComponent]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(ClothingSystem),
		typeof(InventorySystem)
	})]
	public sealed class ClothingComponent : Component
	{
		// Token: 0x0400106E RID: 4206
		[Nullable(1)]
		[DataField("clothingVisuals", false, 1, false, false, null)]
		[Access]
		public Dictionary<string, List<SharedSpriteComponent.PrototypeLayerData>> ClothingVisuals = new Dictionary<string, List<SharedSpriteComponent.PrototypeLayerData>>();

		// Token: 0x0400106F RID: 4207
		[ViewVariables]
		[DataField("quickEquip", false, 1, false, false, null)]
		public bool QuickEquip = true;

		// Token: 0x04001070 RID: 4208
		[ViewVariables]
		[DataField("slots", false, 1, true, false, null)]
		[Access]
		public SlotFlags Slots;

		// Token: 0x04001071 RID: 4209
		[ViewVariables]
		[DataField("equipSound", false, 1, false, false, null)]
		public SoundSpecifier EquipSound;

		// Token: 0x04001072 RID: 4210
		[ViewVariables]
		[DataField("unequipSound", false, 1, false, false, null)]
		public SoundSpecifier UnequipSound;

		// Token: 0x04001073 RID: 4211
		[Access(new Type[]
		{
			typeof(ClothingSystem)
		})]
		[ViewVariables]
		[DataField("equippedPrefix", false, 1, false, false, null)]
		public string EquippedPrefix;

		// Token: 0x04001074 RID: 4212
		[ViewVariables]
		[DataField("sprite", false, 1, false, false, null)]
		public string RsiPath;

		// Token: 0x04001075 RID: 4213
		[ViewVariables]
		[DataField("femaleMask", false, 1, false, false, null)]
		public FemaleClothingMask FemaleMask = FemaleClothingMask.UniformFull;

		// Token: 0x04001076 RID: 4214
		public string InSlot;
	}
}
