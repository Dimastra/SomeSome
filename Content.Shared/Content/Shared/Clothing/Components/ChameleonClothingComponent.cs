using System;
using System.Runtime.CompilerServices;
using Content.Shared.Clothing.EntitySystems;
using Content.Shared.Inventory;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Clothing.Components
{
	// Token: 0x020005B0 RID: 1456
	[RegisterComponent]
	[NetworkedComponent]
	[Access(new Type[]
	{
		typeof(SharedChameleonClothingSystem)
	})]
	public sealed class ChameleonClothingComponent : Component
	{
		// Token: 0x04001065 RID: 4197
		[ViewVariables]
		[DataField("slot", false, 1, true, false, null)]
		public SlotFlags Slot;

		// Token: 0x04001066 RID: 4198
		[Nullable(2)]
		[ViewVariables]
		[DataField("default", false, 1, true, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string SelectedId;

		// Token: 0x04001067 RID: 4199
		[ViewVariables]
		public EntityUid? User;
	}
}
