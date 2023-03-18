using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.DeviceNetwork.Components
{
	// Token: 0x02000591 RID: 1425
	[RegisterComponent]
	[ComponentProtoName("WirelessNetworkConnection")]
	public sealed class WirelessNetworkComponent : Component
	{
		// Token: 0x17000461 RID: 1121
		// (get) Token: 0x06001DCF RID: 7631 RVA: 0x0009E9E2 File Offset: 0x0009CBE2
		// (set) Token: 0x06001DD0 RID: 7632 RVA: 0x0009E9EA File Offset: 0x0009CBEA
		[DataField("range", false, 1, false, false, null)]
		public int Range { get; set; }
	}
}
