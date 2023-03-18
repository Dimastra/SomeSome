using System;
using System.Runtime.CompilerServices;
using Content.Shared.Containers.ItemSlots;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Cabinet
{
	// Token: 0x0200063D RID: 1597
	[NullableContext(2)]
	[Nullable(0)]
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class ItemCabinetComponent : Component
	{
		// Token: 0x04001336 RID: 4918
		[DataField("doorSound", false, 1, false, false, null)]
		[ViewVariables]
		public SoundSpecifier DoorSound;

		// Token: 0x04001337 RID: 4919
		[Nullable(1)]
		[DataField("cabinetSlot", false, 1, false, false, null)]
		[ViewVariables]
		public ItemSlot CabinetSlot = new ItemSlot();

		// Token: 0x04001338 RID: 4920
		[DataField("opened", false, 1, false, false, null)]
		public bool Opened;

		// Token: 0x04001339 RID: 4921
		[DataField("openState", false, 1, false, false, null)]
		[ViewVariables]
		public string OpenState;

		// Token: 0x0400133A RID: 4922
		[DataField("closedState", false, 1, false, false, null)]
		[ViewVariables]
		public string ClosedState;
	}
}
