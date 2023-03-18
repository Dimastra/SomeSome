using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos
{
	// Token: 0x02000690 RID: 1680
	[NetSerializable]
	[Serializable]
	public enum Gas : sbyte
	{
		// Token: 0x04001469 RID: 5225
		Oxygen,
		// Token: 0x0400146A RID: 5226
		Nitrogen,
		// Token: 0x0400146B RID: 5227
		CarbonDioxide,
		// Token: 0x0400146C RID: 5228
		Plasma,
		// Token: 0x0400146D RID: 5229
		Tritium,
		// Token: 0x0400146E RID: 5230
		WaterVapor,
		// Token: 0x0400146F RID: 5231
		Miasma,
		// Token: 0x04001470 RID: 5232
		NitrousOxide,
		// Token: 0x04001471 RID: 5233
		Frezon
	}
}
