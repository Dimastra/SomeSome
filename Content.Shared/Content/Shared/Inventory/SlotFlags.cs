using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Inventory
{
	// Token: 0x020003B1 RID: 945
	[NetSerializable]
	[Flags]
	[Serializable]
	public enum SlotFlags
	{
		// Token: 0x04000ACF RID: 2767
		NONE = 0,
		// Token: 0x04000AD0 RID: 2768
		PREVENTEQUIP = 1,
		// Token: 0x04000AD1 RID: 2769
		HEAD = 2,
		// Token: 0x04000AD2 RID: 2770
		EYES = 4,
		// Token: 0x04000AD3 RID: 2771
		EARS = 8,
		// Token: 0x04000AD4 RID: 2772
		MASK = 16,
		// Token: 0x04000AD5 RID: 2773
		OUTERCLOTHING = 32,
		// Token: 0x04000AD6 RID: 2774
		INNERCLOTHING = 64,
		// Token: 0x04000AD7 RID: 2775
		NECK = 128,
		// Token: 0x04000AD8 RID: 2776
		BACK = 256,
		// Token: 0x04000AD9 RID: 2777
		BELT = 512,
		// Token: 0x04000ADA RID: 2778
		GLOVES = 1024,
		// Token: 0x04000ADB RID: 2779
		IDCARD = 2048,
		// Token: 0x04000ADC RID: 2780
		POCKET = 4096,
		// Token: 0x04000ADD RID: 2781
		LEGS = 8192,
		// Token: 0x04000ADE RID: 2782
		FEET = 16384,
		// Token: 0x04000ADF RID: 2783
		SUITSTORAGE = 32768,
		// Token: 0x04000AE0 RID: 2784
		All = -1
	}
}
