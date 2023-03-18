using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.IgnitionSource
{
	// Token: 0x02000455 RID: 1109
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(IgnitionSourceSystem)
	})]
	public sealed class IgnitionSourceComponent : Component
	{
		// Token: 0x04000E03 RID: 3587
		[DataField("ignited", false, 1, false, false, null)]
		public bool Ignited;

		// Token: 0x04000E04 RID: 3588
		[DataField("temperature", false, 1, true, false, null)]
		public int Temperature;
	}
}
