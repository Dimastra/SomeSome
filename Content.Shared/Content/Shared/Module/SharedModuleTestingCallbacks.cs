using System;
using System.Runtime.CompilerServices;
using Robust.Shared.ContentPack;

namespace Content.Shared.Module
{
	// Token: 0x020002FA RID: 762
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedModuleTestingCallbacks : ModuleTestingCallbacks
	{
		// Token: 0x1700019E RID: 414
		// (get) Token: 0x06000882 RID: 2178 RVA: 0x0001CCF2 File Offset: 0x0001AEF2
		// (set) Token: 0x06000883 RID: 2179 RVA: 0x0001CCFA File Offset: 0x0001AEFA
		public Action SharedBeforeIoC { get; set; }
	}
}
