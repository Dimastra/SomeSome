using System;
using System.Runtime.CompilerServices;
using Content.Shared.Whitelist;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Xenoarchaeology.XenoArtifacts
{
	// Token: 0x02000014 RID: 20
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("artifactEffect", 1)]
	[DataDefinition]
	public sealed class ArtifactEffectPrototype : IPrototype
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x0600001C RID: 28 RVA: 0x0000237C File Offset: 0x0000057C
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x0400001D RID: 29
		[DataField("components", false, 1, false, true, null)]
		public EntityPrototype.ComponentRegistry Components = new EntityPrototype.ComponentRegistry();

		// Token: 0x0400001E RID: 30
		[DataField("permanentComponents", false, 1, false, false, null)]
		public EntityPrototype.ComponentRegistry PermanentComponents = new EntityPrototype.ComponentRegistry();

		// Token: 0x0400001F RID: 31
		[DataField("targetDepth", false, 1, false, false, null)]
		public int TargetDepth;

		// Token: 0x04000020 RID: 32
		[Nullable(2)]
		[DataField("effectHint", false, 1, false, false, null)]
		public string EffectHint;

		// Token: 0x04000021 RID: 33
		[Nullable(2)]
		[DataField("whitelist", false, 1, false, false, null)]
		public EntityWhitelist Whitelist;

		// Token: 0x04000022 RID: 34
		[Nullable(2)]
		[DataField("blacklist", false, 1, false, false, null)]
		public EntityWhitelist Blacklist;
	}
}
