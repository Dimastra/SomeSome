using System;
using System.Runtime.CompilerServices;
using Content.Server.Maps;
using Content.Shared.Chemistry.Reaction;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Chemistry.TileReactions
{
	// Token: 0x02000651 RID: 1617
	[DataDefinition]
	public sealed class PryTileReaction : ITileReaction
	{
		// Token: 0x06002240 RID: 8768 RVA: 0x000B3557 File Offset: 0x000B1757
		[NullableContext(1)]
		public FixedPoint2 TileReact(TileRef tile, ReagentPrototype reagent, FixedPoint2 reactVolume)
		{
			IoCManager.Resolve<IEntityManager>().System<TileSystem>().PryTile(tile);
			return reactVolume;
		}
	}
}
