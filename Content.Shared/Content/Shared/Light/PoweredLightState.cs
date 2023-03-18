using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Light
{
	// Token: 0x02000367 RID: 871
	[NetSerializable]
	[Serializable]
	public enum PoweredLightState : byte
	{
		// Token: 0x040009FC RID: 2556
		Empty,
		// Token: 0x040009FD RID: 2557
		On,
		// Token: 0x040009FE RID: 2558
		Off,
		// Token: 0x040009FF RID: 2559
		Broken,
		// Token: 0x04000A00 RID: 2560
		Burned
	}
}
