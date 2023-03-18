using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.DoAfter
{
	// Token: 0x020004F2 RID: 1266
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class DoAfterComponent : Component
	{
		// Token: 0x04000E93 RID: 3731
		[DataField("doAfters", false, 1, false, false, null)]
		public readonly Dictionary<byte, DoAfter> DoAfters = new Dictionary<byte, DoAfter>();

		// Token: 0x04000E94 RID: 3732
		[DataField("cancelledDoAfters", false, 1, false, false, null)]
		public readonly Dictionary<byte, DoAfter> CancelledDoAfters = new Dictionary<byte, DoAfter>();

		// Token: 0x04000E95 RID: 3733
		[DataField("runningIndex", false, 1, false, false, null)]
		public byte RunningIndex;
	}
}
