using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Cargo.Components
{
	// Token: 0x020006ED RID: 1773
	[RegisterComponent]
	public sealed class StaticPriceComponent : Component
	{
		// Token: 0x040016C5 RID: 5829
		[DataField("price", false, 1, true, false, null)]
		public double Price;
	}
}
