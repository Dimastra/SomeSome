using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Server.Destructible.Thresholds.Behaviors
{
	// Token: 0x020005AA RID: 1450
	[NullableContext(1)]
	public interface IThresholdBehavior
	{
		// Token: 0x06001E1A RID: 7706
		void Execute(EntityUid owner, DestructibleSystem system, EntityUid? cause = null);
	}
}
