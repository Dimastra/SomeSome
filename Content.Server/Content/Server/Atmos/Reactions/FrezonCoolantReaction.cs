using System;
using System.Runtime.CompilerServices;
using Content.Server.Atmos.EntitySystems;
using Content.Shared.Atmos;

namespace Content.Server.Atmos.Reactions
{
	// Token: 0x0200073D RID: 1853
	public sealed class FrezonCoolantReaction : IGasReactionEffect
	{
		// Token: 0x060026F2 RID: 9970 RVA: 0x000CCDA4 File Offset: 0x000CAFA4
		[NullableContext(1)]
		public ReactionResult React(GasMixture mixture, [Nullable(2)] IGasMixtureHolder holder, AtmosphereSystem atmosphereSystem)
		{
			float oldHeatCapacity = atmosphereSystem.GetHeatCapacity(mixture);
			float temperature = mixture.Temperature;
			float energyModifier = 1f;
			float scale = (temperature - 23.15f) / 350f;
			if (scale > 1f)
			{
				energyModifier = Math.Min(scale, 10f);
				scale = 1f;
			}
			if (scale <= 0f)
			{
				return ReactionResult.NoReaction;
			}
			float initialNit = mixture.GetMoles(Gas.Nitrogen);
			float initialFrezon = mixture.GetMoles(Gas.Frezon);
			float burnRate = initialFrezon * scale / 20f;
			float energyReleased = 0f;
			if (burnRate > 0.0003f)
			{
				float nitAmt = Math.Min(burnRate * 5f, initialNit);
				float frezonAmt = Math.Min(burnRate, initialFrezon);
				mixture.AdjustMoles(Gas.Nitrogen, -nitAmt);
				mixture.AdjustMoles(Gas.Frezon, -frezonAmt);
				mixture.AdjustMoles(Gas.NitrousOxide, nitAmt + frezonAmt);
				energyReleased = burnRate * -3000000f * energyModifier;
			}
			if (energyReleased >= 0f)
			{
				return ReactionResult.NoReaction;
			}
			float newHeatCapacity = atmosphereSystem.GetHeatCapacity(mixture);
			if (newHeatCapacity > 0.0003f)
			{
				mixture.Temperature = (temperature * oldHeatCapacity + energyReleased) / newHeatCapacity;
			}
			return ReactionResult.Reacting;
		}
	}
}
