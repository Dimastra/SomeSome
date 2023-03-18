using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.DeviceNetwork.Components;
using Robust.Shared.GameObjects;

namespace Content.Server.DeviceNetwork.Systems
{
	// Token: 0x02000585 RID: 1413
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class BeforeBroadcastAttemptEvent : CancellableEntityEventArgs
	{
		// Token: 0x06001DA1 RID: 7585 RVA: 0x0009DC6C File Offset: 0x0009BE6C
		public BeforeBroadcastAttemptEvent(IReadOnlySet<DeviceNetworkComponent> recipients)
		{
			this.Recipients = recipients;
		}

		// Token: 0x040012FC RID: 4860
		public readonly IReadOnlySet<DeviceNetworkComponent> Recipients;

		// Token: 0x040012FD RID: 4861
		[Nullable(new byte[]
		{
			2,
			1
		})]
		public HashSet<DeviceNetworkComponent> ModifiedRecipients;
	}
}
