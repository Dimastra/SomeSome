using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Shuttles.Events
{
	// Token: 0x020001C5 RID: 453
	[NetSerializable]
	[Serializable]
	public sealed class IFFShowVesselMessage : BoundUserInterfaceMessage
	{
		// Token: 0x04000520 RID: 1312
		public bool Show;
	}
}
