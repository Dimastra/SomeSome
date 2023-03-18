using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Shuttles.Events
{
	// Token: 0x020001C7 RID: 455
	[NetSerializable]
	[Serializable]
	public sealed class StopAutodockRequestMessage : BoundUserInterfaceMessage
	{
		// Token: 0x04000522 RID: 1314
		public EntityUid DockEntity;
	}
}
