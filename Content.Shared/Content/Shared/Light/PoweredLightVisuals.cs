using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Light
{
	// Token: 0x02000366 RID: 870
	[NetSerializable]
	[Serializable]
	public enum PoweredLightVisuals : byte
	{
		// Token: 0x040009F9 RID: 2553
		BulbState,
		// Token: 0x040009FA RID: 2554
		Blinking
	}
}
