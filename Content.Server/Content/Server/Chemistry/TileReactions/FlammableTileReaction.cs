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
	// Token: 0x02000650 RID: 1616
	[DataDefinition]
	public sealed class FlammableTileReaction : ITileReaction
	{
		// Token: 0x0600223E RID: 8766 RVA: 0x000B3498 File Offset: 0x000B1698
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
			environment.Temperature *= MathF.Max(this._temperatureMultiplier * reactVolume.Float(), 1f);
			atmosphereSystem.ReactTile(tile.GridUid, tile.GridIndices);
			return reactVolume;
		}

		// Token: 0x0400152B RID: 5419
		[DataField("temperatureMultiplier", false, 1, false, false, null)]
		private float _temperatureMultiplier = 1.15f;
	}
}
