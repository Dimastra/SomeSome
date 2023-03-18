using System;
using System.Runtime.CompilerServices;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Atmos.Reactions;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Atmos
{
	// Token: 0x02000735 RID: 1845
	[NullableContext(1)]
	[ImplicitDataDefinitionForInheritors]
	public interface IGasReactionEffect
	{
		// Token: 0x060026C0 RID: 9920
		ReactionResult React(GasMixture mixture, [Nullable(2)] IGasMixtureHolder holder, AtmosphereSystem atmosphereSystem);
	}
}
