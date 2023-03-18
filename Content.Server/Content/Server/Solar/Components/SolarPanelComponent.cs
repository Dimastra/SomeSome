using System;
using Content.Server.Solar.EntitySystems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Solar.Components
{
	// Token: 0x020001E1 RID: 481
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(PowerSolarSystem)
	})]
	public sealed class SolarPanelComponent : Component
	{
		// Token: 0x1700017A RID: 378
		// (get) Token: 0x06000920 RID: 2336 RVA: 0x0002E01D File Offset: 0x0002C21D
		// (set) Token: 0x06000921 RID: 2337 RVA: 0x0002E025 File Offset: 0x0002C225
		[ViewVariables]
		public float Coverage { get; set; }

		// Token: 0x0400058B RID: 1419
		[DataField("maxSupply", false, 1, false, false, null)]
		public int MaxSupply = 1500;
	}
}
