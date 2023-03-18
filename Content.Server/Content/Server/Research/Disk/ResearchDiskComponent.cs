using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Research.Disk
{
	// Token: 0x02000240 RID: 576
	[RegisterComponent]
	public sealed class ResearchDiskComponent : Component
	{
		// Token: 0x04000716 RID: 1814
		[DataField("points", false, 1, false, false, null)]
		[ViewVariables]
		public int Points = 1000;

		// Token: 0x04000717 RID: 1815
		[DataField("unlockAllTech", false, 1, false, false, null)]
		public bool UnlockAllTech;
	}
}
