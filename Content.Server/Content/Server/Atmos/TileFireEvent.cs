using System;
using Robust.Shared.GameObjects;

namespace Content.Server.Atmos
{
	// Token: 0x0200073B RID: 1851
	[ByRefEvent]
	public readonly struct TileFireEvent
	{
		// Token: 0x060026EC RID: 9964 RVA: 0x000CCAD7 File Offset: 0x000CACD7
		public TileFireEvent(float temperature, float volume)
		{
			this.Temperature = temperature;
			this.Volume = volume;
		}

		// Token: 0x04001834 RID: 6196
		public readonly float Temperature;

		// Token: 0x04001835 RID: 6197
		public readonly float Volume;
	}
}
