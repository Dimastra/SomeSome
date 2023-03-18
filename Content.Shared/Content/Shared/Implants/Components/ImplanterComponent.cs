using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Shared.Containers.ItemSlots;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Implants.Components
{
	// Token: 0x020003F2 RID: 1010
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class ImplanterComponent : Component
	{
		// Token: 0x04000BCB RID: 3019
		public const string ImplanterSlotId = "implanter_slot";

		// Token: 0x04000BCC RID: 3020
		public const string ImplantSlotId = "implant";

		// Token: 0x04000BCD RID: 3021
		[Nullable(2)]
		[ViewVariables]
		[DataField("implant", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string Implant;

		// Token: 0x04000BCE RID: 3022
		[ViewVariables]
		[DataField("implantTime", false, 1, false, false, null)]
		public float ImplantTime = 5f;

		// Token: 0x04000BCF RID: 3023
		[ViewVariables]
		[DataField("drawTime", false, 1, false, false, null)]
		public float DrawTime = 300f;

		// Token: 0x04000BD0 RID: 3024
		[ViewVariables]
		[DataField("implantOnly", false, 1, false, false, null)]
		public bool ImplantOnly;

		// Token: 0x04000BD1 RID: 3025
		[ViewVariables]
		[DataField("currentMode", false, 1, false, false, null)]
		public ImplanterToggleMode CurrentMode;

		// Token: 0x04000BD2 RID: 3026
		[Nullable(new byte[]
		{
			0,
			1,
			1
		})]
		[ViewVariables]
		[DataField("implantData", false, 1, false, false, null)]
		public ValueTuple<string, string> ImplantData;

		// Token: 0x04000BD3 RID: 3027
		[ViewVariables]
		[DataField("implanterSlot", false, 1, false, false, null)]
		public ItemSlot ImplanterSlot = new ItemSlot();

		// Token: 0x04000BD4 RID: 3028
		public bool UiUpdateNeeded;

		// Token: 0x04000BD5 RID: 3029
		[Nullable(2)]
		public CancellationTokenSource CancelToken;
	}
}
