using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Anomaly
{
	// Token: 0x02000701 RID: 1793
	[NetSerializable]
	[Serializable]
	public sealed class AnomalyGeneratorUserInterfaceState : BoundUserInterfaceState
	{
		// Token: 0x06001581 RID: 5505 RVA: 0x00046370 File Offset: 0x00044570
		public AnomalyGeneratorUserInterfaceState(TimeSpan cooldownEndTime, int fuelAmount, int fuelCost)
		{
			this.CooldownEndTime = cooldownEndTime;
			this.FuelAmount = fuelAmount;
			this.FuelCost = fuelCost;
		}

		// Token: 0x040015CD RID: 5581
		public TimeSpan CooldownEndTime;

		// Token: 0x040015CE RID: 5582
		public int FuelAmount;

		// Token: 0x040015CF RID: 5583
		public int FuelCost;
	}
}
