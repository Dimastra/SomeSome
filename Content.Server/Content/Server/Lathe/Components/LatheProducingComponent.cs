using System;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Server.Lathe.Components
{
	// Token: 0x02000422 RID: 1058
	[RegisterComponent]
	public sealed class LatheProducingComponent : Component
	{
		// Token: 0x04000D57 RID: 3415
		[ViewVariables]
		public TimeSpan StartTime;

		// Token: 0x04000D58 RID: 3416
		[ViewVariables]
		public TimeSpan ProductionLength;
	}
}
