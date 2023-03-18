using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Botany
{
	// Token: 0x02000649 RID: 1609
	[NetSerializable]
	[Serializable]
	public enum PlantHolderVisuals
	{
		// Token: 0x0400136E RID: 4974
		PlantRsi,
		// Token: 0x0400136F RID: 4975
		PlantState,
		// Token: 0x04001370 RID: 4976
		HealthLight,
		// Token: 0x04001371 RID: 4977
		WaterLight,
		// Token: 0x04001372 RID: 4978
		NutritionLight,
		// Token: 0x04001373 RID: 4979
		AlertLight,
		// Token: 0x04001374 RID: 4980
		HarvestLight
	}
}
