using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Placeable
{
	// Token: 0x02000270 RID: 624
	[RegisterComponent]
	[NetworkedComponent]
	[Access(new Type[]
	{
		typeof(PlaceableSurfaceSystem)
	})]
	public sealed class PlaceableSurfaceComponent : Component
	{
		// Token: 0x17000166 RID: 358
		// (get) Token: 0x0600071F RID: 1823 RVA: 0x000186BF File Offset: 0x000168BF
		// (set) Token: 0x06000720 RID: 1824 RVA: 0x000186C7 File Offset: 0x000168C7
		[DataField("isPlaceable", false, 1, false, false, null)]
		public bool IsPlaceable { get; set; } = true;

		// Token: 0x17000167 RID: 359
		// (get) Token: 0x06000721 RID: 1825 RVA: 0x000186D0 File Offset: 0x000168D0
		// (set) Token: 0x06000722 RID: 1826 RVA: 0x000186D8 File Offset: 0x000168D8
		[DataField("placeCentered", false, 1, false, false, null)]
		public bool PlaceCentered { get; set; }

		// Token: 0x17000168 RID: 360
		// (get) Token: 0x06000723 RID: 1827 RVA: 0x000186E1 File Offset: 0x000168E1
		// (set) Token: 0x06000724 RID: 1828 RVA: 0x000186E9 File Offset: 0x000168E9
		[DataField("positionOffset", false, 1, false, false, null)]
		public Vector2 PositionOffset { get; set; }
	}
}
