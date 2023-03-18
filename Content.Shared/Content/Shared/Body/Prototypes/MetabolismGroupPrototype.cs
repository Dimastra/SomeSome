using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Prototypes;

namespace Content.Shared.Body.Prototypes
{
	// Token: 0x02000656 RID: 1622
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("metabolismGroup", 1)]
	public sealed class MetabolismGroupPrototype : IPrototype
	{
		// Token: 0x170003F6 RID: 1014
		// (get) Token: 0x060013CB RID: 5067 RVA: 0x00042AD8 File Offset: 0x00040CD8
		[IdDataField(1, null)]
		public string ID { get; }
	}
}
