using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Shuttles.Events
{
	// Token: 0x020001BE RID: 446
	[NetSerializable]
	[Serializable]
	public sealed class AutodockRequestMessage : BoundUserInterfaceMessage
	{
		// Token: 0x0400051C RID: 1308
		public EntityUid DockEntity;
	}
}
