using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Atmos.Components
{
	// Token: 0x020007A0 RID: 1952
	[RegisterComponent]
	public sealed class AtmosFixMarkerComponent : Component
	{
		// Token: 0x17000664 RID: 1636
		// (get) Token: 0x06002A63 RID: 10851 RVA: 0x000DFAF9 File Offset: 0x000DDCF9
		// (set) Token: 0x06002A64 RID: 10852 RVA: 0x000DFB01 File Offset: 0x000DDD01
		[DataField("mode", false, 1, false, false, null)]
		public int Mode { get; set; }
	}
}
