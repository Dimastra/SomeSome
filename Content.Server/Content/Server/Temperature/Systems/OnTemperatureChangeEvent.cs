using System;
using Robust.Shared.GameObjects;

namespace Content.Server.Temperature.Systems
{
	// Token: 0x02000125 RID: 293
	public sealed class OnTemperatureChangeEvent : EntityEventArgs
	{
		// Token: 0x170000EB RID: 235
		// (get) Token: 0x06000552 RID: 1362 RVA: 0x0001A29C File Offset: 0x0001849C
		public float CurrentTemperature { get; }

		// Token: 0x170000EC RID: 236
		// (get) Token: 0x06000553 RID: 1363 RVA: 0x0001A2A4 File Offset: 0x000184A4
		public float LastTemperature { get; }

		// Token: 0x170000ED RID: 237
		// (get) Token: 0x06000554 RID: 1364 RVA: 0x0001A2AC File Offset: 0x000184AC
		public float TemperatureDelta { get; }

		// Token: 0x06000555 RID: 1365 RVA: 0x0001A2B4 File Offset: 0x000184B4
		public OnTemperatureChangeEvent(float current, float last, float delta)
		{
			this.CurrentTemperature = current;
			this.LastTemperature = last;
			this.TemperatureDelta = delta;
		}
	}
}
