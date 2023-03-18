using System;
using System.Runtime.CompilerServices;
using Content.Server.Destructible.Thresholds;
using Robust.Shared.GameObjects;

namespace Content.Server.Destructible
{
	// Token: 0x02000597 RID: 1431
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DamageThresholdReached : EntityEventArgs
	{
		// Token: 0x06001DDD RID: 7645 RVA: 0x0009ECB4 File Offset: 0x0009CEB4
		public DamageThresholdReached(DestructibleComponent parent, DamageThreshold threshold)
		{
			this.Parent = parent;
			this.Threshold = threshold;
		}

		// Token: 0x0400132A RID: 4906
		public readonly DestructibleComponent Parent;

		// Token: 0x0400132B RID: 4907
		public readonly DamageThreshold Threshold;
	}
}
