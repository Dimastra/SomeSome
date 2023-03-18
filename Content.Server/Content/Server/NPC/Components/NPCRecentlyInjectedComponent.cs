using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.NPC.Components
{
	// Token: 0x02000371 RID: 881
	[RegisterComponent]
	public sealed class NPCRecentlyInjectedComponent : Component
	{
		// Token: 0x04000B18 RID: 2840
		[ViewVariables]
		[DataField("accumulator", false, 1, false, false, null)]
		public float Accumulator;

		// Token: 0x04000B19 RID: 2841
		[ViewVariables]
		[DataField("removeTime", false, 1, false, false, null)]
		public TimeSpan RemoveTime = TimeSpan.FromMinutes(1.0);
	}
}
