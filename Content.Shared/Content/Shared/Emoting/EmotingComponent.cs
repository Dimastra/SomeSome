using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Emoting
{
	// Token: 0x020004C5 RID: 1221
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class EmotingComponent : Component
	{
		// Token: 0x04000DDC RID: 3548
		[DataField("enabled", false, 1, false, false, null)]
		[Access]
		public bool Enabled = true;
	}
}
