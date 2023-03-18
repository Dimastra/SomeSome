using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Storage.Components
{
	// Token: 0x02000134 RID: 308
	[RegisterComponent]
	public sealed class DumpableComponent : Component
	{
		// Token: 0x040003A9 RID: 937
		[DataField("delayPerItem", false, 1, false, false, null)]
		public TimeSpan DelayPerItem = TimeSpan.FromSeconds(0.2);

		// Token: 0x040003AA RID: 938
		[DataField("multiplier", false, 1, false, false, null)]
		public float Multiplier = 1f;
	}
}
