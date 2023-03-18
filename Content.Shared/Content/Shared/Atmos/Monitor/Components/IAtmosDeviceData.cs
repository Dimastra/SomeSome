using System;

namespace Content.Shared.Atmos.Monitor.Components
{
	// Token: 0x020006D5 RID: 1749
	public interface IAtmosDeviceData
	{
		// Token: 0x1700046E RID: 1134
		// (get) Token: 0x0600153A RID: 5434
		// (set) Token: 0x0600153B RID: 5435
		bool Enabled { get; set; }

		// Token: 0x1700046F RID: 1135
		// (get) Token: 0x0600153C RID: 5436
		// (set) Token: 0x0600153D RID: 5437
		bool Dirty { get; set; }

		// Token: 0x17000470 RID: 1136
		// (get) Token: 0x0600153E RID: 5438
		// (set) Token: 0x0600153F RID: 5439
		bool IgnoreAlarms { get; set; }
	}
}
