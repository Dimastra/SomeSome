using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.CartridgeLoader.Cartridges;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.CartridgeLoader.Cartridges
{
	// Token: 0x020006DC RID: 1756
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class NetProbeCartridgeComponent : Component
	{
		// Token: 0x1700057F RID: 1407
		// (get) Token: 0x060024AE RID: 9390 RVA: 0x000BF05C File Offset: 0x000BD25C
		// (set) Token: 0x060024AF RID: 9391 RVA: 0x000BF064 File Offset: 0x000BD264
		[DataField("maxSavedDevices", false, 1, false, false, null)]
		public int MaxSavedDevices { get; set; } = 9;

		// Token: 0x04001688 RID: 5768
		[DataField("probedDevices", false, 1, false, false, null)]
		public List<ProbedNetworkDevice> ProbedDevices = new List<ProbedNetworkDevice>();

		// Token: 0x0400168A RID: 5770
		[DataField("soundScan", false, 1, false, false, null)]
		public SoundSpecifier SoundScan = new SoundPathSpecifier("/Audio/Machines/scan_finish.ogg", null);
	}
}
