using System;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Parallax.Biomes
{
	// Token: 0x0200029E RID: 670
	[ImplicitDataDefinitionForInheritors]
	public interface IBiomeLayer
	{
		// Token: 0x1700016E RID: 366
		// (get) Token: 0x06000775 RID: 1909
		float Threshold { get; }

		// Token: 0x1700016F RID: 367
		// (get) Token: 0x06000776 RID: 1910
		int SeedOffset { get; }

		// Token: 0x17000170 RID: 368
		// (get) Token: 0x06000777 RID: 1911
		float Frequency { get; }
	}
}
