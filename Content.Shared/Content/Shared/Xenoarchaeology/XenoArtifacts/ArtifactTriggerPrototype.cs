using System;
using System.Runtime.CompilerServices;
using Content.Shared.Whitelist;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Xenoarchaeology.XenoArtifacts
{
	// Token: 0x02000015 RID: 21
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("artifactTrigger", 1)]
	[DataDefinition]
	public sealed class ArtifactTriggerPrototype : IPrototype
	{
		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600001E RID: 30 RVA: 0x000023A2 File Offset: 0x000005A2
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x04000024 RID: 36
		[DataField("components", false, 1, false, true, null)]
		public EntityPrototype.ComponentRegistry Components = new EntityPrototype.ComponentRegistry();

		// Token: 0x04000025 RID: 37
		[DataField("targetDepth", false, 1, false, false, null)]
		public int TargetDepth;

		// Token: 0x04000026 RID: 38
		[Nullable(2)]
		[DataField("triggerHint", false, 1, false, false, null)]
		public string TriggerHint;

		// Token: 0x04000027 RID: 39
		[Nullable(2)]
		[DataField("whitelist", false, 1, false, false, null)]
		public EntityWhitelist Whitelist;

		// Token: 0x04000028 RID: 40
		[Nullable(2)]
		[DataField("blacklist", false, 1, false, false, null)]
		public EntityWhitelist Blacklist;
	}
}
