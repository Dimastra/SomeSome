using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Prototypes;

namespace Content.Server.Botany
{
	// Token: 0x020006F6 RID: 1782
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("seed", 1)]
	public sealed class SeedPrototype : SeedData, IPrototype
	{
		// Token: 0x17000581 RID: 1409
		// (get) Token: 0x06002549 RID: 9545 RVA: 0x000C3810 File Offset: 0x000C1A10
		// (set) Token: 0x0600254A RID: 9546 RVA: 0x000C3818 File Offset: 0x000C1A18
		[IdDataField(1, null)]
		public string ID { get; private set; }
	}
}
