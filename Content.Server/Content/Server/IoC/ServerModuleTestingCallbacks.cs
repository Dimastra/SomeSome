using System;
using System.Runtime.CompilerServices;
using Content.Shared.Module;

namespace Content.Server.IoC
{
	// Token: 0x02000442 RID: 1090
	[NullableContext(2)]
	[Nullable(0)]
	public sealed class ServerModuleTestingCallbacks : SharedModuleTestingCallbacks
	{
		// Token: 0x170002F5 RID: 757
		// (get) Token: 0x06001602 RID: 5634 RVA: 0x000745E9 File Offset: 0x000727E9
		// (set) Token: 0x06001603 RID: 5635 RVA: 0x000745F1 File Offset: 0x000727F1
		public Action ServerBeforeIoC { get; set; }
	}
}
