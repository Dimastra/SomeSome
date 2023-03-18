using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Content.Server.Salvage
{
	// Token: 0x02000223 RID: 547
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SalvageGridState
	{
		// Token: 0x170001A5 RID: 421
		// (get) Token: 0x06000ADD RID: 2781 RVA: 0x00039354 File Offset: 0x00037554
		// (set) Token: 0x06000ADE RID: 2782 RVA: 0x0003935C File Offset: 0x0003755C
		public TimeSpan CurrentTime { get; set; }

		// Token: 0x170001A6 RID: 422
		// (get) Token: 0x06000ADF RID: 2783 RVA: 0x00039365 File Offset: 0x00037565
		public List<SalvageMagnetComponent> ActiveMagnets { get; } = new List<SalvageMagnetComponent>();
	}
}
