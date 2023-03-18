using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Client.Atmos.Visualizers
{
	// Token: 0x02000432 RID: 1074
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class PortableScrubberVisualsComponent : Component
	{
		// Token: 0x04000D58 RID: 3416
		[DataField("idleState", false, 1, true, false, null)]
		public string IdleState;

		// Token: 0x04000D59 RID: 3417
		[DataField("runningState", false, 1, true, false, null)]
		public string RunningState;

		// Token: 0x04000D5A RID: 3418
		[DataField("readyState", false, 1, true, false, null)]
		public string ReadyState;

		// Token: 0x04000D5B RID: 3419
		[DataField("fullState", false, 1, true, false, null)]
		public string FullState;
	}
}
