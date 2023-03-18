using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.NPC
{
	// Token: 0x020002C6 RID: 710
	[NetSerializable]
	[Serializable]
	public sealed class RequestPathfindingDebugMessage : EntityEventArgs
	{
		// Token: 0x040007F8 RID: 2040
		public PathfindingDebugMode Mode;
	}
}
