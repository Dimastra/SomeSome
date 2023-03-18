using System;
using Robust.Shared.GameObjects;

namespace Content.Server.Atmos
{
	// Token: 0x02000737 RID: 1847
	public abstract class PressureEvent : EntityEventArgs
	{
		// Token: 0x170005C2 RID: 1474
		// (get) Token: 0x060026C5 RID: 9925 RVA: 0x000CC8BE File Offset: 0x000CAABE
		public float Pressure { get; }

		// Token: 0x170005C3 RID: 1475
		// (get) Token: 0x060026C6 RID: 9926 RVA: 0x000CC8C6 File Offset: 0x000CAAC6
		// (set) Token: 0x060026C7 RID: 9927 RVA: 0x000CC8CE File Offset: 0x000CAACE
		public float Modifier { get; set; }

		// Token: 0x170005C4 RID: 1476
		// (get) Token: 0x060026C8 RID: 9928 RVA: 0x000CC8D7 File Offset: 0x000CAAD7
		// (set) Token: 0x060026C9 RID: 9929 RVA: 0x000CC8DF File Offset: 0x000CAADF
		public float Multiplier { get; set; } = 1f;

		// Token: 0x060026CA RID: 9930 RVA: 0x000CC8E8 File Offset: 0x000CAAE8
		protected PressureEvent(float pressure)
		{
			this.Pressure = pressure;
		}
	}
}
