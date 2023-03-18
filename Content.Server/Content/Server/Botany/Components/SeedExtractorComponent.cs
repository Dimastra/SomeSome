using System;
using System.Runtime.CompilerServices;
using Content.Server.Botany.Systems;
using Content.Shared.Construction.Prototypes;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Server.Botany.Components
{
	// Token: 0x02000704 RID: 1796
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(SeedExtractorSystem)
	})]
	public sealed class SeedExtractorComponent : Component
	{
		// Token: 0x04001751 RID: 5969
		[DataField("baseMinSeeds", false, 1, false, false, null)]
		[ViewVariables]
		public int BaseMinSeeds = 1;

		// Token: 0x04001752 RID: 5970
		[DataField("baseMaxSeeds", false, 1, false, false, null)]
		[ViewVariables]
		public int BaseMaxSeeds = 3;

		// Token: 0x04001753 RID: 5971
		[ViewVariables]
		public float SeedAmountMultiplier;

		// Token: 0x04001754 RID: 5972
		[Nullable(1)]
		[DataField("machinePartYieldAmount", false, 1, false, false, typeof(PrototypeIdSerializer<MachinePartPrototype>))]
		public string MachinePartSeedAmount = "Manipulator";

		// Token: 0x04001755 RID: 5973
		[DataField("partRatingSeedAmountMultiplier", false, 1, false, false, null)]
		public float PartRatingSeedAmountMultiplier = 1.5f;
	}
}
