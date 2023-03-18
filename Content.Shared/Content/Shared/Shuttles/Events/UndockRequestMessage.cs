using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Shuttles.Events
{
	// Token: 0x020001C8 RID: 456
	[NetSerializable]
	[Serializable]
	public sealed class UndockRequestMessage : BoundUserInterfaceMessage
	{
		// Token: 0x04000523 RID: 1315
		public EntityUid DockEntity;
	}
}
