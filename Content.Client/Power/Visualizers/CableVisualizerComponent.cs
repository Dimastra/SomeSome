using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Client.Power.Visualizers
{
	// Token: 0x020001A1 RID: 417
	[RegisterComponent]
	public sealed class CableVisualizerComponent : Component
	{
		// Token: 0x04000553 RID: 1363
		[Nullable(2)]
		[DataField("statePrefix", false, 1, false, false, null)]
		public string StatePrefix;
	}
}
