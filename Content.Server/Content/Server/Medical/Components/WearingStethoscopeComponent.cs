using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Medical.Components
{
	// Token: 0x020003C0 RID: 960
	[RegisterComponent]
	public sealed class WearingStethoscopeComponent : Component
	{
		// Token: 0x04000C23 RID: 3107
		[Nullable(2)]
		public CancellationTokenSource CancelToken;

		// Token: 0x04000C24 RID: 3108
		[DataField("delay", false, 1, false, false, null)]
		public float Delay = 2.5f;

		// Token: 0x04000C25 RID: 3109
		public EntityUid Stethoscope;
	}
}
