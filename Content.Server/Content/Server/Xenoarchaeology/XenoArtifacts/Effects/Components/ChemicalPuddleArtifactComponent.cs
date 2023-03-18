using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Xenoarchaeology.XenoArtifacts.Effects.Systems;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;
using Robust.Shared.ViewVariables;

namespace Content.Server.Xenoarchaeology.XenoArtifacts.Effects.Components
{
	// Token: 0x02000051 RID: 81
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(ChemicalPuddleArtifactSystem)
	})]
	public sealed class ChemicalPuddleArtifactComponent : Component
	{
		// Token: 0x040000B8 RID: 184
		[DataField("puddlePrototype", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		[ViewVariables]
		public string PuddlePrototype = "PuddleSmear";

		// Token: 0x040000B9 RID: 185
		[DataField("chemicalSolution", false, 1, true, false, null)]
		[ViewVariables]
		public Solution ChemicalSolution;

		// Token: 0x040000BA RID: 186
		[DataField("possibleChemicals", false, 1, true, false, typeof(PrototypeIdListSerializer<ReagentPrototype>))]
		public List<string> PossibleChemicals;

		// Token: 0x040000BB RID: 187
		[DataField("chemAmount", false, 1, false, false, null)]
		public int ChemAmount = 3;
	}
}
