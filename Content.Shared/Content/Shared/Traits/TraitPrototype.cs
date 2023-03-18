using System;
using System.Runtime.CompilerServices;
using Content.Shared.Whitelist;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Traits
{
	// Token: 0x020000AD RID: 173
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("trait", 1)]
	public sealed class TraitPrototype : IPrototype
	{
		// Token: 0x17000053 RID: 83
		// (get) Token: 0x060001DD RID: 477 RVA: 0x0000A1A8 File Offset: 0x000083A8
		[ViewVariables]
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x060001DE RID: 478 RVA: 0x0000A1B0 File Offset: 0x000083B0
		// (set) Token: 0x060001DF RID: 479 RVA: 0x0000A1B8 File Offset: 0x000083B8
		[DataField("name", false, 1, false, false, null)]
		public string Name { get; private set; } = "";

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x060001E0 RID: 480 RVA: 0x0000A1C1 File Offset: 0x000083C1
		// (set) Token: 0x060001E1 RID: 481 RVA: 0x0000A1C9 File Offset: 0x000083C9
		[Nullable(2)]
		[DataField("description", false, 1, false, false, null)]
		public string Description { [NullableContext(2)] get; [NullableContext(2)] private set; }

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x060001E2 RID: 482 RVA: 0x0000A1D2 File Offset: 0x000083D2
		[DataField("components", false, 1, false, false, null)]
		public EntityPrototype.ComponentRegistry Components { get; }

		// Token: 0x04000264 RID: 612
		[Nullable(2)]
		[DataField("whitelist", false, 1, false, false, null)]
		public EntityWhitelist Whitelist;

		// Token: 0x04000265 RID: 613
		[Nullable(2)]
		[DataField("blacklist", false, 1, false, false, null)]
		public EntityWhitelist Blacklist;
	}
}
