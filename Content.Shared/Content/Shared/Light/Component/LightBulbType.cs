using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Light.Component
{
	// Token: 0x0200036E RID: 878
	[NetSerializable]
	[Serializable]
	public enum LightBulbType : byte
	{
		// Token: 0x04000A19 RID: 2585
		Bulb,
		// Token: 0x04000A1A RID: 2586
		Tube
	}
}
