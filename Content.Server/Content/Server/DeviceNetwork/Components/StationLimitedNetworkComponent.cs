using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.DeviceNetwork.Components
{
	// Token: 0x0200058F RID: 1423
	[RegisterComponent]
	public sealed class StationLimitedNetworkComponent : Component
	{
		// Token: 0x04001318 RID: 4888
		[ViewVariables]
		public EntityUid? StationId;

		// Token: 0x04001319 RID: 4889
		[DataField("allowNonStationPackets", false, 1, false, false, null)]
		[ViewVariables]
		public bool AllowNonStationPackets;
	}
}
