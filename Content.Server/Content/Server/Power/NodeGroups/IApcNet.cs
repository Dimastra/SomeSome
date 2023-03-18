using System;
using System.Runtime.CompilerServices;
using Content.Server.Power.Components;

namespace Content.Server.Power.NodeGroups
{
	// Token: 0x02000281 RID: 641
	[NullableContext(1)]
	public interface IApcNet : IBasePowerNet
	{
		// Token: 0x06000CC6 RID: 3270
		void AddApc(ApcComponent apc);

		// Token: 0x06000CC7 RID: 3271
		void RemoveApc(ApcComponent apc);

		// Token: 0x06000CC8 RID: 3272
		void AddPowerProvider(ApcPowerProviderComponent provider);

		// Token: 0x06000CC9 RID: 3273
		void RemovePowerProvider(ApcPowerProviderComponent provider);

		// Token: 0x06000CCA RID: 3274
		void QueueNetworkReconnect();
	}
}
