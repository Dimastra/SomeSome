using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.DeviceNetwork
{
	// Token: 0x0200050F RID: 1295
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[NetworkedComponent]
	[Access(new Type[]
	{
		typeof(SharedNetworkConfiguratorSystem)
	})]
	public sealed class NetworkConfiguratorComponent : Component
	{
		// Token: 0x04000EF2 RID: 3826
		[DataField("activeDeviceList", false, 1, false, false, null)]
		public EntityUid? ActiveDeviceList;

		// Token: 0x04000EF3 RID: 3827
		[DataField("devices", false, 1, false, false, null)]
		public Dictionary<string, EntityUid> Devices = new Dictionary<string, EntityUid>();

		// Token: 0x04000EF4 RID: 3828
		[DataField("soundNoAccess", false, 1, false, false, null)]
		public SoundSpecifier SoundNoAccess = new SoundPathSpecifier("/Audio/Machines/custom_deny.ogg", null);
	}
}
