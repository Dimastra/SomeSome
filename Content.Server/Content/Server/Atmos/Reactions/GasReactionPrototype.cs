using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Atmos.EntitySystems;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Atmos.Reactions
{
	// Token: 0x02000741 RID: 1857
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("gasReaction", 1)]
	public sealed class GasReactionPrototype : IPrototype
	{
		// Token: 0x170005D5 RID: 1493
		// (get) Token: 0x060026F6 RID: 9974 RVA: 0x000CCF3C File Offset: 0x000CB13C
		[ViewVariables]
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x170005D6 RID: 1494
		// (get) Token: 0x060026F7 RID: 9975 RVA: 0x000CCF44 File Offset: 0x000CB144
		[DataField("minimumRequirements", false, 1, false, false, null)]
		public float[] MinimumRequirements { get; } = new float[9];

		// Token: 0x170005D7 RID: 1495
		// (get) Token: 0x060026F8 RID: 9976 RVA: 0x000CCF4C File Offset: 0x000CB14C
		[DataField("maximumTemperature", false, 1, false, false, null)]
		public float MaximumTemperatureRequirement { get; } = float.MaxValue;

		// Token: 0x170005D8 RID: 1496
		// (get) Token: 0x060026F9 RID: 9977 RVA: 0x000CCF54 File Offset: 0x000CB154
		[DataField("minimumTemperature", false, 1, false, false, null)]
		public float MinimumTemperatureRequirement { get; } = 2.7f;

		// Token: 0x170005D9 RID: 1497
		// (get) Token: 0x060026FA RID: 9978 RVA: 0x000CCF5C File Offset: 0x000CB15C
		[DataField("minimumEnergy", false, 1, false, false, null)]
		public float MinimumEnergyRequirement { get; }

		// Token: 0x170005DA RID: 1498
		// (get) Token: 0x060026FB RID: 9979 RVA: 0x000CCF64 File Offset: 0x000CB164
		[DataField("priority", false, 1, false, false, null)]
		public int Priority { get; } = int.MinValue;

		// Token: 0x060026FC RID: 9980 RVA: 0x000CCF6C File Offset: 0x000CB16C
		public ReactionResult React(GasMixture mixture, [Nullable(2)] IGasMixtureHolder holder, AtmosphereSystem atmosphereSystem)
		{
			ReactionResult result = ReactionResult.NoReaction;
			foreach (IGasReactionEffect effect in this._effects)
			{
				result |= effect.React(mixture, holder, atmosphereSystem);
			}
			return result;
		}

		// Token: 0x04001842 RID: 6210
		[DataField("effects", false, 1, false, false, null)]
		private List<IGasReactionEffect> _effects = new List<IGasReactionEffect>();
	}
}
