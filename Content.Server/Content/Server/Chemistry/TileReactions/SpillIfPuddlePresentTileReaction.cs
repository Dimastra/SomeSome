using System;
using System.Runtime.CompilerServices;
using Content.Server.Fluids.Components;
using Content.Server.Fluids.EntitySystems;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Reaction;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Chemistry.TileReactions
{
	// Token: 0x02000652 RID: 1618
	[DataDefinition]
	public sealed class SpillIfPuddlePresentTileReaction : ITileReaction
	{
		// Token: 0x06002242 RID: 8770 RVA: 0x000B3574 File Offset: 0x000B1774
		[NullableContext(1)]
		public FixedPoint2 TileReact(TileRef tile, ReagentPrototype reagent, FixedPoint2 reactVolume)
		{
			SpillableSystem spillSystem = EntitySystem.Get<SpillableSystem>();
			PuddleComponent puddleComponent;
			if (reactVolume < 5 || !spillSystem.TryGetPuddle(tile, out puddleComponent))
			{
				return FixedPoint2.Zero;
			}
			if (spillSystem.SpillAt(tile, new Solution(reagent.ID, reactVolume), "PuddleSmear", true, false, true, true) == null)
			{
				return FixedPoint2.Zero;
			}
			return reactVolume;
		}
	}
}
