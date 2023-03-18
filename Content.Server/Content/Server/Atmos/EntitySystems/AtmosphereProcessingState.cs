using System;

namespace Content.Server.Atmos.EntitySystems
{
	// Token: 0x02000795 RID: 1941
	public enum AtmosphereProcessingState : byte
	{
		// Token: 0x040019E3 RID: 6627
		Revalidate,
		// Token: 0x040019E4 RID: 6628
		TileEqualize,
		// Token: 0x040019E5 RID: 6629
		ActiveTiles,
		// Token: 0x040019E6 RID: 6630
		ExcitedGroups,
		// Token: 0x040019E7 RID: 6631
		HighPressureDelta,
		// Token: 0x040019E8 RID: 6632
		Hotspots,
		// Token: 0x040019E9 RID: 6633
		Superconductivity,
		// Token: 0x040019EA RID: 6634
		PipeNet,
		// Token: 0x040019EB RID: 6635
		AtmosDevices
	}
}
