using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Shuttles.Events
{
	// Token: 0x020001C0 RID: 448
	[NetSerializable]
	[Serializable]
	public sealed class EmergencyShuttlePositionMessage : EntityEventArgs
	{
		// Token: 0x0400051D RID: 1309
		public EntityUid? StationUid;

		// Token: 0x0400051E RID: 1310
		public Box2? Position;
	}
}
