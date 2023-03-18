using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Clothing.Components
{
	// Token: 0x0200063A RID: 1594
	[RegisterComponent]
	public sealed class GloveHeatResistanceComponent : Component
	{
		// Token: 0x040014CA RID: 5322
		[DataField("heatResistance", false, 1, false, false, null)]
		public int HeatResistance = 323;
	}
}
