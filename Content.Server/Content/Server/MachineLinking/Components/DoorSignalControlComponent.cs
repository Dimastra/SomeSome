using System;
using System.Runtime.CompilerServices;
using Content.Shared.MachineLinking;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.MachineLinking.Components
{
	// Token: 0x020003FE RID: 1022
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class DoorSignalControlComponent : Component
	{
		// Token: 0x04000CDC RID: 3292
		[DataField("openPort", false, 1, false, false, typeof(PrototypeIdSerializer<ReceiverPortPrototype>))]
		public string OpenPort = "Open";

		// Token: 0x04000CDD RID: 3293
		[DataField("closePort", false, 1, false, false, typeof(PrototypeIdSerializer<ReceiverPortPrototype>))]
		public string ClosePort = "Close";

		// Token: 0x04000CDE RID: 3294
		[DataField("togglePort", false, 1, false, false, typeof(PrototypeIdSerializer<ReceiverPortPrototype>))]
		public string TogglePort = "Toggle";
	}
}
