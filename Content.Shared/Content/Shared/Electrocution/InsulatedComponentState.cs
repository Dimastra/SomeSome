using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Electrocution
{
	// Token: 0x020004CF RID: 1231
	[NetSerializable]
	[Serializable]
	public sealed class InsulatedComponentState : ComponentState
	{
		// Token: 0x17000311 RID: 785
		// (get) Token: 0x06000EE1 RID: 3809 RVA: 0x0002FDAD File Offset: 0x0002DFAD
		// (set) Token: 0x06000EE2 RID: 3810 RVA: 0x0002FDB5 File Offset: 0x0002DFB5
		public float SiemensCoefficient { get; private set; }

		// Token: 0x06000EE3 RID: 3811 RVA: 0x0002FDBE File Offset: 0x0002DFBE
		public InsulatedComponentState(float siemensCoefficient)
		{
			this.SiemensCoefficient = siemensCoefficient;
		}
	}
}
