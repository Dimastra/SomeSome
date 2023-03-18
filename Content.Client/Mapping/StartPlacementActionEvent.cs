using System;
using System.Runtime.CompilerServices;
using Content.Shared.Actions;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Client.Mapping
{
	// Token: 0x0200024C RID: 588
	[NullableContext(2)]
	[Nullable(0)]
	public sealed class StartPlacementActionEvent : InstantActionEvent
	{
		// Token: 0x04000761 RID: 1889
		[DataField("entityType", false, 1, false, false, null)]
		public string EntityType;

		// Token: 0x04000762 RID: 1890
		[DataField("tileId", false, 1, false, false, null)]
		public string TileId;

		// Token: 0x04000763 RID: 1891
		[DataField("placementOption", false, 1, false, false, null)]
		public string PlacementOption;

		// Token: 0x04000764 RID: 1892
		[DataField("eraser", false, 1, false, false, null)]
		public bool Eraser;
	}
}
