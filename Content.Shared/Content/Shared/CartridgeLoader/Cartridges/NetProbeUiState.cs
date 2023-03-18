using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.CartridgeLoader.Cartridges
{
	// Token: 0x02000622 RID: 1570
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class NetProbeUiState : BoundUserInterfaceState
	{
		// Token: 0x0600130B RID: 4875 RVA: 0x0003F9B9 File Offset: 0x0003DBB9
		public NetProbeUiState(List<ProbedNetworkDevice> probedDevices)
		{
			this.ProbedDevices = probedDevices;
		}

		// Token: 0x040012E8 RID: 4840
		public List<ProbedNetworkDevice> ProbedDevices;
	}
}
