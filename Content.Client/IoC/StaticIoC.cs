using System;
using System.Runtime.CompilerServices;
using Robust.Client.ResourceManagement;
using Robust.Shared.IoC;

namespace Content.Client.IoC
{
	// Token: 0x020002A2 RID: 674
	[NullableContext(1)]
	[Nullable(0)]
	public static class StaticIoC
	{
		// Token: 0x170003A6 RID: 934
		// (get) Token: 0x060010F6 RID: 4342 RVA: 0x00065550 File Offset: 0x00063750
		public static IResourceCache ResC
		{
			get
			{
				return IoCManager.Resolve<IResourceCache>();
			}
		}
	}
}
