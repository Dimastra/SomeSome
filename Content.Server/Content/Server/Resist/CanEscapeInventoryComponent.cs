using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Resist
{
	// Token: 0x02000236 RID: 566
	[RegisterComponent]
	public sealed class CanEscapeInventoryComponent : Component
	{
		// Token: 0x040006F6 RID: 1782
		[DataField("baseResistTime", false, 1, false, false, null)]
		public float BaseResistTime = 5f;
	}
}
