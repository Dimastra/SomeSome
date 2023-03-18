using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.MachineLinking;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Conveyor
{
	// Token: 0x0200055D RID: 1373
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class ConveyorComponent : Component
	{
		// Token: 0x04000F98 RID: 3992
		[ViewVariables]
		[DataField("angle", false, 1, false, false, null)]
		public Angle Angle = Angle.Zero;

		// Token: 0x04000F99 RID: 3993
		[ViewVariables]
		[DataField("speed", false, 1, false, false, null)]
		public float Speed = 2f;

		// Token: 0x04000F9A RID: 3994
		[ViewVariables]
		public ConveyorState State;

		// Token: 0x04000F9B RID: 3995
		[ViewVariables]
		public bool Powered;

		// Token: 0x04000F9C RID: 3996
		[DataField("forwardPort", false, 1, false, false, typeof(PrototypeIdSerializer<ReceiverPortPrototype>))]
		public string ForwardPort = "Forward";

		// Token: 0x04000F9D RID: 3997
		[DataField("reversePort", false, 1, false, false, typeof(PrototypeIdSerializer<TransmitterPortPrototype>))]
		public string ReversePort = "Reverse";

		// Token: 0x04000F9E RID: 3998
		[DataField("offPort", false, 1, false, false, typeof(PrototypeIdSerializer<TransmitterPortPrototype>))]
		public string OffPort = "Off";

		// Token: 0x04000F9F RID: 3999
		[ViewVariables]
		public readonly HashSet<EntityUid> Intersecting = new HashSet<EntityUid>();
	}
}
