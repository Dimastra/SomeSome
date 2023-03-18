using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Atmos.EntitySystems;
using Content.Shared.Atmos;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Atmos.Reactions
{
	// Token: 0x02000743 RID: 1859
	[DataDefinition]
	public sealed class PlasmaFireReaction : IGasReactionEffect
	{
		// Token: 0x06002700 RID: 9984 RVA: 0x000CD05C File Offset: 0x000CB25C
		[NullableContext(1)]
		public ReactionResult React(GasMixture mixture, [Nullable(2)] IGasMixtureHolder holder, AtmosphereSystem atmosphereSystem)
		{
			float energyReleased = 0f;
			float oldHeatCapacity = atmosphereSystem.GetHeatCapacity(mixture);
			float temperature = mixture.Temperature;
			TileAtmosphere location = holder as TileAtmosphere;
			mixture.ReactionResults[GasReaction.Fire] = 0f;
			float temperatureScale;
			if (temperature > 1643.15f)
			{
				temperatureScale = 1f;
			}
			else
			{
				temperatureScale = (temperature - 373.15f) / 1270f;
			}
			if (temperatureScale > 0f)
			{
				float oxygenBurnRate = 1.4f - temperatureScale;
				float initialOxygenMoles = mixture.GetMoles(Gas.Oxygen);
				float initialPlasmaMoles = mixture.GetMoles(Gas.Plasma);
				bool supersaturation = initialOxygenMoles / initialPlasmaMoles > 96f;
				float plasmaBurnRate;
				if (initialOxygenMoles > initialPlasmaMoles * 10f)
				{
					plasmaBurnRate = initialPlasmaMoles * temperatureScale / 9f;
				}
				else
				{
					plasmaBurnRate = temperatureScale * (initialOxygenMoles / 10f) / 9f;
				}
				if (plasmaBurnRate > 0.0003f)
				{
					plasmaBurnRate = MathF.Min(plasmaBurnRate, MathF.Min(initialPlasmaMoles, initialOxygenMoles / oxygenBurnRate));
					mixture.SetMoles(Gas.Plasma, initialPlasmaMoles - plasmaBurnRate);
					mixture.SetMoles(Gas.Oxygen, initialOxygenMoles - plasmaBurnRate * oxygenBurnRate);
					mixture.AdjustMoles(supersaturation ? Gas.Tritium : Gas.CarbonDioxide, plasmaBurnRate);
					energyReleased += 3000000f * plasmaBurnRate;
					Dictionary<GasReaction, float> reactionResults = mixture.ReactionResults;
					reactionResults[GasReaction.Fire] = reactionResults[GasReaction.Fire] + plasmaBurnRate * (1f + oxygenBurnRate);
				}
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
				float mixTemperature = mixture.Temperature;
				if (mixTemperature > 373.15f)
				{
					atmosphereSystem.HotspotExpose(location.GridIndex, location.GridIndices, mixTemperature, mixture.Volume, false);
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
