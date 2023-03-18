using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Shuttles.BUIStates
{
	// Token: 0x020001D1 RID: 465
	[NetSerializable]
	[Serializable]
	public sealed class EmergencyConsoleBoundUserInterfaceState : BoundUserInterfaceState
	{
		// Token: 0x0400053C RID: 1340
		public TimeSpan? EarlyLaunchTime;

		// Token: 0x0400053D RID: 1341
		[Nullable(1)]
		public List<string> Authorizations = new List<string>();

		// Token: 0x0400053E RID: 1342
		public int AuthorizationsRequired;

		// Token: 0x0400053F RID: 1343
		public TimeSpan? TimeToLaunch;
	}
}
