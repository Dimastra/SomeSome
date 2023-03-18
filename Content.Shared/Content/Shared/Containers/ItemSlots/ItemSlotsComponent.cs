using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Containers.ItemSlots
{
	// Token: 0x02000562 RID: 1378
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(ItemSlotsSystem)
	})]
	[NetworkedComponent]
	public sealed class ItemSlotsComponent : Component
	{
		// Token: 0x04000FAD RID: 4013
		[Nullable(1)]
		[DataField("slots", true, 1, false, false, null)]
		public readonly Dictionary<string, ItemSlot> Slots = new Dictionary<string, ItemSlot>();
	}
}
