using System;
using Robust.Shared.GameObjects;

namespace Content.Server.NodeContainer.EntitySystems
{
	// Token: 0x02000389 RID: 905
	[ByRefEvent]
	public readonly struct NodeGroupsRebuilt
	{
		// Token: 0x06001299 RID: 4761 RVA: 0x000606F6 File Offset: 0x0005E8F6
		public NodeGroupsRebuilt(EntityUid nodeOwner)
		{
			this.NodeOwner = nodeOwner;
		}

		// Token: 0x04000B6B RID: 2923
		public readonly EntityUid NodeOwner;
	}
}
