using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.GPS
{
	// Token: 0x020000B0 RID: 176
	public abstract class SharedHandheldGPSComponent : Component
	{
		// Token: 0x04000269 RID: 617
		[DataField("updateRate", false, 1, false, false, null)]
		public float UpdateRate = 1.5f;
	}
}
