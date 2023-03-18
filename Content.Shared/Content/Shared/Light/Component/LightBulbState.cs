using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Light.Component
{
	// Token: 0x0200036C RID: 876
	[NetSerializable]
	[Serializable]
	public enum LightBulbState : byte
	{
		// Token: 0x04000A12 RID: 2578
		Normal,
		// Token: 0x04000A13 RID: 2579
		Broken,
		// Token: 0x04000A14 RID: 2580
		Burned
	}
}
