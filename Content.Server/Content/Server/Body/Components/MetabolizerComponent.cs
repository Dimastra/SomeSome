using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Body.Systems;
using Content.Shared.Body.Prototypes;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Set;

namespace Content.Server.Body.Components
{
	// Token: 0x02000715 RID: 1813
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(MetabolizerSystem)
	})]
	public sealed class MetabolizerComponent : Component
	{
		// Token: 0x170005A2 RID: 1442
		// (get) Token: 0x06002628 RID: 9768 RVA: 0x000C9715 File Offset: 0x000C7915
		// (set) Token: 0x06002629 RID: 9769 RVA: 0x000C971D File Offset: 0x000C791D
		[DataField("solution", false, 1, false, false, null)]
		public string SolutionName { get; set; } = BloodstreamComponent.DefaultChemicalsSolutionName;

		// Token: 0x040017A3 RID: 6051
		public float AccumulatedFrametime;

		// Token: 0x040017A4 RID: 6052
		[DataField("updateFrequency", false, 1, false, false, null)]
		public float UpdateFrequency = 1f;

		// Token: 0x040017A6 RID: 6054
		[DataField("solutionOnBody", false, 1, false, false, null)]
		public bool SolutionOnBody = true;

		// Token: 0x040017A7 RID: 6055
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[DataField("metabolizerTypes", false, 1, false, false, typeof(PrototypeIdHashSetSerializer<MetabolizerTypePrototype>))]
		[Access]
		public HashSet<string> MetabolizerTypes;

		// Token: 0x040017A8 RID: 6056
		[DataField("removeEmpty", false, 1, false, false, null)]
		public bool RemoveEmpty;

		// Token: 0x040017A9 RID: 6057
		[DataField("maxReagents", false, 1, false, false, null)]
		public int MaxReagentsProcessable = 3;

		// Token: 0x040017AA RID: 6058
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[DataField("groups", false, 1, false, false, null)]
		public List<MetabolismGroupEntry> MetabolismGroups;
	}
}
