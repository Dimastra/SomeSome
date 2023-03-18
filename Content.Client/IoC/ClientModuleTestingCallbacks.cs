using System;
using System.Runtime.CompilerServices;
using Content.Shared.Module;

namespace Content.Client.IoC
{
	// Token: 0x020002A1 RID: 673
	[NullableContext(2)]
	[Nullable(0)]
	public sealed class ClientModuleTestingCallbacks : SharedModuleTestingCallbacks
	{
		// Token: 0x170003A5 RID: 933
		// (get) Token: 0x060010F3 RID: 4339 RVA: 0x00065537 File Offset: 0x00063737
		// (set) Token: 0x060010F4 RID: 4340 RVA: 0x0006553F File Offset: 0x0006373F
		public Action ClientBeforeIoC { get; set; }
	}
}
