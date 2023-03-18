using System;
using System.Runtime.CompilerServices;
using Content.Server.Atmos.EntitySystems;
using Content.Shared.Atmos;

namespace Content.Server.Atmos.Reactions
{
	// Token: 0x0200073E RID: 1854
	public sealed class FrezonProductionReaction : IGasReactionEffect
	{
		// Token: 0x060026F4 RID: 9972 RVA: 0x000CCEA4 File Offset: 0x000CB0A4
		[NullableContext(1)]
		public ReactionResult React(GasMixture mixture, [Nullable(2)] IGasMixtureHolder holder, AtmosphereSystem atmosphereSystem)
		{
			float moles = mixture.GetMoles(Gas.Nitrogen);
			float initialOxy = mixture.GetMoles(Gas.Oxygen);
			float initialTrit = mixture.GetMoles(Gas.Tritium);
			float efficiency = mixture.Temperature / 73.15f;
			float loss = 1f - efficiency;
			float minimumN2 = (initialOxy + initialTrit) / (10f * efficiency);
			if (moles < minimumN2)
			{
				return ReactionResult.NoReaction;
			}
			float oxyConversion = initialOxy / 50f;
			float tritConversion = initialTrit / 50f;
			float total = oxyConversion + tritConversion;
			mixture.AdjustMoles(Gas.Oxygen, -oxyConversion);
			mixture.AdjustMoles(Gas.Tritium, -tritConversion);
			mixture.AdjustMoles(Gas.Frezon, total * efficiency);
			mixture.AdjustMoles(Gas.Nitrogen, total * loss);
			return ReactionResult.Reacting;
		}
	}
}
