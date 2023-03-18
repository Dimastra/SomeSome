using System;
using System.Runtime.CompilerServices;
using Content.Server.Power.Components;
using Content.Server.Power.Pow3r;

namespace Content.Server.Power.NodeGroups
{
	// Token: 0x02000284 RID: 644
	[NullableContext(1)]
	public interface IBasePowerNet
	{
		// Token: 0x06000CDD RID: 3293
		void AddConsumer(PowerConsumerComponent consumer);

		// Token: 0x06000CDE RID: 3294
		void RemoveConsumer(PowerConsumerComponent consumer);

		// Token: 0x170001D4 RID: 468
		// (get) Token: 0x06000CDF RID: 3295
		PowerState.Network NetworkNode { get; }
	}
}
