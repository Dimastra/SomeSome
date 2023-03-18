using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Client.Disease
{
	// Token: 0x02000357 RID: 855
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class DiseaseMachineVisualsComponent : Component
	{
		// Token: 0x04000AF8 RID: 2808
		[DataField("idleState", false, 1, true, false, null)]
		public string IdleState;

		// Token: 0x04000AF9 RID: 2809
		[DataField("runningState", false, 1, true, false, null)]
		public string RunningState;
	}
}
