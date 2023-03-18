using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Singularity.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;

namespace Content.Server.Singularity.Events
{
	// Token: 0x020001E6 RID: 486
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class TilesConsumedByEventHorizonEvent : EntityEventArgs
	{
		// Token: 0x0600092B RID: 2347 RVA: 0x0002E21C File Offset: 0x0002C41C
		public TilesConsumedByEventHorizonEvent([Nullable(new byte[]
		{
			1,
			0
		})] IReadOnlyList<ValueTuple<Vector2i, Tile>> tiles, MapGridComponent mapGrid, EventHorizonComponent eventHorizon)
		{
			this.Tiles = tiles;
			this.MapGrid = mapGrid;
			this.EventHorizon = eventHorizon;
		}

		// Token: 0x04000595 RID: 1429
		[Nullable(new byte[]
		{
			1,
			0
		})]
		public readonly IReadOnlyList<ValueTuple<Vector2i, Tile>> Tiles;

		// Token: 0x04000596 RID: 1430
		public readonly MapGridComponent MapGrid;

		// Token: 0x04000597 RID: 1431
		public readonly EventHorizonComponent EventHorizon;
	}
}
