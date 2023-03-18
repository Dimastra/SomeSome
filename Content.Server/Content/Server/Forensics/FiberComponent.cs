using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Forensics
{
	// Token: 0x020004E0 RID: 1248
	[RegisterComponent]
	public sealed class FiberComponent : Component
	{
		// Token: 0x04001028 RID: 4136
		[Nullable(1)]
		[DataField("fiberMaterial", false, 1, false, false, null)]
		public string FiberMaterial = "fibers-synthetic";

		// Token: 0x04001029 RID: 4137
		[Nullable(2)]
		[DataField("fiberColor", false, 1, false, false, null)]
		public string FiberColor;
	}
}
