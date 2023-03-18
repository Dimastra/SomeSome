using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Access
{
	// Token: 0x0200076E RID: 1902
	[NullableContext(2)]
	[Nullable(0)]
	[Prototype("accessLevel", 1)]
	public sealed class AccessLevelPrototype : IPrototype
	{
		// Token: 0x170004E8 RID: 1256
		// (get) Token: 0x0600177A RID: 6010 RVA: 0x0004C4D2 File Offset: 0x0004A6D2
		[Nullable(1)]
		[ViewVariables]
		[IdDataField(1, null)]
		public string ID { [NullableContext(1)] get; }

		// Token: 0x170004E9 RID: 1257
		// (get) Token: 0x0600177B RID: 6011 RVA: 0x0004C4DA File Offset: 0x0004A6DA
		// (set) Token: 0x0600177C RID: 6012 RVA: 0x0004C4E2 File Offset: 0x0004A6E2
		[DataField("name", false, 1, false, false, null)]
		public string Name { get; set; }
	}
}
