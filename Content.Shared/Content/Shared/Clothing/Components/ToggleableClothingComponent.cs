using System;
using System.Runtime.CompilerServices;
using Content.Shared.Actions.ActionTypes;
using Content.Shared.Clothing.EntitySystems;
using Content.Shared.Inventory;
using Robust.Shared.Analyzers;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Clothing.Components
{
	// Token: 0x020005B8 RID: 1464
	[NullableContext(1)]
	[Nullable(0)]
	[Access(new Type[]
	{
		typeof(ToggleableClothingSystem)
	})]
	[RegisterComponent]
	public sealed class ToggleableClothingComponent : Component
	{
		// Token: 0x0400107C RID: 4220
		public const string DefaultClothingContainerId = "toggleable-clothing";

		// Token: 0x0400107D RID: 4221
		[DataField("actionId", false, 1, false, false, typeof(PrototypeIdSerializer<InstantActionPrototype>))]
		public string ActionId = "ToggleSuitHelmet";

		// Token: 0x0400107E RID: 4222
		[Nullable(2)]
		public InstantAction ToggleAction;

		// Token: 0x0400107F RID: 4223
		[DataField("clothingPrototype", false, 1, true, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public readonly string ClothingPrototype;

		// Token: 0x04001080 RID: 4224
		[DataField("slot", false, 1, false, false, null)]
		public string Slot = "head";

		// Token: 0x04001081 RID: 4225
		[DataField("requiredSlot", false, 1, false, false, null)]
		public SlotFlags RequiredFlags = SlotFlags.OUTERCLOTHING;

		// Token: 0x04001082 RID: 4226
		[DataField("containerId", false, 1, false, false, null)]
		public string ContainerId = "toggleable-clothing";

		// Token: 0x04001083 RID: 4227
		[Nullable(2)]
		[ViewVariables]
		public ContainerSlot Container;

		// Token: 0x04001084 RID: 4228
		[DataField("clothingUid", false, 1, false, false, null)]
		public EntityUid? ClothingUid;
	}
}
