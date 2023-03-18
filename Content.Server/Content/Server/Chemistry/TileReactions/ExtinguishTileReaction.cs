using System;
using System.Runtime.CompilerServices;
using Content.Server.Atmos;
using Content.Server.Atmos.EntitySystems;
using Content.Shared.Chemistry.Reaction;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Chemistry.TileReactions
{
	// Token: 0x0200064F RID: 1615
	[DataDefinition]
	public sealed class ExtinguishTileReaction : ITileReaction
	{
		// Token: 0x0600223C RID: 8764 RVA: 0x000B33B0 File Offset: 0x000B15B0
		[NullableContext(1)]
		public FixedPoint2 TileReact(TileRef tile, ReagentPrototype reagent, FixedPoint2 reactVolume)
		{
			if (reactVolume <= FixedPoint2.Zero || tile.Tile.IsEmpty)
			{
				return FixedPoint2.Zero;
			}
			AtmosphereSystem atmosphereSystem = EntitySystem.Get<AtmosphereSystem>();
			GasMixture environment = atmosphereSystem.GetTileMixture(new EntityUid?(tile.GridUid), null, tile.GridIndices, true);
			if (environment == null || !atmosphereSystem.IsHotspotActive(tile.GridUid, tile.GridIndices))
			{
				return FixedPoint2.Zero;
			}
			environment.Temperature = MathF.Max(MathF.Min(environment.Temperature - this._coolingTemperature * 1000f, environment.Temperature / this._coolingTemperature), 2.7f);
			atmosphereSystem.ReactTile(tile.GridUid, tile.GridIndices);
			atmosphereSystem.HotspotExtinguish(tile.GridUid, tile.GridIndices);
			return FixedPoint2.Zero;
		}

		// Token: 0x0400152A RID: 5418
		[DataField("coolingTemperature", false, 1, false, false, null)]
		private float _coolingTemperature = 2f;
	}
}
