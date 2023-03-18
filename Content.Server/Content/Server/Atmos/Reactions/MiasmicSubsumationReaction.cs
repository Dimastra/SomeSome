using System;
using System.Runtime.CompilerServices;
using Content.Server.Atmos.EntitySystems;
using Content.Shared.Atmos;

namespace Content.Server.Atmos.Reactions
{
	// Token: 0x02000742 RID: 1858
	public sealed class MiasmicSubsumationReaction : IGasReactionEffect
	{
		// Token: 0x060026FE RID: 9982 RVA: 0x000CD014 File Offset: 0x000CB214
		[NullableContext(1)]
		public ReactionResult React(GasMixture mixture, [Nullable(2)] IGasMixtureHolder holder, AtmosphereSystem atmosphereSystem)
		{
			float initialMiasma = mixture.GetMoles(Gas.Miasma);
			float convert = Math.Min(Math.Min(mixture.GetMoles(Gas.Frezon), initialMiasma), 5f);
			mixture.AdjustMoles(Gas.Miasma, convert);
			mixture.AdjustMoles(Gas.Frezon, -convert);
			return ReactionResult.Reacting;
		}
	}
}
