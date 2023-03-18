using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Client.SurveillanceCamera
{
	// Token: 0x02000106 RID: 262
	[RegisterComponent]
	public sealed class ActiveSurveillanceCameraMonitorVisualsComponent : Component
	{
		// Token: 0x04000364 RID: 868
		public float TimeLeft = 10f;

		// Token: 0x04000365 RID: 869
		[Nullable(2)]
		public Action OnFinish;
	}
}
