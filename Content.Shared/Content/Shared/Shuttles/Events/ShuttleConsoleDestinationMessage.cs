using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Shuttles.Events
{
	// Token: 0x020001C6 RID: 454
	[NetSerializable]
	[Serializable]
	public sealed class ShuttleConsoleDestinationMessage : BoundUserInterfaceMessage
	{
		// Token: 0x04000521 RID: 1313
		public EntityUid Destination;
	}
}
