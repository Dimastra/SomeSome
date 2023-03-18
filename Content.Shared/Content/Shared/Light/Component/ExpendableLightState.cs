using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Light.Component
{
	// Token: 0x02000376 RID: 886
	[NetSerializable]
	[Serializable]
	public enum ExpendableLightState
	{
		// Token: 0x04000A2E RID: 2606
		BrandNew,
		// Token: 0x04000A2F RID: 2607
		Lit,
		// Token: 0x04000A30 RID: 2608
		Fading,
		// Token: 0x04000A31 RID: 2609
		Dead
	}
}
