using System;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Serialization;

namespace Content.Shared.Physics
{
	// Token: 0x02000279 RID: 633
	[Flags]
	[FlagsFor(typeof(CollisionLayer))]
	[FlagsFor(typeof(CollisionMask))]
	public enum CollisionGroup
	{
		// Token: 0x04000725 RID: 1829
		None = 0,
		// Token: 0x04000726 RID: 1830
		Opaque = 1,
		// Token: 0x04000727 RID: 1831
		Impassable = 2,
		// Token: 0x04000728 RID: 1832
		MidImpassable = 4,
		// Token: 0x04000729 RID: 1833
		HighImpassable = 8,
		// Token: 0x0400072A RID: 1834
		LowImpassable = 16,
		// Token: 0x0400072B RID: 1835
		GhostImpassable = 32,
		// Token: 0x0400072C RID: 1836
		BulletImpassable = 64,
		// Token: 0x0400072D RID: 1837
		InteractImpassable = 128,
		// Token: 0x0400072E RID: 1838
		MapGrid = -2147483648,
		// Token: 0x0400072F RID: 1839
		AllMask = -1,
		// Token: 0x04000730 RID: 1840
		MobMask = 30,
		// Token: 0x04000731 RID: 1841
		MobLayer = 65,
		// Token: 0x04000732 RID: 1842
		SmallMobMask = 18,
		// Token: 0x04000733 RID: 1843
		SmallMobLayer = 65,
		// Token: 0x04000734 RID: 1844
		FlyingMobMask = 10,
		// Token: 0x04000735 RID: 1845
		FlyingMobLayer = 65,
		// Token: 0x04000736 RID: 1846
		LargeMobMask = 30,
		// Token: 0x04000737 RID: 1847
		LargeMobLayer = 93,
		// Token: 0x04000738 RID: 1848
		MachineMask = 22,
		// Token: 0x04000739 RID: 1849
		MachineLayer = 85,
		// Token: 0x0400073A RID: 1850
		TableMask = 6,
		// Token: 0x0400073B RID: 1851
		TableLayer = 4,
		// Token: 0x0400073C RID: 1852
		TabletopMachineMask = 10,
		// Token: 0x0400073D RID: 1853
		TabletopMachineLayer = 73,
		// Token: 0x0400073E RID: 1854
		GlassAirlockLayer = 204,
		// Token: 0x0400073F RID: 1855
		AirlockLayer = 205,
		// Token: 0x04000740 RID: 1856
		HumanoidBlockLayer = 12,
		// Token: 0x04000741 RID: 1857
		SlipLayer = 20,
		// Token: 0x04000742 RID: 1858
		ItemMask = 10,
		// Token: 0x04000743 RID: 1859
		ThrownItem = 74,
		// Token: 0x04000744 RID: 1860
		WallLayer = 223,
		// Token: 0x04000745 RID: 1861
		GlassLayer = 222,
		// Token: 0x04000746 RID: 1862
		HalfWallLayer = 20,
		// Token: 0x04000747 RID: 1863
		FullTileMask = 158,
		// Token: 0x04000748 RID: 1864
		FullTileLayer = 221,
		// Token: 0x04000749 RID: 1865
		SubfloorMask = 18
	}
}
