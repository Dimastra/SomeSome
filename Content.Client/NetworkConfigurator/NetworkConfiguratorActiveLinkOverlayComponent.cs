using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Client.NetworkConfigurator
{
	// Token: 0x0200021D RID: 541
	[RegisterComponent]
	public sealed class NetworkConfiguratorActiveLinkOverlayComponent : Component
	{
		// Token: 0x040006FF RID: 1791
		[Nullable(1)]
		public HashSet<EntityUid> Devices = new HashSet<EntityUid>();
	}
}
