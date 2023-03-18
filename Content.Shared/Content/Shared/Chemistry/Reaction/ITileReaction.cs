using System;
using System.Runtime.CompilerServices;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Shared.Map;

namespace Content.Shared.Chemistry.Reaction
{
	// Token: 0x020005EA RID: 1514
	[NullableContext(1)]
	public interface ITileReaction
	{
		// Token: 0x06001248 RID: 4680
		FixedPoint2 TileReact(TileRef tile, ReagentPrototype reagent, FixedPoint2 reactVolume);
	}
}
