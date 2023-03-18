using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Atmos.EntitySystems;
using Content.Shared.Atmos;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Atmos.Reactions
{
	// Token: 0x02000744 RID: 1860
	[DataDefinition]
	public sealed class TritiumFireReaction : IGasReactionEffect
	{
		// Token: 0x06002702 RID: 9986 RVA: 0x000CD218 File Offset: 0x000CB418
		[NullableContext(1)]
		public ReactionResult React(GasMixture mixture, [Nullable(2)] IGasMixtureHolder holder, AtmosphereSystem atmosphereSystem)
		{
			float energyReleased = 0f;
			float oldHeatCapacity = atmosphereSystem.GetHeatCapacity(mixture);
			float temperature = mixture.Temperature;
			TileAtmosphere location = holder as TileAtmosphere;
			mixture.ReactionResults[GasReaction.Fire] = 0f;
			float initialTrit = mixture.GetMoles(Gas.Tritium);
			float burnedFuel;
			if (mixture.GetMoles(Gas.Oxygen) < initialTrit || 2000000f > temperature * oldHeatCapacity)
			{
				burnedFuel = mixture.GetMoles(Gas.Oxygen) / 100f;
				if (burnedFuel > initialTrit)
				{
					burnedFuel = initialTrit;
				}
				mixture.AdjustMoles(Gas.Tritium, -burnedFuel);
			}
			else
			{
				burnedFuel = initialTrit;
				mixture.SetMoles(Gas.Tritium, mixture.GetMoles(Gas.Tritium) * 0.9f);
				mixture.AdjustMoles(Gas.Oxygen, -mixture.GetMoles(Gas.Tritium));
				energyReleased += 560000f * burnedFuel * 9f;
			}
			if (burnedFuel > 0f)
			{
				energyReleased += 560000f * burnedFuel;
				mixture.AdjustMoles(Gas.WaterVapor, burnedFuel);
				Dictionary<GasReaction, float> reactionResults = mixture.ReactionResults;
				reactionResults[GasReaction.Fire] = reactionResults[GasReaction.Fire] + burnedFuel;
			}
			if (energyReleased > 0f)
			{
				float newHeatCapacity = atmosphereSystem.GetHeatCapacity(mixture);
				if (newHeatCapacity > 0.0003f)
				{
					mixture.Temperature = (temperature * oldHeatCapacity + energyReleased) / newHeatCapacity;
				}
			}
			if (location != null)
			{
				temperature = mixture.Temperature;
				if (temperature > 373.15f)
				{
					atmosphereSystem.HotspotExpose(location.GridIndex, location.GridIndices, temperature, mixture.Volume, false);
				}
			}
			if (mixture.ReactionResults[GasReaction.Fire] == 0f)
			{
				return ReactionResult.NoReaction;
			}
			return ReactionResult.Reacting;
		}
	}
}
