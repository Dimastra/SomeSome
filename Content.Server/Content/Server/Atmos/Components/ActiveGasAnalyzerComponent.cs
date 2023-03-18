using System;
using Robust.Shared.GameObjects;

namespace Content.Server.Atmos.Components
{
	// Token: 0x020007A6 RID: 1958
	[RegisterComponent]
	public sealed class ActiveGasAnalyzerComponent : Component
	{
		// Token: 0x04001A46 RID: 6726
		public float AccumulatedFrametime = 2.01f;

		// Token: 0x04001A47 RID: 6727
		public float UpdateInterval = 1f;
	}
}
