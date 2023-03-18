using System;
using System.Runtime.CompilerServices;
using Content.Shared.Access.Components;
using Content.Shared.Containers.ItemSlots;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Shared.PDA
{
	// Token: 0x02000282 RID: 642
	[NullableContext(2)]
	[Nullable(0)]
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class PDAComponent : Component
	{
		// Token: 0x04000754 RID: 1876
		[Nullable(1)]
		public const string PDAIdSlotId = "PDA-id";

		// Token: 0x04000755 RID: 1877
		[Nullable(1)]
		public const string PDAPenSlotId = "PDA-pen";

		// Token: 0x04000756 RID: 1878
		[DataField("state", false, 1, false, false, null)]
		public string State;

		// Token: 0x04000757 RID: 1879
		[Nullable(1)]
		[DataField("idSlot", false, 1, false, false, null)]
		public ItemSlot IdSlot = new ItemSlot();

		// Token: 0x04000758 RID: 1880
		[Nullable(1)]
		[DataField("penSlot", false, 1, false, false, null)]
		public ItemSlot PenSlot = new ItemSlot();

		// Token: 0x04000759 RID: 1881
		[DataField("id", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string IdCard;

		// Token: 0x0400075A RID: 1882
		[ViewVariables]
		public IdCardComponent ContainedID;

		// Token: 0x0400075B RID: 1883
		[ViewVariables]
		public bool FlashlightOn;

		// Token: 0x0400075C RID: 1884
		[ViewVariables]
		public string OwnerName;

		// Token: 0x0400075D RID: 1885
		[ViewVariables]
		public string StationName;
	}
}
