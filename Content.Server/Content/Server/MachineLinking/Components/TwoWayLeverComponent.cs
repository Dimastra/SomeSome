using System;
using System.Runtime.CompilerServices;
using Content.Shared.MachineLinking;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.MachineLinking.Components
{
	// Token: 0x02000406 RID: 1030
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class TwoWayLeverComponent : Component
	{
		// Token: 0x04000CF6 RID: 3318
		[DataField("state", false, 1, false, false, null)]
		public TwoWayLeverState State;

		// Token: 0x04000CF7 RID: 3319
		[DataField("nextSignalLeft", false, 1, false, false, null)]
		public bool NextSignalLeft;

		// Token: 0x04000CF8 RID: 3320
		[DataField("leftPort", false, 1, false, false, typeof(PrototypeIdSerializer<TransmitterPortPrototype>))]
		public string LeftPort = "Left";

		// Token: 0x04000CF9 RID: 3321
		[DataField("rightPort", false, 1, false, false, typeof(PrototypeIdSerializer<TransmitterPortPrototype>))]
		public string RightPort = "Right";

		// Token: 0x04000CFA RID: 3322
		[DataField("middlePort", false, 1, false, false, typeof(PrototypeIdSerializer<TransmitterPortPrototype>))]
		public string MiddlePort = "Middle";
	}
}
