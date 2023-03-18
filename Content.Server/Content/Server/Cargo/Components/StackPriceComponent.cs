using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Cargo.Components
{
	// Token: 0x020006EC RID: 1772
	[RegisterComponent]
	public sealed class StackPriceComponent : Component
	{
		// Token: 0x040016C4 RID: 5828
		[DataField("price", false, 1, true, false, null)]
		public double Price;
	}
}
