using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.NPC.Components
{
	// Token: 0x0200036B RID: 875
	[RegisterComponent]
	public sealed class NPCAvoidanceComponent : Component
	{
		// Token: 0x04000B02 RID: 2818
		[ViewVariables]
		[DataField("enabled", false, 1, false, false, null)]
		public bool Enabled = true;
	}
}
