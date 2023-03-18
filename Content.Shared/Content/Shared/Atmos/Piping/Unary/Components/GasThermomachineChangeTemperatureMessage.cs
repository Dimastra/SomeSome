using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Piping.Unary.Components
{
	// Token: 0x020006AB RID: 1707
	[NetSerializable]
	[Serializable]
	public sealed class GasThermomachineChangeTemperatureMessage : BoundUserInterfaceMessage
	{
		// Token: 0x17000425 RID: 1061
		// (get) Token: 0x060014B8 RID: 5304 RVA: 0x00044D55 File Offset: 0x00042F55
		public float Temperature { get; }

		// Token: 0x060014B9 RID: 5305 RVA: 0x00044D5D File Offset: 0x00042F5D
		public GasThermomachineChangeTemperatureMessage(float temperature)
		{
			this.Temperature = temperature;
		}
	}
}
