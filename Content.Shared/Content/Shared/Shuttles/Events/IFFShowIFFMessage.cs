using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Shuttles.Events
{
	// Token: 0x020001C4 RID: 452
	[NetSerializable]
	[Serializable]
	public sealed class IFFShowIFFMessage : BoundUserInterfaceMessage
	{
		// Token: 0x0400051F RID: 1311
		public bool Show;
	}
}
