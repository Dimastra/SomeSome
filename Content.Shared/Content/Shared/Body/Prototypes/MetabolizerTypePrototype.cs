using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Prototypes;

namespace Content.Shared.Body.Prototypes
{
	// Token: 0x02000657 RID: 1623
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("metabolizerType", 1)]
	public sealed class MetabolizerTypePrototype : IPrototype
	{
		// Token: 0x170003F7 RID: 1015
		// (get) Token: 0x060013CD RID: 5069 RVA: 0x00042AE8 File Offset: 0x00040CE8
		[IdDataField(1, null)]
		public string ID { get; }
	}
}
