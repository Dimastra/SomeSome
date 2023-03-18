using System;
using System.Runtime.CompilerServices;
using Content.Server.Nutrition.EntitySystems;
using Content.Shared.Containers.ItemSlots;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Nutrition.Components
{
	// Token: 0x0200031E RID: 798
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(SmokingSystem)
	})]
	public sealed class SmokingPipeComponent : Component
	{
		// Token: 0x040009A9 RID: 2473
		public const string BowlSlotId = "bowl_slot";

		// Token: 0x040009AA RID: 2474
		[DataField("bowl_slot", false, 1, false, false, null)]
		public ItemSlot BowlSlot = new ItemSlot();
	}
}
