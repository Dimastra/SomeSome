using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.DrawDepth
{
	// Token: 0x020004D5 RID: 1237
	[ConstantsFor(typeof(DrawDepth))]
	public enum DrawDepth
	{
		// Token: 0x04000DFE RID: 3582
		LowFloors = -10,
		// Token: 0x04000DFF RID: 3583
		ThickPipe,
		// Token: 0x04000E00 RID: 3584
		ThickWire,
		// Token: 0x04000E01 RID: 3585
		ThinPipe,
		// Token: 0x04000E02 RID: 3586
		ThinWire,
		// Token: 0x04000E03 RID: 3587
		BelowFloor = -6,
		// Token: 0x04000E04 RID: 3588
		FloorTiles,
		// Token: 0x04000E05 RID: 3589
		FloorObjects,
		// Token: 0x04000E06 RID: 3590
		SmallMobs,
		// Token: 0x04000E07 RID: 3591
		Walls,
		// Token: 0x04000E08 RID: 3592
		WallTops,
		// Token: 0x04000E09 RID: 3593
		Objects,
		// Token: 0x04000E0A RID: 3594
		SmallObjects,
		// Token: 0x04000E0B RID: 3595
		WallMountedItems,
		// Token: 0x04000E0C RID: 3596
		Items,
		// Token: 0x04000E0D RID: 3597
		Mobs,
		// Token: 0x04000E0E RID: 3598
		Doors,
		// Token: 0x04000E0F RID: 3599
		Overdoors,
		// Token: 0x04000E10 RID: 3600
		Effects,
		// Token: 0x04000E11 RID: 3601
		Ghosts,
		// Token: 0x04000E12 RID: 3602
		Overlays
	}
}
